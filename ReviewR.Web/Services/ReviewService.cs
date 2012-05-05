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
        public DiffService Diff { get; set; }

        protected ReviewService() { }
        public ReviewService(IDataRepository data, DiffService diff)
        {
            Data = data;
            Diff = diff;
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
                StartedOn = DateTimeOffset.UtcNow,
            };
            r.Iterations.Add(i);
            Data.SaveChanges();
            return i;
        }

        public bool? DeleteIteration(int iterationId, int currentUserId)
        {
            Iteration iter = GetIteration(iterationId);
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

        public bool? AddDiffToIteration(int id, string diff, int currentUserId)
        {
            Iteration iter = GetIteration(id);
            if (iter == null)
            {
                return null;
            }
            else if (iter.Review.UserId != currentUserId)
            {
                return false;
            }
            
            ICollection<FileChange> changes;
            using(TextReader reader = new StringReader(diff)) {
                changes = Diff.CreateFromGitDiff(reader);
            }
            foreach (FileChange change in changes)
            {
                iter.Files.Add(change);
            }
            Data.SaveChanges();
            return true;
        }

        public Iteration GetIteration(int iterationId)
        {
            return Data.Iterations
                       .Include("Review")
                       .Include("Review.Participants")
                       .Where(i => i.Id == iterationId)
                       .FirstOrDefault();
        }

        public FileChange GetChange(int id)
        {
            return Data.Changes
                       .Include("Iteration.Review")
                       .Include("Iteration.Review.Participants")
                       .Include("Comments")
                       .Where(c => c.Id == id)
                       .FirstOrDefault();
        }

        public bool? SetIterationPublished(int id, bool published, int userId)
        {
            Iteration i = Data.Iterations
                              .Include("Review")
                              .Where(iter => iter.Id == id)
                              .FirstOrDefault();
            if (i == null)
            {
                return null;
            }
            else if (i.Review.UserId != userId)
            {
                return false;
            }
            i.Published = published;
            Data.SaveChanges();
            return true;
        }

        public Comment CreateComment(int changeId, int line, string body, int userId)
        {
            FileChange chg = Data.Changes.Where(c => c.Id == changeId).FirstOrDefault();
            if (chg == null)
            {
                return null;
            }
            Comment cmt = new Comment()
            {
                Content = body,
                DiffLineIndex = line,
                UserId = userId,
                PostedOn = DateTime.UtcNow
            };
            chg.Comments.Add(cmt);
            Data.SaveChanges();
            return cmt;
        }

        public bool? DeleteComment(int id, int userId)
        {
            Comment cmt = Data.Comments.Where(c => c.Id == id).FirstOrDefault();
            if (cmt == null)
            {
                return null;
            }
            if (cmt.UserId != userId)
            {
                return false;
            }
            Data.Comments.Remove(cmt);
            Data.SaveChanges();
            return true;
        }
    }
}