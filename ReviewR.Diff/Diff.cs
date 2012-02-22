using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewR.Diff
{
    public class Diff : IEquatable<Diff>
    {
        public string OriginalFile { get; private set; }
        public string ModifiedFile { get; private set; }
        public ICollection<DiffHunk> Hunks { get; private set; }

        public Diff(string originalFile, string modifiedFile) : this(originalFile, modifiedFile, null) { }
        public Diff(string originalFile, string modifiedFile, params DiffHunk[] hunks)
        {
            OriginalFile = originalFile;
            ModifiedFile = modifiedFile;
            Hunks = hunks == null ? new List<DiffHunk>() : hunks.ToList();
        }

        public override bool Equals(object obj)
        {
            Diff other = obj as Diff;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            return (OriginalFile.GetHashCode() ^ ModifiedFile.GetHashCode() ^ Hunks.GetHashCode()).GetHashCode();
        }

        public bool Equals(Diff other)
        {
            return String.Equals(OriginalFile, other.OriginalFile, StringComparison.Ordinal) &&
                   String.Equals(ModifiedFile, other.ModifiedFile, StringComparison.Ordinal) &&
                   Enumerable.SequenceEqual(Hunks, other.Hunks);
        }
    }
}
