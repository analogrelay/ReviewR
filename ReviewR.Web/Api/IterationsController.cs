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

namespace ReviewR.Web.Api
{
    public class IterationsController : ReviewRApiController
    {
        public IterationService Iterations { get; set; }
        public DiffService Diff { get; set; }

        public IterationsController(IterationService iterations, DiffService diff)
        {
            Iterations = iterations;
            Diff = diff;
        }

        public HttpResponseMessage Get(int id)
        {
            Iteration iter = Iterations.GetIteration(id);
            if (iter == null)
            {
                return NotFound();
            }
            else if (iter.Review.UserId != User.Identity.UserId && !iter.Published)
            {
                return Forbidden();
            }
            return Ok(iter.Files.GroupBy(GetDirectoryName).Select(g => new
            {
                Name = g.Key,
                Files = g.Select(f => new
                {
                    Id = f.Id,
                    FileName = f.DisplayFileName.Substring(g.Key.Length + 1),
                    FullPath = f.DisplayFileName,
                    ChangeType = f.ChangeType
                })
            }));
        }

        public HttpResponseMessage Post(int reviewId)
        {
            Iteration iter = Iterations.AddIteration(reviewId, User.Identity.UserId);
            if (iter == null)
            {
                return Forbidden();
            }
            return Created(new
            {
                Id = iter.Id,
                Href = Url.Resource(iter)
            });
        }

        public HttpResponseMessage Delete(int id)
        {
            var result = Iterations.DeleteIteration(id, User.Identity.UserId);
            if (result == DatabaseActionResult.ObjectNotFound)
            {
                return NotFound();
            }
            else if (result == DatabaseActionResult.Forbidden)
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
                if (result == DatabaseActionResult.ObjectNotFound)
                {
                    return NotFound();
                }
                else if (result == DatabaseActionResult.Forbidden)
                {
                    return Forbidden();
                }
            }
            
            if (!String.IsNullOrEmpty(diff))
            {
                var result = Iterations.AddDiffToIteration(id, diff, User.Identity.UserId);
                if (result == DatabaseActionResult.ObjectNotFound)
                {
                    return NotFound();
                }
                else if (result == DatabaseActionResult.Forbidden)
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
            return fileName;
        }
    }
}