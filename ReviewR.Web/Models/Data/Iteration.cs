using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Web.Models.Data
{
    public class Iteration
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public string Description { get; set; }
        public DateTimeOffset StartedOn { get; set; }

        public virtual Review Review { get; set; }
        public virtual ICollection<FileChange> Files { get; set; }
    }
}
