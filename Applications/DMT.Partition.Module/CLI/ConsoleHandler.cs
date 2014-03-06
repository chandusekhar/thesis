using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Partition.Module.CLI
{
    class ConsoleHandler
    {
        private List<CommandBase> commands;

        public CommandBase DefaultComamnd { get; set; }

        public bool HasDefault
        {
            get { return this.DefaultComamnd != null; }
        }

        public ConsoleHandler()
        {
            this.commands = new List<CommandBase>();
            this.commands.Add(new QuitCommand());
        }

        public ConsoleHandler(CommandBase @default, params CommandBase[] commands)
            : this((IEnumerable<CommandBase>)commands)
        {
            this.DefaultComamnd = @default;
        }

        public ConsoleHandler(params CommandBase[] commands) : this((IEnumerable<CommandBase>)commands) { }

        public ConsoleHandler(IEnumerable<CommandBase> commands)
            : this()
        {
            this.commands.AddRange(commands.Distinct(new CommandBase.CommandComparer()));
        }

        public void RegisterCommand(CommandBase command)
        {
            if (this.commands.Any(c => c.Code == command.Code))
            {
                throw new ArgumentException("The provided command's code has been already registered.");
            }

            this.commands.Add(command);
        }

        public void Execute()
        {
            var c = Choose();
            c.Execute();
        }

        private CommandBase Choose()
        {
            while (true)
            {
                PrintMenu();
                var code = Console.ReadKey();
                // add new line
                Console.WriteLine();
                if (this.HasDefault && code.Key == ConsoleKey.Enter)
                {
                    return this.DefaultComamnd;
                }

                var command = commands.FirstOrDefault(c => c.Code == code.KeyChar);
                if (command != null)
                {
                    return command;
                }

                Console.WriteLine("Invalid command...");
            }

        }

        private void PrintMenu()
        {
            Console.WriteLine("Choose from the following options:");
            foreach (var c in this.commands)
            {
                Console.WriteLine("{0} [{1}]", c.Description, c.Code);
            }

            if (this.DefaultComamnd != null)
            {
                Console.WriteLine("Default: {0} []", this.DefaultComamnd.Description);
            }

            Console.Write("Choose one: ");
        }

    }
}
