using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Web.ViewModels
{
    public class FolderChangeViewModel
    {
        public string Name { get; set; }
        public IList<FileChangeViewModel> Files { get; set; }
    }
}
