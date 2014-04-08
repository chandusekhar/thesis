using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using DMT.Common;
using DMT.Core.Interfaces;
using DMT.Matcher.Interfaces;
using DMT.Matcher.Module.Exceptions;
using DMT.Matcher.Module.Partitioner;
using DMT.Module.Common.Service;

namespace DMT.Matcher.Module
{
    internal class Job
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private IMatcherJob job;
        private bool jobStarted;

        public Job(JobTypeResult result)
        {
            this.job = InstantiateJob(result.JobFactoryType);
            job.Done += HandleJobDone;
        }

        public void Start(IModel model, MatchMode mode)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            if (this.jobStarted)
            {
                throw new JobAlreadyStartedException(string.Format("Job with name {0} has already started.", this.job.Name));
            }
            this.jobStarted = true;

            // start the task on a background thread
            Task.Run(() => this.job.Start(model, mode));

            logger.Info("Matcher job (name: {0}) has been started in {1} mode", this.job.Name, mode);
        }

        private void HandleJobDone(object sender, MatcherJobDoneEventArgs e)
        {
            this.jobStarted = false;

            logger.Info("Matcher job (name: {0}) has been finished.", this.job.Name);
            var client = MatcherModule.Instance.CreatePartitionServiceClient();
            client.MarkMatcherDone(MatcherModule.Instance.Id, new MatchFoundRequest { MatchFound = e.HasMatches });
            logger.Debug("Matcher job reported done to partition module.");
        }

        private IMatcherJob InstantiateJob(Type jobFactoryType)
        {
            var factory = (IMatcherJobFactory)Activator.CreateInstance(jobFactoryType);

            var job = factory.CreateMatcherJob();
            job.Initialize(new MatcherFrameworkLink());
            logger.Info("Matcher job has been initialized successfully.");

            return job;
        }
    }
}
