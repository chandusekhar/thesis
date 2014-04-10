using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.VIR.Data;

namespace DMT.VIR.Matcher.Local.Patterns
{
    static class PatternCriteria
    {
        public const string GroupLeaderPost = "körvezető";
        public const string ExGroupLeaderPost = "volt körvezető";
        public const int CommunityScoreThreshold = 60;

        public static Semester Semester
        {
            get
            {
                return new Semester(2012, 2013, Semester.SemesterPeriod.Spring);
            }
        }
    }
}
