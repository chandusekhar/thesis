using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Master.Module.Remote
{
    /// <summary>
    /// Instantiates the new 
    /// </summary>
    class LocalPorcessInstantiationStrategy : IInstantiationStrategy
    {
        private const string MatcherPathKey = "matcher-path";

        public void StartRemote(Arguments args)
        {
            var path = GetMatcherModulePath();

            Process.Start(path, args.ToCommandLineArgs());
        }

        private string GetMatcherModulePath()
        {
            return ConfigurationManager.AppSettings[MatcherPathKey];
        }
    }
}
