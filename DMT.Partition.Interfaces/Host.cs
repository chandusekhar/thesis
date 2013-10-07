using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace DMT.Partition.Interfaces
{
    public class Host
    {
        private IPAddress _ip;

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
