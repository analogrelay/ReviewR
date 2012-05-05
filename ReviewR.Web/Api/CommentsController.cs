using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Models.Data;
using ReviewR.Web.Models.Response;
using ReviewR.Web.Services;

namespace ReviewR.Web.Api
{
    public class CommentsController : ReviewRApiController
    {
        public CommentService Comments { get; set; }

        public CommentsController(CommentService comments)
        {
            Comments = comments;
        }

        public HttpResponseMessage Delete(int id)
        {
            var result = Comments.DeleteComment(id, User.Identity.UserId);
            if (result == DatabaseActionResult.ObjectNotFound)
            {
                return NotFound();
            }
            else if (result == DatabaseActionResult.Forbidden)
            {
                return Forbidden();
            }
            return NoContent();
        }

        public HttpResponseMessage Post(int changeId, int line, string body)
        {
            Comment cmt = Comments.CreateComment(changeId, line, body, User.Identity.UserId);
            if (cmt == null)
            {
                return NotFound();
            }

            return Created(new CommentModel()
            {
                Id = cmt.Id,
                Body = cmt.Content,
                Author = UserModel.FromUser(User),
                PostedOn = cmt.PostedOn,
                IsAuthor = true // You created it, you own it!
            });
        }
    }
}