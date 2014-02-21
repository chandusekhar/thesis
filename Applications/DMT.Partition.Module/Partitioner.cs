using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Common.Composition;
using DMT.Core.Interfaces;
using DMT.Partition.Interfaces;

namespace DMT.Partition.Module
{
    /// <summary>
    /// A light wrapper around the IPartitionManager interface.
    /// 
    /// After partitioning it sets up the nodes and edges to know about their partitions.
    /// </summary>
    class Partitioner : InjectableBase
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private IModel model;
        private IEnumerable<IPartition> partitions = null;

        [Import]
        private IPartitionManager partitionManager;

        public IEnumerable<IPartition> Partitions
        {
            get { return this.partitions; }
        }

        public Partitioner(IModel model)
        {
            this.model = model;
        }

        public IEnumerable<IPartition> Partition()
        {
            this.ConfigurePartitioner();
            this.partitions = this.partitionManager.PartitionModel(this.model);

            this.PartitionPostProcessing();

            logger.Info("Partitioning is done, ready to send partitions to matcher modules.");
            return this.partitions;
        }

        private void ConfigurePartitioner()
        {
            // TODO: fine tuning of partitioner
            // currently it does nothing
        }

        /// <summary>
        /// Sets up the nodes and edges to know about the partitions they are in.
        /// </summary>
        private void PartitionPostProcessing()
        {
        }

    }
}
