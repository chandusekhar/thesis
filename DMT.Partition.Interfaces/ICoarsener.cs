using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.Partition.Interfaces
{
    /// <summary>
    /// Graph coarsener interface. It supports coarsening and uncoarsening. This is a
    /// high-level interface. It hides all the details behind the (un)coarsening.
    ///
    /// Graph coarsening is the first step of the partitioning process. It's basically
    /// edge contraction. The goal is to indentify and merge suitable vertices (nodes)
    /// in the graph. The coarsening produces a new graph with lesser vertices.
    /// </summary>
    public interface ICoarsener
    {
        /// <summary>
        /// Gets or sets the factor of coarsening.
        ///
        /// The factor shows how big the coearsend graph should be compared to
        /// the original graph. So ideally the number_of_node * factor = coarsening_steps.
        /// </summary>
        double Factor { get; set; }

        /// <summary>
        /// Coarsen a graph. It produces a (final) coarsening that tries to match the <c>Factor</c>
        /// as much as possible. In its implementation it might do more than one step of
        /// coarsening.
        /// </summary>
        /// <param name="nodes">the list of nodes in the graph</param>
        /// <returns></returns>
        IEnumerable<ISuperNode> Coarsen(IEnumerable<INode> nodes);

        /// <summary>
        /// Uncoarsen a corsened graph. It restores the original graph by transforming the list of
        /// nodes held by the partition objects. Optionally it supports refining the partitions on
        /// every uncoarsening step. The number of steps is implementetion dependent.
        ///
        /// The uncoarsening stops when every node in every partition is resolved to the original
        /// single node.
        /// </summary>
        /// <param name="partitions">The partitions which contain the nodes of the graph.</param>
        /// <param name="refiner">The refiner object.</param>
        void Uncoarsen(IEnumerable<IPartition> partitions, IPartitionRefiner refiner);
    }
}
