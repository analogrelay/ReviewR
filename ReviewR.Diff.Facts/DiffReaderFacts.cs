using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ReviewR.Diff.Facts
{
    public class DiffReaderFacts
    {
        public class ReadMethod
        {
            [Fact]
            public void RequiresNonNullTextReader()
            {
                ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => new DiffReader().Read(null));
                Assert.Equal("source", ex.ParamName);
            }

            [Fact]
            public void ThrowsFormatExceptionOnMalformedTargetFileHeader()
            {
                const string doc = @"--- Foo
GLORB!";

                FormatException ex = Assert.Throws<FormatException>(() => new DiffReader().Read(new StringReader(doc)));
                Assert.Equal("Invalid Unified Diff header", ex.Message);
            }

            [Fact]
            public void IgnoresMalfomedLineAndLooksForNextHunk()
            {
                // Arrange
                const string doc = @"--- Foo
+++ Bar
@@ -1,2 +3,4 @@
?sdfkjlkw
+Foo";

                // Act
                DiffSet diff = ReadDiff(doc);

                // Assert
                Assert.Equal(new DiffSet(new FileDiff("Foo", "Bar", hunks: new[] {
                    new DiffHunk(new SourceCoordinate(1, 2), new SourceCoordinate(3, 4), String.Empty)
                })), diff);
            }

            [Fact]
            public void IgnoresMalfomedHunkHeaderAndLooksForNextFile()
            {
                // Arrange
                const string doc = @"--- Foo
+++ Bar
*4k2ok3)??
@@ -1,2 +3,4 @@";

                // Act
                DiffSet diff = ReadDiff(doc);

                // Assert
                Assert.Equal(new DiffSet(new FileDiff("Foo", "Bar")), diff);
            }

            [Fact]
            public void CorrectlyLoadsDiffOfIdenticalFiles()
            {
                // Arrange
                const string doc = @"--- Foo
+++ Bar";

                // Act
                DiffSet diff = ReadDiff(doc);

                // Assert
                Assert.Equal(new DiffSet(new FileDiff(originalFile: "Foo", modifiedFile: "Bar", hunks: new DiffHunk[0])), diff);
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
                DiffSet diff = ReadDiff(doc);

                // Assert
                Assert.Equal(new DiffSet(new FileDiff(originalFile: "Foo", modifiedFile: "Bar", hunks: new DiffHunk[] {
                    new DiffHunk(originalLocation: new SourceCoordinate(1, 42), modifiedLocation: new SourceCoordinate(89, 92), comment: String.Empty, lines: new LineDiff[] {
                        new LineDiff(LineDiffType.Same, "Biz Boz"),
                        new LineDiff(LineDiffType.Same, "Quux")
                    })
                })), diff);
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
                DiffSet diff = ReadDiff(doc);

                // Assert
                Assert.Equal(new DiffSet(new FileDiff(originalFile: "Foo", modifiedFile: "Bar", hunks: new DiffHunk[] {
                    new DiffHunk(originalLocation: new SourceCoordinate(1, 42), modifiedLocation: new SourceCoordinate(89, 92), comment: String.Empty, lines: new LineDiff[] {
                        new LineDiff(LineDiffType.Same, "Biz Boz"),
                        new LineDiff(LineDiffType.Added, "Zoop"),
                        new LineDiff(LineDiffType.Same, "Quux"),
                        new LineDiff(LineDiffType.Removed, "Zork"),
                        new LineDiff(LineDiffType.Same, "Blork")
                    })
                })), diff);
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
                DiffSet diff = ReadDiff(doc);

                // Assert
                Assert.Equal(new DiffSet(new FileDiff(originalFile: "Foo", modifiedFile: "Bar", hunks: new DiffHunk[] {
                    new DiffHunk(originalLocation: new SourceCoordinate(1, 42), modifiedLocation: new SourceCoordinate(89, 92), comment: String.Empty, lines: new LineDiff[] {
                        new LineDiff(LineDiffType.Same, "Biz Boz"),
                        new LineDiff(LineDiffType.Added, "Zoop"),
                        new LineDiff(LineDiffType.Same, "Quux"),
                        new LineDiff(LineDiffType.Removed, "Zork"),
                        new LineDiff(LineDiffType.Same, "Blork")
                    }),
                    new DiffHunk(originalLocation: new SourceCoordinate(12, 442), modifiedLocation: new SourceCoordinate(859, 892), comment: String.Empty, lines: new LineDiff[] {
                        new LineDiff(LineDiffType.Same, "Zing"),
                        new LineDiff(LineDiffType.Added, "Zam"),
                        new LineDiff(LineDiffType.Removed, "Zoom")
                    })
                })), diff);
            }

            [Fact]
            public void ReturnsEmptyDiffSetIfNoFileHeadersFound()
            {
                // Arrange
                const string doc = @"GLORB!";

                // Act
                DiffSet diff = ReadDiff(doc);

                // Assert
                Assert.Equal(new DiffSet(), diff);
            }

            [Fact]
            public void CorrectlyParsesMultiFileGitDiff()
            {
                // Arrange
                const string doc = @"diff --git a/Foo.cs b/Foo.cs
index 123456..123456 100644
--- a/Foo.cs
+++ b/Foo.cs
@@ -1,2 +3,4 @@ Foo Bar
 Biz
+Foo
-Bar
diff --git a/Bar.cs b/Bar.cs
deleted file mode 100644
index 123456..000000
--- a/Bar.cs
+++ /dev/null
@@ -1,2 +0,0 @@
-Foo
-Bar
-Baz
Here are some random comments that will be ignored";

                // Act
                DiffSet diff = ReadDiff(doc);

                // Assert
                Assert.Equal(new DiffSet(
                    new FileDiff("a/Foo.cs", "b/Foo.cs", hunks: new[] {
                        new DiffHunk(new SourceCoordinate(1, 2), new SourceCoordinate(3, 4), "Foo Bar", lines: new [] {
                            new LineDiff(LineDiffType.Same, "Biz"),
                            new LineDiff(LineDiffType.Added, "Foo"),
                            new LineDiff(LineDiffType.Removed, "Bar")
                        })
                    }),
                    new FileDiff("a/Bar.cs", "/dev/null", hunks: new[] {
                        new DiffHunk(new SourceCoordinate(1, 2), new SourceCoordinate(0, 0), String.Empty, lines: new[] {
                            new LineDiff(LineDiffType.Removed, "Foo"),
                            new LineDiff(LineDiffType.Removed, "Bar"),
                            new LineDiff(LineDiffType.Removed, "Baz")
                        })
                    })
                ), diff);
            }

            private static DiffSet ReadDiff(string doc)
            {
                DiffSet diff;
                using (TextReader rdr = new StringReader(doc))
                {
                    diff = new DiffReader().Read(rdr);
                }
                return diff;
            }
        }
    }
}
