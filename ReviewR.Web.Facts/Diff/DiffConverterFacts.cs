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
                    Lines = new List<Data.DiffLine>() {
                        new Data.DiffLineAdd() { SourceLine = 0, ModifiedLine = 10, Content = "Foo" },
                        new Data.DiffLineAdd() { SourceLine = 1, ModifiedLine = 11, Content = "Bar" },
                        new Data.DiffLineAdd() { SourceLine = 2, ModifiedLine = 12, Content = "Baz" }
                    }
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
                    Lines = new List<Data.DiffLine>()
                    {
                        new Data.DiffLineRemove() { SourceLine = 0, ModifiedLine = 10, Content = "Foo" },
                        new Data.DiffLineRemove() { SourceLine = 1, ModifiedLine = 11, Content = "Bar" },
                        new Data.DiffLineRemove() { SourceLine = 1, ModifiedLine = 11, Content = "Baz" },
                    }
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
                    Lines = new List<Data.DiffLine>() {
                        new Data.DiffLineAdd() { SourceLine = 0, ModifiedLine = 10, Content = "Foo" },
                        new Data.DiffLineRemove() { SourceLine = 1, ModifiedLine = 11, Content = "Bar" },
                        new Data.DiffLineContext() { SourceLine = 2, ModifiedLine = 12, Content = "Baz" }
                    }
                }, actual, new PropertyEqualityComparer());
            }
        }
    }
}
