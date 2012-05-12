using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReviewR.Web.Models.Data;

namespace ReviewR.Web.Models.Response
{
    public class FileModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FullPath { get; set; }
        public FileChangeType ChangeType { get; set; }
    }
}
