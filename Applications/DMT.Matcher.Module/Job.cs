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

            lock (this)
            {
                this.jobStarted = true;
            }

            // start the task on a background thread
            Task.Run(() => this.job.Start(mode));
        }

        private void HandleJobDone(object sender, MatcherJobDoneEventArgs e)
        {
            // TODO signal back to partitioner
        }
    }
}
