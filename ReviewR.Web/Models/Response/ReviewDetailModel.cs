using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Web.Models.Response
{
    public class ReviewDetailModel : ReviewModel
    {
        public string Description { get; set; }
        public IEnumerable<IterationModel> Iterations { get; set; }
        public bool Owner { get; set; }
    }
}
