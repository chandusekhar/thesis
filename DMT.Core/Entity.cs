using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
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

        #region IXmlSerializable members

        public XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(XmlReader reader)
        {
            // id
            if (!reader.ReadToFollowing(CoreConstants.IdTagName))
            {
                logger.Error("No id for entity.");
            }
            ((IXmlSerializable)_id).ReadXml(reader);
            logger.Trace("Read entity's id from xml. [{0}]", _id);
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(CoreConstants.IdTagName);
            ((IXmlSerializable)_id).WriteXml(writer);
            writer.WriteEndElement();
            logger.Trace("Written entity's id to xml. [{0}]", _id);
        }
 
        #endregion
    }
}
