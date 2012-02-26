using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Web.ViewModels
{
    public class DiffFileViewModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public int Deletions { get; set; }
        public int Insertions { get; set; }
        public bool Binary { get; set; }
        public IList<DiffLineViewModel> DiffLines { get; set; }

        public DiffFileViewModel()
        {
        }
    }
}
