using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using DMT.Common.Rest;

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
            string checksum = GetChecksum(binary);

            response.AddHeader(ChecksumHeaderName, checksum);
            using (var body = response.Body)
            {
                body.Write(binary, 0, binary.Length);
            }
        }

        private string GetChecksum(byte[] binary)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(binary);
                return string.Join(string.Empty, hash.Select(h => h.ToString("X2")));
            }
        }
    }
}
