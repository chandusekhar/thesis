using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.VIR.Data.Parser
{
    class CommunitiyScoreConnector
    {
        private Dictionary<int, CommunityScoreWrapper> scores;
        private Dictionary<int, UserWrapper> users;

        public CommunitiyScoreConnector(Dictionary<int, CommunityScoreWrapper> scores, Dictionary<int, UserWrapper> users)
        {
            this.scores = scores;
            this.users = users;
        }

        public void Connect()
        {
            foreach (var s in scores.Values)
            {
                s.Entity.ConnectTo(users[s.UserId].Entity, EdgeDirection.Both);
            }
        }
    }
}
