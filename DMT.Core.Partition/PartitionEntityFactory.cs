using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Serialization;
using DMT.Partition.Interfaces;

namespace DMT.Core.Partition
{
    [Export(typeof(IPartitionEntityFactory))]
    internal class PartitionEntityFactory : CoreEntityFactory, IPartitionEntityFactory
    {
        public IMultiNode CreateMultiNode()
        {
            throw new NotImplementedException();
        }
    }
}
