using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Common.Composition;
using DMT.Core.Interfaces;
using DMT.Partition.Interfaces;
using DMT.Common.Extensions;
using System.Diagnostics;

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
        private TimeSpan lastRunDuration = TimeSpan.Zero;

        [ImportMany]
        private Lazy<IPartitionManager, IDictionary<string, object>>[] PartitionManagers { get; set; }

        public IEnumerable<IPartition> Partitions
        {
            get { return this.partitions; }
        }

        public TimeSpan LastRunDuration
        {
            get { return this.lastRunDuration; }
        }

        public Partitioner(IModel model)
        {
            this.model = model;
        }

        public IEnumerable<IPartition> Partition()
        {
            var w = Stopwatch.StartNew();

            var pm = ChosePartitioner();

            ConfigurePartitioner(pm);
            this.partitions = pm.PartitionModel(this.model);

            PartitionPostProcessing(this.partitions);

            w.Stop();
            this.lastRunDuration = w.Elapsed;

            logger.Info("Partitioning is done, ready to send partitions to matcher modules. Took: {0}", w.Elapsed);
            return this.partitions;
        }

        /// <summary>
        /// Chose the correct partitioner implementation for the available choices.
        /// TODO: make it configurable
        /// </summary>
        /// <returns></returns>
        private IPartitionManager ChosePartitioner()
        {
            if (this.PartitionManagers.Length == 1)
            {
                return this.PartitionManagers.First().Value;
            }

            return this.PartitionManagers.First(pm => pm.Metadata.ContainsKey("name") && "simple".Equals(pm.Metadata["name"])).Value;
        }

        private void ConfigurePartitioner(IPartitionManager pm)
        {
            // TODO: fine tuning of partitioner
            // currently it does nothing
        }

        /// <summary>
        /// Sets up the nodes and edges to know about the partitions they are in.
        /// </summary>
        private void PartitionPostProcessing(IEnumerable<IPartition> partitions)
        {
            foreach (var partition in partitions)
            {
                foreach (var n in partition.Nodes)
                {
                    SetPartitionOnNode(n, partition);
                }
            }
        }

        private void SetPartitionOnNode(INode node, IPartition partition)
        {
            var pNode = node as IPartitionNode;

            if (pNode == null)
            {
                logger.Error("Could not cast node to IPartitionNode. During partitioning, IPartitionNodes should be used.");
                // TODO: introduce new exception type?
                throw new Exception("Wrong node type for partitioning");
            }

            pNode.Partition = partition;
        }

    }
}
