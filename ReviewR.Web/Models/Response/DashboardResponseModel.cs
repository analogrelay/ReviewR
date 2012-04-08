using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Web.Models.Response
{
    public class DashboardResponseModel
    {
        public IEnumerable<ReviewResponseModel> Created { get; set; }
        public IEnumerable<ReviewResponseModel> Assigned { get; set; }
    }
}
