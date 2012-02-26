using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ReviewR.Web.Models;
using ReviewR.Web.Services;
using ReviewR.Web.ViewModels;

namespace ReviewR.Web.Controllers
{
    [Authorize]
    public class ChangesController : Controller
    {
        public ReviewService Reviews { get; set; }
        public AuthenticationService Auth { get; set; }
        public DiffService Diff { get; set; }

        public ChangesController(ReviewService reviews, AuthenticationService auth, DiffService diff)
        {
            Reviews = reviews;
            Auth = auth;
            Diff = diff;
        }

        public ActionResult View(int id, int reviewId)
        {
            // Get the review
            Review r = Reviews.GetReview(reviewId);
            if (r == null || r.UserId != Auth.GetCurrentUserId())
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            // Get the change in question
            FileChange chg = r.Files.Where(c => c.Id == id).FirstOrDefault();
            if (chg == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            // Build a diff model for it
            DiffFileViewModel diffModel = Diff.CreateViewModelFromUnifiedDiff(chg.FileName, chg.Diff);
            diffModel.Id = chg.Id;

            // Return a view based on it
            ICollection<FolderChangeViewModel> folders = FileChangeViewModelMapper.MapFiles(r.Files);

            return View(new ChangeDetailViewModel()
            {
                Review = new ReviewDetailViewModel()
                {
                    Id = r.Id,
                    Name = r.Name,
                    Folders = folders,
                    Selected = folders.SelectMany(folder => folder.Files).Where(file => file.Id == id).FirstOrDefault()
                },
                Diff = diffModel
            });     
        }
    }
}
