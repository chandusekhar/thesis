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

        public Arguments(Uri serviceUri)
        {
            this.ServiceUri = serviceUri;
        }

        public string ToCommandLineArgs()
        {
            return this.ServiceUri.AbsoluteUri;
        }
    }
}
