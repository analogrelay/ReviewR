using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReviewR.Diff;
using VibrantUtils;
using Data = ReviewR.Web.Models.Data;

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
                chg = new Data.FileRemoval()
                {
                    FileName = CleanFileName(fileDiff.OriginalFile)
                };
            }
            else if (String.Equals(fileDiff.OriginalFile, "/dev/null"))
            {
                // Addition
                chg = new Data.FileAddition()
                {
                    FileName = CleanFileName(fileDiff.ModifiedFile)
                };
            }
            else
            {
                // Modification
                chg = new Data.FileModification()
                {
                    FileName = CleanFileName(fileDiff.OriginalFile),
                    NewFileName = CleanFileName(fileDiff.ModifiedFile)
                };
            }

            // Fill the lines
            chg.Diff = String.Join(Environment.NewLine, fileDiff.Hunks.Select(WriteHunk));
            return chg;
        }

        public string CleanFileName(string path)
        {
            return path.TrimStart('a', 'b');
        }

        private string WriteHunk(DiffHunk arg)
        {
            return String.Format(
                "@@ -{0},{1} +{2},{3} @@{4}",
                arg.OriginalLocation.Line,
                arg.OriginalLocation.Column,
                arg.ModifiedLocation.Line,
                arg.ModifiedLocation.Column,
                String.IsNullOrEmpty(arg.Comment) ? "" : (" " + arg.Comment)) +
                Environment.NewLine +
                String.Join(Environment.NewLine, arg.Lines.Select(WriteLine));
        }

        private string WriteLine(LineDiff arg)
        {
            switch (arg.Type)
            {
                case LineDiffType.Added:
                    return "+" + arg.Content;
                case LineDiffType.Removed:
                    return "-" + arg.Content;
                default:
                    return " " + arg.Content;
            }
        }
    }
}