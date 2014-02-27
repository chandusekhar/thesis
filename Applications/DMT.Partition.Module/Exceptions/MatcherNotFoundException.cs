using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Partition.Module.Exceptions
{
    [Serializable]
    public class MatcherNotFoundException : Exception
    {
        public MatcherNotFoundException() { }
        public MatcherNotFoundException(string message) : base(message) { }
        public MatcherNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected MatcherNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
