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

        public Type JobFactoryType { get; private set; }

        public JobTypeResult(Assembly jobAssembly)
        {
            this.JobFactoryType = jobAssembly.GetTypes().Single(t => typeof(IMatcherJobFactory).IsAssignableFrom(t));
            logger.Debug("Found type for matcher job factory: {0}", JobFactoryType);
        }
    }
}
