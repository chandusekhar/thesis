using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Partition.Module.Remote
{
    /// <summary>
    /// Instantiates the new 
    /// </summary>
    class LocalPorcessInstantiationStrategy : IInstantiationStrategy
    {
        private const string MatcherPathKey = "matcher-path";
        private const string MatcherBasePortKey = "matcher-base-port";

        /// <summary>
        /// The number of instantiation. Needed to determine port for matcher.
        /// </summary>
        private int instatiation = 1;
        private int basePort;

        public LocalPorcessInstantiationStrategy()
        {
            this.basePort = int.Parse(ConfigurationManager.AppSettings[MatcherBasePortKey]);
        }

        public void StartRemote(Arguments args)
        {
            var argsLocal = new Arguments(args.ServiceUri, GetMatcherPort());
            var path = GetMatcherModulePath();

            Process.Start(path, argsLocal.ToCommandLineArgs());
        }

        private string GetMatcherModulePath()
        {
            return ConfigurationManager.AppSettings[MatcherPathKey];
        }

        private int GetMatcherPort()
        {
            return this.basePort + (instatiation++);
        }
    }
}
