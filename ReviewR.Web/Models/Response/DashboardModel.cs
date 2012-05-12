using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Web.Models.Response
{
    public class DashboardModel
    {
        public IEnumerable<ReviewModel> Created { get; set; }
        public IEnumerable<ReviewModel> Assigned { get; set; }
    }
}
