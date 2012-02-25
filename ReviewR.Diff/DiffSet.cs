using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Diff
{
    public class DiffSet : IEquatable<DiffSet>
    {
        public ICollection<FileDiff> Files { get; private set; }

        public DiffSet(params FileDiff[] files) : this(files.ToList()) { }
        public DiffSet(ICollection<FileDiff> files)
        {
            Files = files;
        }

        public override bool Equals(object obj)
        {
            DiffSet other = obj as DiffSet;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            return Files.GetHashCode();
        }

        public bool Equals(DiffSet other)
        {
            return Enumerable.SequenceEqual(Files, other.Files);
        }
    }
}
