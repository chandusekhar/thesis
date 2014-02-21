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

        public RemoteMatcherInstantiator()
        {
            this.instantiationStrategy = new LocalPorcessInstantiationStrategy();
        }

        /// <summary>
        /// Starts up given number of instances of the matcher
        /// </summary>
        /// <param name="numberOfInstances">the number of instances to start</param>
        public void Start(int numberOfInstances)
        {
            Arguments args = new Arguments(GetBindAddress(), GetPort());

            for (int i = 0; i < numberOfInstances; i++)
            {
                this.instantiationStrategy.StartRemote(args);
            }
        }

        /// <summary>
        /// Gets the address that the server process binds to. Defaults to 'localhost'.
        /// </summary>
        /// <returns></returns>
        private string GetBindAddress()
        {
            return ConfigurationManager.AppSettings[BindAddressKey] ?? "localhost";
        }

        /// <summary>
        /// Gets the port that the server uses. Defaults to 7878.
        /// </summary>
        /// <returns></returns>
        private ushort GetPort()
        {
            string portString = ConfigurationManager.AppSettings[PortKey] ?? "7878";

            return ushort.Parse(portString);
        }
    }
}
