using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Common
{
    public class Configuration
    {
        public const string EnvironmentKey = "env";
        public const string EnvironmentFile = "." + EnvironmentKey;
        
        private Environment env = Environment.Development;
        private bool isEnvSet = false;

        #region singleton

        private static Configuration current;
        public static Configuration Current
        {
            get
            {
                if (current == null)
                {
                    current = new Configuration();
                }
                return current;
            }
        }

        /// <summary>
        /// Override the current configuration object
        /// </summary>
        /// <param name="config"></param>
        public static void OverrideCurrent(Configuration config)
        {
            current = config;
        }

        #endregion

        /// <summary>
        /// Gets the enviroment for the currently running process. 
        /// 
        /// <para>The enviroment can only be set once. The lookup path is the following:</para>
        /// 
        /// <para>1. 'env' environment variable</para>
        /// <para>2. a file called '.env' in the <c>System.Environment.CurrentDirectory</c> which is by default the directory of the dll/exe.</para>
        /// <para>3. 'env' appsettings in the application config file</para>
        /// <para>4. it defaults to DEVELOPMENT</para>
        /// </summary>
        /// <returns>The current environment</returns>
        public Environment Environment
        {
            get
            {
                if (!this.isEnvSet)
                {
                    InitializeEnvironment();
                }

                return this.env;
            }
        }

        internal void SetEnvironment(Environment env)
        {
            this.isEnvSet = true;
            this.env = env;
        }

        private void InitializeEnvironment()
        {
            this.isEnvSet = true;

            String envString = System.Environment.GetEnvironmentVariable(Configuration.EnvironmentKey);
            string path = Path.Combine(System.Environment.CurrentDirectory, Configuration.EnvironmentFile);
            if (envString == null && File.Exists(path))
            {
                envString = File.ReadAllText(path).Trim();
            }
            envString = envString ?? ConfigurationManager.AppSettings[Configuration.EnvironmentKey];

            if (!Enum.TryParse(envString, true, out env))
            {
                this.env = Environment.Development;
            }
        }
    }

    public enum Environment
    {
        Development,
        Test,
        Production
    }
}
