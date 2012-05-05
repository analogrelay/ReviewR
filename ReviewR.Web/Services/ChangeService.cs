using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ReviewR.Web.Models.Data;

namespace ReviewR.Web.Services
{
    public class ChangeService
    {
        public IDataRepository Data { get; set; }
        
        public ChangeService(IDataRepository data)
        {
            Data = data;
        }

        public virtual FileChange GetChange(int id)
        {
            return Data.Changes
                       .Include("Iteration.Review")
                       .Include("Iteration.Review.Participants")
                       .Include("Comments")
                       .Where(c => c.Id == id)
                       .FirstOrDefault();
        }
    }
}