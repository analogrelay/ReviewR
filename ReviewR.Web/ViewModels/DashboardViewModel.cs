using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReviewR.Web.ViewModels
{
    public class DashboardViewModel
    {
        public ICollection<ReviewViewModel> Reviews { get; set; }
    }
}