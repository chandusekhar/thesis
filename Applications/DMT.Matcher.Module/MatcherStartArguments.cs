using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Common.Extensions;

namespace DMT.Matcher.Module
{
    struct MatcherStartArguments
    {
        public readonly Uri PartitionServiceUri;
        public readonly int Port;

        public bool HasPort
        {
            get { return this.Port > 0; }
        }

        /// <summary>
        /// Parses the string arguments. 
        /// 
        /// args[0]: partition service uri
        /// args[1]: port to run on
        /// </summary>
        /// <param name="args"></param>
        public MatcherStartArguments(string[] args)
        {
            this.PartitionServiceUri = new Uri(args[0]);

            string portString;
            // trying to get port out of parameters, fallback to a random port in 1201..1210
            if (args.TryGet(1, out portString))
            {
                this.Port = int.Parse(portString);
            }
            else
            {
                this.Port = new Random().Next(1201, 1211);
            }
        }
    }
}
