using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Matcher.Module.Exceptions
{
    [Serializable]
    public class JobAlreadyStartedException : Exception
    {
        public JobAlreadyStartedException() { }
        public JobAlreadyStartedException(string message) : base(message) { }
        public JobAlreadyStartedException(string message, Exception inner) : base(message, inner) { }
        protected JobAlreadyStartedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
