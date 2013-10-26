using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Common
{
    public static class Configuration
    {
        public const string EnvironmentKey = "env";
        
        private static Environment env = Environment.DEVELOPMENT;
        private static bool isEnvSet = false;

        /// <summary>
        /// Gets the enviroment for the currently running process. 
        /// 
        /// <para>The enviroment can only be set once. The lookup path is the following:</para>
        /// 
        /// <para>1. 'env' environment variable</para>
        /// <para>2. a file called 'env' in the <c>System.Environment.CurrentDirectory</c> which is by default the directory of the dll/exe.</para>
        /// <para>3. 'env' appsettings in the application config file</para>
        /// <para>4. it defaults to DEVELOPMENT</para>
        /// </summary>
        /// <returns>The current environment</returns>
        public static Environment GetEnvironment()
        {
            if (!isEnvSet)
            {
                isEnvSet = true;

                String envString = System.Environment.GetEnvironmentVariable(Configuration.EnvironmentKey);
                string path = Path.Combine(System.Environment.CurrentDirectory, Configuration.EnvironmentKey);
                if (envString == null && File.Exists(path))
                {
                    envString = File.ReadAllText(path).Trim();
                }
                envString = envString ?? ConfigurationManager.AppSettings[Configuration.EnvironmentKey];

                if (!Enum.TryParse(envString, true, out env))
                {
                    env = Environment.DEVELOPMENT;
                }
            }

            return env;
        }
    }

    public enum Environment
    {
        DEVELOPMENT,
        TEST,
        PRODUCTION
    }
}
