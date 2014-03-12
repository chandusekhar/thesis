using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Partition.Module.CLI
{
    class QuitCommand : CommandBase
    {
        public bool Immediate { get; set; }

        public QuitCommand() : base('q', "Quit") { }

        public override void Execute()
        {
            Console.WriteLine("Byebye...");
            if (this.Immediate)
            {
                Environment.Exit(0);
            }
            else
            {
                PartitionModule.Instance.Exit();
            }
        }
    }
}
