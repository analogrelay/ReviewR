using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewR.Diff
{
    public class FileDiff : IEquatable<FileDiff>
    {
        public string OriginalFile { get; private set; }
        public string ModifiedFile { get; private set; }
        public ICollection<DiffHunk> Hunks { get; private set; }

        public FileDiff(string originalFile, string modifiedFile) : this(originalFile, modifiedFile, null) { }
        public FileDiff(string originalFile, string modifiedFile, params DiffHunk[] hunks) : this(originalFile, modifiedFile, (IEnumerable<DiffHunk>)hunks) { }
        public FileDiff(string originalFile, string modifiedFile, IEnumerable<DiffHunk> hunks)
        {
            OriginalFile = originalFile;
            ModifiedFile = modifiedFile;
            Hunks = hunks == null ? new List<DiffHunk>() : hunks.ToList();
        }

        public override bool Equals(object obj)
        {
            FileDiff other = obj as FileDiff;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            return (OriginalFile.GetHashCode() ^ ModifiedFile.GetHashCode() ^ Hunks.GetHashCode()).GetHashCode();
        }

        public bool Equals(FileDiff other)
        {
            return String.Equals(OriginalFile, other.OriginalFile, StringComparison.Ordinal) &&
                   String.Equals(ModifiedFile, other.ModifiedFile, StringComparison.Ordinal) &&
                   Enumerable.SequenceEqual(Hunks, other.Hunks);
        }
    }
}
