using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.Core.Exceptions
{
    [Serializable]
    public class NodeMissingException : Exception
    {
        private IId id;

        public NodeMissingException(IId id)
            : base(string.Format("Node with {0} id is missing", id))
        {
            this.id = id;
        }

        protected NodeMissingException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
