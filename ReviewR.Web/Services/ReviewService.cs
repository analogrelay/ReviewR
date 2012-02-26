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
    }
}
