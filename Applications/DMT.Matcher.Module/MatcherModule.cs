using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DMT.Common.Composition;
using DMT.Core.Interfaces;
using DMT.Matcher.Interfaces;
using DMT.Matcher.Module.Exceptions;
using DMT.Matcher.Module.Partitioner;
using DMT.Matcher.Module.Service;
using DMT.Module.Common.Service;
using NLog;
using NLog.Targets;

namespace DMT.Matcher.Module
{
    public class MatcherModule
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        #region static instance

        private static MatcherModule instance;

        internal static MatcherModule Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new InvalidOperationException("Module has not been started yet.");
                }

                return instance;
            }
        }

        #endregion

        private readonly Guid id;
        private ManualResetEvent done;
        private IModel model;
        private Job job;
        private Uri partitionServiceUri;

        public Guid Id { get { return this.id; } }

        private Job Job
        {
            get { return this.job; }
            set { this.job = value; }
        }

        public MatcherModule()
        {
            this.id = Guid.NewGuid();
            this.done = new ManualResetEvent(false);
        }

        public static void StartModule(string[] argv)
        {
            if (instance != null)
            {
                throw new InvalidOperationException("Matcher module has already started.");
            }

            instance = new MatcherModule();
            instance.RedirectLogFile();
            instance.Start(argv);
        }

        internal void Done()
        {
            this.done.Set();
        }

        internal void StartJob(MatchMode mode)
        {
            if (this.Job == null)
            {
                throw new NoMatcherJobException("No matcher job has been received.");
            }

            this.Job.Start(this.model, mode);
        }

        internal PartitionBrokerServiceClient CreatePartitionServiceClient()
        {
            return new PartitionBrokerServiceClient(this.partitionServiceUri);
        }

        internal void AcquireJob()
        {
            var client = CreatePartitionServiceClient();
            this.Job = new Job(client.GetJob());
            // signal back
            client.MarkMatcherReady(this.id);
        }

        internal INode GetNode(IId nodeid)
        {
            if (nodeid == null)
            {
                throw new ArgumentNullException("nodeid");
            }

            return this.model.Nodes.FirstOrDefault(n => n.Id.Equals(nodeid));
        }

        /// <summary>
        /// Start the matcher. The rocess is the following:
        /// 
        /// 1. start matcher service
        /// 2. register matcher at partition module
        /// 3. ask for a partition
        /// 4. ask for a job
        /// 5. send ready signal to parititon module
        /// 6. wait for and 'done' signal
        /// </summary>
        private void Start(string[] argv)
        {
            CompositionService.Default.Initialize();

            MatcherStartArguments startArgs = new MatcherStartArguments(argv);
            this.partitionServiceUri = startArgs.PartitionServiceUri;

            MatcherService service = new MatcherService(startArgs.Port);
            service.Start();

            var client = new PartitionBrokerServiceClient(startArgs.PartitionServiceUri);
            if (!client.RegisterMatcher(new MatcherInfo { Id = this.id, Port = startArgs.Port, Host = GetHost() }))
            {
                logger.Fatal("Could not register with partitioning module. Shutting down.");
                return;
            }

            this.model = client.GetPartition(this.id);

            // get a job, and signal back
            AcquireJob();

            // wait for an exis signal
            this.done.WaitOne();
            service.Close();
        }

        private void RedirectLogFile()
        {
            FileTarget ft = LogManager.Configuration.FindTargetByName("logfile-dev") as FileTarget;
            ft.FileName = string.Format("${{basedir}}/log/dmt-matcher-dev-{0}.log", this.id);
        }

        private string GetHost()
        {
            // TODO: return actual host, make is configurable
            return "localhost";
        }
    }
}
