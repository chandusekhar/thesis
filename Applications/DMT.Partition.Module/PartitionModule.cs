using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Partition.Module.Remote;
using NLog;

namespace DMT.Partition.Module
{
    public sealed class PartitionModule
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static PartitionModule instance;


        public static PartitionModule Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new InvalidOperationException("Module has not been started yet");
                }
                return instance;
            }
        }

        /// <summary>
        /// Initializes the module and starts the services.
        /// </summary>
        /// <param name="argv"></param>
        /// <returns></returns>
        public static PartitionModule StartModule(string[] argv)
        {
            instance = new PartitionModule();
            instance.Start(argv);

            return instance;
        }

        private async void Start(string[] argv)
        {
            logger.Info("Master module started.");

            if (argv.Length < 2)
            {
                Console.WriteLine("Usage {0} /path/to/model.xml", AppDomain.CurrentDomain.FriendlyName);
                Environment.Exit(0);
            }

            ModelLoader loader = new ModelLoader(argv[1]);
            var model = await loader.LoadModel();

            Partitioner partitioner = new Partitioner(model);
            var partitions = partitioner.Partition();

            RemoteMatcherInstantiator rmi = new RemoteMatcherInstantiator();
            rmi.Start(partitions.Count());
        }
    }
}
