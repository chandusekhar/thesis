using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace DMT.Partition.Interfaces
{
    /// <summary>
    /// Host descriptor.
    /// </summary>
    public class Host
    {
        private readonly IPAddress _ip;

        public Host(string ip)
            : this(IPAddress.Parse(ip))
        {

        }

        public Host(IPAddress ip)
        {
            _ip = ip;
        }

        public IPAddress IP { get { return _ip; } }
    }
}
