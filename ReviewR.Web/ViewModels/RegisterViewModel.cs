using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReviewR.Web.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "You must specify an email address")]
        [DisplayName("Email address")]
        [StringLength(maximumLength: 255, ErrorMessage = "Email address cannot be longer than 255 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "You must specify a display name")]
        [DisplayName("Display name")]
        [StringLength(maximumLength: 255, ErrorMessage = "Display name cannot be longer than 255 characters")]
        public string DisplayName { get; set; }

        [Required(ErrorMessage = "You must specify a password")]
        [DataType(DataType.Password)]
        [StringLength(maximumLength: Int32.MaxValue, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "You must confirm your new password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password confirmation must match password")]
        [DisplayName("Confirm password")]
        public string ConfirmPassword { get; set; }
    }
}