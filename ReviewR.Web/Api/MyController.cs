using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Models.Data;
using ReviewR.Web.Models.Response;
using ReviewR.Web.Services;
using VibrantUtils;

namespace ReviewR.Web.Api
{
    public class MyController : ReviewRApiController
    {
        public ReviewService Reviews { get; set; }

        protected MyController() { }
        public MyController(ReviewService reviews)
        {
            Requires.NotNull(reviews, "reviews");

            Reviews = reviews;
        }

        // GET /api/v1/my/reviews
        [HttpGet]
        [ActionName("Reviews")]
        [CacheControl(CacheabilityValue.NoCache, noStore: true)]
        public HttpResponseMessage GetReviews()
        {
            // Get all reviews created by this user
            IEnumerable<Review> myReviews = Reviews.GetReviewsCreatedBy(User.Identity.UserId).ToArray();
            IEnumerable<Review> assignedReviews = Reviews.GetReviewsAssignedTo(User.Identity.UserId).ToArray();
            return Ok(new DashboardModel()
            {
                Created = myReviews.Select(ConvertReview),
                Assigned = assignedReviews.Select(ConvertReview)
            });
        }

        private static ReviewModel ConvertReview(Review r)
        {
            return new ReviewModel()
            {
                Id = r.Id,
                Title = r.Name,
                Author = r.Creator == null ? null : UserModel.FromUser(r.Creator)
            };
        }
    }
}