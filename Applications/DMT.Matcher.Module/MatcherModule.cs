using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Matcher.Module
{
    public class MatcherModule
    {
        #region static instance

        private static MatcherModule instance;

        internal static MatcherModule Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new InvalidOperationException("Module has not been started yet.");
                }

                return instance;
            }
        }

        #endregion

        public readonly Guid Id;

        public MatcherModule()
        {
            this.Id = Guid.NewGuid();
        }

        public static MatcherModule StartModule(string[] argv)
        {
            if (instance != null)
            {
                throw new InvalidOperationException("Matcher module has already started.");
            }

            instance = new MatcherModule();
            instance.Start(argv);

            return instance;
        }

        private void Start(string[] argv)
        {
            new Partition.PartitionBrokerServiceClient().RegisterMatcher(new Partition.MatcherInfo() { Id = this.Id });
        }
    }
}
