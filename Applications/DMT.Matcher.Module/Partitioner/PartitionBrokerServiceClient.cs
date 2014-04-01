using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DMT.Common.Rest;
using DMT.Core.Interfaces;
using DMT.Matcher.Interfaces;
using DMT.Module.Common;
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
            RestClientHelper.SendRequestWithoutBody(HttpMethod.Put, url);
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

        public IMatcherJob GetJob()
        {
            Assembly jobAssembly = null;
            using (WebClient wc = new WebClient() { BaseAddress = this.BaseAddress })
            {
                byte[] binary = wc.DownloadData(string.Format("{0}/job", MatchersPath));
                string checksum = wc.ResponseHeaders["X-Matcher-Job-Checksum"];
                logger.Debug("Downloaded matcher job binary with checksum: {0}", checksum);

                if (!Checksum.Check(checksum, binary))
                {
                    throw new InvalidJobException("Checksum is different.");
                }

                jobAssembly = Assembly.Load(binary);
                logger.Info("Successfully loaded job assembly into domain.");
            }
            // TODO: add assembly to CompositionService
            var type = jobAssembly.GetTypes().Single(t => typeof(IMatcherJob).IsAssignableFrom(t));
            IMatcherJob job = (IMatcherJob)Activator.CreateInstance(type);
            job.Initialize(new MatcherFrameworkLink());

            logger.Info("Matcher job has been initialized successfully.");

            return job;
        }

        public IModel GetPartition(Guid id)
        {
            string url = string.Format("{0}/{1}/partition", MatchersPath, id);

            using (var wc = new WebClient { BaseAddress = this.BaseAddress})
            using (var stream = wc.OpenRead(url))
            {
                var deserializer = new MatcherDataDeserializer();
                return deserializer.Deserialize(stream);
            }
        }

        public void MarkMatcherDone(Guid id, MatchFoundRequest req)
        {
            string url = string.Format("{0}{1}/{2}/done", this.BaseAddress, MatchersPath, id);
            using (var wc = new WebClient())
            using (var stream = wc.OpenWrite(url, HttpMethod.Put))
            {
                var s = new XmlSerializer(typeof(MatchFoundRequest));
                s.Serialize(stream, req);
            }
        }
    }
}
