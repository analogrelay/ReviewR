using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReviewR.Diff;

namespace ReviewR.Web.ViewModels
{
    public class DiffLineViewModel
    {
        public DiffLineType Type { get; set; }
        public string Text { get; set; }
        public int? LeftLine { get; set; }
        public int? RightLine { get; set; }
    }
}
