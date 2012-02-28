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
            return Data.Reviews
                       .Where(r => r.UserId == userId);
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

        public virtual IEnumerable<Review> GetAllReviews()
        {
            return Data.Reviews;
        }

        public virtual Comment CreateComment(int changeId, int? line, int userId, string body)
        {
            Comment c = Data.Comments.Add(new Comment()
            {
                FileId = changeId,
                DiffLineIndex = line,
                UserId = userId,
                Content = body,
                PostedOn = DateTime.UtcNow
            });
            Data.SaveChanges();
            return c;
        }

        public Comment GetComment(int id)
        {
            return Data.Comments.Where(c => c.Id == id).FirstOrDefault();
        }

        public void DeleteComment(Comment c)
        {
            Data.Comments.Remove(c);
            Data.SaveChanges();
        }

        public IEnumerable<Review> GetParticipatingReviews(int userId)
        {
            return Data.Participants
                       .Where(p => p.UserId == userId)
                       .Select(p => p.Review);
        }
    }
}
