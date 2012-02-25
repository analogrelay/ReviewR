using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Web.Models
{
    public class DiffHunk
    {
        public int Id { get; set; }
        public int ChangeId { get; set; }
        public int SourceLine { get; set; }
        public int ModifiedLine { get; set; }

        public virtual TextFileModification Change { get; set; }
        public virtual ICollection<DiffLine> Lines { get; set; }
    }
}
