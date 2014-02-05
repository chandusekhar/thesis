using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using DMT.Core.Interfaces.Serialization;
using DMT.Core.Interfaces;
using NLog;

namespace DMT.Core.Entities
{
    public abstract class Entity : IEntity
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private IId _id;
        protected IEntityFactory factory;

        public IId Id
        {
            get { return _id; }
        }

        public Entity(IEntityFactory factory)
        {
            this.factory = factory;
            _id = factory.CreateId();
        }

        public abstract bool Remove();

        #region ISerializable members

        public virtual void Serialize(XmlWriter writer)
        {
            writer.WriteStartElement(DMTId.IdTagName);
            ((ISerializable)_id).Serialize(writer);
            writer.WriteEndElement();
            logger.Trace("Written {0}'s id to xml. [{1}]", this.GetType().Name, _id);
        }

        public virtual void Deserialize(XmlReader reader, IContext context)
        {
            // id
            if (!reader.ReadToFollowing(DMTId.IdTagName))
            {
                logger.Error("No id for entity.");
            }
            ((ISerializable)_id).Deserialize(reader, context);
            logger.Trace("Read {0}'s id from xml. [{1}]", this.GetType().Name, _id);
        } 

        #endregion

        public override string ToString()
        {
            return string.Format("{0} [{1}]", this.GetType().FullName, this.Id);
        }

        public bool Equals(IEntity other)
        {
            if (other == null)
            {
                return false;
            }
            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            Entity e = obj as Entity;
            if (e == null)
            {
                return false;
            }
            return this.Equals(e);
        }

        public override int GetHashCode()
        {
            return 17 * _id.GetHashCode();
        }
    }
}
