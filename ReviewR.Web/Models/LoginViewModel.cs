using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ReviewR.Web.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "You must specify an email address")]
        [DisplayName("Email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "You must specify a password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("Remember me")]
        public bool RememberMe { get; set; }
    }
}