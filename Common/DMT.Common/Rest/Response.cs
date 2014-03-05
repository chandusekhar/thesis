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
        private HttpListenerResponse innerResponse;

        public HttpStatusCode Status
        {
            get { return (HttpStatusCode)this.innerResponse.StatusCode; }
            set { this.innerResponse.StatusCode = (int)value; }
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
