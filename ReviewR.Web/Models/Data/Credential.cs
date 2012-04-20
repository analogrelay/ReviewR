using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Web.Models.Data
{
    public class Credential
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Provider { get; set; }
        public string Identifier { get; set; }

        public virtual User User { get; set; }
    }
}
