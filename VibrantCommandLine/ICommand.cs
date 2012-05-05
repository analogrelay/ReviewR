using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace VibrantCommandLine
{
    [InheritedExport]
    public interface ICommand
    {
        CommandAttribute CommandAttribute { get; }

        IList<string> Arguments { get; }

        int Execute();
    }
}
