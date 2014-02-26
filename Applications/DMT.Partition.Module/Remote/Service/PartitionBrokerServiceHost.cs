using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Partition.Module.Remote.Service
{
    class PartitionBrokerServiceHost : IDisposable
    {
        private const string EndpointPath = "partition";
        private const string ContextPath = "service";

        private readonly string host;
        private readonly int port;
        private bool opened = false;

        private ServiceHost serviceHost;

        public string Host { get { return this.host; } }
        public int Port { get { return this.port; } }

        public PartitionBrokerServiceHost(string host = "localhost", int port = 8080)
        {
            this.host = host;
            this.port = port;
        }

        public void Open()
        {
            if (this.opened)
            {
                return;
            }

            this.serviceHost = new ServiceHost(typeof(PartitionBrokerService), GetBaseAddress());
            SetMetadata();
            SetEndpoint();

            this.serviceHost.Open();
            this.opened = true;
        }

        private void SetEndpoint()
        {
            // TODO: setup tcp binding
            NetTcpBinding binding = new NetTcpBinding();

            binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
            binding.Security.Mode = SecurityMode.None;
            binding.TransferMode = TransferMode.StreamedResponse;

            this.serviceHost.AddServiceEndpoint(typeof(IPartitionBrokerService), binding, GetServiceAddress());
        }

        private void SetMetadata()
        {
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = false;
            smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
            this.serviceHost.Description.Behaviors.Add(smb);
        }

        private Uri GetBaseAddress()
        {
            var builder = new UriBuilder();
            builder.Host = this.host;
            builder.Path = PartitionBrokerServiceHost.ContextPath;
            builder.Port = this.port;
            builder.Scheme = "net.tcp";

            return builder.Uri;
        }

        private Uri GetServiceAddress()
        {
            var builder = new UriBuilder(GetBaseAddress());
            builder.Path = string.Format("{0}/{1}", builder.Path, PartitionBrokerServiceHost.EndpointPath);
            return builder.Uri;
        }
        
        void IDisposable.Dispose()
        {
            this.serviceHost.Close();
            ((IDisposable)this.serviceHost).Dispose();
        }
    }
}
