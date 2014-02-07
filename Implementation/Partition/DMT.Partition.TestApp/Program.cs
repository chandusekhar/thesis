using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Common.Composition;
using DMT.Core.Interfaces;
using DMT.Partition.Interfaces;
using DMT.Partition.Interfaces.Events;

namespace DMT.Partition.TestApp
{
    class Program
    {
        private const int NumberOfNodes = 10000;
        private const int DegreeOfNode = 100;

        static void Main(string[] args)
        {
            CompositionService.Instance.Initialize();

            IModelGenerator gen = new RandomModelGenerator(1234);
            //IModel model = gen.Generate(NumberOfNodes, DegreeOfNode);
            IModel model = gen.Generate(1000, 10);

            var pm = CompositionService.Instance.GetExport<IThreeStepPartitionManager>();
            pm.AfterCoarsening += PrintAfterCoarseningStatistics;

            var partitions = pm.PartitionModel(model);

            Console.WriteLine("All done. Let's see the statistics:");
            PrintNodeCountInPartition(partitions);
            Console.ReadKey();
        }

        static void PrintNodeCountInPartition(IEnumerable<IPartition> partitions)
        {
            Console.WriteLine("Number of partitions: {0}", partitions.Count());
            foreach (var p in partitions)
            {
                Console.WriteLine("{0} has {1} nodes", p.Id, p.Nodes.Count);
            }
        }

        static void PrintAfterCoarseningStatistics(object sender, AfterCoarseningEventArgs e)
        {
            foreach (var node in e.CoarsenedNodes)
            {
                Console.WriteLine("Size of {0}: {1}", node, node.Size);
            }
        }
    }
}
