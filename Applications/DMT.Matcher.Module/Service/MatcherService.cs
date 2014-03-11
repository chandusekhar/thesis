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
        }
    }
}
