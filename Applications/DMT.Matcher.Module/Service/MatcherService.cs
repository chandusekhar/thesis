using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Common.Rest;
using DMT.Module.Common.Service;

namespace DMT.Matcher.Module.Service
{
    class MatcherService : ServiceBase
    {
        public MatcherService(int port) : base(port) { }

        protected override void Initialize()
        {
            RegisterRoute(HttpMethod.Post, "/quit", new QuitHandler());
            RegisterRoute(HttpMethod.Post, "/start", new StartHandler());
            RegisterRoute(HttpMethod.Post, "/restart", new RestartHandler());
            RegisterRoute(HttpMethod.Post, "/cancel", new CancelHandler());
            RegisterRoute(HttpMethod.Get, "/nodes/{id}", new GetNodeHandler());
        }
    }
}
