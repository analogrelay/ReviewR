using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ReviewR.Web.Models.Data;

namespace ReviewR.Web.Services
{
    public class CommentService
    {
        public IDataRepository Data { get; set; }

        public CommentService(IDataRepository data)
        {
            Data = data;
        }

        public virtual Comment CreateComment(int changeId, int line, string body, int userId)
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

        public virtual DatabaseActionResult DeleteComment(int id, int userId)
        {
            Comment cmt = Data.Comments.Where(c => c.Id == id).FirstOrDefault();
            if (cmt == null)
            {
                return DatabaseActionResult.ObjectNotFound;
            }
            if (cmt.UserId != userId)
            {
                return DatabaseActionResult.Forbidden;
            }
            Data.Comments.Remove(cmt);
            Data.SaveChanges();
            return DatabaseActionResult.Success;
        }
    }
}