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
        private const string FindPartialPath = "/partial/find";
        private const string DonePartialPath = "/partial/done";

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
            string path = Uri.EscapeUriString(string.Format("{0}/{1}?callback={2}", FindPartialPath, id, MatcherModule.Instance.Info.Url));
            SendPartialPattern(path, pattern);
        }

        public void DonePartialMatch(Guid sessionId, IPattern pattern)
        {
            string path = string.Format("{0}/{1}", DonePartialPath, sessionId);
            SendPartialPattern(path, pattern);
        }

        private void SendPartialPattern(string path, IPattern pattern)
        {
            try
            {
                using (WebClient wc = new WebClient { BaseAddress = this.baseAddress })
                using (Stream stream = wc.OpenWrite(path, HttpMethod.Post))
                using (XmlWriter writer = XmlWriter.Create(stream))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Pattern");

                    if (pattern != null)
                    {
                        pattern.Serialize(writer);
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            catch (WebException ex)
            {
                using (StreamReader r = new StreamReader(ex.Response.GetResponseStream()))
                {
                    logger.Error("Error happend during SendPartition: {0}", r.ReadToEnd());
                }

                throw;
            }
        }
    }
}
