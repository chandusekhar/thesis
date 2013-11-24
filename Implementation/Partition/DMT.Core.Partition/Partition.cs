using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DMT.Core.Interfaces;
using DMT.Core.Interfaces.Serialization;
using DMT.Partition.Interfaces;

namespace DMT.Core.Partition
{
    internal class Partition : IPartition
    {
        private IId id;
        private List<INode> nodes;

        public ICollection<INode> Nodes
        {
            get { return nodes; }
        }

        public IHost Host { get; set; }

        public IId Id
        {
            get { return id; }
        }

        public Partition(IEntityFactory factory)
        {
            this.id = factory.CreateId();
            this.nodes = new List<INode>();
        }

        public Task<ISendPartitionResponse> SendToHost()
        {
            throw new NotImplementedException();
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
            List<INode> newNodes = new List<INode>();
            ISuperNode sn;

            foreach (var node in nodes)
            {
                sn = node as ISuperNode;

                if (sn != null)
                {
                    newNodes.AddRange(sn.Nodes);
                }
                else
                {
                    newNodes.Add(node);
                }
            }

            this.nodes = newNodes;
        }
    }
}
