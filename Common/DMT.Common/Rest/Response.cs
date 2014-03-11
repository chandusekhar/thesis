using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Common.Rest
{
    public class Response
    {
        public const string XmlUtf8ContentType = "text/xml; charset=utf-8";

        private HttpListenerResponse innerResponse;

        public HttpStatusCode Status
        {
            get { return (HttpStatusCode)this.innerResponse.StatusCode; }
            set { this.innerResponse.StatusCode = (int)value; }
        }

        public bool Chunked
        {
            get { return this.innerResponse.SendChunked; }
            set { this.innerResponse.SendChunked = value; }
        }

        public string ContentType
        {
            get { return this.innerResponse.ContentType; }
            set { this.innerResponse.ContentType = value; }
        }

        public Stream Body
        {
            get { return this.innerResponse.OutputStream; }
        }

        internal Response(HttpListenerResponse response)
        {
            this.innerResponse = response;
        }

        internal void EnsureClosed()
        {
            this.innerResponse.OutputStream.Close();
        }

    }
}
