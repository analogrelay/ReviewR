using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ReviewR.Web.Models.Request
{
    public class ReviewRequestModel
    {
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
