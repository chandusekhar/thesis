using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Common.Rest
{
    public static class RestClientHelper
    {
        public static void SendRequestWithoutBody(HttpMethod method, string url)
        {
            using (CreateRequestWhithoutBody(method, url).GetResponse()) ;
        }

        public static Task SendRequestWithoutBodyAsync(HttpMethod method, string url)
        {
            return CreateRequestWhithoutBody(method, url).GetResponseAsync();
        }

        private static HttpWebRequest CreateRequestWhithoutBody(HttpMethod method, string url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = method;
            req.ContentLength = 0;
            return req;
        }

    }
}
