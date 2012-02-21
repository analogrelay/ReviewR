using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Diff
{
    public class DiffLine
    {
        public DiffLineType Type { get; private set; }
        public string Content { get; private set; }

        public DiffLine(DiffLineType type, string content)
        {
            Type = type;
            Content = content;
        }
    }
}
