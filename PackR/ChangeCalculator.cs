using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;
using PackR.Commands;

namespace PackR
{
    public static class ChangeCalculator
    {
        public static IEnumerable<Change> Calculate(Tree start, Tree end)
        {
            return Calculate(start, end, String.Empty).SelectMany(c => c);
        }

        private static IEnumerable<IEnumerable<Change>> Calculate(Tree start, Tree end, string basePath)
        {
            foreach (TreeEntry original in start)
            {
                TreeEntry modified = end[original.Name];
                
            }
        }
    }
}
