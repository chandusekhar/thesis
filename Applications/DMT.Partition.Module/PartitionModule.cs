using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Common.Composition;
using DMT.Partition.Module.Remote;
using DMT.Partition.Module.Remote.Service;
using NLog;

namespace DMT.Partition.Module
{
    public sealed class PartitionModule
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static PartitionModule instance;

        private MatcherRegistry matcherRegistry = new MatcherRegistry();
        private PartitionRegistry partitionRegistry;
        private string modelFileName;

        internal static PartitionModule Instance
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

        internal MatcherRegistry MatcherRegistry { get { return matcherRegistry; } }
        internal PartitionRegistry PartitionRegistry { get { return this.partitionRegistry; } }
        internal string ModelFileName { get { return this.modelFileName; } }

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

            CompositionService.Instance.Initialize();
            //logger.Info("CompositionService initalized successfully.");

            //if (argv.Length < 2)
            //{
            //    Console.WriteLine("Usage {0} /path/to/model.xml", AppDomain.CurrentDomain.FriendlyName);
            //    Environment.Exit(0);
            //}

            //this.modelFileName = argv[1];
            //ModelLoader loader = new ModelLoader(this.modelFileName);
            //var model = await loader.LoadModel();

            //Partitioner partitioner = new Partitioner(model);
            //var partitions = partitioner.Partition();
            //this.partitionRegistry = new PartitionRegistry(partitions);

            var service = new PartitionBrokerService();
            service.Start();
            RemoteMatcherInstantiator rmi = new RemoteMatcherInstantiator(new Uri(service.BaseAddress));
            //rmi.Start(partitions.Count());
            rmi.Start(1);

            Console.ReadKey();
        }
    }
}
