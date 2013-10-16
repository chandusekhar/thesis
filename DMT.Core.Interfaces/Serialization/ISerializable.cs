using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DMT.Core.Serialization
{
    /// <summary>
    /// Generic serializable interface. It provides context for the deserialization.
    /// </summary>
    public interface ISerializable
    {
        /// <summary>
        /// Serialize the inner state of the object.
        /// </summary>
        /// <param name="writer">the target xml writer</param>
        void Serialize(XmlWriter writer);

        /// <summary>
        /// Deserialize the inner state from xml into the object.
        /// </summary>
        /// <param name="reader">(xml)reader to read from</param>
        /// <param name="context">context</param>
        void Deserialize(XmlReader reader, IContext context);
    }
}
