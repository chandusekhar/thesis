using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DMT.Core.Interfaces;
using DMT.Core.Interfaces.Serialization;
using NLog;

namespace DMT.Core.Entities
{
    internal sealed class DMTId : IId
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static readonly DMTId Empty = new DMTId(Guid.Empty);
        public const string IdTagName = "Id";
        
        private Guid value;

        public static DMTId NewId()
        {
            return new DMTId(Guid.NewGuid());
        }

        public static DMTId FromGuid(Guid guid)
        {
            return new DMTId(guid);
        }

        public static IId FromString(string idStr)
        {
            return new DMTId(Guid.Parse(idStr));
        }

        public DMTId()
        {
            this.value = Guid.Empty;
        }

        private DMTId(Guid value)
        {
            this.value = value;
        }

        public bool Equals(IId other)
        {
            if (other == null)
            {
                return false;
            }

            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            if (!(other is DMTId))
            {
                logger.Error("Comparing incompatible IId implementations. {0} and {1}", typeof(DMTId), other.GetType());
                return false;
            }

            DMTId other2 = (DMTId)other;

            return this.value.Equals(other2.value);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as IId);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("DMT.Core.Id [{0}]", value);
        }

        #region ISerializable

        void ISerializable.Serialize(XmlWriter writer)
        {
            logger.Trace("Id [{0}] was written to xml", this.value);
            writer.WriteValue(this.value.ToString());
        }

        void ISerializable.Deserialize(XmlReader reader, IContext context)
        {
            string idString = reader.ReadElementContentAsString();
            logger.Trace("Id [{0}] was read from xml.", idString);
            this.value = Guid.Parse(idString);
        }
        #endregion

    }
}
