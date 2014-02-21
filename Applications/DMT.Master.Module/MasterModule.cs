using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Master.Module.Remote;
using NLog;

namespace DMT.Master.Module
{
    public sealed class MasterModule
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();


        public async void Start(string[] argv)
        {
            logger.Info("Master module started.");

            if (argv.Length < 2)
            {
                Console.WriteLine("Usage {0} /path/to/model.xml", AppDomain.CurrentDomain.FriendlyName);
                Environment.Exit(0);
            }

            ModelLoader loader = new ModelLoader(argv[1]);
            var model = await loader.LoadModel();

            Partitioner partitioner = new Partitioner(model);
            var partitions = partitioner.Partition();

            RemoteMatcherInstantiator rmi = new RemoteMatcherInstantiator();
            rmi.Start(partitions.Count());
        }
    }
}
