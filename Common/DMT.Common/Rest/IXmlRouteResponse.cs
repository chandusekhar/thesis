using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Common.Rest
{
    public interface IXmlRouteResponse
    {
        bool Success { get; set; }
        string ErrorMessage { get; set; }
    }
}
