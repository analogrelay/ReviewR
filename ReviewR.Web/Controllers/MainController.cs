using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ReviewR.Diff;
using ReviewR.Web.Services;
using ReviewR.Web.ViewModels;

namespace ReviewR.Web.Controllers
{
    public class MainController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Template(string template)
        {
            return View(template);
        }
    }
}
