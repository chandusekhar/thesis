using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Entities;
using DMT.Core.Interfaces;
using DMT.Core.Partition;

namespace DMT.Partition.Test
{
    static class SuperNodeTestFactory
    {
        public static SuperNode CreateWithChildren(int count, Func<IEntityFactory, INode> createFunc)
        {
            IEntityFactory f = new CoreEntityFactory();
            return CreateWithChildren(count, f, createFunc);
        }

        public static SuperNode CreateWithChildren(int count, IEntityFactory f, Func<IEntityFactory, INode> createFunc)
        {
            SuperNode sn = new SuperNode(f);

            for (int i = 0; i < count; i++)
            {
                sn.Nodes.Add(createFunc(f));
            }

            return sn;
        }

    }
}
