// Seriously, I actually need an assembly alias:
extern alias da;
        
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;
using ReviewR.Web.Infrastructure;

namespace ReviewR.Web.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "You must specify an email address")]
        [DisplayName("Email address")]
        [StringLength(maximumLength: 255, ErrorMessage = "Email address cannot be longer than 255 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "You must specify a password")]
        [DataType(DataType.Password)]
        [StringLength(maximumLength: Int32.MaxValue, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }

        [DisplayName("Remember me")]
        public bool RememberMe { get; set; }
    }
}