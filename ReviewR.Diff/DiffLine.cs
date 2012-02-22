using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Diff
{
    public class DiffLine : IEquatable<DiffLine>
    {
        public DiffLineType Type { get; private set; }
        public string Content { get; private set; }

        public DiffLine(DiffLineType type, string content)
        {
            Type = type;
            Content = content;
        }

        public override bool Equals(object obj)
        {
            DiffLine other = obj as DiffLine;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            return (Type.GetHashCode() ^ Content.GetHashCode()).GetHashCode();
        }

        public bool Equals(DiffLine other)
        {
            return Type == other.Type &&
                   String.Equals(Content, other.Content, StringComparison.Ordinal);
        }
    }
}
