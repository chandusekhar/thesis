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

            var wc = new WebClient();
            wc.BaseAddress = this.BaseAddress;
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

            return parsedResonse.Value;
        }
    }
}
