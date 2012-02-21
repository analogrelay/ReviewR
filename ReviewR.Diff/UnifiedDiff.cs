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

        public static async Task<Diff> Read(TextReader unifiedDiff)
        {
            // Start by reading the file names
            string original = await ReadFileName(unifiedDiff, "---");
            string modified = await ReadFileName(unifiedDiff, "+++");
            Diff diff = new Diff(original, modified);

            // Read Hunks
            DiffHunk hunk;
            while ((hunk = await ReadHunk(unifiedDiff)) != null)
            {
                diff.Hunks.Add(hunk);
            }

            return diff;
        }

        private static async Task<DiffHunk> ReadHunkAsync(TextReader reader)
        {
            // Read hunk header
            string header = await reader.ReadLineAsync();
            Match m = HunkHeaderRegex.Match(header);
            if (!m.Success)
            {
                throw new FormatException("Invalid Hunk Header: " + header);
            }

            SourceCoordinate original = ReadSourceCoord(m.Groups["l1"].Value, m.Groups["c1"].Value);
            SourceCoordinate modified = ReadSourceCoord(m.Groups["l2"].Value, m.Groups["c2"].Value);
            DiffHunk hunk = new DiffHunk(original, modified);

            DiffLine line;
            while (((await line = ReadLineAsync(reader)) != null)
            {
                hunk.Lines.Add(line);
            }
        }

        private static async Task<DiffLine> ReadLineAsync(TextReader reader)
        {
            int typeChar = reader.Peek();
            DiffLineType type;
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
                default:
                    throw new FormatException("Unknown diff line type: " + typeChar);
            }
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

        private static async Task<string> ReadFileName(TextReader reader, string expectedPrefix)
        {
            string line = (await reader.ReadLineAsync()).Trim();
            if (!line.StartsWith(expectedPrefix))
            {
                throw new FormatException("Invalid Unified Diff file");
            }
            // Extract file name
            return line.Substring(3).Trim();
        }
    }
}
