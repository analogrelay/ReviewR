using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReviewR.Diff;
using VibrantUtils;
using Data = ReviewR.Web.Models;

namespace ReviewR.Web.Services
{
    public class DiffConverter
    {
        public virtual Data.FileChange ConvertFile(FileDiff fileDiff)
        {
            Requires.NotNull(fileDiff, "fileDiff");

            // Figure out the change type
            Data.FileChange chg;
            if (String.Equals(fileDiff.ModifiedFile, "/dev/null"))
            {
                // Deletion
                chg = new Data.FileDeletion() { FileName = fileDiff.OriginalFile };
            }
            else if (String.Equals(fileDiff.OriginalFile, "/dev/null"))
            {
                // Addition
                chg = new Data.FileAddition() { FileName = fileDiff.ModifiedFile };
            }
            else
            {
                // Modification
                chg = new Data.FileModification() {
                    FileName = fileDiff.OriginalFile,
                    NewFileName = fileDiff.ModifiedFile
                };
            }

            // Fill the lines
            chg.Lines = fileDiff.Hunks.SelectMany(ConvertHunk).ToList();
            return chg;
        }

        public virtual IEnumerable<Data.DiffLine> ConvertHunk(DiffHunk arg)
        {
            return arg.Lines.Select((l, i) => ConvertLine(i, arg, l));
        }

        public virtual Data.DiffLine ConvertLine(int lineIndex, DiffHunk hunk, LineDiff arg)
        {
            int source = hunk.OriginalLocation.Line + lineIndex;
            int modified = hunk.ModifiedLocation.Line + lineIndex;
            switch (arg.Type)
            {
                case LineDiffType.Added:
                    return FillLine(new Data.DiffLineAdd(), source, modified, arg.Content);
                case LineDiffType.Removed:
                    return FillLine(new Data.DiffLineRemove(), source, modified, arg.Content);
                default:
                    return FillLine(new Data.DiffLineContext(), source, modified, arg.Content);
            }
        }

        private static Data.DiffLine FillLine(Data.DiffLine line, int source, int modified, string content)
        {
            line.SourceLine = source;
            line.ModifiedLine = modified;
            line.Content = content;
            return line;
        }
    }
}
