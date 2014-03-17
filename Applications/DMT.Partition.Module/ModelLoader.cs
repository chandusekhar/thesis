using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DMT.Common.Composition;
using DMT.Core.Interfaces;
using DMT.Core.Interfaces.Serialization;

namespace DMT.Partition.Module
{
    class ModelLoader : InjectableBase
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        [Import]
        private IModelXmlSerializer serializer;

        private string filePath;

        /// <summary>
        /// Creates a new model loader
        /// </summary>
        /// <param name="filePath">Path to the file, that contains the model.</param>
        public ModelLoader(string filePath)
        {
            this.filePath = filePath;
        }

        public IModel LoadModel()
        {
            logger.Debug("Started loading model");

            IModel model;
            using (XmlReader reader = XmlReader.Create(this.filePath))
            {
                model = serializer.Deserialize(reader);
            }

            logger.Debug("Finished loading model.");
            return model;
        }
    }
}
