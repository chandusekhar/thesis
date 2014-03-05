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
    // TODO: would be greate to unify these classes
    public abstract class XmlRouteHandlerBase<TRequest, TResponse> : IRouteHandler where TResponse : IXmlRouteResponse
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        void IRouteHandler.Handle(Request request, Response response)
        {
            TResponse res = Activator.CreateInstance<TResponse>();
            
            if (request.HasBody)
            {
                var requestBodySerializer = new XmlSerializer(typeof(TRequest));
                var req = (TRequest)requestBodySerializer.Deserialize(request.Body);

                try
                {
                    res = Handle(req, request.Params);
                }
                catch (Exception ex)
                {
                    response.Status = HttpStatusCode.InternalServerError;
                    res.ErrorMessage = ex.Message;
                    res.Success = false;
                }
            }
            else
            {
                throw new InvalidOperationException("Request should have body.");
            }

            using (response.Body)
            {
                if (res != null)
                {
                    var responseBodySerializer = new XmlSerializer(typeof(TResponse));
                    responseBodySerializer.Serialize(response.Body, res);
                }
            }
        }

        protected abstract TResponse Handle(TRequest request, NameValueCollection urlParams);
    }

    public abstract class XmlResponseOnlyRouteHandlerBase<TResponse> : IRouteHandler where TResponse : IXmlRouteResponse
    {
        protected abstract TResponse Handle(NameValueCollection urlParams);

        void IRouteHandler.Handle(Request request, Response response)
        {
            TResponse res = Activator.CreateInstance<TResponse>();

            try {
                res = Handle(request.Params);
            }
            catch (Exception ex)
            {
                response.Status = HttpStatusCode.InternalServerError;
                res.Success = false;
                res.ErrorMessage = ex.Message;
            }

            using (response.Body)
            {
                if (res != null)
                {
                    var responseBodySerializer = new XmlSerializer(typeof(TResponse));
                    responseBodySerializer.Serialize(response.Body, res);
                }
            }
        }
    }
}
