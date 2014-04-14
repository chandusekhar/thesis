using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;
using DMT.Matcher.Data.Interfaces;

namespace DMT.Matcher.Interfaces
{
    public delegate void FoundPartialMatchEventHandler(object sender, FoundPartialMatchEventArgs e);

    public class FoundPartialMatchEventArgs : EventArgs
    {
        public IId PartitionId { get; private set; }
        public IEnumerable<IPattern> Matches { get; private set; }

        public FoundPartialMatchEventArgs(IEnumerable<IPattern> matches, IId partitionId)
        {
            this.Matches = matches;
            this.PartitionId = partitionId;
        }
    }
}
