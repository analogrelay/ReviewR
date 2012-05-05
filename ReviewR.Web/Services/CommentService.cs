using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ReviewR.Web.Models.Data;
using VibrantUtils;

namespace ReviewR.Web.Services
{
    public class CommentService
    {
        public IDataRepository Data { get; set; }

        protected CommentService() { }
        public CommentService(IDataRepository data)
        {
            Requires.NotNull(data, "data");

            Data = data;
        }

        public virtual Comment CreateComment(int changeId, int line, string body, int userId)
        {
            Requires.InRange(changeId >= 0, "changeId");
            Requires.InRange(line >= 0, "line");
            Requires.NotNullOrEmpty(body, "body");
            Requires.InRange(userId >= 0, "userId");

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
            Requires.InRange(id >= 0, "id");
            Requires.InRange(userId >= 0, "userId");

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