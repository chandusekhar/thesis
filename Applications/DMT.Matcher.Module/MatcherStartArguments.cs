using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Matcher.Module
{
    struct MatcherStartArguments
    {
        public readonly Uri PartitionServiceUri;

        public MatcherStartArguments(string[] args)
        {
            this.PartitionServiceUri = new Uri(args[0]);
        }
    }
}
