using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Diff
{
    public class DiffHunk : IEquatable<DiffHunk>
    {
        public SourceCoordinate OriginalLocation { get; private set; }
        public SourceCoordinate ModifiedLocation { get; private set; }
        public ICollection<LineDiff> Lines { get; private set; }
        public string Comment { get; private set; }

        public DiffHunk(SourceCoordinate originalLocation, SourceCoordinate modifiedLocation, string comment) : this(originalLocation, modifiedLocation, comment, null) {}
        public DiffHunk(SourceCoordinate originalLocation, SourceCoordinate modifiedLocation, string comment, params LineDiff[] lines)
        {
            OriginalLocation = originalLocation;
            ModifiedLocation = modifiedLocation;
            Comment = comment;
            Lines = lines == null ? new List<LineDiff>() : lines.ToList();
        }

        public override bool Equals(object obj)
        {
            DiffHunk other = obj as DiffHunk;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            return (OriginalLocation.GetHashCode() ^ ModifiedLocation.GetHashCode() ^ Lines.GetHashCode()).GetHashCode();
        }

        public bool Equals(DiffHunk other)
        {
            return OriginalLocation.Equals(other.OriginalLocation) &&
                   ModifiedLocation.Equals(other.ModifiedLocation) &&
                   Enumerable.SequenceEqual(Lines, other.Lines);
        }

        public override string ToString()
        {
            return String.Format(
                "@@ -{0},{1} +{2},{3} @@{4}",
                OriginalLocation.Line,
                OriginalLocation.Column,
                ModifiedLocation.Line,
                ModifiedLocation.Column,
                String.IsNullOrEmpty(Comment) ? String.Empty : (" " + Comment));
        }
    }
}
