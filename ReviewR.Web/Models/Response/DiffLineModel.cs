using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReviewR.Diff;
using ReviewR.Web.Models;
using ReviewR.Web.Models.Response;

namespace ReviewR.Web.Models
{
    public class DiffLineModel
    {
        public LineDiffType Type { get; set; }
        public string Text { get; set; }
        public int Index { get; set; }
        public int? LeftLine { get; set; }
        public int? RightLine { get; set; }
        public ICollection<CommentModel> Comments { get; set; }

        public DiffLineModel()
        {
            Comments = new List<CommentModel>();
        }
    }
}

