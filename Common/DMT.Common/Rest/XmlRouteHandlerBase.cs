using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DMT.Common.Rest
{
    public abstract class XmlRouteHandlerBase<TRequest, TResponse> : IRouteHandler
    {
        void IRouteHandler.Handle(HttpListenerRequest request, HttpListenerResponse response)
        {
            TResponse res;
            
            if (request.HasEntityBody)
            {
                var requestBodySerializer = new XmlSerializer(typeof(TRequest));
                var req = (TRequest)requestBodySerializer.Deserialize(request.InputStream);
                res = Handle(req, request.QueryString);
            }
            else
            {
                res = Handle(request.QueryString);
            }

            using (response.OutputStream)
            {
                if (res != null)
                {
                    var responseBodySerializer = new XmlSerializer(typeof(TResponse));
                    responseBodySerializer.Serialize(response.OutputStream, res);
                }
            }
        }

        protected abstract TResponse Handle(NameValueCollection urlParams);

        protected abstract TResponse Handle(TRequest request, NameValueCollection urlParams);
    }
}
