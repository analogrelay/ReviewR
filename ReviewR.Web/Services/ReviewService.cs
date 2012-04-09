using System;
using System.Collections.Generic;
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
                    new Iteration()
                }
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

        public Iteration AddIteration(int reviewId, int currentUserId)
        {
            Review r = GetReview(reviewId);
            if (r.UserId != currentUserId)
            {
                return null;
            }
            Iteration i = new Iteration()
            {
                Published = false,
                StartedOn = DateTimeOffset.UtcNow
            };
            r.Iterations.Add(i);
            Data.SaveChanges();
            return i;
        }

        public bool? DeleteIteration(int iterationId, int currentUserId)
        {
            Iteration iter = Data.Iterations.Include("Review").Where(i => i.Id == iterationId).FirstOrDefault();
            if (iter == null)
            {
                return null;
            }
            else if (iter.Review.UserId != currentUserId)
            {
                return false;
            }
            iter.Review.Iterations.Remove(iter);
            Data.Iterations.Remove(iter);
            Data.SaveChanges();
            return true;
        }
    }
}