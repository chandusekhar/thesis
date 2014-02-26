using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DMT.Partition.Module.Remote.Service
{
    public class PartitionResponse : ResponseBase
    {
        public PartitionResponse() { }
        public PartitionResponse(bool success) : base(success) { }
    }
}
