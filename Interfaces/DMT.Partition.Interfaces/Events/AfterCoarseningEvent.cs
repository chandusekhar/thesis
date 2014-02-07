using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Partition.Interfaces.Events
{
    public delegate void AfterCoarseningEventHandler(object sender, AfterCoarseningEventArgs e);
    
    public class AfterCoarseningEventArgs : EventArgs
    {
        public IEnumerable<ISuperNode> CoarsenedNodes { get; private set; }

        public AfterCoarseningEventArgs(IEnumerable<ISuperNode> nodes)
        {
            this.CoarsenedNodes = nodes;
        }
    }
}
