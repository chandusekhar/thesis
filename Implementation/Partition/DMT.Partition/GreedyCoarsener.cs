using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;
using DMT.Partition.Interfaces;
using NLog;

namespace DMT.Partition
{
    [Export(typeof(ICoarsener))]
    internal class GreedyCoarsener : ICoarsener
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private double reductionFactor;
        private IPartitionEntityFactory factory;
        
        /// <summary>
        /// Node cache to speed up node lookup when building connections between supernodes.
        /// </summary>
        private Dictionary<IId, ISuperNode> nodeCache;

        /// <summary>
        /// Gets or sets the reduction factor.
        /// 
        /// This number is the ratio of the nodes between two coarsening passes.
        /// </summary>
        /// <example>
        ///     0.5 means to halve to nodes in every pass
        ///     0.2 means to leave only 1/5 of the nodes after every pass.
        /// </example>
        public double ReductionFactor
        {
            get { return this.reductionFactor; }
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("value", "Reduction factor must be between 0 and 1.");
                }
                this.reductionFactor = value;
            }
        }
        public double Factor { get; set; }

        [ImportingConstructor]
        public GreedyCoarsener(IPartitionEntityFactory factory)
        {
            this.factory = factory;

            this.Factor = 0.1;
            // halve the number of nodes on every pass
            this.ReductionFactor = 0.5;
        }

        /// <summary>
        /// Coarsen a graph by greedily picking nodes with the highest (sum) degree.
        /// </summary>
        public IEnumerable<ISuperNode> Coarsen(IEnumerable<INode> nodes)
        {
            int count = nodes.Count();
            int passes = this.GetNumberOfRequiredPasses(count);
            logger.Info("Starting coarsening with {0} factor in {1}-node graph.", this.Factor, count);

            IEnumerable<ISuperNode> result = null;
            IEnumerable<INode> source = nodes;

            while (passes > 0)
            {
                result = this.CoarsenOnce(source);
                // lets coarsen again
                source = result;
                passes--;
                logger.Info("Pass of coarsening is done. Remaining passes: {0}. New graph has {1} nodes.", passes, result.Count());
            }

            return result;
        }

        public void Uncoarsen(IEnumerable<IPartition> partitions, IPartitionRefiner refiner)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the approximate number of passes required to reduce the graph.
        /// </summary>
        /// <param name="count">Count of nodes in the graph</param>
        /// <returns></returns>
        public int GetNumberOfRequiredPasses(int count)
        {
            double newCount = this.Factor * count;
            return (int)Math.Ceiling(Math.Log(newCount, 1 / this.ReductionFactor));
        }

        public IEnumerable<ISuperNode> CoarsenOnce(IEnumerable<INode> nodes)
        {
            IEnumerable<ISuperNode> clusters = BuildClusters(nodes);
            BuildEdgesBetweenClusters(clusters);
            return clusters;
        }


        public void BuildEdgesBetweenClusters(IEnumerable<ISuperNode> clusters)
        {
            if (nodeCache == null)
            {
                throw new InvalidOperationException("Run 'BuildClasters' first to fill up the cache");
            }

            foreach (var supernode in clusters)
            {
                foreach (var node in supernode.Nodes)
                {
                    foreach (var edge in node.InboundEdges)
                    {
                        ConnectSupernodes(supernode, edge.Source.Id, EdgeDirection.Inbound);
                    }

                    foreach (var edge in node.OutboundEdges)
                    {
                        ConnectSupernodes(supernode, edge.Target.Id, EdgeDirection.Outbound);
                    }
                }
            }

            this.nodeCache = null;
        }

        public IEnumerable<ISuperNode> BuildClusters(IEnumerable<INode> nodes)
        {
            this.nodeCache = new Dictionary<IId, ISuperNode>();

            var nodeList = new List<INode>(nodes);
            nodeList.Sort(CompareNodesBasedOnDegreeDesc);

            // number of nodes to remove by making node clasters
            int countOfNodesToLose = (int)(nodeList.Count * (1 - this.ReductionFactor));
            var result = new List<ISuperNode>();

            INode node;
            IEnumerable<INode> neighbours;
            ISuperNode cluster;

            while (countOfNodesToLose > 0 && nodeList.Count > 0)
            {
                // get the node with the highest degree
                node = nodeList[0];
                // create supernode and add node with highest degree
                cluster = this.factory.CreateSuperNode(node);
                this.nodeCache.Add(node.Id, cluster);

                // remove nodes from potentials
                nodeList.Remove(node);
                neighbours = node.GetAdjacentNodes();
                foreach (var neighbour in neighbours)
                {
                    nodeList.Remove(neighbour);
                    // add nodes only once to supernodes
                    if (!nodeCache.ContainsKey(neighbour.Id))
                    {
                        // add nodes to cluster
                        cluster.Nodes.Add(neighbour);
                        this.nodeCache.Add(neighbour.Id, cluster);
                    }
                }

                // decrease node count by the count of neighbours + 1 (for self)
                countOfNodesToLose -= neighbours.Count() + 1;
                result.Add(cluster);
            }

            // wrap leftover single nodes in supernodes for further processing
            foreach (var n in nodeList)
            {
                cluster = this.factory.CreateSuperNode(n);
                result.Add(cluster);
                this.nodeCache.Add(n.Id, cluster);

            }
            return result;
        }

        private int CompareNodesBasedOnDegreeDesc(INode a, INode b)
        {
            return b.Degree - a.Degree;
        }

        private void ConnectSupernodes(ISuperNode baseNode, IId nodeid, EdgeDirection direction)
        {
            ISuperNode node;
            if (this.nodeCache.TryGetValue(nodeid, out node))
            {
                if (node != baseNode)
                {
                    baseNode.ConnectTo(node, direction);
                }
                this.nodeCache.Remove(nodeid);
            }
        }

    }
}
