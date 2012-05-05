using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ReviewR.Web.Models;
using ReviewR.Web.Models.Data;

namespace ReviewR.Web.Services
{
    public class ReviewService
    {
        public IDataRepository Data { get; set; }
        
        protected ReviewService() { }
        public ReviewService(IDataRepository data)
        {
            Data = data;
        }

        public virtual Review CreateReview(string name, string description, int ownerId)
        {
            Review r = new Review()
            {
                Name = name,
                Description = description,
                UserId = ownerId,
                Iterations = new List<Iteration>() {
                    new Iteration() { StartedOn = DateTimeOffset.UtcNow }
                },
                CreatedOn = DateTimeOffset.UtcNow
            };
            Data.Reviews.Add(r);
            Data.SaveChanges();
            return r;
        }

        public virtual IEnumerable<Review> GetReviewsCreatedBy(int userId)
        {
            return Data.Reviews
                       .Where(r => r.UserId == userId);
        }

        public IEnumerable<Review> GetReviewsAssignedTo(int userId)
        {
            // For now, all reviews not created by a user are assigned to that user
            return Data.Reviews
                       .Where(r => r.UserId != userId);
        }

        public virtual Review GetReview(int id)
        {
            return Data.Reviews
                       .Include("Iterations")
                       .Include("Participants")
                       .Include("Participants.User")
                       .Include("Creator")
                       .Where(r => r.Id == id)
                       .FirstOrDefault();
        }

        public virtual IEnumerable<Review> GetAllReviews()
        {
            return Data.Reviews;
        }
    }
}