using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ReviewR.Web.Models.Data;

namespace ReviewR.Web.Services
{
    public class IterationService
    {
        public IDataRepository Data { get; set; }
        public ReviewService Reviews { get; set; }
        public DiffService Diff { get; set; }

        protected IterationService() { }
        public IterationService(IDataRepository data, ReviewService reviews, DiffService diff)
        {
            Data = data;
            Reviews = reviews;
            Diff = diff;
        }

        public virtual Iteration AddIteration(int reviewId, int currentUserId)
        {
            Review r = Reviews.GetReview(reviewId);
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

        public virtual DatabaseActionResult DeleteIteration(int iterationId, int currentUserId)
        {
            Iteration iter = GetIteration(iterationId);
            if (iter == null)
            {
                return DatabaseActionResult.ObjectNotFound;
            }
            else if (iter.Review.UserId != currentUserId)
            {
                return DatabaseActionResult.Forbidden;
            }
            iter.Review.Iterations.Remove(iter);
            Data.Iterations.Remove(iter);
            Data.SaveChanges();
            return DatabaseActionResult.Success;
        }

        public virtual DatabaseActionResult AddDiffToIteration(int id, string diff, int currentUserId)
        {
            Iteration iter = GetIteration(id);
            if (iter == null)
            {
                return DatabaseActionResult.ObjectNotFound;
            }
            else if (iter.Review.UserId != currentUserId)
            {
                return DatabaseActionResult.Forbidden;
            }

            ICollection<FileChange> changes;
            using (TextReader reader = new StringReader(diff))
            {
                changes = Diff.CreateFromGitDiff(reader);
            }
            foreach (FileChange change in changes)
            {
                iter.Files.Add(change);
            }
            Data.SaveChanges();
            return DatabaseActionResult.Success;
        }

        public virtual Iteration GetIteration(int iterationId)
        {
            return Data.Iterations
                       .Include("Review")
                       .Include("Review.Participants")
                       .Where(i => i.Id == iterationId)
                       .FirstOrDefault();
        }

        public virtual DatabaseActionResult SetIterationPublished(int id, bool published, int userId)
        {
            Iteration i = Data.Iterations
                              .Include("Review")
                              .Where(iter => iter.Id == id)
                              .FirstOrDefault();
            if (i == null)
            {
                return DatabaseActionResult.ObjectNotFound;
            }
            else if (i.Review.UserId != userId)
            {
                return DatabaseActionResult.Forbidden;
            }
            i.Published = published;
            Data.SaveChanges();
            return DatabaseActionResult.Success;
        }
    }
}