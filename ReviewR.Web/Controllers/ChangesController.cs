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

        public ActionResult View(int id)
        {
            // Get the change
            FileChange chg = Reviews.GetChange(id);
            int userId = Auth.GetCurrentUserId();
            if (chg == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            // Get the review
            Review r = chg.Review;
            if (r == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            // Build a diff model for it
            DiffFileViewModel diffModel = Diff.CreateViewModelFromUnifiedDiff(chg.FileName, chg.Diff);
            diffModel.Id = chg.Id;

            // Attach comments
            foreach (Comment c in chg.Comments.Where(c => c.DiffLineIndex.HasValue && c.DiffLineIndex >= 0 && c.DiffLineIndex < diffModel.DiffLines.Count))
            {
                diffModel.DiffLines[c.DiffLineIndex.Value]
                         .Comments
                         .Add(new LineCommentViewModel()
                {
                     Id = c.Id,
                     AuthorName = c.User.DisplayName,
                     AuthorEmail = c.User.Email,
                     Body = c.Content,
                     PostedOn = c.PostedOn
                });
            }

            // Return a view based on it
            ReviewDetailViewModel review = ReviewHelpers.Map(r, userId, selectedFileId: id);
            
            return View(new ChangeDetailViewModel()
            {
                Review = review,
                Diff = diffModel
            });     
        }
    }
}
