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
        public ICollection<DiffLine> Lines { get; private set; }

        public DiffHunk(SourceCoordinate originalLocation, SourceCoordinate modifiedLocation) : this(originalLocation, modifiedLocation, null) {}

        public DiffHunk(SourceCoordinate originalLocation, SourceCoordinate modifiedLocation, params DiffLine[] lines)
        {
            OriginalLocation = originalLocation;
            ModifiedLocation = modifiedLocation;
            Lines = lines == null ? new List<DiffLine>() : lines.ToList();
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
    }
}
