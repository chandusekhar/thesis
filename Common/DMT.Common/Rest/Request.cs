using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Common.Rest
{
    public class Request
    {
        private HttpListenerRequest innerRequest;
        private NameValueCollection _params;

        public bool HasBody { get { return this.innerRequest.HasEntityBody; } }

        public Stream Body { get { return this.innerRequest.InputStream; } }

        public NameValueCollection Params
        {
            get { return this._params; }
        }

        internal Request(HttpListenerRequest request, NameValueCollection routeParams)
        {
            this.innerRequest = request;
            _params = new NameValueCollection(request.QueryString);
            _params.Add(routeParams);
        }
    }
}
