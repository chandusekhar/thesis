using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using DMT.Core.Serialization;
using DMT.Core.Interfaces;
using NLog;

namespace DMT.Core
{
    public abstract class Entity : IIdentity
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private Id _id;

        public Entity()
        {
            _id = Core.Id.NewId();
        }

        public IId Id
        {
            get { return _id; }
        }

        #region ISerializable members

        public virtual void Serialize(XmlWriter writer)
        {
            writer.WriteStartElement(CoreConstants.IdTagName);
            ((ISerializable)_id).Serialize(writer);
            writer.WriteEndElement();
            logger.Trace("Written entity's id to xml. [{0}]", _id);
        }

        public virtual void Deserialize(XmlReader reader, IContext context)
        {
            // id
            if (!reader.ReadToFollowing(CoreConstants.IdTagName))
            {
                logger.Error("No id for entity.");
            }
            ((ISerializable)_id).Deserialize(reader, context);
            logger.Trace("Read entity's id from xml. [{0}]", _id);
        } 

        #endregion
    }
}
