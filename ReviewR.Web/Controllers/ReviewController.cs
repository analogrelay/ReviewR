using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using ReviewR.Web.Models;
using ReviewR.Web.Services;
using ReviewR.Web.ViewModels;

namespace ReviewR.Web.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        public DiffService Diff { get; set; }
        public IDataRepository Data { get; set; }
        public AuthenticationService Auth { get; set; }

        public ReviewController(DiffService diff, AuthenticationService auth, IDataRepository data)
        {
            Diff = diff;
            Data = data;
            Auth = auth;
        }

        [HttpGet]
        public ActionResult New()
        {
            return View(new NewReviewViewModel());
        }

        [HttpPost]
        public ActionResult New(NewReviewViewModel model)
        {
            if (ModelState.IsValid)
            {
                IEnumerable<FileChange> changes = null;
                try
                {
                    using (StreamReader rdr = new StreamReader(model.Diff.InputStream))
                    {
                        changes = Diff.CreateFromGitDiff(rdr);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Diff", "An error occurred reading the diff: " + ex.Message);
                }
                if (changes != null)
                {
                    // Create the review
                    Review r = new Review()
                    {
                        Name = model.Name,
                        Files = changes.ToList(),
                        UserId = Auth.GetCurrentUserId()
                    };
                    Data.Reviews.Add(r);
                    Data.SaveChanges();
                    return RedirectToAction("View", "Review", new { id = r.Id });
                }
            }
            return View(model);
        }

        public ActionResult View(int id)
        {
            return Content("View Review " + id.ToString());
        }
    }
}
