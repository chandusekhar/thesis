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
    public interface IHost
    {
        IPAddress IP { get; }
    }
}
