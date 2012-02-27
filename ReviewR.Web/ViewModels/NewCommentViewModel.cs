using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ReviewR.Web.ViewModels
{
    public class NewCommentViewModel
    {
        [Required]
        [DataType(DataType.MultilineText)]
        public string Body { get; set; }
    }
}
