using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;

namespace ReviewR.Web.ViewModels
{
    public class NewReviewViewModel
    {
        [Required]
        [StringLength(maximumLength: 255, ErrorMessage = "Name must be less than 255 characters")]
        public string Name { get; set; }

        [Required]
        public HttpPostedFileBase Diff { get; set; }
    }
}
