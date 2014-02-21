using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Partition.Module.Remote
{
    struct Arguments
    {
        public readonly string Host;
        public readonly ushort Port;

        public Arguments(string host, ushort port)
        {
            this.Host = host;
            this.Port = port;
        }

        public string ToCommandLineArgs()
        {
            return string.Join(" ", Host, Port);
        }
    }
}
