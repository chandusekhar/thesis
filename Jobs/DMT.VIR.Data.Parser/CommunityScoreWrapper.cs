using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.VIR.Data.Parser
{
    class CommunityScoreWrapper : EntityWrapper<CommunityScore>
    {
        public int UserId { get; set; }
        public int Score { get; set; }
        public Semester Semester { get; set; }

        public override CommunityScore CreateEntity()
        {
            return new CommunityScore(factory) { Semester = Semester, Score = Score };
        }
    }
}
