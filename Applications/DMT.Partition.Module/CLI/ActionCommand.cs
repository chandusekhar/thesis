using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Partition.Module.CLI
{
    class ActionCommand : CommandBase
    {
        private Action action;

        public ActionCommand(char code, string desc, Action action)
            : base(code, desc)
        {
            this.action = action;
        }

        public override void Execute()
        {
            action();
        }
    }
}
