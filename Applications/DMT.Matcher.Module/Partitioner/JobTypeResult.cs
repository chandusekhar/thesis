using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DMT.Matcher.Interfaces;

namespace DMT.Matcher.Module.Partitioner
{
    class JobTypeResult
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public Type JobType { get; private set; }
        public IMatcherDependencyProvider DependencyProvider { get; private set; }

        public JobTypeResult(Assembly jobAssembly)
        {
            this.JobType = jobAssembly.GetTypes().Single(t => typeof(IMatcherJob).IsAssignableFrom(t));
            logger.Debug("Found type for matcher job: {0}", JobType);

            var type = jobAssembly.GetTypes().Single(t => typeof(IMatcherDependencyProvider).IsAssignableFrom(t));
            this.DependencyProvider = (IMatcherDependencyProvider)Activator.CreateInstance(type);
            logger.Debug("Found & instantiated IMatcherDependencyProvider type: {0}", type);
        }
    }
}
