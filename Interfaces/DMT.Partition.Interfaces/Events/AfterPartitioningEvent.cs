using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Partition.Interfaces.Events
{
    public delegate void AfterPartitioningEventHandler(object sender, AfterPartitioningEventArgs e);

    public class AfterPartitioningEventArgs : EventArgs
    {
        public IEnumerable<IPartition> Partitions { get; private set; }

        public AfterPartitioningEventArgs(IEnumerable<IPartition> partitions)
        {
            this.Partitions = partitions;
        }
    }
}
