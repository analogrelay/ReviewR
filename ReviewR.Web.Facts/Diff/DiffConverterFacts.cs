using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviewR.Diff;
using Data = ReviewR.Web.Models;
using ReviewR.Web.Services;
using VibrantUtils;
using Xunit;

namespace ReviewR.Web.Facts.Diff
{
    public class DiffConverterFacts
    {
        public class ConvertFile
        {
            [Fact]
            public void RequiresNonNullFileDiff()
            {
                ContractAssert.NotNull(() => new DiffConverter().ConvertFile(null), "fileDiff");
            }

            [Fact]
            public void CorrectlyConvertsAdd()
            {
                // Arrange
                var diff = new FileDiff("/dev/null", "NewFile",
                    new DiffHunk(new SourceCoordinate(0, 0), new SourceCoordinate(10, 0), String.Empty,
                        new LineDiff(LineDiffType.Added, "Foo"),
                        new LineDiff(LineDiffType.Added, "Bar"),
                        new LineDiff(LineDiffType.Added, "Baz")));

                // Act
                var actual = new DiffConverter().ConvertFile(diff);

                // Assert
                Assert.Equal(new Data.FileAddition()
                {
                    FileName = "NewFile",
                    Diff = @"@@ -0,0 +10,0 @@
+Foo
+Bar
+Baz"
                }, actual, new PropertyEqualityComparer());
            }

            [Fact]
            public void CorrectlyConvertsDelete()
            {
                // Arrange
                var diff = new FileDiff("OldFile", "/dev/null",
                    new DiffHunk(new SourceCoordinate(0, 0), new SourceCoordinate(10, 0), String.Empty,
                        new LineDiff(LineDiffType.Removed, "Foo"),
                        new LineDiff(LineDiffType.Removed, "Bar"),
                        new LineDiff(LineDiffType.Removed, "Baz")));

                // Act
                var actual = new DiffConverter().ConvertFile(diff);

                // Assert
                Assert.Equal(new Data.FileDeletion()
                {
                    FileName = "NewFile",
                    Diff = @"@@ -0,0 +10,0 @@
-Foo
-Bar
-Baz"
                }, actual, new PropertyEqualityComparer());
            }

            [Fact]
            public void CorrectlyConvertsModification()
            {
                // Arrange
                var diff = new FileDiff("OldFile", "NewFile",
                    new DiffHunk(new SourceCoordinate(0, 0), new SourceCoordinate(10, 0), String.Empty,
                        new LineDiff(LineDiffType.Added, "Foo"),
                        new LineDiff(LineDiffType.Removed, "Bar"),
                        new LineDiff(LineDiffType.Same, "Baz")));

                // Act
                var actual = new DiffConverter().ConvertFile(diff);

                // Assert
                Assert.Equal(new Data.FileModification()
                {
                    FileName = "OldFile",
                    NewFileName = "NewFile",
                    Diff = @"@@ -0,0 +10,0 @@
+Foo
-Bar
 Baz"
                }, actual, new PropertyEqualityComparer());
            }

            [Fact]
            public void RemovesPrefixesFromGitPaths()
            {
                // Arrange
                var diff = new FileDiff("a/Foo/Bar", "b/Biz/Baz",
                    new DiffHunk(new SourceCoordinate(0, 0), new SourceCoordinate(10, 0), String.Empty,
                        new LineDiff(LineDiffType.Added, "Foo"),
                        new LineDiff(LineDiffType.Removed, "Bar"),
                        new LineDiff(LineDiffType.Same, "Baz")));

                // Act
                var actual = new DiffConverter().ConvertFile(diff);

                // Assert
                Assert.Equal(new Data.FileModification()
                {
                    FileName = "/Foo/Bar",
                    NewFileName = "/Biz/Baz",
                    Diff = @"@@ -0,0 +10,0 @@
+Foo
-Bar
 Baz"
                }, actual, new PropertyEqualityComparer());
            }
        }
    }
}
