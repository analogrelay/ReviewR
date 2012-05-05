using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ReviewR.Web.Models.Data;
using VibrantUtils;

namespace ReviewR.Web.Services
{
    public class ChangeService
    {
        public IDataRepository Data { get; set; }

        protected ChangeService() { }
        public ChangeService(IDataRepository data)
        {
            Requires.NotNull(data, "data");

            Data = data;
        }

        public virtual FileChange GetChange(int id)
        {
            Requires.InRange(id >= 0, "id");

            return Data.Changes
                       .Include("Iteration.Review")
                       .Include("Iteration.Review.Participants")
                       .Include("Comments")
                       .Where(c => c.Id == id)
                       .FirstOrDefault();
        }
    }
}