using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.Partition.Interfaces
{
    /// <summary>
    /// General interface for graph partitions.
    /// </summary>
    public interface IPartition : IIdentifiable
    {
        /// <summary>
        /// Gets the collection of nodes in the partition.
        /// </summary>
        ICollection<INode> Nodes { get; }

        /// <summary>
        /// Determines whether the partition contains any nodes or not.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Gets or sets the host machine descriptor of this partition.
        /// </summary>
        IHost Host { get; set; }

        /// <summary>
        /// Inflate partition by swapping SuperNodes with their children.
        /// </summary>
        void Inflate();

        /// <summary>
        /// Determines whether the partition has the node or not.
        /// </summary>
        /// <param name="node">The node to check for.</param>
        /// <returns>true only if the partition has that particular node, false otherwise</returns>
        bool HasNode(INode node);

        /// <summary>
        /// Gets the edges which connect to a node which is not in this partition.
        /// </summary>
        /// <returns>List of edges.</returns>
        IEnumerable<IEdge> GetExternalEdges();

        /// <summary>
        /// Gets the edges between two paritions.
        /// 
        /// <para>It only takes outbound edges into account.</para>
        /// </summary>
        /// <param name="other">the other partition to search in</param>
        /// <returns>a collection of edges</returns>
        /// <exception cref="ArgumentNullException"></exception>
        IEnumerable<IEdge> GetEdgesBetween(IPartition other);

        /// <summary>
        /// Add a node to the partition.
        /// </summary>
        /// <param name="node"></param>
        void Add(INode node);
    }
}
