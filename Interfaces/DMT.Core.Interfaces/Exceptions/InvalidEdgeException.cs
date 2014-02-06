using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Core.Interfaces.Exceptions
{
    [Serializable]
    public class InvalidEdgeException : Exception
    {
        public InvalidEdgeException() { }
        public InvalidEdgeException(string message) : base(message) { }
        public InvalidEdgeException(string message, Exception inner) : base(message, inner) { }
        protected InvalidEdgeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
