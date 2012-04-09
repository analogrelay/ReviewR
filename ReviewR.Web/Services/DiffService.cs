using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ReviewR.Diff;
using ReviewR.Web.Models;
using VibrantUtils;
using Data = ReviewR.Web.Models.Data;

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

        public virtual DiffFileModel CreateViewModelFromUnifiedDiff(string fileName, string diff)
        {
            IEnumerable<DiffHunk> hunks;
            using (TextReader rdr = new StringReader(diff))
            {
                hunks = Reader.ReadHunks(rdr);
            }

            // Flatten out to lines
            IList<DiffLineModel> lines = new List<DiffLineModel>();
            int lineIndex = 0;
            foreach (var hunk in hunks)
            {
                int leftLine = hunk.OriginalLocation.Line;
                int rightLine = hunk.ModifiedLocation.Line;
                lines.Add(new DiffLineModel() { Index = lineIndex++, Type = LineDiffType.HunkHeader, Text = hunk.ToString() });
                foreach (var line in hunk.Lines)
                {
                    DiffLineModel newLine = new DiffLineModel()
                    {
                        Index = lineIndex++,
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
                    lines.Add(newLine);
                }
            }

            return new DiffFileModel()
            {
                FileName = Converter.CleanFileName(fileName),
                Binary = false,
                Deletions = lines.Count(m => m.Type == LineDiffType.Removed),
                Insertions = lines.Count(m => m.Type == LineDiffType.Added),
                Lines = lines.ToList()
            };
        }
    }
}
