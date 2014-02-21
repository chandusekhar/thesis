using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Matcher.Module;

namespace DMT.Matcher.App
{
    class Program
    {
        static void Main(string[] args)
        {
            new MatcherModule().Start(args);
        }
    }
}
