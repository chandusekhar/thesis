using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Common.Composition;
using DMT.Core.Interfaces;
using NLog;

namespace DMT.Partition.TestApp
{
    class RandomModelGenerator : InjectableBase, IModelGenerator
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private int seed;

        [Import]
        private IEntityFactory entityFactory;

        [Import]
        private IModelFactory modelFactory;

        public RandomModelGenerator(int seed)
        {
            this.seed = seed;
        }

        public IModel Generate(int n, int d)
        {
            var nodes = BuildConnections(GenerateNodes(n), d);
            return modelFactory.Create(nodes);
        }

        private List<INode> GenerateNodes(int n)
        {
            List<INode> nodes = new List<INode>();
            for (int i = 0; i < n; i++)
            {
                nodes.Add(entityFactory.CreateNode());
            }

            logger.Info("Generated {0} node(s).", n);

            return nodes;
        }

        // each node has d outbound connections
        private IEnumerable<INode> BuildConnections(List<INode> nodes, int d)
        {
            logger.Info("Started building connections between nodes with seed {0}", this.seed);

            Random rnd = new Random(this.seed);

            foreach (var node in nodes)
            {
                int i = d;
                while (i > 0)
                {
                    int next = rnd.Next(nodes.Count);
                    if (nodes[next] != node)
                    {
                        node.ConnectTo(nodes[next], EdgeDirection.Outbound);
                        i--;
                    }
                }
            }

            logger.Info("Finished building connections between nodes. Every node has {0} outbound connections.", d);

            return nodes;
        }
    }
}
