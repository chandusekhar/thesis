using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Common.Composition;
using DMT.Core.Interfaces;
using DMT.Core.Interfaces.Serialization;

namespace DMT.Partition.Module
{
    class ModelLoader : InjectableBase
    {
        [Import]
        private IDataSource dataSource;

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
            return dataSource.LoadModel(new FileStream(this.filePath, FileMode.Open));
        }
    }
}
