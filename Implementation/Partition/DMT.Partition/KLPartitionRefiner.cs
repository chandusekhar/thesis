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
    /// <summary>
    /// Basic implementation of the Kernighan/Lin algorithm.
    /// </summary>
    [Export(typeof(IPartitionRefiner))]
    internal class KLPartitionRefiner : IPartitionRefiner
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private const double refinePercentage = 0.1;

        public int Seed { get; set; }

        public KLPartitionRefiner()
        {
            this.Seed = (int)(DateTime.Now - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
        }

        public void Refine(IPartition p1, IPartition p2)
        {
            logger.Trace("Refining {0} and {1} partitions.", p1, p2);
            if (p1 == null) { throw new ArgumentNullException("p1"); }
            if (p2 == null) { throw new ArgumentNullException("p2"); }

            int gain = 0;
            do
            {
                List<PartitioningNode> nodes = ComputeDValues(p1, p2);
                List<PartitioningNodePair> selected = new List<PartitioningNodePair>();
                PartitioningNodePair pair;

                while (nodes.Any())
                {
                    // selected unmarked max gain
                    pair = SelectMaxGain(nodes);

                    if (pair == null)
                    {
                        // local minimum reached early
                        break;
                    }

                    // mark pair
                    nodes.Remove(pair.NodeA);
                    nodes.Remove(pair.NodeB);
                    selected.Add(pair);

                    // update D value as pair's node have been swapped
                    UpdateDValues(nodes, pair);
                }

                var pairs = PickPairsToSwap(selected, out gain);

                if (gain > 0)
                {
                    SwapPairs(pairs);
                }

            } while (gain > 0);
        }

        // refine only some percentage of all the pairs
        public void Refine(IEnumerable<IPartition> partitions)
        {
            logger.Info("Refinement started.");
            var pairs = MakePairsOfPartitions(partitions.ToList());
            foreach (var pair in pairs)
            {
                this.Refine(pair.Item1, pair.Item2);
            }
            logger.Info("Refinement finished.");
        }

        /// <summary>
        /// T in the KL algorithm
        /// </summary>
        /// <param name="pNA"></param>
        /// <param name="pNB"></param>
        /// <returns></returns>
        internal int ComputeInitialInterPartitionCost(IPartition p1, IPartition p2)
        {
            return p1.GetEdgesBetween(p2).Count() + p2.GetEdgesBetween(p1).Count();
        }

        /// <summary>
        /// E(a) in KL
        /// </summary>
        /// <param name="node"></param>
        /// <param name="otherPartition"></param>
        /// <returns></returns>
        internal int ComputeExternalCost(INode node, IPartition otherPartition)
        {
            // node is in the same otherPartition, then there is no e cost for that otherPartition
            if (otherPartition.HasNode(node))
            {
                logger.Error("A node mustn't be a part of the partition for computing external costs.");
                throw new ArgumentException();
            }
            return ComputeCost(node, otherPartition);
        }

        /// <summary>
        /// I(a) in KL
        /// </summary>
        /// <param name="node"></param>
        /// <param name="otherPartition"></param>
        /// <returns></returns>
        internal int ComputeInternalCost(INode node, IPartition partition)
        {
            // node is not in the partiton -> no internal cost
            if (!partition.HasNode(node))
            {
                logger.Error("A node must be part of the partition to calculate internal cost.");
                throw new ArgumentException();
            }
            return ComputeCost(node, partition);
        }

        internal List<PartitioningNode> ComputeDValues(IPartition p1, IPartition p2)
        {
            List<PartitioningNode> nodes = new List<PartitioningNode>();
            // nodes in 
            foreach (var node in p1.Nodes)
            {
                nodes.Add(new PartitioningNode
                {
                    Node = node,
                    Partition = p1,
                    D = ComputeExternalCost(node, p2) - ComputeInternalCost(node, p1),
                });
            }

            foreach (var node in p2.Nodes)
            {
                nodes.Add(new PartitioningNode
                {
                    Node = node,
                    Partition = p2,
                    D = ComputeExternalCost(node, p1) - ComputeInternalCost(node, p2),
                });
            }

            return nodes;
        }

        internal List<PartitioningNodePair> MakePairs(IEnumerable<PartitioningNode> nodes)
        {
            List<PartitioningNodePair> pairs = new List<PartitioningNodePair>();
            foreach (var nodeOuter in nodes)
            {
                foreach (var nodeInner in nodes)
                {
                    // TODO: == might be sufficient, but an overridden Equals might be better
                    // do not add self-pairs and pairs from the same partition
                    if (nodeOuter == nodeInner || nodeOuter.Partition == nodeInner.Partition)
                    {
                        continue;
                    }

                    pairs.Add(new PartitioningNodePair(nodeOuter, nodeInner));
                }
            }

            return pairs;
        }

        internal void UpdateDValues(IEnumerable<PartitioningNode> unmarkedNodes, PartitioningNodePair selected)
        {
            var pNA = selected.NodeA.Partition;
            var pNB = selected.NodeB.Partition;

            UpdateDValueForNode(selected.NodeA, unmarkedNodes, pNA, pNB);
            UpdateDValueForNode(selected.NodeB, unmarkedNodes, pNB, pNA);
        }

        private void UpdateDValueForNode(PartitioningNode selectedNode, IEnumerable<PartitioningNode> unmarkedNodes, IPartition thisPartition, IPartition otherPartition)
        {
            foreach (var edge in selectedNode.Node.Edges)
            {
                var otherNode = edge.GetOtherNode(selectedNode.Node);
                // get the partitioning node for the otherNode
                var pNode = unmarkedNodes.FirstOrDefault(n => n.Node.Equals(otherNode));
                if (pNode == null)
                {
                    continue;
                }

                // otherNode is in the same partition -> this increases (worsens) the D value of the node
                if (thisPartition.Nodes.Any(n => n.Equals(pNode.Node)))
                {
                    // 2 * w(a', a)
                    pNode.D += 2 * 1;
                }
                else if (otherPartition.Nodes.Any(n => n.Equals(pNode.Node)))
                {
                    pNode.D -= 2 * 1;
                }
            }
        }

        internal int ComputeCost(INode node, IPartition partition)
        {
            // self-loops dont count toward the cost.
            return node.Edges.Count(e => !e.GetOtherNode(node).Equals(node) && partition.HasNode(e.GetOtherNode(node)));
        }

        internal PartitioningNodePair SelectMaxGain(IEnumerable<PartitioningNode> nodes)
        {
            var pairs = MakePairs(nodes);
            var maxPair = pairs.FirstOrDefault();
            if (maxPair == null)
            {
                logger.Trace("No pairs found: every node is in the same partition. No more refinement required.");
                return null;
            }

            foreach (var item in pairs)
            {
                if (maxPair.Gain < item.Gain)
                {
                    maxPair = item;
                }
            }

            return maxPair;
        }

        internal IEnumerable<PartitioningNodePair> PickPairsToSwap(IEnumerable<PartitioningNodePair> pairs, out int sumGain)
        {
            Dictionary<int, int> sums = new Dictionary<int, int>();
            int i = 0;
            int sum = 0;
            foreach (var pair in pairs)
            {
                sum += pair.Gain;
                sums[i] = sum;
            }

            int maxIndex = sums.Keys.Max();
            sumGain = sums[maxIndex];
            return pairs.Take(maxIndex + 1);
        }

        internal void SwapPairs(IEnumerable<PartitioningNodePair> pairs)
        {
            foreach (var pair in pairs)
            {
                if (pair.NodeA.Partition == pair.NodeB.Partition)
                {
                    logger.Error("Both nodes are in the same partition.");
                    throw new InvalidOperationException("Both nodes of the pair is in the same parition.");
                }

                // get partition one and remove node
                var p1 = pair.NodeA.Partition;
                p1.Nodes.Remove(pair.NodeA.Node);

                // get partition two and remove node
                var p2 = pair.NodeB.Partition;
                p2.Nodes.Remove(pair.NodeB.Node);

                // re-add nodes to the other partitions.
                p1.Add(pair.NodeB.Node);
                p2.Nodes.Add(pair.NodeA.Node);

                logger.Trace("Swapped {0} and {1} nodes between {2} and {3} partitions.", pair.NodeA.Node, pair.NodeB.Node, p1, p2);
            }
        }

        internal IEnumerable<Tuple<IPartition, IPartition>> MakePairsOfPartitions(List<IPartition> partitions)
        {
            List<Tuple<IPartition, IPartition>> pairs = new List<Tuple<IPartition, IPartition>>();

            for (int i = 0; i < partitions.Count; i++)
            {
                for (int j = i + 1; j < partitions.Count; j++)
                {
                    pairs.Add(Tuple.Create(partitions[i], partitions[j]));
                }
            }

            return PickN((int)(refinePercentage * pairs.Count), pairs);
        }

        internal IEnumerable<Tuple<IPartition, IPartition>> PickN(int n, List<Tuple<IPartition, IPartition>> pairs)
        {
            Random rnd = new Random(this.Seed);
            List<Tuple<IPartition, IPartition>> selected = new List<Tuple<IPartition, IPartition>>();

            while (n > 0)
            {
                int i = rnd.Next(pairs.Count);
                selected.Add(pairs[i]);
                pairs.RemoveAt(i);
                --n;
            }

            return selected;
        }
    }
}
