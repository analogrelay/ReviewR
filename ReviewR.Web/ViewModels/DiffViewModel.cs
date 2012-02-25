using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReviewR.Web.ViewModels
{
    public class DiffViewModel
    {
        public IDictionary<string, DiffFileViewModel> Files { get; set; }
    }
}