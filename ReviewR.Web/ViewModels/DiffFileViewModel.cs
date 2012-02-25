using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Web.ViewModels
{
    public class DiffFileViewModel
    {
        public int Deletions { get; set; }
        public int Insertions { get; set; }
        public bool Binary { get; set; }
        public int Status { get; set; }
        public ICollection<DiffLineViewModel> DiffLines { get; set; }

        public DiffFileViewModel()
        {
            Status = 3;
        }
    }
}
