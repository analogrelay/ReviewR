using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace ReviewR.Web.Models.Data
{
    public abstract class FileChange
    {
        public int Id { get; set; }
        public int IterationId { get; set; }

        [StringLength(255)]
        public string FileName { get; set; }

        [StringLength(255)]
        public string NewFileName { get; set; }


        public abstract FileChangeType ChangeType { get; }

        [UIHint("Code")]
        public string Diff { get; set; }

        public virtual Iteration Iteration { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

        [NotMapped]
        public string DisplayFileName { get { return String.IsNullOrEmpty(NewFileName) ? FileName : NewFileName; } }
    }

    public class FileAddition : FileChange
    {
        public override FileChangeType ChangeType
        {
            get { return FileChangeType.Added; }
        }
    }

    public class FileModification : FileChange
    {
        public override FileChangeType ChangeType
        {
            get { return FileChangeType.Modified; }
        }
    }

    public class FileRemoval : FileChange
    {
        public override FileChangeType ChangeType
        {
            get { return FileChangeType.Removed; }
        }
    }

}
