using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ReviewR.Diff
{
    internal class LineReader
    {
        private TextReader _inner;

        public string Current { get; private set; }
        public bool EndOfFile { get { return Current == null; } }
        
        public LineReader(TextReader inner)
        {
            _inner = inner;
        }

        public void NextLine()
        {
            Current = _inner.ReadLine();
        }
    }
}
