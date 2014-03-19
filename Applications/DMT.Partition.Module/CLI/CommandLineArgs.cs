using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Matcher.Interfaces;
using NDesk.Options;

namespace DMT.Partition.Module.CLI
{
    class CommandLineArgs
    {
        private string[] args;
        private bool showHelp = false;

        public string ModelFilePath { get; private set; }
        public string JobBinaryPath { get; private set; }
        public MatchMode? MatchMode { get; private set; }

        public CommandLineArgs(string[] args)
        {
            this.args = args;
        }

        public void Parse()
        {
            var p = new OptionSet()
            {
                { "m|model=", "Path of the model file.", v => this.ModelFilePath = v },
                { "j|job=", "Path of the first job's binary.", v => this.JobBinaryPath = v },
                { "h|?|help", "Show this message.", v => this.showHelp = v != null },
                { "mode=", "Mode of the matcher jobs. (either all or firstonly)", var => this.MatchMode = (MatchMode)Enum.Parse(typeof(MatchMode), var, true) },
            };

            p.Parse(this.args);

            if (this.showHelp)
            {
                Console.WriteLine("Usage: {0} [OPTIONS]", AppDomain.CurrentDomain.FriendlyName);
                p.WriteOptionDescriptions(Console.Out);
                Environment.Exit(0);
            }

            if (string.IsNullOrEmpty(this.ModelFilePath))
            {
                AskForModelPath();
            }
            if (string.IsNullOrEmpty(this.JobBinaryPath))
            {
                AskForJobBinaryPath();
            }
        }

        private void AskForModelPath()
        {
            var cmd = new StringCommand('m', "Enter the path of model file", "Path:");
            new ConsoleHandler(cmd, cmd) { ImmediateExit = true }.Execute();
            this.ModelFilePath = cmd.Answer;
        }

        private void AskForJobBinaryPath()
        {
            var cmd = new StringCommand('j', "Enter the path of the first job's binary", "Path:");
            new ConsoleHandler(cmd, cmd) { ImmediateExit = true }.Execute();
            this.JobBinaryPath = cmd.Answer;
        }
    }
}
