using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Diff
{
    public struct SourceCoordinate
    {
        public int Line { get; private set; }
        public int Column { get; private set; }

        public SourceCoordinate(int line, int column) : this()
        {
            Line = line;
            Column = column;
        }
    }
}
