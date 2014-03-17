using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using DMT.Core.Interfaces;
using DMT.Core.Interfaces.Exceptions;
using DMT.Core.Interfaces.Serialization;
using NLog;

namespace DMT.Core.Serialization
{
    /// <summary>
    /// Basic xml implementation of the model loader/saver. 
    /// 
    /// It only loads only the a stub of nodes and edges.
    /// Loads the node's and edge's id-s, and also it builds the connection between
    /// 
    /// </summary>
    [Export(typeof(IDataSource))]
    public class XmlDataSource : IDataSource
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private IModelXmlSerializer serializer;

        [ImportingConstructor]
        public XmlDataSource(IModelXmlSerializer serializer)
        {
            this.serializer = serializer;
        }

        public void SaveModel(Stream stream, IModel model)
        {
            logger.Debug("Model saving started");

            using (var writer = XmlWriter.Create(stream))
            {
                serializer.Serialize(writer, model);
            }

            logger.Debug("Model saving done.");
        }

        public IModel LoadModel(Stream stream)
        {
            logger.Debug("Started loading model.");
            IModel model;

            using (var reader = XmlReader.Create(stream))
            {
                model = serializer.Deserialize(reader);
            }

            logger.Debug("Finished loading model.");
            return model;
        }
    }
}
