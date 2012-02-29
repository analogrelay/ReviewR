using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ReviewR.Web.Models
{
    public class Role
    {
        public virtual int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public virtual string RoleName { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}