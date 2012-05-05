using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Web.Models.Response
{
    public class CommentModel
    {
        public int Id { get; set; }
        public UserModel Author { get; set; }
        public string Body { get; set; }
        public bool IsAuthor { get; set; }
        public DateTime PostedOn { get; set; }
    }
}
