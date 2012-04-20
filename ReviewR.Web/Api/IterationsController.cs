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
        public ReviewService Reviews { get; set; }
        public DiffService Diff { get; set; }

        public IterationsController(ReviewService reviews, DiffService diff)
        {
            Reviews = reviews;
            Diff = diff;
        }

        public HttpResponseMessage Get(int id)
        {
            Iteration iter = Reviews.GetIteration(id);
            if (iter == null)
            {
                return NotFound();
            }
            //else if (iter.Review.UserId != User.Identity.Id && !iter.Review.Participants.Any(p => p.UserId == User.Identity.Id))
            //{
            //    return Forbidden();
            //}
            return Ok(iter.Files.GroupBy(GetDirectoryName).Select(g => new
            {
                Name = g.Key,
                Files = g.Select(f => new
                {
                    Id = f.Id,
                    FileName = f.NewFileName.Substring(g.Key.Length + 1),
                    ChangeType = f.ChangeType
                })
            }));
        }

        public HttpResponseMessage Post(int reviewId)
        {
            //Iteration iter = Reviews.AddIteration(reviewId, User.Identity.Id);
            //if (iter == null)
            //{
            //    return Forbidden();
            //}
            //return Created(new
            //{
            //    Id = iter.Id,
            //    Href = Url.Route("DefaultApi", new { controller = "Iterations", id = iter.Id })
            //});
            throw new NotImplementedException();
        }

        public HttpResponseMessage Delete(int id)
        {
            //bool? result = Reviews.DeleteIteration(id, User.Identity.Id);
            //if (result == null)
            //{
            //    return NotFound();
            //}
            //else if (result.Value)
            //{
            //    return NoContent();
            //}
            //else
            //{
            //    return Forbidden();
            //}
            throw new NotImplementedException();
        }

        public HttpResponseMessage Put(int id, string diff)
        {
            //if (String.IsNullOrEmpty(diff))
            //{
            //    return BadRequest();
            //}

            //bool? result = Reviews.AddDiffToIteration(id, diff, User.Identity.Id);
            //if (result == null)
            //{
            //    return NotFound();
            //}
            //else if (result.Value)
            //{
            //    return NoContent();
            //}
            //else
            //{
            //    return Forbidden();
            //}
            throw new NotImplementedException();
        }

        private static string GetDirectoryName(FileChange f)
        {
            int lastSlash = f.NewFileName.LastIndexOf('/');
            if (lastSlash > -1)
            {
                return f.NewFileName.Substring(0, lastSlash);
            }
            return f.NewFileName;
        }
    }
}