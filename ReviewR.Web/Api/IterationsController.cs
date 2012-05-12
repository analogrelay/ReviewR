using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Models.Response;
using ReviewR.Web.Models.Request;
using ReviewR.Web.Models.Data;
using ReviewR.Web.Services;
using System.IO;
using VibrantUtils;

namespace ReviewR.Web.Api
{
    public class IterationsController : ReviewRApiController
    {
        public IterationService Iterations { get; set; }
        public DiffService Diff { get; set; }

        protected IterationsController() { }
        public IterationsController(IterationService iterations, DiffService diff)
        {
            Requires.NotNull(iterations, "iterations");
            Requires.NotNull(diff, "diff");

            Iterations = iterations;
            Diff = diff;
        }

        public HttpResponseMessage Get(int id)
        {
            Requires.InRange(id >= 0, "id");

            Iteration iter = Iterations.GetIteration(id);
            if (iter == null)
            {
                return NotFound();
            }
            else if (iter.Review.UserId != User.Identity.UserId && !iter.Published)
            {
                return Forbidden();
            }
            return Ok(iter.Files.GroupBy(GetDirectoryName).Select(g => new FolderModel
            {
                Name = g.Key ?? "/",
                Files = g.Select(f => new FileModel
                {
                    Id = f.Id,
                    FileName = g.Key == null ? f.DisplayFileName : f.DisplayFileName.Substring(g.Key.Length + 1),
                    FullPath = f.DisplayFileName,
                    ChangeType = f.ChangeType
                }).ToArray()
            }).ToArray());
        }

        public HttpResponseMessage Post(int reviewId)
        {
            var result = Iterations.AddIteration(reviewId, User.Identity.UserId);
            switch (result.Outcome)
            {
                case DatabaseActionOutcome.ObjectNotFound:
                    return NotFound();
                case DatabaseActionOutcome.Forbidden:
                    return Forbidden();
                case DatabaseActionOutcome.Success:
                    return Created(new
                    {
                        Id = result.Object.Id,
                        Href = Url.Resource(result.Object)
                    });
                default:
                    throw new HttpException("Unknown error!");
            }
        }

        public HttpResponseMessage Delete(int id)
        {
            var result = Iterations.DeleteIteration(id, User.Identity.UserId);
            if (result == DatabaseActionOutcome.ObjectNotFound)
            {
                return NotFound();
            }
            else if (result == DatabaseActionOutcome.Forbidden)
            {
                return Forbidden();
            }
            return NoContent();
        }

        public HttpResponseMessage Put(int id, string diff, bool? published)
        {
            // TODO: Organize this a bit better so that we only fetch the iteration once if the request does a batch update
            if (published.HasValue)
            {
                var result = Iterations.SetIterationPublished(id, published.Value, User.Identity.UserId);
                if (result == DatabaseActionOutcome.ObjectNotFound)
                {
                    return NotFound();
                }
                else if (result == DatabaseActionOutcome.Forbidden)
                {
                    return Forbidden();
                }
            }
            
            if (!String.IsNullOrEmpty(diff))
            {
                var result = Iterations.AddDiffToIteration(id, diff, User.Identity.UserId);
                if (result == DatabaseActionOutcome.ObjectNotFound)
                {
                    return NotFound();
                }
                else if (result == DatabaseActionOutcome.Forbidden)
                {
                    return Forbidden();
                }
            }
            return NoContent();
        }

        private static string GetDirectoryName(FileChange f)
        {
            var fileName = String.IsNullOrEmpty(f.NewFileName) ? f.FileName : f.NewFileName;
            int lastSlash = fileName.LastIndexOf('/');
            if (lastSlash > -1)
            {
                return fileName.Substring(0, lastSlash);
            }
            return null;
        }
    }
}