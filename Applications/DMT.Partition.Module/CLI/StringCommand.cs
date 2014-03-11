using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Partition.Module.CLI
{
    class StringCommand : CommandBase
    {
        public string Question { get; private set; }
        public string Answer { get; private set; }

        public StringCommand(char code, string description, string question)
            : base(code, description)
        {
            this.Question = question;
        }

        public override void Execute()
        {
            Console.Write(this.Question + " ");
            this.Answer = Console.ReadLine();
        }
    }
}
