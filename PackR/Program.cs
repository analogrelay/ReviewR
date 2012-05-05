using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VibrantCommandLine;

namespace PackR
{
    class Program : VibrantProgram
    {
        static int Main(string[] args)
        {
            return VibrantProgram.Run<Program>(args);
        }
    }
}
