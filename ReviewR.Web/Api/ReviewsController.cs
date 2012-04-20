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

        // GET /api/reviews
        public DashboardResponseModel Get()
        {
            // Get all reviews created by this user
            //IEnumerable<Review> reviews = Reviews.GetReviewsCreatedBy(User.Identity.Id).ToArray();
            //return new DashboardResponseModel()
            //{
            //    Created = reviews.Select(ConvertReview)
            //};
            throw new NotImplementedException();
        }

        public HttpResponseMessage Get(int id)
        {
            //Review review = Reviews.GetReview(id);
            //if (review == null)
            //{
            //    return NotFound();
            //}
            //else if (review.UserId != User.Identity.Id && !review.Participants.Any(p => p.UserId == User.Identity.Id))
            //{
            //    return Forbidden();
            //}
            //else
            //{
            //    return Ok(new ReviewDetailResponseModel()
            //    {
            //        Id = review.Id,
            //        Title = review.Name,
            //        Author = UserModel.FromUser(review.Creator),
            //        Description = review.Description,
            //        Iterations = review.Iterations.OrderBy(i => i.StartedOn).Select((i, idx) => new IterationModel() {
            //            Id = i.Id,
            //            Order = idx,
            //            Description = i.Description
            //        }),
            //        Participants = review.Participants.Select(p => new ParticipantModel() {
            //            User = UserModel.FromUser(p.User),
            //            Status = p.Status,
            //            Required = p.Required
            //        })
            //    });
            //}
            throw new NotImplementedException();
        }

        // POST /api/reviews
        public HttpResponseMessage Post(ReviewRequestModel model)
        {
            //if (ModelState.IsValid)
            //{
            //    Review r = Reviews.CreateReview(model.Title, model.Description, User.Identity.Id);
            //    return Created(ConvertReview(r));
            //}
            //return ValidationErrors();
            throw new NotImplementedException();
        }

        private static ReviewResponseModel ConvertReview(Review r)
        {
            return new ReviewResponseModel()
            {
                Id = r.Id,
                Title = r.Name,
                Author = r.Creator == null ? null : UserModel.FromUser(r.Creator)
            };
        }
    }
}