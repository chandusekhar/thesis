using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;
using DMT.Partition.Interfaces;
using DMT.Partition.Interfaces.Events;

namespace DMT.Partition
{
    [Export(typeof(IPartitionManager))]
    [Export(typeof(IThreeStepPartitionManager))]
    internal class PartitionManager : IThreeStepPartitionManager
    {
        private ICoarsener coarsener;
        private IPartitionRefiner refiner;
        private IPartitioner partitioner;

        public ICoarsener Coarsener
        {
            get { return coarsener; }
        }

        public IPartitionRefiner Refiner
        {
            get { return refiner; }
        }


        public IPartitioner Partitioner
        {
            get { return partitioner; }
        }

        public event AfterCoarseningEventHandler AfterCoarsening;

        public event AfterPartitioningEventHandler AfterPartitioning;

        [ImportingConstructor]
        public PartitionManager(ICoarsener coarsener, IPartitioner partitioner, IPartitionRefiner refiner)
        {
            this.partitioner = partitioner;
            this.coarsener = coarsener;
            this.refiner = refiner;
        }

        public IEnumerable<IPartition> PartitionModel(IModel model)
        {
            IEnumerable<ISuperNode> coarsenedGraph =  this.coarsener.Coarsen(model.Nodes);
            OnAfterCoarsening(coarsenedGraph);

            IEnumerable<IPartition> partitions = partitioner.Partition(coarsenedGraph);
            OnAfterPartitioning(partitions);

            // uncoarsen and refine partitions in place
            this.coarsener.Uncoarsen(partitions, this.refiner);

            return partitions;
        }

        private void OnAfterCoarsening(IEnumerable<ISuperNode> nodes)
        {
            var handler = this.AfterCoarsening;
            if (handler != null)
            {
                handler(this, new AfterCoarseningEventArgs(nodes));
            }
        }

        private void OnAfterPartitioning(IEnumerable<IPartition> paritions)
        {
            var handler = this.AfterPartitioning;
            if (handler != null)
            {
                handler(this, new AfterPartitioningEventArgs(paritions));
            }
        }
    }
}
