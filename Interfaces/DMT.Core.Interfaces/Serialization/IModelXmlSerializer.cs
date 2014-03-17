using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DMT.Core.Interfaces.Serialization
{
    /// <summary>
    /// Model serializer
    /// 
    /// We have to be able to serialize the model into xml, because the modules
    /// use that format.
    /// </summary>
    public interface IModelXmlSerializer
    {
        /// <summary>
        /// Serialize the whole model.
        /// </summary>
        /// <param name="writer">destination</param>
        /// <param name="model">model to serialize</param>
        void Serialize(XmlWriter writer, IModel model);

        /// <summary>
        /// Deserialize the model
        /// </summary>
        /// <param name="reader">source</param>
        /// <returns>the model</returns>
        IModel Deserialize(XmlReader reader);

        /// <summary>
        /// Copy some nodes to the destination from the source.
        /// </summary>
        /// <param name="source">the xml source</param>
        /// <param name="destination">destination writer</param>
        /// <param name="nodeIdSet">set of node ids to copy</param>
        void CopyNodes(XmlReader source, XmlWriter destination, HashSet<IId> nodeIdSet);

        /// <summary>
        /// Copy some edges to destination.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="edgeIdSet">set of edge ids to copy</param>
        void CopyEdges(XmlReader source, XmlWriter destination, HashSet<IId> edgeIdSet);
    }
}
