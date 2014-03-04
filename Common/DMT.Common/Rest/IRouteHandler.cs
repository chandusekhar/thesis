using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Common.Rest
{
    public interface IRouteHandler
    {
        void Handle(HttpListenerRequest request, HttpListenerResponse response);
    }
}
