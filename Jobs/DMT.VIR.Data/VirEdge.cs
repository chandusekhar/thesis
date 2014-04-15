using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DMT.Core.Entities;
using DMT.Core.Interfaces;
using DMT.Core.Interfaces.Serialization;
using DMT.Matcher.Data.Interfaces;

namespace DMT.VIR.Data
{
    public class VirEdge : Edge, IMatchEdge
    {
        private IId partitionId;

        public bool IsRemote
        {
            get { return this.partitionId != null; }
        }

        public IId RemotePartitionId
        {
            get { return this.partitionId; }
            set { this.partitionId = value; }
        }

        public VirEdge(INode nodeA, INode nodeB, EdgeDirection direction, IEntityFactory factory)
            : base(nodeA, nodeB, direction, factory)
        {

        }

        public override void Serialize(XmlWriter writer)
        {
            base.Serialize(writer);

            writer.WriteStartElement("RemotePartitionId");
            if (IsRemote)
            {
                this.RemotePartitionId.Serialize(writer);
            }
            writer.WriteEndElement();
        }

        public override void Deserialize(XmlReader reader, IContext context)
        {
            base.Deserialize(reader, context);

            if (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.Name != "RemotePartitionId") { reader.ReadToFollowing("RemotePartitionId"); }
                if (!reader.IsEmptyElement)
                {
                    this.RemotePartitionId = context.EntityFactory.CreateId();
                    this.RemotePartitionId.Deserialize(reader, context);
                }
            }

        }
    }
}
