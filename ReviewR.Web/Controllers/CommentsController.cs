using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using ReviewR.Web.Models;
using ReviewR.Web.Services;
using ReviewR.Web.ViewModels;

namespace ReviewR.Web.Controllers
{
    public class CommentsController : Controller
    {
        public ReviewService Reviews { get; set; }
        public AuthenticationService Auth { get; set; }

        public CommentsController(ReviewService reviews, AuthenticationService auth)
        {
            Reviews = reviews;
            Auth = auth;
        }

        [HttpGet]
        public ActionResult New(int id, int? line)
        {
            // Get the change
            FileChange chg = Reviews.GetChange(id);
            if (chg == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            // Check the line number if specified
            if (line == null || line.Value < 0 || line.Value >= chg.Diff.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Length)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            // Return the view
            return View(new NewCommentViewModel());
        }

        [HttpPost]
        public ActionResult New(int id, int? line, NewCommentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Get the change
            FileChange chg = Reviews.GetChange(id);
            if (chg == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            // Check the line number if specified
            if (line == null || line.Value < 0 || line.Value >= chg.Diff.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Length)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            Reviews.CreateComment(chg.Id, line, Auth.GetCurrentUserId(), model.Body);
            return RedirectToAction("View", "Changes", new { id = chg.Id });
        }

        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            // Get the comment
            Comment c = Reviews.GetComment(id);
            if (c == null || c.UserId != Auth.GetCurrentUserId())
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            Reviews.DeleteComment(c);
            return RedirectToAction("View", "Changes", new { id = c.FileId });
        }
    }
}
