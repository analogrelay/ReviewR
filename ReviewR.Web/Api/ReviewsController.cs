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
            IEnumerable<Review> reviews = Reviews.GetReviewsCreatedBy(User.Identity.UserId).ToArray();
            return new DashboardResponseModel()
            {
                Created = reviews.Select(ConvertReview)
            };
        }

        // POST /api/reviews
        public HttpResponseMessage Post(ReviewRequestModel model)
        {
            if (ModelState.IsValid)
            {
                Review r = Reviews.CreateReview(model.Title, model.Description, User.Identity.UserId);
                return new HttpResponseMessage<ReviewResponseModel>(
                    ConvertReview(r),
                    HttpStatusCode.Created);
            }
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }

        private static ReviewResponseModel ConvertReview(Review r)
        {
            return new ReviewResponseModel()
            {
                Id = r.Id,
                Title = r.Name
            };
        }
    }
}