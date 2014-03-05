using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DMT.Common.Rest;
using DMT.ServiceParams;

namespace DMT.Matcher.Module.Partitioner
{
    class PartitionBrokerServiceClient
    {
        public readonly string BaseAddress;

        public const string RegisterPath = "/register";
        public const string MatchersPath = "/matchers";

        public PartitionBrokerServiceClient(string baseAddress)
        {
            this.BaseAddress = baseAddress;
        }

        public PartitionBrokerServiceClient(Uri baseAddress)
            : this(baseAddress.AbsoluteUri)
        {

        }

        public bool RegisterMatcher(MatcherInfo matcher)
        {
            byte[] response;
            XmlSerializer s;
            BoolResponse parsedResonse;

            using (var wc = new WebClient { BaseAddress = this.BaseAddress })
            using (var stream = new MemoryStream())
            {
                s = new XmlSerializer(typeof(MatcherInfo));
                s.Serialize(stream, matcher);
                response = wc.UploadData(RegisterPath, HttpMethod.Post, stream.ToArray());
            }

            s = new XmlSerializer(typeof(BoolResponse));
            using (var stream = new MemoryStream(response))
            {
                parsedResonse = (BoolResponse)s.Deserialize(stream);
            }

            return parsedResonse.Result;
        }

        public bool DeleteMatcher(Guid id)
        {
            BoolResponse response = null;
            string url = string.Format("{0}{1}/{2}", this.BaseAddress.TrimEnd('/'), MatchersPath, id);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = HttpMethod.Delete;
            using (var res = req.GetResponse())
            {
                var s = new XmlSerializer(typeof(BoolResponse));
                response = (BoolResponse)s.Deserialize(res.GetResponseStream());
            }

            return response.Result;
        }

        public void MarkMatcherReady(Guid id)
        {
            string url = string.Format("{0}{1}/{2}/ready", this.BaseAddress.TrimEnd('/'), MatchersPath, id);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = HttpMethod.Put;
            req.ContentLength = 0;
            using (req.GetResponse()) ;
        }
    }
}
