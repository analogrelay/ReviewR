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

        //public virtual FileChange GetChange(int id)
        //{
        //    return Data.Changes
        //               .Include("Review")
        //               .Where(c => c.Id == id)
        //               .FirstOrDefault();
        //}

        //public virtual Comment CreateComment(int changeId, int? line, int userId, string body)
        //{
        //    Comment c = Data.Comments.Add(new Comment()
        //    {
        //        FileId = changeId,
        //        DiffLineIndex = line,
        //        UserId = userId,
        //        Content = body,
        //        PostedOn = DateTime.UtcNow
        //    });
        //    Data.SaveChanges();
        //    return c;
        //}

        //public Comment GetComment(int id)
        //{
        //    return Data.Comments.Where(c => c.Id == id).FirstOrDefault();
        //}

        //public void DeleteComment(Comment c)
        //{
        //    Data.Comments.Remove(c);
        //    Data.SaveChanges();
        //}

        //public void DeleteReview(Review r)
        //{
        //    Data.Reviews.Remove(r);
        //    Data.SaveChanges();
        //}
    }
}