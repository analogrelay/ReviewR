using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace ReviewR.Web.Models.Data
{
    public class FileChange
    {
        public int Id { get; set; }
        public int IterationId { get; set; }
        
        [MaxLength(255)]
        public string FileName { get; set; }

        [MaxLength(255)]
        public string NewFileName { get; set; }

        
        public FileChangeType ChangeType { get; set; }

        [UIHint("Code")]
        public string Diff { get; set; }
    
        public virtual Iteration Iteration { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

        [NotMapped]
        public string DisplayFileName { get { return String.IsNullOrEmpty(NewFileName) ? FileName : NewFileName; } }
    }
}
