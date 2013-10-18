using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Core.Exceptions
{
    [Serializable]
    public class EdgeNotYetConnectedException : Exception
    {
        public EdgeNotYetConnectedException() { }
        public EdgeNotYetConnectedException(string message) : base(message) { }
        public EdgeNotYetConnectedException(string message, Exception inner) : base(message, inner) { }
        protected EdgeNotYetConnectedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
