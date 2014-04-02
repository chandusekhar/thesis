using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Matcher.Interfaces;

namespace DMT.VIR.Matcher.Local
{
    class VirDependencyProvider : IMatcherDependencyProvider
    {
        public string[] GetDependencies()
        {
            return new[] { "DMT.VIR.Data", "DMT.Core" };
        }
    }
}
