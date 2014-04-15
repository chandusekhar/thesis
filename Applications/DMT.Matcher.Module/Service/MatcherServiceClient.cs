using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DMT.Common.Rest;
using DMT.Core.Interfaces;
using DMT.Matcher.Data.Interfaces;

namespace DMT.Matcher.Module.Service
{
    class MatcherServiceClient
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private const string NodesPath = "/nodes";
        private const string FindPartialPath = "/find_partial";

        private string baseAddress;

        public MatcherServiceClient(string baseAddress)
        {
            this.baseAddress = baseAddress.TrimEnd('/');
        }

        public INode GetNode(IId id)
        {
            try
            {
                using (WebClient wc = new WebClient { BaseAddress = this.baseAddress })
                using (var stream = wc.OpenRead(string.Format("{0}/{1}", NodesPath, id.ToUrlSafe())))
                {
                    return new NodeSerializer(stream).Deserialize();
                }
            }
            catch (WebException ex)
            {
                logger.WarnException("Could not find node in partition.", ex);
                return null;
            }
        }

        public void FindPartialMatch(Guid id, IPattern pattern)
        {
            string path = string.Format("{0}/{1}", FindPartialPath, id);
            using (WebClient wc = new WebClient { BaseAddress = this.baseAddress })
            using (Stream stream = wc.OpenWrite(path, HttpMethod.Post))
            using (XmlWriter writer = XmlWriter.Create(stream))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Pattern");

                pattern.Serialize(writer);

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
    }
}
