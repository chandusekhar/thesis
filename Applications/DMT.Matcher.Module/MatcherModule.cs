using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DMT.Matcher.Interfaces;
using DMT.Matcher.Module.Exceptions;
using DMT.Matcher.Module.Partitioner;
using DMT.Matcher.Module.Service;
using DMT.Module.Common.Service;

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
        private Job job;

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
            instance.Start(argv);
        }

        internal void Done()
        {
            this.done.Set();
        }

        internal void StartJob(MatchMode mode)
        {
            if (this.job == null)
            {
                throw new NoMatcherJobException("No matcher job has been received.");
            }

            this.job.Start(mode);
        }

        private void Start(string[] argv)
        {
            MatcherStartArguments startArgs = new MatcherStartArguments(argv);

            MatcherService service = new MatcherService(startArgs.Port);
            service.Start();

            var client = new PartitionBrokerServiceClient(startArgs.PartitionServiceUri);
            if (!client.RegisterMatcher(new MatcherInfo { Id = this.id, Port = startArgs.Port, Host = GetHost() }))
            {
                logger.Fatal("Could not register with partitioning module. Shutting down.");
                return;
            }
            // TODO: get partition, parse it
            // TODO: get matcher job

            // signal back
            client.MarkMatcherReady(this.id);

            // wait for an exis signal
            this.done.WaitOne();
            service.Close();
        }

        private string GetHost()
        {
            // TODO: return actual host, make is configurable
            return "localhost";
        }
    }
}
