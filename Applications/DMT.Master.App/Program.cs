using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Master.Module;

namespace DMT.Master.App
{
    class Program
    {
        static void Main(string[] args)
        {
            new MasterModule().Start(args);
        }
    }
}
