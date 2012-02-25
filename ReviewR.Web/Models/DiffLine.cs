using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Web.Models
{
    public abstract class DiffLine
    {
        public int Id { get; set; }
        public int HunkId { get; set; }
        public string Content { get; set; }

        public virtual DiffHunk Hunk { get; set; }
    }

    public class DiffLineAdd : DiffLine { }
    public class DiffLineRemove : DiffLine { }
    public class DiffLineContext : DiffLine { }
}
