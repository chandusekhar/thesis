using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DMT.Common.Rest;

namespace DMT.Partition.Module.Remote
{
    class MatcherServiceClient
    {
        private const string QuitPath = "/quit";

        private string baseAddress;

        public MatcherServiceClient(string baseAddress)
        {
            this.baseAddress = baseAddress.TrimEnd('/');
        }

        public Task ReleaseMatcher()
        {
            string url = string.Format("{0}{1}", this.baseAddress, QuitPath);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = HttpMethod.Post;
            // no body for this request
            request.ContentLength = 0;

            return request.GetResponseAsync();
        }

    }
}
