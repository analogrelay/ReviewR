using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReviewR.Diff;
using ReviewR.Web.Models;

namespace ReviewR.Web.ViewModels
{
    public class DiffLineViewModel
    {
        public LineDiffType Type { get; set; }
        public string Text { get; set; }
        public int Index { get; set; }
        public int? LeftLine { get; set; }
        public int? RightLine { get; set; }
        public ICollection<LineCommentViewModel> Comments { get; set; }

        public DiffLineViewModel()
        {
            Comments = new List<LineCommentViewModel>();
        }
    }
}
