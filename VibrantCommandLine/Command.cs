using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Threading;

namespace VibrantCommandLine
{
    public abstract class Command : ICommand
    {
        private const string CommandSuffix = "Command";
        private CommandAttribute _commandAttribute;

        protected Command()
        {
            Arguments = new List<string>();
        }

        public IList<string> Arguments { get; private set; }

        [Import]
        public IConsole Console { get; set; }

        [Import]
        public HelpCommandBase HelpCommand { get; set; }

        [Import]
        public ICommandManager Manager { get; set; }

        [Option("help", AltName = "?")]
        public bool Help { get; set; }

#if DEBUG
        [Option("debug", AltName = "d")]
        public bool Debug { get; set; }
#endif

        public CommandAttribute CommandAttribute
        {
            get
            {
                if (_commandAttribute == null)
                {
                    _commandAttribute = GetCommandAttribute();
                }
                return _commandAttribute;
            }
        }

        public int Execute()
        {
            WaitForDebugger();
            if (Help)
            {
                HelpCommand.ViewHelpForCommand(CommandAttribute.CommandName);
                return 0;
            }
            else
            {
                if (Debug)
                {
                    return ExecuteCommand();
                }
                else
                {
                    try
                    {
                        return ExecuteCommand();
                    }
                    catch (Exception e)
                    {
                        Console.WriteError(e.Message);
                        return 1;
                    }
                }
            }
        }

        public abstract int ExecuteCommand();

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method does quite a bit of processing.")]
        public virtual CommandAttribute GetCommandAttribute()
        {
            var attributes = GetType().GetCustomAttributes(typeof(CommandAttribute), true);
            if (attributes.Any())
            {
                return (CommandAttribute)attributes.FirstOrDefault();
            }

            // Use the command name minus the suffix if present and default description
            string name = GetType().Name;
            int idx = name.LastIndexOf(CommandSuffix, StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
            {
                name = name.Substring(0, idx);
            }
            if (!String.IsNullOrEmpty(name))
            {
                return new CommandAttribute(name, Resources.DefaultCommandDescription);
            }
            return null;
        }

        [Conditional("DEBUG")]
        protected void WaitForDebugger()
        {
#if DEBUG
            if (Debug)
            {
                Console.WriteLine("Waiting for Debugger...");
                while (!Debugger.IsAttached) { Thread.Yield(); }
                Console.WriteLine("Debugger attached");
            }
#endif
        }
    }
}
