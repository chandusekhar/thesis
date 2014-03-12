using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Matcher.Module.Exceptions
{
    [Serializable]
    public class NoMatcherJobException : Exception
    {
        public NoMatcherJobException() { }
        public NoMatcherJobException(string message) : base(message) { }
        public NoMatcherJobException(string message, Exception inner) : base(message, inner) { }
        protected NoMatcherJobException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
