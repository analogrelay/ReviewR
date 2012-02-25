using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ReviewR.Diff;
using VibrantUtils;
using Data = ReviewR.Web.Models;

namespace ReviewR.Web.Services
{
    public class DiffService
    {
        public DiffReader Reader { get; set; }
        public DiffConverter Converter { get; set; }

        protected DiffService() { }

        public DiffService(DiffReader reader, DiffConverter converter)
        {
            Reader = reader;
            Converter = converter;
        }

        public virtual ICollection<Data.FileChange> CreateFromGitDiff(TextReader source)
        {
            Requires.NotNull(source, "source");

            // Parse the diff
            DiffSet parsedDiff = Reader.Read(source);

            // Build the data model
            return parsedDiff.Files.Select(Converter.ConvertFile).ToList();
        }
    }
}