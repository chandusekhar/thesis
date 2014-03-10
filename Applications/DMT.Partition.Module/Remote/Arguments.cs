using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Partition.Module.Remote
{
    struct Arguments
    {
        public readonly Uri ServiceUri;
        public readonly int MatcherPort;

        public Arguments(Uri serviceUri) : this(serviceUri, -1) { }

        public Arguments(Uri serviceUri, int matcherPort)
        {
            this.ServiceUri = serviceUri;
            this.MatcherPort = matcherPort;
        }

        public string ToCommandLineArgs()
        {
            return string.Join(" ", this.ServiceUri, this.MatcherPort);
        }
    }
}
