using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ReviewR.Web.Models
{
    public abstract class FileChange
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }
        
        [MaxLength(255)]
        public string FileName { get; set; }

        public virtual Review Review { get; set; }
    }

    public abstract class FileContentChange : FileChange
    {
        public string Content { get; set; }
    }

    // EF doesn't like Enums, so we use sub-classes and EF's default Table-per-Hierarchy inheritance style
    public class FileAddition : FileContentChange
    {
    }

    public class TextFileAddition : FileAddition
    {
    }

    public class FileDeletion : FileContentChange
    { 
    }

    public class TextFileDeletion : FileDeletion
    {
    }

    public class FileModification : FileContentChange
    {
        [MaxLength(255)]
        public string NewFileName { get; set; }
        public string NewContent { get; set; }
    }

    public class TextFileModification : FileChange
    {
        public virtual ICollection<DiffHunk> Hunks { get; set; }
    }
}
