using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Partition.Interfaces;

namespace DMT.Module.Common
{
    /// <summary>
    /// IPartitionSerializer defines the interface for a serializer that
    /// serializes a partition for sending it over to a matcher module.
    /// 
    /// The serialized partition should at least contain the following:
    /// - edges that crossing the partition border (and the id of the partition which they lead to)
    /// - the nodes from the data source
    /// - the edges from the data source
    /// </summary>
    public interface IPartitionSerializer
    {
        /// <summary>
        /// Serializes the partition into a stream.
        /// </summary>
        /// <param name="partition">the partition to be serialized</param>
        /// <param name="source">the model source</param>
        /// <param name="destination">stream to writer to</param>
        void Serialize(IPartition partition, XmlReader source, Stream destination);
    }
}
