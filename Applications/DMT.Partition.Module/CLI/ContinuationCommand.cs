using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Partition.Module.CLI
{
    class ContinuationCommand : CommandBase
    {
        private List<CommandBase> commands;
        private Action done;
        
        public ContinuationCommand(char code, string desc, params CommandBase[] cmds)
            :base(code, desc)
        {
            this.commands = cmds.ToList();
        }

        public override void Execute()
        {
            foreach (var cmd in this.commands)
            {
                cmd.Execute();
            }
            if (done != null)
            {
                done();
            }
        }

        public ContinuationCommand Then(CommandBase cmd)
        {
            this.commands.Add(cmd);
            return this;
        }

        public void Done(Action action)
        {
            this.done = action;
        }
    }
}
