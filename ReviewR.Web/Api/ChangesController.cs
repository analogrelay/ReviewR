using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Models;
using ReviewR.Web.Models.Data;
using ReviewR.Web.Models.Response;
using ReviewR.Web.Services;

namespace ReviewR.Web.Api
{
    public class ChangesController : ReviewRApiController
    {
        public ChangeService Changes { get; set; }
        public DiffService Diff { get; set; }

        public ChangesController(ChangeService changes, DiffService diff)
        {
            Changes = changes;
            Diff = diff;
        }

        public HttpResponseMessage Get(int id)
        {
            FileChange change = Changes.GetChange(id);
            if (change == null)
            {
                return NotFound();
            }
            DiffFileModel file = Diff.CreateViewModelFromUnifiedDiff(change.FileName, change.Diff);
            AttachComments(change, file);
            return Ok(file);
        }

        private void AttachComments(FileChange change, DiffFileModel file)
        {
            foreach (var groups in change.Comments.GroupBy(c => c.DiffLineIndex))
            {
                foreach (var comment in groups.OrderBy(c => c.PostedOn))
                {
                    file.Lines[groups.Key.Value].Comments.Add(new CommentModel()
                    {
                        Id = comment.Id,
                        Author = UserModel.FromUser(comment.User),
                        Body = comment.Content,
                        IsAuthor = comment.UserId == User.Identity.UserId,
                        PostedOn = comment.PostedOn.ToLocalTime()
                    });
                }
            }
        }

        private bool IsAuthorized(FileChange change)
        {
            Review r = change.Iteration.Review;
            return r.UserId == User.Identity.UserId ||
                   r.Participants.Any(p => p.UserId == User.Identity.UserId);
        }
    }
}