using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ReviewR.Web.Models.Data
{
    public class User
    {
        public virtual int Id { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [MaxLength(255)]
        public virtual string Email { get; set; }

        [MaxLength(255)]
        public virtual string Password { get; set; }
        [MaxLength(255)]
        public virtual string PasswordSalt { get; set; }

        [Required]
        [MaxLength(255)]
        public virtual string DisplayName { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}