using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.Matcher.Module.Partitioner
{
    class CrossingEdge
    {
        public IId Id { get; private set; }
        public IId PartitionId { get; private set; }

        public CrossingEdge(IId id, IId partitionId)
        {
            this.Id = id;
            this.PartitionId = partitionId;
        }
    }
}
