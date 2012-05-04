using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Models.Data;
using ReviewR.Web.Models.Response;
using ReviewR.Web.Services;

namespace ReviewR.Web.Api
{
    public class MyController : ReviewRApiController
    {
        public ReviewService Reviews { get; set; }

        public MyController(ReviewService reviews)
        {
            Reviews = reviews;
        }

        // GET /api/reviews
        [HttpGet]
        [ActionName("Reviews")]
        [CacheControl(CacheabilityValue.NoCache, noStore: true)]
        public DashboardResponseModel GetReviews()
        {
            // Get all reviews created by this user
            IEnumerable<Review> myReviews = Reviews.GetReviewsCreatedBy(User.Identity.UserId).ToArray();
            IEnumerable<Review> assignedReviews = Reviews.GetReviewsAssignedTo(User.Identity.UserId).ToArray();
            return new DashboardResponseModel()
            {
                Created = myReviews.Select(ConvertReview),
                Assigned = assignedReviews.Select(ConvertReview)
            };
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