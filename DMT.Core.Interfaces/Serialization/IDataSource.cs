using System;
using System.Collections.Generic;
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
        /// Loads the model.
        /// </summary>
        /// <returns>The root nodes of the graph components as a collection.</returns>
        Task<IModel> LoadModelAsync();

        /// <summary>
        /// Saves the model.
        /// </summary>
        Task SaveModelAsync(IModel model);
    }
}
