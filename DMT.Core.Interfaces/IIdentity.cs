using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DMT.Core.Interfaces
{
    public interface IIdentity: IXmlSerializable
    {
        /// <summary>
        /// Gets the unique identifier for the entity.
        /// </summary>
        IId Id { get; }
    }
}
