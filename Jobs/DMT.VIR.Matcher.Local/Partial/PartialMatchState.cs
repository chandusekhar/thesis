﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.VIR.Matcher.Local.Partial
{
    enum PartialMatchState
    {
        MatchFound,
        MatchNotFound,
        Pending,
        ReadyToStart,
        Running,
        Cancelled,
    }
}
