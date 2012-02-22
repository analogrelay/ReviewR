using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ReviewR.Diff.Facts
{
    public class UnifiedDiffFacts
    {
        public class ReadMethod
        {
            [Fact]
            public void RequiresNonNullTextReader()
            {
                ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => UnifiedDiff.Read(null));
                Assert.Equal("source", ex.ParamName);
            }

            [Fact]
            public void ThrowsFormatExceptionOnMalformedSourceFileHeader()
            {
                const string doc = @"GLORB!";

                FormatException ex = Assert.Throws<FormatException>(() => UnifiedDiff.Read(new StringReader(doc)));
                Assert.Equal("Invalid Unified Diff header", ex.Message);
            }

            [Fact]
            public void ThrowsFormatExceptionOnMalformedTargetFileHeader()
            {
                const string doc = @"--- Foo
GLORB!";

                FormatException ex = Assert.Throws<FormatException>(() => UnifiedDiff.Read(new StringReader(doc)));
                Assert.Equal("Invalid Unified Diff header", ex.Message);
            }

            [Fact]
            public void ThrowsFormatExceptionOnMalformedHunkHeader()
            {
                const string doc = @"--- Foo
+++ Bar
*4k2ok3)??";

                FormatException ex = Assert.Throws<FormatException>(() => UnifiedDiff.Read(new StringReader(doc)));
                Assert.Equal("Invalid Hunk Header: *4k2ok3)??", ex.Message);
            }

            [Fact]
            public void ThrowsFormatExceptionOnMalformedLine()
            {
                const string doc = @"--- Foo
+++ Bar
@@ -1,2 +3,4 @@
?sdfkjlkw";

                FormatException ex = Assert.Throws<FormatException>(() => UnifiedDiff.Read(new StringReader(doc)));
                Assert.Equal("Unknown diff line type: ?", ex.Message);
            }

            [Fact]
            public void CorrectlyLoadsDiffOfIdenticalFiles()
            {
                // Arrange
                const string doc = @"--- Foo
+++ Bar";

                // Act
                Diff diff = ReadDiff(doc);

                // Assert
                Assert.Equal(new Diff(originalFile: "Foo", modifiedFile: "Bar", hunks: new DiffHunk[0]), diff);
            }

            [Fact]
            public void CorrectlyLoadsDiffWithOneHunkAndNoModifiedLines()
            {
                // Arrange
                const string doc = @"--- Foo
+++ Bar
@@ -1,42 +89,92 @@
 Biz Boz
 Quux";

                // Act
                Diff diff = ReadDiff(doc);

                // Assert
                Assert.Equal(new Diff(originalFile: "Foo", modifiedFile: "Bar", hunks: new DiffHunk[] {
                    new DiffHunk(originalLocation: new SourceCoordinate(1, 42), modifiedLocation: new SourceCoordinate(89, 92), lines: new DiffLine[] {
                        new DiffLine(DiffLineType.Same, "Biz Boz"),
                        new DiffLine(DiffLineType.Same, "Quux")
                    })
                }), diff);
            }

            [Fact]
            public void CorrectlyLoadsDiffWithOneHunkAndModifiedLines()
            {
                // Arrange
                const string doc = @"--- Foo
+++ Bar
@@ -1,42 +89,92 @@
 Biz Boz
+Zoop
 Quux
-Zork
 Blork";

                // Act
                Diff diff = ReadDiff(doc);

                // Assert
                Assert.Equal(new Diff(originalFile: "Foo", modifiedFile: "Bar", hunks: new DiffHunk[] {
                    new DiffHunk(originalLocation: new SourceCoordinate(1, 42), modifiedLocation: new SourceCoordinate(89, 92), lines: new DiffLine[] {
                        new DiffLine(DiffLineType.Same, "Biz Boz"),
                        new DiffLine(DiffLineType.Added, "Zoop"),
                        new DiffLine(DiffLineType.Same, "Quux"),
                        new DiffLine(DiffLineType.Removed, "Zork"),
                        new DiffLine(DiffLineType.Same, "Blork")
                    })
                }), diff);
            }

            [Fact]
            public void CorrectlyLoadsDiffWithMultipleModifiedHunks()
            {
                // Arrange
                const string doc = @"--- Foo
+++ Bar
@@ -1,42 +89,92 @@
 Biz Boz
+Zoop
 Quux
-Zork
 Blork
@@ -12,442 +859,892 @@
 Zing
+Zam
-Zoom";

                // Act
                Diff diff = ReadDiff(doc);

                // Assert
                Assert.Equal(new Diff(originalFile: "Foo", modifiedFile: "Bar", hunks: new DiffHunk[] {
                    new DiffHunk(originalLocation: new SourceCoordinate(1, 42), modifiedLocation: new SourceCoordinate(89, 92), lines: new DiffLine[] {
                        new DiffLine(DiffLineType.Same, "Biz Boz"),
                        new DiffLine(DiffLineType.Added, "Zoop"),
                        new DiffLine(DiffLineType.Same, "Quux"),
                        new DiffLine(DiffLineType.Removed, "Zork"),
                        new DiffLine(DiffLineType.Same, "Blork")
                    }),
                    new DiffHunk(originalLocation: new SourceCoordinate(12, 442), modifiedLocation: new SourceCoordinate(859, 892), lines: new DiffLine[] {
                        new DiffLine(DiffLineType.Same, "Zing"),
                        new DiffLine(DiffLineType.Added, "Zam"),
                        new DiffLine(DiffLineType.Removed, "Zoom")
                    })
                }), diff);
            }

            private static Diff ReadDiff(string doc)
            {
                Diff diff;
                using (TextReader rdr = new StringReader(doc))
                {
                    diff = UnifiedDiff.Read(rdr);
                }
                return diff;
            }
        }
    }
}
