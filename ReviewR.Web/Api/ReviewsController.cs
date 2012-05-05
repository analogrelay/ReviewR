using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Models.Data;
using ReviewR.Web.Models.Response;
using ReviewR.Web.Services;
using ReviewR.Web.Models.Request;
using System.Net;

namespace ReviewR.Web.Api.Controllers
{
    public class ReviewsController : ReviewRApiController
    {
        public ReviewService Reviews { get; private set; }

        public ReviewsController(ReviewService reviews)
        {
            Reviews = reviews;
        }

        public HttpResponseMessage Get(int id)
        {
            Review review = Reviews.GetReview(id);

            if (review == null)
            {
                return NotFound();
            }
            else
            {
                // Filter iterations if this user isn't the owner
                bool owner = review.UserId == User.Identity.UserId;
                IEnumerable<Iteration> iters = review.Iterations;
                if (!owner)
                {
                    iters = review.Iterations.Where(i => i.Published);
                }

                return Ok(new ReviewDetailResponseModel()
                {
                    Id = review.Id,
                    Title = review.Name,
                    Author = UserModel.FromUser(review.Creator),
                    Description = review.Description,
                    Iterations = iters.OrderBy(i => i.StartedOn).Select((i, idx) => new IterationModel()
                    {
                        Id = i.Id,
                        Order = idx,
                        Published = i.Published,
                        Description = i.Description
                    }),
                    Owner = owner
                });
            }
        }

        // POST /api/reviews
        public HttpResponseMessage Post(ReviewRequestModel model)
        {
            if (ModelState.IsValid)
            {
                Review r = Reviews.CreateReview(model.Title, model.Description, User.Identity.UserId);
                return Created(new
                {
                    Id = r.Id,
                    Href = Url.Resource(r)
                });
            }
            return ValidationErrors();
        }
    }
}