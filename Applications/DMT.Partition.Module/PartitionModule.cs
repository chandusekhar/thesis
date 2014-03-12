using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DMT.Common.Composition;
using DMT.Module.Common;
using DMT.Partition.Module.CLI;
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

        private ManualResetEvent exit;
        private bool matchersStarted = false;

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

        internal MatcherRegistry MatcherRegistry
        {
            get { return matcherRegistry; }
        }

        internal PartitionRegistry PartitionRegistry
        {
            get { return this.partitionRegistry; }
        }

        internal string ModelFileName { get; private set; }

        internal string JobBinaryPath { get; private set; }

        private PartitionModule()
        {
            this.exit = new ManualResetEvent(false);
        }

        /// <summary>
        /// Initializes the module and starts the services.
        /// </summary>
        /// <param name="argv"></param>
        /// <returns></returns>
        public static void StartModule(string[] argv)
        {
            instance = new PartitionModule();
            instance.Start(argv);
        }

        /// <summary>
        /// Exit the application.
        /// This method does clean up after the application.
        /// </summary>
        internal void Exit()
        {
            if (this.matchersStarted)
            {
                this.matcherRegistry.ReleaseMatchers();
            }
            this.exit.Set();
        }

        private void Start(string[] argv)
        {
            logger.Info("Partition module started.");

            Console.CancelKeyPress += HandleInterupt;

            CompositionService.Default.Initialize(typeof(IPartitionSerializer).Assembly);
            logger.Info("CompositionService initalized successfully.");

            var args = new CommandLineArgs(argv);
            args.Parse();
            this.JobBinaryPath = args.JobBinaryPath;
            this.ModelFileName = args.ModelFilePath;

            //ModelLoader loader = new ModelLoader(this.modelFileName);
            //var model = await loader.LoadModel();

            //Partitioner partitioner = new Partitioner(model);
            //var partitions = partitioner.Partition();
            //this.partitionRegistry = new PartitionRegistry(partitions);

            var service = new PartitionBrokerService();
            service.Start();
            RemoteMatcherInstantiator rmi = new RemoteMatcherInstantiator(service.BaseAddress);
            //rmi.Start(partitions.Count());
            rmi.Start(1);
            this.matchersStarted = true;

            this.exit.WaitOne();
            service.Close();
        }

        private void HandleInterupt(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            Exit();
        }
    }
}
