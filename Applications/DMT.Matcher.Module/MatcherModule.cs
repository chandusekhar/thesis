﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Matcher.Module.Partitioner;
using DMT.Module.Common.Service;

namespace DMT.Matcher.Module
{
    public class MatcherModule
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

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

        public static void StartModule(string[] argv)
        {
            if (instance != null)
            {
                throw new InvalidOperationException("Matcher module has already started.");
            }

            instance = new MatcherModule();
            instance.Start(argv);

        }

        private void Start(string[] argv)
        {
            MatcherStartArguments startArgs = new MatcherStartArguments(argv);

            // TODO: srart matcher service
            var client = new PartitionBrokerServiceClient(startArgs.PartitionServiceUri);
            if (!client.RegisterMatcher(new MatcherInfo { Id = this.Id }))
            {
                logger.Fatal("Could not register with partitioning module. Shutting down.");
                return;
            }
            // TODO: get partition, parse it

            // signal back
            client.MarkMatcherReady(this.Id);

            client.DeleteMatcher(this.Id);
        }
    }
}
