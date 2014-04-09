using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.Matcher.Module.Service
{
    class MatcherServiceClient
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private const string NodesPath = "/nodes";

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
    }
}
