using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Web.Models.Response
{
    public class FolderModel
    {
        public string Name { get; set; }
        public IEnumerable<FileModel> Files { get; set; }
    }
}
