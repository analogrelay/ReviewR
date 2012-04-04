using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReviewR.Web.Models.Data
{
    public class Comment
    {
        public int Id { get; set; }
        public int FileId { get; set; }
        public int UserId { get; set; }
        public int? DiffLineIndex { get; set; }
        public string Content { get; set; }
        public DateTime PostedOn { get; set; }

        public virtual User User { get; set; }
        public virtual FileChange File { get; set; }
    }
}