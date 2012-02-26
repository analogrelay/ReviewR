using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using ReviewR.Web.Models;
using ReviewR.Web.Services;
using ReviewR.Web.ViewModels;

namespace ReviewR.Web.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        public DiffService Diff { get; set; }
        public ReviewService Reviews { get; set; }
        public AuthenticationService Auth { get; set; }

        public ReviewsController(DiffService diff, AuthenticationService auth, ReviewService reviews)
        {
            Diff = diff;
            Reviews = reviews;
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
                    Review created = Reviews.CreateReview(model.Name, changes.ToList(), Auth.GetCurrentUserId());
                    return RedirectToAction("View", "Reviews", new { id = created.Id });
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Index()
        {
            // Get the users reviews
            IEnumerable<Review> reviews = Reviews.GetReviewsCreatedBy(userId: Auth.GetCurrentUserId());
            return View(new DashboardViewModel() { Reviews = reviews.Select(r => new ReviewSummaryViewModel() { Id = r.Id, Name = r.Name }).ToList() });
        }

        [HttpGet]
        public ActionResult View(int id)
        {
            Review review = Reviews.GetReview(id);
            if (review == null || review.UserId != Auth.GetCurrentUserId())
            {
                // No such review, or user not authorized
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            return View(new ReviewDetailViewModel()
            {
                Id = review.Id,
                Name = review.Name,
                Folders = FileChangeViewModelMapper.MapFiles(review.Files)
            });
        }
    }
}
