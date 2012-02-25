using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Web.Models
{
    public abstract class FileChange
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public string FileName { get; set; }

        public virtual Review Review { get; set; }
    }

    // EF doesn't like Enums, so we use sub-classes and EF's default Table-per-Hierarchy inheritance style
    public class FileAddition : FileChange
    {
    }

    public class TextFileAddition : FileAddition
    {
        public string Content { get; set; }
    }

    public class FileDeletion : FileChange
    { 
    }

    public class TextFileDeletion : FileDeletion
    {
        public string Content { get; set; }
    }

    public class FileModification : FileChange
    {
        public string NewFileName { get; set; }
    }

    public class TextFileModification : FileChange
    {
        public virtual ICollection<DiffHunk> Hunks { get; set; }
    }
}
