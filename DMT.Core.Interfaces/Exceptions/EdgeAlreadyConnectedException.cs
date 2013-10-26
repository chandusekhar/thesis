using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Core.Interfaces.Exceptions
{
    [Serializable]
    public class EdgeAlreadyConnectedException : Exception
    {
        public EdgeAlreadyConnectedException() { }
        public EdgeAlreadyConnectedException(string message) : base(message) { }
        public EdgeAlreadyConnectedException(string message, Exception inner) : base(message, inner) { }
        protected EdgeAlreadyConnectedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
