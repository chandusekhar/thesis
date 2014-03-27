using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.VIR.Data.Parser
{
    class SemesterValuationConnector
    {
        private List<SemesterValuationWrapper> valuations;
        private Dictionary<int, UserWrapper> users;
        private Dictionary<int, GroupWrapper> groups;

        public SemesterValuationConnector(List<SemesterValuationWrapper> valuations, Dictionary<int, UserWrapper> users, Dictionary<int, GroupWrapper> groups)
        {
            this.valuations = valuations;
            this.users = users;
            this.groups = groups;
        }

        public void Connect()
        {
            foreach (var val in valuations)
            {
                val.Entity.ConnectTo(users[val.UserId].Entity, EdgeDirection.Both);
                val.Entity.ConnectTo(groups[val.GroupId].Entity, EdgeDirection.Both);
                ConnectNext(val);
            }
        }

        private void ConnectNext(SemesterValuationWrapper val)
        {
            if (val.NextValuationId < 0)
            {
                return;
            }

            var next = valuations.Find(v => v.ValuationId == val.NextValuationId && v.UserId == val.UserId);

            if (next != null)
            {
                val.Entity.ConnectTo(next.Entity, EdgeDirection.Both);
            }
        }
    }
}
