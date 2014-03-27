using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.VIR.Data.Parser
{
    class MembershipWrapper : EntityWrapper<Membership>
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public List<string> Posts { get; set; }

        public int UserId { get; set; }
        public int GroupId { get; set; }

        public override Membership CreateEntity()
        {
            return new Membership(factory) { End = End, Posts = Posts.ToArray(), Start = Start };
        }
    }
}
