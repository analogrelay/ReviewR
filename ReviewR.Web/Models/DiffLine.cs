using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Web.Models
{
    public abstract class DiffLine
    {
        public int Id { get; set; }
        public int ChangeId { get; set; }
        public string Content { get; set; }
        public int SourceLine { get; set; }
        public int ModifiedLine { get; set; }

        public virtual FileChange Change { get; set; }
    }

    public class DiffLineAdd : DiffLine { }
    public class DiffLineRemove : DiffLine { }
    public class DiffLineContext : DiffLine { }
}
