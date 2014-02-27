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
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private const string EndpointPath = "partition";
        private const string MexEndpointPath = "mex";
        private const string ContextPath = "service";

        private readonly string host;
        private readonly int port;
        private bool opened = false;

        private ServiceHost serviceHost;

        public string Host { get { return this.host; } }
        public int Port { get { return this.port; } }

        public PartitionBrokerServiceHost(string host = "localhost", int port = 8088)
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
            SetServiceBehaviors();
            SetEndpoint();

            this.serviceHost.Open();
            this.opened = true;

            logger.Info("Service started successfully.");
        }

        public Uri GetBaseAddress()
        {
            var builder = new UriBuilder();
            builder.Host = this.host;
            builder.Path = PartitionBrokerServiceHost.ContextPath;
            builder.Port = this.port;
            builder.Scheme = "net.tcp";

            return builder.Uri;
        }

        public Uri GetServiceAddress()
        {
            var builder = new UriBuilder(GetBaseAddress());
            builder.Path = string.Format("{0}/{1}", builder.Path, PartitionBrokerServiceHost.EndpointPath);
            return builder.Uri;
        }

        public Uri GetMexAddress()
        {
            var builder = new UriBuilder(GetBaseAddress());
            builder.Path = string.Format("{0}/{1}", builder.Path, PartitionBrokerServiceHost.MexEndpointPath);
            return builder.Uri;
        }

        private void SetEndpoint()
        {
            // for the service
            NetTcpBinding binding = new NetTcpBinding();
            binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
            binding.Security.Mode = SecurityMode.None;
            binding.TransferMode = TransferMode.StreamedResponse;
            this.serviceHost.AddServiceEndpoint(typeof(IPartitionBrokerService), binding, GetServiceAddress());

            // for metadata exchange -- discoverability
            var mexBinding = MetadataExchangeBindings.CreateMexTcpBinding();
            this.serviceHost.AddServiceEndpoint(typeof(IMetadataExchange), mexBinding, GetMexAddress());
        }

        private void SetServiceBehaviors()
        {
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            this.serviceHost.Description.Behaviors.Add(smb);
        }
        
        void IDisposable.Dispose()
        {
            this.serviceHost.Close();
            ((IDisposable)this.serviceHost).Dispose();
            logger.Info("Service closed and disposed.");
        }
    }
}
