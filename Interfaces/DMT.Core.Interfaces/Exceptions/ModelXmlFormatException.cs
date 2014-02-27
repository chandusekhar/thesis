using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Core.Interfaces.Exceptions
{
    [Serializable]
    public class ModelXmlFormatException : Exception
    {
        public ModelXmlFormatException() { }
        public ModelXmlFormatException(string message) : base(message) { }
        public ModelXmlFormatException(string message, Exception inner) : base(message, inner) { }
        protected ModelXmlFormatException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }    
}
