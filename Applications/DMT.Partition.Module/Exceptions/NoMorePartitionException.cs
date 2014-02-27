using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Partition.Module.Exceptions
{
    [Serializable]
    public class NoMorePartitionException : Exception
    {
        public NoMorePartitionException() { }
        public NoMorePartitionException(string message) : base(message) { }
        public NoMorePartitionException(string message, Exception inner) : base(message, inner) { }
        protected NoMorePartitionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
