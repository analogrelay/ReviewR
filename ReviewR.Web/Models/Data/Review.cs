using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReviewR.Web.Models.Data
{
    public class Review
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual User Creator { get; set; }
        public virtual ICollection<Participant> Participants { get; set; }
        public virtual ICollection<Iteration> Iterations { get; set; }
    }
}