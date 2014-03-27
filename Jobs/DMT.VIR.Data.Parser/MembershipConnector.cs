using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.VIR.Data.Parser
{
    class MembershipConnector
    {
        Dictionary<int, MembershipWrapper> memberships;
        Dictionary<int, GroupWrapper> groups;
        Dictionary<int, UserWrapper> users;

        public MembershipConnector(Dictionary<int, MembershipWrapper> memberships, Dictionary<int, GroupWrapper> groups, Dictionary<int, UserWrapper> users)
        {
            this.memberships = memberships;
            this.users = users;
            this.groups = groups;
        }

        public void Connect()
        {
            foreach (var ms in memberships.Values)
            {
                ms.Entity.ConnectTo(users[ms.UserId].Entity, EdgeDirection.Both);
                ms.Entity.ConnectTo(groups[ms.GroupId].Entity, EdgeDirection.Both);
            }
        }
    }
}
