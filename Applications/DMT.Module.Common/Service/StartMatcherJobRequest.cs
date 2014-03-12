using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Matcher.Interfaces;

namespace DMT.Module.Common.Service
{
    [Serializable]
    public class StartMatcherJobRequest
    {
        public MatchMode Mode { get; set; }
    }
}
