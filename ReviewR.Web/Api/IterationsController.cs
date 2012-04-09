using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Models.Response;
using ReviewR.Web.Models.Request;
using ReviewR.Web.Models.Data;
using ReviewR.Web.Services;

namespace ReviewR.Web.Api
{
    public class IterationsController : ReviewRApiController
    {
        public ReviewService Reviews { get; set; }

        public IterationsController(ReviewService reviews)
        {
            Reviews = reviews;
        }

        public HttpResponseMessage Post(int reviewId)
        {
            Iteration iter = Reviews.AddIteration(reviewId, User.Identity.Id);
            if (iter == null)
            {
                return Forbidden();
            }
            return Created(new
            {
                Id = iter.Id,
                Href = Url.Route("DefaultApi", new { controller = "Iterations", id = iter.Id })
            });
        }

        public HttpResponseMessage Delete(int id)
        {
            bool? result = Reviews.DeleteIteration(id, User.Identity.Id);
            if (result == null)
            {
                return NotFound();
            }
            else if (result.Value)
            {
                return Ok();
            }
            else
            {
                return Forbidden();
            }
        }
    }
}