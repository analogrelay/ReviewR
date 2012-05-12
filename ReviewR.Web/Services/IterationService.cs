using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ReviewR.Web.Models.Data;
using VibrantUtils;

namespace ReviewR.Web.Services
{
    public class IterationService
    {
        public IDataRepository Data { get; set; }
        public DiffService Diff { get; set; }

        protected IterationService() { }
        public IterationService(IDataRepository data, DiffService diff)
        {
            Requires.NotNull(data, "data");
            Requires.NotNull(diff, "diff");

            Data = data;
            Diff = diff;
        }

        public virtual DatabaseActionResult<Iteration> AddIteration(int reviewId, int currentUserId)
        {
            Requires.InRange(reviewId >= 0, "reviewId");
            Requires.InRange(currentUserId >= 0, "currentUserId");

            Review review = Data.Reviews
                                .Include("Iterations")
                                .Where(r => r.Id == reviewId)
                                .FirstOrDefault();
            if (review == null)
            {
                return DatabaseActionResult<Iteration>.NotFound();
            }
            else if (review.UserId != currentUserId)
            {
                return DatabaseActionResult<Iteration>.Forbidden();
            }
            Iteration i = new Iteration()
            {
                Published = false,
                StartedOn = DateTime.UtcNow,
            };
            review.Iterations.Add(i);
            Data.SaveChanges();
            return DatabaseActionResult<Iteration>.Success(i);
        }

        public virtual DatabaseActionOutcome DeleteIteration(int iterationId, int currentUserId)
        {
            Requires.InRange(iterationId >= 0, "iterationId");
            Requires.InRange(currentUserId >= 0, "currentUserId");

            Iteration iter = GetIteration(iterationId);
            if (iter == null)
            {
                return DatabaseActionOutcome.ObjectNotFound;
            }
            else if (iter.Review.UserId != currentUserId)
            {
                return DatabaseActionOutcome.Forbidden;
            }
            Data.Iterations.Remove(iter);
            Data.SaveChanges();
            return DatabaseActionOutcome.Success;
        }

        public virtual DatabaseActionResult<Iteration> AddDiffToIteration(int id, string diff, int currentUserId)
        {
            Requires.InRange(id >= 0, "id");
            Requires.NotNullOrEmpty(diff, "diff");
            Requires.InRange(currentUserId >= 0, "currentUserId");

            Iteration iter = GetIteration(id);
            if (iter == null)
            {
                return DatabaseActionResult<Iteration>.NotFound();
            }
            else if (iter.Review.UserId != currentUserId)
            {
                return DatabaseActionResult<Iteration>.Forbidden();
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
            return DatabaseActionResult<Iteration>.Success(iter);
        }

        public virtual Iteration GetIteration(int iterationId)
        {
            Requires.InRange(iterationId >= 0, "iterationId");

            return Data.Iterations
                       .Include("Review")
                       .Where(i => i.Id == iterationId)
                       .FirstOrDefault();
        }

        public virtual DatabaseActionResult<Iteration> SetIterationPublished(int id, bool published, int userId)
        {
            Requires.InRange(id >= 0, "id");
            Requires.InRange(userId >= 0, "userId");

            Iteration i = Data.Iterations
                              .Include("Review")
                              .Where(iter => iter.Id == id)
                              .FirstOrDefault();
            if (i == null)
            {
                return DatabaseActionResult<Iteration>.NotFound();
            }
            else if (i.Review.UserId != userId)
            {
                return DatabaseActionResult<Iteration>.Forbidden();
            }
            i.Published = published;
            Data.SaveChanges();
            return DatabaseActionResult<Iteration>.Success(i);
        }
    }
}