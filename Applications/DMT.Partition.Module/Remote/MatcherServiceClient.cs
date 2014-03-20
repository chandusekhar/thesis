using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DMT.Common.Rest;
using DMT.Matcher.Interfaces;
using DMT.Module.Common.Service;

namespace DMT.Partition.Module.Remote
{
    class MatcherServiceClient
    {
        private const string QuitPath = "/quit";
        private const string StartPath = "/start";
        private const string RestartPath = "/restart";

        private string baseAddress;

        public MatcherServiceClient(string baseAddress)
        {
            this.baseAddress = baseAddress.TrimEnd('/');
        }

        public Task ReleaseMatcher()
        {
            return RestClientHelper.SendRequestWithoutBodyAsync(HttpMethod.Post, CreateUrl(QuitPath));
        }

        public Task StartMatcher(MatchMode mode)
        {
            return Task.Run(() =>
            {
                string url = CreateUrl(StartPath);
                StartMatcherJobRequest req = new StartMatcherJobRequest();
                req.Mode = mode;

                using (WebClient wc = new WebClient { BaseAddress = this.baseAddress })
                using (var stream = wc.OpenWrite(url, HttpMethod.Post))
                {
                    XmlSerializer s = new XmlSerializer(typeof(StartMatcherJobRequest));
                    s.Serialize(stream, req);
                }
            });
        }

        public Task RestartWithNewJob()
        {
            return RestClientHelper.SendRequestWithoutBodyAsync(HttpMethod.Post, CreateUrl(RestartPath));
        }

        private string CreateUrl(string path)
        {
            return string.Format("{0}{1}", this.baseAddress, path);
        }
    }
}
