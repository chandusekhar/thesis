using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.Matcher.Interfaces
{
    public delegate void FoundPartialMatchEventHandler(object sender, FoundPartialMatchEventArgs e);

    public class FoundPartialMatchEventArgs : EventArgs
    {
        public IId PartitionId { get; private set; }
        public IEnumerable<object> Matches { get; private set; }

        public FoundPartialMatchEventArgs(IEnumerable<object> matches, IId partitionId)
        {
            this.Matches = matches;
            this.PartitionId = partitionId;
        }
    }
}
