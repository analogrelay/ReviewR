using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace PackR
{
    public static class GitExtensions
    {
        public static IEnumerable<FileBlob> Flatten(this IEnumerable<TreeEntry> self)
        {
            return Flatten(self, String.Empty);
        }

        private static IEnumerable<FileBlob> Flatten(this IEnumerable<TreeEntry> self, string currentPath)
        {
            return self.SelectMany(t =>
            {
                string objectPath = Path.Combine(currentPath, t.Name);
                if(t.Type == GitObjectType.Blob) {
                    return new[] { new FileBlob(objectPath, (Blob)t.Target) };
                }
                else if (t.Type == GitObjectType.Tree)
                {
                    return ((Tree)t.Target).Flatten(objectPath);
                }
                else
                {
                    return Enumerable.Empty<FileBlob>();
                }
            });
        }
    }
}
