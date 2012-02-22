using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Diff
{
    public struct SourceCoordinate : IEquatable<SourceCoordinate>
    {
        public int Line { get; private set; }
        public int Column { get; private set; }

        public SourceCoordinate(int line, int column) : this()
        {
            Line = line;
            Column = column;
        }
        
        public override bool Equals(object obj)
        {
            return Equals((SourceCoordinate)obj);
        }

        public override int GetHashCode()
        {
            return (Line.GetHashCode() ^ Line.GetHashCode()).GetHashCode();
        }

        public bool Equals(SourceCoordinate other)
        {
            return Line == other.Line && Column == other.Column;
        }
    }
}
