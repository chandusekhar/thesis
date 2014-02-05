using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Core.Interfaces.Exceptions
{
    [Serializable]
    public class InvalidNodeException : Exception
    {
        public InvalidNodeException() { }
        public InvalidNodeException(string message) : base(message) { }
        public InvalidNodeException(string message, Exception inner) : base(message, inner) { }
        public InvalidNodeException(string format, params object[] args) : base(string.Format(format, args)) { }
        public InvalidNodeException(INode node) : base(string.Format("Node (id: {0}) is invalid", node.Id)) { }

        protected InvalidNodeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

    }
}
