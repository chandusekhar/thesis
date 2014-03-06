using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Partition.Module.CLI
{
    abstract class CommandBase
    {
        private readonly char code;
        private readonly string description;

        public char Code { get { return this.code; } }
        public string Description { get { return this.description; } }

        protected CommandBase(char code, string description)
        {
            this.code = code;
            this.description = description;
        }

        public abstract void Execute();

        public class CommandComparer : IEqualityComparer<CommandBase>
        {
            public bool Equals(CommandBase x, CommandBase y)
            {
                return x.Code == y.Code;
            }

            public int GetHashCode(CommandBase obj)
            {
                return obj.Code.GetHashCode();
            }
        }
    }
}
