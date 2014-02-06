using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;
using DMT.Core.Interfaces.Exceptions;
using DMT.Partition.Interfaces;

namespace DMT.Partition
{
    internal class GraphContractor
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private IPartitionEntityFactory factory;
        private HashSet<INode> nodes;
        /// <summary>
        /// Storing the supernode for every 'source' node.
        /// </summary>
        private Dictionary<IId, ISuperNode> superNodeCache;

        public GraphContractor(IEnumerable<INode> nodes, IPartitionEntityFactory factory)
        {
            this.nodes = new HashSet<INode>(nodes);
            this.factory = factory;
        }

        /// <summary>
        /// Creates a new graph based on the original one, but contracts
        /// nodes pointed by the <c>edges</c> collection. Adjacency will be kept intact.
        /// </summary>
        /// <param name="edges">edges which endpoints will be contracted into one</param>
        /// <returns>A list of edges that is the new contracted graph</returns>
        public IEnumerable<ISuperNode> ContractEdges(IEnumerable<IEdge> edges)
        {
            // bust cache
            this.superNodeCache = new Dictionary<IId, ISuperNode>();
            var superNodes = CreateSuperNodes(edges);
            RebuildEdges(superNodes);
            return superNodes;
        }

        private List<ISuperNode> CreateSuperNodes(IEnumerable<IEdge> edges)
        {
            List<ISuperNode> newNodes = new List<ISuperNode>();

            ISuperNode node;
            foreach (var edge in edges)
            {
                // create a new supernode and add edges ---> basically contracting them into one
                node = this.factory.CreateSuperNode();
                node.Nodes.Add(edge.EndA);
                node.Nodes.Add(edge.EndB);

                try
                {
                    superNodeCache.Add(edge.EndA.Id, node);
                    superNodeCache.Add(edge.EndB.Id, node);
                }
                catch (ArgumentException ex)
                {
                    throw new InvalidEdgeException(string.Format("Edge {0} has an adjacent edge. In a matching it should not have one.", edge), ex);
                }
                
                newNodes.Add(node);

                // remove nodes so we can wrap the leftovers later
                logger.Trace("Removing EndA node ({0}) from graph: {1}", edge.EndA, this.nodes.Remove(edge.EndA));
                logger.Trace("Removing EndB node ({0}) from graph: {1}", edge.EndB, this.nodes.Remove(edge.EndB));
            }

            newNodes.AddRange(WrapLeftoverNodes(this.nodes));

            return newNodes;
        }

        private void RebuildEdges(List<ISuperNode> superNodes)
        {
            var edges = superNodes.SelectMany(sn => sn.Nodes).SelectMany(n => n.Edges).Distinct();
            foreach (var edge in edges)
            {
                ConnectByEdge(edge);
            }
        }

        private void ConnectByEdge(IEdge edge)
        {
            ISuperNode endANode = this.superNodeCache[edge.EndA.Id];
            ISuperNode endBNode = this.superNodeCache[edge.EndB.Id];

            if (endANode == endBNode)
            {
                // do not connect the same supernodes to themselves
                return;
            }

            // edges between supernodes are not duplicated. weight represent
            // number of edges 
            var result = endANode.IsNeighbour(endBNode);
            if (result.Success)
            {
                result.Edge.Weight += 1.0;
            }
            else
            {
                var newedge = endANode.ConnectTo(endBNode, EdgeDirection.Both);
                newedge.Weight = 1.0;
            }
        }

        private List<ISuperNode> WrapLeftoverNodes(IEnumerable<INode> leftovers)
        {
            var superNodes = new List<ISuperNode>();
            foreach (var node in leftovers)
            {
                var sn = this.factory.CreateSuperNode(node);
                superNodes.Add(sn);
                this.superNodeCache.Add(node.Id, sn); 
            }

            return superNodes;
        }
    }
}
