using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ReviewR.Diff;
using ReviewR.Web.ViewModels;
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

        public virtual DiffFileViewModel CreateViewModelFromUnifiedDiff(string fileName, string diff)
        {
            IEnumerable<DiffHunk> hunks;
            using (TextReader rdr = new StringReader(diff))
            {
                hunks = Reader.ReadHunks(rdr);
            }

            // Flatten out to lines
            IEnumerable<DiffLineViewModel> lines = hunks.SelectMany(h => 
                Enumerable.Concat(
                    new [] { new DiffLineViewModel() { Type = LineDiffType.HunkHeader, Text = h.ToString() } },
                    h.Lines.Select((l, i) =>
                        new DiffLineViewModel() { 
                            LeftLine = h.OriginalLocation.Line + i, 
                            RightLine = h.ModifiedLocation.Line + i,
                            Type = l.Type,
                            Text = l.Content
                        }
                    )
                ));

            return new DiffFileViewModel()
            {
                FileName = fileName,
                Binary = false,
                Deletions = lines.Count(m => m.Type == LineDiffType.Removed),
                Insertions = lines.Count(m => m.Type == LineDiffType.Added),
                DiffLines = lines.ToList()
            };
        }
    }
}