using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using DMT.Core.Interfaces;
using DMT.Core.Interfaces.Exceptions;
using DMT.Core.Interfaces.Serialization;
using DMT.Partition.Interfaces;

namespace DMT.Module.Common
{
    public static class PartitionSerializerTags
    {
        public const string CrossingEdgesTag = "CrossingEdges";
        public const string CrossingEdgeTag = "CrossingEdge";
        public const string IdTag = "Id";
        public const string PartitionIdTag = "PartitionId";
        public const string PartitionTag = "Partition";
        public const string ModelTag = "Model";
    }
}
