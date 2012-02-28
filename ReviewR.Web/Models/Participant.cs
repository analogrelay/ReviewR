using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Web.Models
{
    public class Participant
    {
        public int Id { get; set; }
        public bool Required { get; set; }
        public int ReviewId { get; set; }
        public int UserId { get; set; }

        public virtual Review Review { get; set; }
        public virtual User User { get; set; }
    }
}
