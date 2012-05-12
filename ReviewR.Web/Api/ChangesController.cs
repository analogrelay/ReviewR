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
using VibrantUtils;

namespace ReviewR.Web.Api
{
    public class ChangesController : ReviewRApiController
    {
        public ChangeService Changes { get; set; }
        public DiffService Diff { get; set; }

        protected ChangesController() { }
        public ChangesController(ChangeService changes, DiffService diff)
        {
            Requires.NotNull(changes, "changes");
            Requires.NotNull(diff, "diff");

            Changes = changes;
            Diff = diff;
        }

        public HttpResponseMessage Get(int id)
        {
            Requires.InRange(id >= 0, "id");

            FileChange change = Changes.GetChange(id);
            if (change == null)
            {
                return NotFound();
            }
            FileDiffModel file = Diff.ParseFileDiff(change.FileName, change.Diff);
            AttachComments(change, file);
            return Ok(file);
        }

        private void AttachComments(FileChange change, FileDiffModel file)
        {
            if (change.Comments != null)
            {
                foreach (var groups in change.Comments.GroupBy(c => c.DiffLineIndex))
                {
                    if (groups.Key.Value >= 0 && groups.Key.Value < file.Lines.Count)
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
            }
        }
    }
}