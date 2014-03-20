using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Common.Rest;

namespace DMT.Matcher.Module.Service
{
    class RestartHandler : IRouteHandler
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public void Handle(Request request, Response response)
        {
            logger.Info("Matcher job restart initated. Acquiring a new job binary");
            Task.Run(() => MatcherModule.Instance.AcquireJob());
        }
    }
}
