using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Partition.Module.CLI
{
    class EnumSelectCommand<T> : CommandBase
    {
        public T Result { get; private set; }

        public EnumSelectCommand(char code, string desc)
            : base(code, desc)
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enum");
            }
        }

        public override void Execute()
        {
            bool ok = false;
            string[] names = Enum.GetNames(typeof(T));

            while (!ok)
            {
                Console.WriteLine("Select one:");
                for (int i = 0; i < names.Length; ++i)
                {
                    Console.WriteLine("{0} - {1}", i, names[i]);
                }

                var selected = Console.ReadLine();
                int index;
                if (ok = int.TryParse(selected, out index))
                {
                    this.Result = (T)Enum.Parse(typeof(T), names[index]);
                    break;
                }
            }
        }
    }
}
