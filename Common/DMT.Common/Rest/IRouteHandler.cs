using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Common.Rest
{
    public interface IRouteHandler
    {
        void Handle(Request request, Response response);
    }
}
