using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.VIR.Data.Parser
{
    class SemesterValuationWrapper : EntityWrapper<SemesterValuation>
    {
        public int ValuationId { get; set; }
        public int NextValuationId { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }

        public Semester Semester { get; set; }
        public int Score { get; set; }
        public string Status { get; set; }


        public override SemesterValuation CreateEntity()
        {
            return new SemesterValuation(factory) { Semester = Semester, Score = Score, State = Status };
        }
    }
}
