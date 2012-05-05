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
        public ReviewService Reviews { get; set; }

        public CommentsController(ReviewService reviews)
        {
            Reviews = reviews;
        }

        public HttpResponseMessage Delete(int id)
        {
            bool? result = Reviews.DeleteComment(id, User.Identity.UserId);
            if (result == null)
            {
                return NotFound();
            }
            else if (!result.Value)
            {
                return Forbidden();
            }
            return NoContent();
        }

        public HttpResponseMessage Post(int changeId, int line, string body)
        {
            Comment cmt = Reviews.CreateComment(changeId, line, body, User.Identity.UserId);
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