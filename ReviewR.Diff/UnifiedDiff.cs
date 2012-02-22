using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReviewR.Diff
{
    public static class UnifiedDiff
    {
        private static readonly Regex HunkHeaderRegex = new Regex(@"@@\s*\-(?<l1>\d+),(?<c1>\d+)\s*\+(?<l2>\d+),(?<c2>\d+)\s*@@");

        public static Diff Read(TextReader source)
        {
            if (source == null) { throw new ArgumentNullException("source"); }

            // Start by reading the file names
            string original = ReadFileName(source, "---");
            string modified = ReadFileName(source, "+++");
            Diff diff = new Diff(original, modified);

            // Read Hunks
            DiffHunk hunk;
            while ((hunk = ReadHunk(source)) != null)
            {
                diff.Hunks.Add(hunk);
            }

            return diff;
        }

        private static DiffHunk ReadHunk(TextReader reader)
        {
            // Read hunk header
            string header = reader.ReadLine();
            if (String.IsNullOrEmpty(header))
            {
                return null;
            }
            Match m = HunkHeaderRegex.Match(header);
            if (!m.Success)
            {
                throw new FormatException("Invalid Hunk Header: " + header);
            }

            SourceCoordinate original = ReadSourceCoord(m.Groups["l1"].Value, m.Groups["c1"].Value);
            SourceCoordinate modified = ReadSourceCoord(m.Groups["l2"].Value, m.Groups["c2"].Value);
            DiffHunk hunk = new DiffHunk(original, modified);

            DiffLine line;
            while ((line = ReadLine(reader)) != null)
            {
                hunk.Lines.Add(line);
            }
            return hunk;
        }

        private static DiffLine ReadLine(TextReader reader)
        {
            int typeInt = reader.Peek();
            if (typeInt == -1)
            {
                return null;
            }

            DiffLineType type;
            char typeChar = (char)typeInt;
            switch (typeChar)
            {
                case '+':
                    type = DiffLineType.Added;
                    break;
                case '-':
                    type = DiffLineType.Removed;
                    break;
                case ' ':
                    type = DiffLineType.Same;
                    break;
                case '@':
                    // Start of next hunk
                    return null;
                default:
                    throw new FormatException("Unknown diff line type: " + typeChar);
            }
            return new DiffLine(type, reader.ReadLine().Substring(1).TrimEnd('\n', '\r'));
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

        private static string ReadFileName(TextReader reader, string expectedPrefix)
        {
            string line = reader.ReadLine().Trim();
            if (!line.StartsWith(expectedPrefix))
            {
                throw new FormatException("Invalid Unified Diff header");
            }
            // Extract file name
            return line.Substring(3).Trim();
        }
    }
}
