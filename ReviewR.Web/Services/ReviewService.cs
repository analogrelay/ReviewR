using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReviewR.Web.Models;

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

        public virtual Review CreateReview(string name, List<FileChange> changes, int ownerId)
        {
            Review r = new Review()
            {
                Name = name,
                Files = changes,
                UserId = ownerId
            };
            Data.Reviews.Add(r);
            Data.SaveChanges();
            return r;
        }

        public virtual IEnumerable<Review> GetReviewsCreatedBy(int userId)
        {
            return Data.Users
                       .Include("Reviews")
                       .Where(u => u.Id == userId)
                       .Single()
                       .Reviews;
        }

        public virtual Review GetReview(int id)
        {
            return Data.Reviews
                       .Where(r => r.Id == id)
                       .FirstOrDefault();
        }

        public virtual FileChange GetChange(int id)
        {
            return Data.Changes
                       .Include("Review")
                       .Where(c => c.Id == id)
                       .FirstOrDefault();
        }

        public IEnumerable<Review> GetAllReviews()
        {
            return Data.Reviews;
        }

        public void CreateComment(int changeId, int? line, int userId, string body)
        {
            Data.Comments.Add(new Comment()
            {
                FileId = changeId,
                DiffLineIndex = line,
                UserId = userId,
                Content = body,
                PostedOn = DateTime.UtcNow
            });
            Data.SaveChanges();
        }
    }
}
