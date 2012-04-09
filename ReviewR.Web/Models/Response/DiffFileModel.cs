using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Web.Models
{
    public class DiffFileModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public int Deletions { get; set; }
        public int Insertions { get; set; }
        public bool Binary { get; set; }
        public IList<DiffLineModel> Lines { get; set; }

        public DiffFileModel()
        {
        }
    }
}

