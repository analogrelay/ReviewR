using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ReviewR.Web.Models.Data
{
    public class Token
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public bool Persistent { get; set; }
        public string Value { get; set; }
        public DateTimeOffset Expires { get; set; }

        public virtual User User { get; set; }
    }
}
