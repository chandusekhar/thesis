using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Common;
using DMT.Common.Extensions;
using DMT.Core.Interfaces;
using DMT.Partition.Interfaces;

namespace DMT.Partition
{
    /// <summary>
    /// Partition model into n separate partition without any optimization.
    /// </summary>
    [Export(typeof(IPartitionManager))]
    [ExportMetadata("name", "simple")]
    class SimplePartitionManager : IPartitionManager
    {
        private IPartitionEntityFactory partitionFactory;

        [ImportingConstructor]
        public SimplePartitionManager(IPartitionEntityFactory factory)
        {
            this.partitionFactory = factory;
        }

        public IEnumerable<IPartition> PartitionModel(IModel model)
        {
            List<IPartition> partitionList = new List<IPartition>();
            List<INode> nodes = new List<INode>(model.Nodes);
            nodes.Shuffle();

            int numberOfPartitions = int.Parse(Configuration.Current.GetOption("partitions"));
            int numberOfNodesInPartion = (int)Math.Ceiling(nodes.Count / (double)numberOfPartitions);

            int cntr = 0;
            IPartition partition = this.partitionFactory.CreatePartition();

            foreach (var node in nodes)
            {
                if (cntr < numberOfNodesInPartion)
                {
                    partition.Add(node);
                    ++cntr;
                }
                else
                {
                    cntr = 1;
                    partitionList.Add(partition);
                    partition = this.partitionFactory.CreatePartition();
                    partition.Add(node);
                }
            }

            if (!partition.IsEmpty && !partitionList.Contains(partition))
            {
                partitionList.Add(partition);
            }

            return partitionList;
        }
    }
}
