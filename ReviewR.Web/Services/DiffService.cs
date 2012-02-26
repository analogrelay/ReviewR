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
                    ConvertLines(h)
                ));

            // Attach comments

            return new DiffFileViewModel()
            {
                FileName = fileName,
                Binary = false,
                Deletions = lines.Count(m => m.Type == LineDiffType.Removed),
                Insertions = lines.Count(m => m.Type == LineDiffType.Added),
                DiffLines = lines.ToList()
            };
        }

        private IEnumerable<DiffLineViewModel> ConvertLines(DiffHunk h)
        {
            int leftLine = h.OriginalLocation.Line;
            int rightLine = h.ModifiedLocation.Line;
            int index = 0;
            foreach(var line in h.Lines)
            {
                DiffLineViewModel newLine = new DiffLineViewModel()
                {
                    Index = index++,
                    Type = line.Type,
                    Text = line.Content
                };
                switch (line.Type)
                {
                    case LineDiffType.Same:
                        newLine.LeftLine = leftLine++;
                        newLine.RightLine = rightLine++;
                        break;
                    case LineDiffType.Added:
                        newLine.RightLine = rightLine++;
                        break;
                    case LineDiffType.Removed:
                        newLine.LeftLine = leftLine++;
                        break;
                }
                yield return newLine;
            }
        }
    }
}