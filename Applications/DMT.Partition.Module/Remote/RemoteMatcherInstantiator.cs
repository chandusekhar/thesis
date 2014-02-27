using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Partition.Module.Remote
{
    /// <summary>
    /// Instantiates new remote matcher instances.
    /// </summary>
    class RemoteMatcherInstantiator
    {
        private const string BindAddressKey = "bind-address";
        private const string PortKey = "port";

        private IInstantiationStrategy instantiationStrategy;
        private readonly Uri serviceUri;

        public RemoteMatcherInstantiator(Uri serviceUri)
        {
            this.instantiationStrategy = new LocalPorcessInstantiationStrategy();
            this.serviceUri = serviceUri;
        }

        /// <summary>
        /// Starts up given number of instances of the matcher
        /// </summary>
        /// <param name="numberOfInstances">the number of instances to start</param>
        public void Start(int numberOfInstances)
        {
            Arguments args = new Arguments(this.serviceUri);

            for (int i = 0; i < numberOfInstances; i++)
            {
                this.instantiationStrategy.StartRemote(args);
            }
        }
    }
}
