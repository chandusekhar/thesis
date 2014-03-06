using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Partition.Module.CLI
{
    class QuitCommand : CommandBase
    {
        public QuitCommand() : base('q', "Quit") { }

        public override void Execute()
        {
            Console.WriteLine("Byebye...");
            PartitionModule.Instance.Exit();
        }
    }
}
