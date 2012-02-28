using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using ReviewR.Web.ViewModels;

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
        public virtual ICollection<Comment> Comments { get; set; }

        [NotMapped]
        public abstract FileChangeType ChangeType { get; }

        public FileChange()
        {
            Comments = new List<Comment>();
        }
    }

    // EF doesn't like Enums, so we use sub-classes and EF's default Table-per-Hierarchy inheritance style
    public class FileAddition : FileChange
    {
        public override FileChangeType ChangeType
        {
            get { return FileChangeType.Added; }
        }
    }

    public class FileDeletion : FileChange
    {
        public override FileChangeType ChangeType
        {
            get { return FileChangeType.Removed; }
        }
    }

    public class FileModification : FileChange
    {
        [MaxLength(255)]
        public string NewFileName { get; set; }

        public override FileChangeType ChangeType
        {
            get { return FileChangeType.Modified; }
        }
    }
}
