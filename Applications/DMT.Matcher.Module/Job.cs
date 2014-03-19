using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Matcher.Interfaces;
using DMT.Matcher.Module.Exceptions;

namespace DMT.Matcher.Module
{
    internal class Job
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private IMatcherJob job;
        private bool jobStarted;

        public Job(IMatcherJob job)
        {
            this.job = job;
            job.Done += HandleJobDone;
        }

        public void Start(MatchMode mode)
        {
            if (this.jobStarted)
            {
                throw new JobAlreadyStartedException(string.Format("Job with name {0} has already started.", this.job.Name));
            }
            this.jobStarted = true;

            // start the task on a background thread
            Task.Run(() => this.job.Start(mode));

            logger.Info("Matcher job (name: {0}) has been started in {1} mode", this.job.Name, mode);
        }

        private void HandleJobDone(object sender, MatcherJobDoneEventArgs e)
        {
            this.jobStarted = false;

            logger.Info("Matcher job (name: {0}) has been finished.", this.job.Name);
            var client = MatcherModule.Instance.CreatePartitionServiceClient();
            client.MarkMatcherDone(MatcherModule.Instance.Id);
            logger.Debug("Matcher job reported done to partition module.");
        }
    }
}
