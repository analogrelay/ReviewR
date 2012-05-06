using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReviewR.Web.Models.Response;

namespace ReviewR.Web.Models
{
    public class FileDiffModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public int Deletions { get; set; }
        public int Insertions { get; set; }
        public bool Binary { get; set; }
        public IList<LineDiffModel> Lines { get; set; }

        public FileDiffModel()
        {
        }
    }
}

