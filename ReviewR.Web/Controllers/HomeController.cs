using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ReviewR.Diff;
using ReviewR.Web.ViewModels;

namespace ReviewR.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(bool isAuthenticated)
        {
            return View(new DashboardViewModel() { Reviews = new List<ReviewViewModel>() });
        }
    }
}
