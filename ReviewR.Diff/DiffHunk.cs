using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Diff
{
    public class DiffHunk
    {
        public SourceCoordinate OriginalLocation { get; private set; }
        public SourceCoordinate ModifiedLocation { get; private set; }
        public ICollection<DiffLine> Lines { get; private set; }

        public DiffHunk(SourceCoordinate originalLocation, SourceCoordinate modifiedLocation)
        {
            OriginalLocation = originalLocation;
            ModifiedLocation = modifiedLocation;
            Lines = new List<DiffLine>();
        }
    }
}
