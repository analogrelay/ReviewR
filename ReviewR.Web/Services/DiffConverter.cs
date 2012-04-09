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
            Data.FileChange chg = new Data.FileChange();
            if (String.Equals(fileDiff.ModifiedFile, "/dev/null"))
            {
                // Deletion
                chg.ChangeType = Data.FileChangeType.Removed;
                chg.FileName = fileDiff.OriginalFile;
            }
            else if (String.Equals(fileDiff.OriginalFile, "/dev/null"))
            {
                // Addition
                chg.ChangeType = Data.FileChangeType.Added;
                chg.FileName = fileDiff.ModifiedFile;
            }
            else
            {
                // Modification
                chg.ChangeType = Data.FileChangeType.Modified;
                chg.FileName = CleanFileName(fileDiff.ModifiedFile);
                chg.NewFileName = CleanFileName(fileDiff.ModifiedFile);
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