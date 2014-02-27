using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DMT.Matcher.Module.Partition;

namespace DMT.Matcher.Module.Helper
{
    internal static class PartitionServiceHelper
    {
        public static PartitionBrokerServiceClient CreateClient(Uri address)
        {
            var endpointAddress = new EndpointAddress(address);
            var binding = new NetTcpBinding();
            binding.TransferMode = TransferMode.StreamedResponse;
            binding.Security.Mode = SecurityMode.None;

            return new PartitionBrokerServiceClient(binding, endpointAddress);
        }
    }
}
