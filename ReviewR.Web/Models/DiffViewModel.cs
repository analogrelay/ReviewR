using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReviewR.Web.Models
{
    public class DiffViewModel
    {
        public IDictionary<string, DiffFileViewModel> Files { get; set; }
    }
}