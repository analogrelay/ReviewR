using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VibrantCommandLine;

namespace PackR.Commands
{
    [Export(typeof(HelpCommandBase))]
    [Command(typeof(PackRResources), "help", "HelpCommandDescription", AltName = "?", MaxArgs = 1,
        UsageSummaryResourceName = "HelpCommandUsageSummary", UsageDescriptionResourceName = "HelpCommandUsageDescription",
        UsageExampleResourceName = "HelpCommandUsageExamples")]
    public class HelpCommand : HelpCommandBase
    {
        [ImportingConstructor]
        public HelpCommand(ICommandManager command)
            : base(command, typeof(HelpCommand).Assembly.GetName().Name, "packr") { }
    }
}
