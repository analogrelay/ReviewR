using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Web.ViewModels
{
    public class LineCommentViewModel
    {
        public int Id { get; set; }
        public string AuthorName { get; set; }
        public string AuthorEmail { get; set; }
        public DateTime PostedOn { get; set; }
        public string Body { get; set; }
    }
}
