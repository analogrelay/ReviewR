using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReviewR.Diff
{
    public class DiffReader
    {
        private static readonly Regex HunkHeaderRegex = new Regex(@"^\s*@@\s*\-(?<l1>\d+),(?<c1>\d+)\s*\+(?<l2>\d+),(?<c2>\d+)\s*@@(?<c>.*)$", RegexOptions.Multiline);

        public virtual DiffSet Read(TextReader source)
        {
            if (source == null) { throw new ArgumentNullException("source"); }

            LineReader reader = new LineReader(source);
            reader.NextLine();
            
            // Scan for a file start line ("---")
            ICollection<FileDiff> files = new List<FileDiff>();
            while (!reader.EndOfFile)
            {
                while (!reader.EndOfFile && !reader.Current.StartsWith("---")) { reader.NextLine(); }
                if (!reader.EndOfFile)
                {
                    files.Add(ReadFile(reader));
                }
            }
            return new DiffSet(files);
        }

        public virtual IList<DiffHunk> ReadHunks(TextReader source)
        {
            if (source == null) { throw new ArgumentNullException("source"); }

            LineReader reader = new LineReader(source);
            reader.NextLine();

            return ReadHunks(reader);
        }

        private static FileDiff ReadFile(LineReader reader)
        {
            string original = ReadFileName(reader, "---");
            string modified = ReadFileName(reader, "+++");
            return new FileDiff(original, modified, ReadHunks(reader));
        }

        private static IList<DiffHunk> ReadHunks(LineReader reader)
        {
            IList<DiffHunk> hunks = new List<DiffHunk>();
            DiffHunk hunk;
            while ((hunk = ReadHunk(reader)) != null)
            {
                hunks.Add(hunk);
            }
            return hunks;
        }

        private static DiffHunk ReadHunk(LineReader reader)
        {
            // Read hunk header
            string header = reader.Current;
            if (String.IsNullOrEmpty(header))
            {
                return null;
            }
            Match m = HunkHeaderRegex.Match(header);
            if (!m.Success)
            {
                // End of file diff
                return null;
            }
            reader.NextLine();

            SourceCoordinate original = ReadSourceCoord(m.Groups["l1"].Value, m.Groups["c1"].Value);
            SourceCoordinate modified = ReadSourceCoord(m.Groups["l2"].Value, m.Groups["c2"].Value);
            string comment = m.Groups["c"].Value.Trim();
            DiffHunk hunk = new DiffHunk(original, modified, comment);

            LineDiff line;
            while ((line = ReadLine(reader)) != null)
            {
                hunk.Lines.Add(line);
            }
            return hunk;
        }

        private static LineDiff ReadLine(LineReader reader)
        {
            while (!reader.EndOfFile && String.IsNullOrWhiteSpace(reader.Current))
            {
                reader.NextLine();
            }
            if (reader.EndOfFile) { return null; }

            LineDiffType type;
            char typeChar = reader.Current[0];
            switch (typeChar)
            {
                case '+':
                    type = LineDiffType.Added;
                    break;
                case '-':
                    type = LineDiffType.Removed;
                    break;
                case ' ':
                    type = LineDiffType.Same;
                    break;
                default:
                    // Start of next hunk or file
                    return null;
            }
            LineDiff line = new LineDiff(type, reader.Current.Substring(1).TrimEnd('\n', '\r'));
            reader.NextLine();
            return line;
        }

        private static SourceCoordinate ReadSourceCoord(string lineStr, string colStr)
        {
            int line;
            int col;
            if (!Int32.TryParse(lineStr, out line))
            {
                throw new FormatException("Invalid Line Number: " + lineStr);
            }
            if (!Int32.TryParse(colStr, out col))
            {
                throw new FormatException("Invalid Column Number: " + colStr);
            }
            return new SourceCoordinate(line, col);
        }

        private static string ReadFileName(LineReader reader, string expectedPrefix)
        {
            string line = reader.Current.Trim();
            if (!line.StartsWith(expectedPrefix))
            {
                throw new FormatException("Invalid Unified Diff header");
            }
            reader.NextLine();

            // Extract file name
            return line.Substring(3).Trim();
        }
    }
}
