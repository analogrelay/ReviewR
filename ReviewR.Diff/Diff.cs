using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewR.Diff
{
    public class Diff
    {
        public string OriginalFile { get; private set; }
        public string ModifiedFile { get; private set; }
        public ICollection<DiffHunk> Hunks { get; private set; }

        public Diff(string originalFile, string modifiedFile)
        {
            OriginalFile = originalFile;
            ModifiedFile = modifiedFile;
            Hunks = new List<DiffHunk>();
        }
    }
}
