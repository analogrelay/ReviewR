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

        [UIHint("Code")]
        public string Diff { get; set; }
    
        public virtual Review Review { get; set; }
    }

    // EF doesn't like Enums, so we use sub-classes and EF's default Table-per-Hierarchy inheritance style
    public class FileAddition : FileChange
    {
    }

    public class FileDeletion : FileChange
    { 
    }

    public class FileModification : FileChange
    {
        [MaxLength(255)]
        public string NewFileName { get; set; }
    }
}
