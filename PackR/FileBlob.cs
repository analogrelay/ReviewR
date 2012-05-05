using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibGit2Sharp;

namespace PackR
{
    public class FileBlob
    {
        public string FullPath { get; set; }
        public Blob Blob { get; set; }
        
        public FileBlob(string fullPath, Blob blob)
        {
            FullPath = fullPath;
            Blob = blob;
        }
    }
}
