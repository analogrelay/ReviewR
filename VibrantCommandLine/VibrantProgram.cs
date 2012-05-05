using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;

namespace VibrantCommandLine
{
    public abstract class VibrantProgram
    {
        [Import]
        public HelpCommandBase HelpCommand { get; set; }

        [ImportMany]
        public IEnumerable<ICommand> Commands { get; set; }

        [Import]
        public ICommandManager Manager { get; set; }

        public static int Run<T>(string[] args) where T : VibrantProgram, new()
        {
            var console = new VibrantCommandLine.Console();
            var p = new T();
            p.Initialize();

            // Add commands to the manager
            foreach (ICommand cmd in p.Commands)
            {
                p.Manager.RegisterCommand(cmd);
            }

            CommandLineParser parser = new CommandLineParser(p.Manager);

            // Parse the command
            ICommand command = parser.ParseCommandLine(args) ?? p.HelpCommand;

            // Fallback on the help command if we failed to parse a valid command
            if (!ArgumentCountValid(command))
            {
                // Get the command name and add it to the argument list of the help command
                string commandName = command.CommandAttribute.CommandName;

                // Print invalid command then show help
                console.WriteLine(Resources.InvalidArguments, commandName);

                p.HelpCommand.ViewHelpForCommand(commandName);
                return 0;
            }
            else
            {
                return command.Execute();
            }
        }

        private static bool ArgumentCountValid(ICommand command)
        {
            CommandAttribute attribute = command.CommandAttribute;
            return command.Arguments.Count >= attribute.MinArgs &&
                   command.Arguments.Count <= attribute.MaxArgs;
        }

        protected virtual void Initialize()
        {
            using (var catalog = new AggregateCatalog(new AssemblyCatalog(GetType().Assembly), new AssemblyCatalog(typeof(VibrantProgram).Assembly)))
            {
                using (var container = new CompositionContainer(catalog))
                {
                    container.ComposeParts(this);
                }
            }
        }
    }
}
