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

        public void Refine(IPartition p1, IPartition p2)
        {
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

        public void Refine(IEnumerable<IPartition> partitions)
        {
            throw new NotImplementedException();
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

        internal void UpdateDValues(IEnumerable<PartitioningNode> remainingNodes, PartitioningNodePair selected)
        {
            /**
             * csak bejövő éleket nézni
             * ha azonos partícióban van, akkor csere után D+1 lenne
             * ha különböző partícióban van, akkor csere után D-1 lenne
             */

            var pNA = selected.NodeA.Partition;
            var pNB = selected.NodeB.Partition;

            foreach (var edge in selected.NodeA.Node.InboundEdges)
            {
                var node = remainingNodes.FirstOrDefault(n => n.Node.Equals(edge.Source));

                // in the same partition => D += 1
                if (pNA.Nodes.Any(n => n.Equals(edge.Source)))
                {
                    node.D += 1;
                }
                else if (pNB.Nodes.Any(n => n.Equals(edge.Source)))
                {
                    node.D -= 1;
                }
            }

            foreach (var edge in selected.NodeB.Node.InboundEdges)
            {
                var node = remainingNodes.FirstOrDefault(n => n.Node.Equals(edge.Source));

                // in the same partition => D += 1
                if (pNB.Nodes.Any(n => n.Equals(edge.Source)))
                {
                    node.D += 1;
                }
                else if (pNA.Nodes.Any(n => n.Equals(edge.Source)))
                {
                    node.D -= 1;
                }
            }

        }

        internal int ComputeCost(INode node, IPartition partition)
        {
            // self-loops dont count toward the cost.
            return node.OutboundEdges.Count(e => !e.Target.Equals(node) && partition.HasNode(e.Target));
        }

        internal PartitioningNodePair SelectMaxGain(IEnumerable<PartitioningNode> nodes)
        {
            var pairs = MakePairs(nodes);
            var maxPair = pairs.FirstOrDefault();
            if (maxPair == null)
            {
                logger.Warn("No pairs left, this should not happen!");
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
            List<PartitioningNodePair> selected = new List<PartitioningNodePair>();
            sumGain = 0;
            foreach (var item in pairs)
            {
                if (item.Gain < 0)
                {
                    break;
                }

                selected.Add(item);
                sumGain += item.Gain;
            }

            return selected;
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
            }
        }
    }
}
