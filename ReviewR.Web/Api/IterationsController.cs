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
using System.Web.Http;

namespace ReviewR.Web.Api
{
    public class IterationsController : ReviewRApiController
    {
        public IterationService Iterations { get; set; }
        
        protected IterationsController() { }
        public IterationsController(IterationService iterations)
        {
            Requires.NotNull(iterations, "iterations");

            Iterations = iterations;
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
                    ChangeType = f.ChangeType,
                    HasComments = f.Comments.Any()
                }).ToArray()
            }).ToArray());
        }

        public HttpResponseMessage Post(int reviewId)
        {
            Requires.InRange(reviewId >= 0, "reviewId");

            var result = Iterations.AddIteration(reviewId, User.Identity.UserId);
            if (result.Outcome == DatabaseActionOutcome.ObjectNotFound)
            {
                return NotFound();
            }
            else if (result.Outcome == DatabaseActionOutcome.Forbidden)
            {
                return Forbidden();
            }
            return Created(new IterationModel()
            {
                Id = result.Object.Id,
                Description = result.Object.Description,
                Published = result.Object.Published,
                Order = null
            });
        }

        public HttpResponseMessage Delete(int id)
        {
            Requires.InRange(id >= 0, "id");

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

        public HttpResponseMessage Put(int id, bool? published, string diff)
        {
            if(published != null && !String.IsNullOrEmpty(diff)) {
                return BadRequest();
            }
            else if (published != null)
            {
                return PutPublished(id, published.Value);
            }
            else if (!String.IsNullOrEmpty(diff))
            {
                return PutDiff(id, diff);
            }
            return BadRequest();
        }

        internal HttpResponseMessage PutPublished(int id, bool published)
        {
            Requires.InRange(id >= 0, "id");

            var result = Iterations.SetIterationPublished(id, published, User.Identity.UserId);
            if (result.Outcome == DatabaseActionOutcome.ObjectNotFound)
            {
                return NotFound();
            }
            else if (result.Outcome == DatabaseActionOutcome.Forbidden)
            {
                return Forbidden();
            }
            return NoContent(new IterationModel()
            {
                Id = result.Object.Id,
                Description = result.Object.Description,
                Published = result.Object.Published,
                Order = null
            });
        }

        internal HttpResponseMessage PutDiff(int id, string diff)
        {
            Requires.InRange(id >= 0, "id");
            Requires.NotNullOrEmpty(diff, "diff");

            var result = Iterations.AddDiffToIteration(id, diff, User.Identity.UserId);
            if (result.Outcome == DatabaseActionOutcome.ObjectNotFound)
            {
                return NotFound();
            }
            else if (result.Outcome == DatabaseActionOutcome.Forbidden)
            {
                return Forbidden();
            }
            return NoContent(new IterationModel()
            {
                Id = result.Object.Id,
                Description = result.Object.Description,
                Published = result.Object.Published,
                Order = null
            });
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