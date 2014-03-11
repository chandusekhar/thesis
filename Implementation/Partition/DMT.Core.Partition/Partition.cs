using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DMT.Core.Interfaces;
using DMT.Core.Interfaces.Serialization;
using DMT.Partition.Interfaces;
using DMT.Common.Extensions;

namespace DMT.Core.Partition
{
    internal class Partition : IPartition
    {
        private IId id;
        private HashSet<INode> nodes;

        public ICollection<INode> Nodes
        {
            get { return nodes; }
        }

        public IId Id
        {
            get { return id; }
        }

        public bool IsEmpty
        {
            get { return !this.nodes.Any(); }
        }

        public Partition(IEntityFactory factory)
        {
            this.id = factory.CreateId();
            this.nodes = new HashSet<INode>();
        }

        public void Serialize(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(XmlReader reader, IContext context)
        {
            throw new NotImplementedException();
        }

        public void Inflate()
        {
            HashSet<INode> newNodes = new HashSet<INode>();
            ISuperNode sn;

            foreach (var node in nodes)
            {
                sn = node as ISuperNode;

                if (sn != null)
                {
                    newNodes.AddAll(sn.Nodes);
                }
                else
                {
                    newNodes.Add(node);
                }
            }

            this.nodes = newNodes;
        }

        public IEnumerable<IEdge> GetExternalEdges()
        {
            HashSet<IEdge> external = new HashSet<IEdge>(); ;

            foreach (var node in this.nodes)
            {
                foreach (var edge in node.Edges)
                {
                    if (!this.HasNode(edge.GetOtherNode(node)))
                    {
                        external.Add(edge);
                    }
                }
            }

            return external;
        }

        public bool HasNode(INode node)
        {
            if (node == null)
            {
                return false;
            }

            return this.nodes.Contains(node);
        }

        public IEnumerable<IEdge> GetEdgesBetween(IPartition other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            // shortcut for empty partitions and the same partition checking
            if (this.Equals(other) || this.IsEmpty || other.IsEmpty)
            {
                return new IEdge[0];
            }

            List<IEdge> edges = new List<IEdge>();
            foreach (IEdge edge in this.GetExternalEdges())
            {
                if (other.HasNode(edge.EndB))
                {
                    edges.Add(edge);
                }
            }

            return edges;
        }

        public void Add(INode node)
        {
            this.nodes.Add(node);
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}]", this.GetType().Name, this.id);
        }

        public IEnumerable<IEdge> CollectEdges()
        {
            HashSet<IEdge> edges = new HashSet<IEdge>();

            foreach (var node in this.nodes)
            {
                foreach (var edge in node.Edges)
                {
                    edges.Add(edge);
                }
            }

            return edges;
        }
    }
}
