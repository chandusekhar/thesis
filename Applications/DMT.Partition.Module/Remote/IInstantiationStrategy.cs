using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DMT.Partition.Module.Remote
{
    interface IInstantiationStrategy
    {
        /// <summary>
        /// Start the remote service.
        /// </summary>
        /// <param name="args">these args will be passed to the newly started service</param>
        void StartRemote(Arguments args);
    }
}
