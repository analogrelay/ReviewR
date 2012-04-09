using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Diff
{
    public enum LineDiffType
    {
        Same = 0,
        Added = 1,
        Removed = 2,
        HunkHeader = 3
    }
}
