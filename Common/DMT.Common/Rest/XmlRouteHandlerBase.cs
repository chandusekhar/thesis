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
    public abstract class XmlRouteHandlerBase<TRequest, TResponse> : IRouteHandler where TResponse : IXmlRouteResponse
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected virtual bool HasResponseBody { get { return true; } }

        void IRouteHandler.Handle(Request request, Response response)
        {
            TResponse res = Activator.CreateInstance<TResponse>();

            try
            {
                if (request.HasBody)
                {
                    var requestBodySerializer = new XmlSerializer(typeof(TRequest));
                    var req = (TRequest)requestBodySerializer.Deserialize(request.Body);
                    res = Handle(req, request.Params);
                }
                else
                {
                    res = Handle(request.Params);
                }

            }
            catch (Exception ex)
            {
                response.Status = HttpStatusCode.InternalServerError;
                res.ErrorMessage = ex.Message;
                res.Success = false;
            }

            using (response.Body)
            {
                if (this.HasResponseBody)
                {
                    var responseBodySerializer = new XmlSerializer(typeof(TResponse));
                    responseBodySerializer.Serialize(response.Body, res);
                    response.ContentType = Response.XmlUtf8ContentType;
                }
            }
        }

        protected virtual TResponse Handle(TRequest request, NameValueCollection urlParams)
        {
            return default(TResponse);
        }

        protected virtual TResponse Handle(NameValueCollection urlParams)
        {
            return default(TResponse);
        }
    }
}
