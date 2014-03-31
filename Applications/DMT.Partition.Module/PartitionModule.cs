using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DMT.Common.Composition;
using DMT.Matcher.Interfaces;
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

        private MatcherRegistry matcherRegistry;
        private PartitionRegistry partitionRegistry;

        private ManualResetEvent exit;
        private bool matchersStarted = false;
        private MatchMode? matchMode = null;
        private MatchMode? lastMode;

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

        internal bool IsDebug { get; private set; }

        internal string ModelFileName { get; private set; }

        internal string JobBinaryPath { get; private set; }

        private PartitionModule()
        {
            this.exit = new ManualResetEvent(false);
            this.matcherRegistry = new MatcherRegistry();
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
            Console.WriteLine("Partition module started...");
            Console.WriteLine("Press CTRL+C to exit!");

            logger.Info("Partition module started.");

            // event subscriptitons
            Console.CancelKeyPress += HandleInterupt;
            this.matcherRegistry.MatchersReady += StartMatchers;
            this.matcherRegistry.MatchersDone += RestartMatchers;

            CompositionService.Default.Initialize();
            logger.Info("CompositionService initalized successfully.");

            ParseCommandLineArguments(argv);

            logger.Info("Selected {0} for model.", this.ModelFileName);

            ModelLoader loader = new ModelLoader(this.ModelFileName);
            var model = loader.LoadModel();

            Partitioner partitioner = new Partitioner(model);
            var partitions = partitioner.Partition();
            this.partitionRegistry = new PartitionRegistry(partitions);

            var service = new PartitionBrokerService();
            service.Start();

            if (!this.IsDebug)
            {
                RemoteMatcherInstantiator rmi = new RemoteMatcherInstantiator(service.BaseAddress);
                rmi.Start(partitions.Count());
                this.matchersStarted = true;

                Console.WriteLine("Started {0} matcher(s)...", partitions.Count());
            }

            Console.WriteLine("Partitioner started, waitng for matcher modules to come in.");
            this.exit.WaitOne();
            service.Close();
        }

        private void ParseCommandLineArguments(string[] argv)
        {
            var args = new CommandLineArgs(argv);
            args.Parse();
            this.JobBinaryPath = args.JobBinaryPath;
            this.ModelFileName = args.ModelFilePath;
            this.matchMode = args.MatchMode;
            this.IsDebug = args.IsDebug;
        }

        private void HandleInterupt(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            Exit();
        }

        private void StartMatchers(object sender, EventArgs e)
        {
            this.lastMode = this.matchMode;

            if (this.lastMode == null)
            {
                var c = new EnumSelectCommand<MatchMode>('m', "Select how the matcher job will be started.");
                new ConsoleHandler(c, c).Execute();
                this.lastMode = c.Result;
            }

            this.matcherRegistry.StartMatchers((MatchMode)this.lastMode);
            logger.Info("Matcher jobs started.");
        }

        private void RestartMatchers(object sender, EventArgs e)
        {
            logger.Info("Matchers are done. Restarting...");

            var ac = new ActionCommand('r', "Restart matchers with the same options.", () => this.matcherRegistry.StartMatchers((MatchMode)this.lastMode));
            var cc = new ContinuationCommand('s', "Restart matcher with with new job.");

            var binaryPath = new StringCommand("Path of the matcher job binary:");
            cc.Then(binaryPath).Done(() =>
            {
                this.JobBinaryPath = binaryPath.Answer;
                this.matcherRegistry.RestartMatchers();
            });

            var ch = new ConsoleHandler(new CommandBase[] { ac, cc });

            ch.Execute();
        }
    }
}
