using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using DMT.Common.Rest;
using DMT.Module.Common;

namespace DMT.Partition.Module.Remote.Service
{
    class GetMatcherJobHandler : IRouteHandler
    {
        private const string ChecksumHeaderName = "X-Matcher-Job-Checksum";

        public void Handle(Request request, Response response)
        {
            response.Chunked = true;

            string path = PartitionModule.Instance.JobBinaryPath;
            byte[] binary = File.ReadAllBytes(path);
            string checksum = Checksum.Calculate(binary);

            response.AddHeader(ChecksumHeaderName, checksum);
            using (var body = response.Body)
            {
                body.Write(binary, 0, binary.Length);
            }
        }
    }
}
