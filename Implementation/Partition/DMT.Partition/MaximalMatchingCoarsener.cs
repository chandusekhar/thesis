using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Common.Extensions;
using DMT.Core.Interfaces;
using DMT.Partition.Interfaces;

namespace DMT.Partition
{
    [Export(typeof(ICoarsener))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class MaximalMatchingCoarsener : ICoarsener
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private int goalNodeCount;
        private IPartitionEntityFactory factory;
        
        public double Factor { get; set; }

        [ImportingConstructor]
        public MaximalMatchingCoarsener(IPartitionEntityFactory factory)
        {
            this.Factor = 0.1;
            this.goalNodeCount = 0;
            this.factory = factory;
        }

        public IEnumerable<ISuperNode> Coarsen(IEnumerable<INode> nodes)
        {
            this.goalNodeCount = CalculateGoalNodeCount(nodes);
            IEnumerable<INode> source = new List<INode>(nodes);
            IEnumerable<ISuperNode> result = null;
            logger.Debug("Coarsening starts. Node count: {0}. Goal: {1}", source.Count(), this.goalNodeCount);
            
            int passes = 1;
            while (goalNodeCount > source.Count())
            {
                result = CoarsenOnce(source);
                int countBefore = source.Count();
                int decrease = countBefore - result.Count();

                logger.Debug("Nr of nodes after {0}. coarsening pass: {1}", passes, decrease);

                passes += 1;
                source = result;
            }

            logger.Debug("Coarsening finished. Final number of nodes: {0}", result.Count());

            return result;
        }

        public void Uncoarsen(IEnumerable<IPartition> partitions, IPartitionRefiner refiner)
        {
            logger.Debug("Uncoarsening starts");
            // TODO make check faster
            while (partitions.SelectMany(p => p.Nodes).OfType<ISuperNode>().Any())
            {
                // pop last level of supernodes in place
                partitions.ForEach(p => p.Inflate());
                if (refiner != null)
                {
                    refiner.Refine(partitions);
                }
            }

            logger.Debug("Uncoarsening finished.");
        }

        private IEnumerable<ISuperNode> CoarsenOnce(IEnumerable<INode> nodes)
        {
            var matching = new MaximalMatching(nodes);
            var matchingEdges = matching.Find();

            GraphContractor g = new GraphContractor(nodes, factory);
            return g.ContractEdges(matchingEdges);
        }

        private int CalculateGoalNodeCount(IEnumerable<INode> nodes)
        {
            return (int)Math.Floor(this.Factor * nodes.Count());
        }
    }
}
