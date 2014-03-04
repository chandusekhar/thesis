using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Common.Rest
{
    public abstract class XmlRouteResponseBase<T> : IXmlRouteResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public abstract T Result { get; set; }
    }
}
