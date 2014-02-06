using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Common;
using DMT.Core.Interfaces;
using DMT.Core.Interfaces.Exceptions;
using DMT.Core.Interfaces.Results;

namespace DMT.Core.Entities
{
    public class Node : Entity, INode
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected List<IEdge> edges;

        public IEnumerable<IEdge> Edges
        {
            get { return edges; }
        }

        public int Degree
        {
            get { return this.edges.Count; }
        }

        public Node(IEntityFactory factory)
            : base(factory)
        {
            this.edges = new List<IEdge>();
        }

        public override bool Remove()
        {
            if (!this.edges.Any())
            {
                // return false if there are no edges
                return false;
            }

            foreach (var edge in this.edges)
            {
                this.Disconnect(edge);
            }
            this.edges.Clear();

            return true;
        }

        public IEdge ConnectTo(INode otherNode, EdgeDirection direction)
        {
            Objects.RequireNonNull(otherNode);
            var edge = factory.CreateEdge(this, otherNode, direction);
            return this.ConnectTo(otherNode, edge);
        }

        public IEdge ConnectTo(INode otherNode, IEdge edge)
        {
            if (edge.GetOtherNode(this) != otherNode)
            {
                logger.Error("Other node is not valid when connecting to it.");
                throw new InvalidNodeException(otherNode);
            }

            // add edge to nodes
            this.AddEdge(edge);
            otherNode.AddEdge(edge);

            return edge;
        }

        public IEnumerable<INode> GetAdjacentNodes()
        {
            List<INode> neighbours = new List<INode>();
            foreach (var edge in this.edges)
            {
                neighbours.Add(edge.GetOtherNode(this));
            }
            return neighbours;
        }

        public bool Disconnect(IEdge edge)
        {
            bool success = this.RemoveEdge(edge);
            success = success && edge.GetOtherNode(this).RemoveEdge(edge);

            return success;
        }

        public void AddEdge(IEdge edge)
        {
            this.edges.Add(edge);
        }

        public bool RemoveEdge(IEdge edge)
        {
            return this.edges.Remove(edge);
        }

        public NeighbourResult IsNeighbour(INode node)
        {
            foreach (var edge in this.edges)
            {
                if (edge.GetOtherNode(this).Equals(node))
                {
                    return new NeighbourResult(true, edge);
                }
            }
            return new NeighbourResult(false, null);
        }
    }
}
