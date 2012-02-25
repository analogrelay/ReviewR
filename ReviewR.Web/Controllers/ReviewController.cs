using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using ReviewR.Web.ViewModels;

namespace ReviewR.Web.Controllers
{
    public class ReviewController : Controller
    {
        public ActionResult New()
        {
            return View(new NewReviewViewModel());
        }
    }
}
