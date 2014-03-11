using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DMT.Common.Rest;
using DMT.Core.Interfaces;
using DMT.Module.Common.Service;

namespace DMT.Matcher.Module.Partitioner
{
    class PartitionBrokerServiceClient
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public readonly string BaseAddress;

        public const string MatchersPath = "/matchers";

        public PartitionBrokerServiceClient(string baseAddress)
        {
            this.BaseAddress = baseAddress.TrimEnd('/');
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
                response = wc.UploadData(MatchersPath, HttpMethod.Post, stream.ToArray());
            }

            s = new XmlSerializer(typeof(BoolResponse));
            using (var stream = new MemoryStream(response))
            {
                parsedResonse = (BoolResponse)s.Deserialize(stream);
            }

            return parsedResonse.Result;
        }

        public void MarkMatcherReady(Guid id)
        {
            string url = string.Format("{0}{1}/{2}/ready", this.BaseAddress, MatchersPath, id);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = HttpMethod.Put;
            req.ContentLength = 0;
            using (req.GetResponse()) ;
        }

        public MatcherInfo FindMatcher(IId partitionId)
        {
            MatcherInfoResponse response = null;

            string path = string.Format("{0}/find/{1}", MatchersPath, partitionId.ToUrlSafe());
            using (WebClient wc = new WebClient { BaseAddress = this.BaseAddress })
            using (var responseStream = wc.OpenRead(path))
            {
                var s = new XmlSerializer(typeof(MatcherInfoResponse));
                response = (MatcherInfoResponse)s.Deserialize(responseStream);
            }

            if (response.Success)
            {
                return response.Result;
            }
            else
            {
                logger.Error("Error while getting matcher address: {0}.", response.ErrorMessage);
                // TODO: throw an exception?
                return null;
            }
        }
    }
}
