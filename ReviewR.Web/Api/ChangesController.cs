using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Models;
using ReviewR.Web.Models.Data;
using ReviewR.Web.Services;

namespace ReviewR.Web.Api
{
    public class ChangesController : ReviewRApiController
    {
        public ReviewService Reviews { get; set; }
        public DiffService Diff { get; set; }

        public ChangesController(ReviewService reviews, DiffService diff)
        {
            Reviews = reviews;
            Diff = diff;
        }

        public HttpResponseMessage Get(int id)
        {
            FileChange change = Reviews.GetChange(id);
            if (change == null)
            {
                return NotFound();
            }
            //else if (!IsAuthorized(change))
            //{
            //    return Forbidden();
            //}
            return Ok(Diff.CreateViewModelFromUnifiedDiff(change.FileName, change.Diff));
        }
    }
}