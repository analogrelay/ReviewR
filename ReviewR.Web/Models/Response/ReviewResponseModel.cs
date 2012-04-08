using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;

namespace ReviewR.Web.Models.Response
{
    public class ReviewResponseModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public UserModel Author { get; set; }
    }
}