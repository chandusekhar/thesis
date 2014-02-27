using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using DMT.Partition.Interfaces;

namespace DMT.Partition.Module.Remote.Service
{
    [DataContract]
    public class MatcherInfo
    {
        private bool ready = false;

        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public int Port { get; set; }
        [DataMember]
        public string Host { get; set; }

        public bool Ready { get { return this.ready; } }

        public IPartition Partition { get; set; }

        public IPAddress GetHostAddress()
        {
            if (this.Host == "localhost")
            {
                return IPAddress.Loopback;
            }

            return IPAddress.Parse(this.Host);
        }

        public void MarkReady()
        {
            this.ready = true;
        }
    }
}
