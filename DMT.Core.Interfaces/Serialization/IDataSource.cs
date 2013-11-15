using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Core.Interfaces.Serialization
{
    /// <summary>
    /// Interface for loading nodes from raw source.
    /// </summary>
    public interface IDataSource
    {
        /// <summary>
        /// Loads the model from the given stream.
        ///
        /// The disposal of the stream is the responsibility of the caller.
        /// </summary>
        /// <param name="source">The source stream of the data</param>
        /// <returns>The nodes of the graph components as a collection.</returns>
        Task<IModel> LoadModelAsync(Stream source);

        /// <summary>
        /// Saves the model to a stream.
        ///
        /// The disposal of the stream is the responsibility of the caller.
        /// </summary>
        Task SaveModelAsync(Stream stream, IModel model);
    }
}
