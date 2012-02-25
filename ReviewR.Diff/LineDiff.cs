using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Diff
{
    public class LineDiff : IEquatable<LineDiff>
    {
        public LineDiffType Type { get; private set; }
        public string Content { get; private set; }

        public LineDiff(LineDiffType type, string content)
        {
            Type = type;
            Content = content;
        }

        public override bool Equals(object obj)
        {
            LineDiff other = obj as LineDiff;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            return (Type.GetHashCode() ^ Content.GetHashCode()).GetHashCode();
        }

        public bool Equals(LineDiff other)
        {
            return Type == other.Type &&
                   String.Equals(Content, other.Content, StringComparison.Ordinal);
        }
    }
}
