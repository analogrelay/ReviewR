using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using ReviewR.Diff;
using ReviewR.Web.Models;
using ReviewR.Web.Models.Data;
using ReviewR.Web.Services;
using VibrantUtils;
using Xunit;

namespace ReviewR.Web.Facts.Diff
{
    public class DiffServiceFacts
    {
        public class CreateFromGitDiff
        {
            [Fact]
            public void RequiresNonNullTextReader()
            {
                ContractAssert.NotNull(() => CreateService().CreateFromGitDiff(null), "source");
            }

            [Fact]
            public void ReadsFromInputAndPassesToConverter()
            {
                // Arrange
                var service = CreateService();
                var expected = new FileChange();
                var input = new StringReader("abc");
                var set = new DiffSet(new FileDiff("Orig", "Mod"));
                service.MockReader.Setup(r => r.Read(input)).Returns(set);
                service.MockConverter.Setup(c => c.ConvertFile(set.Files.First())).Returns(expected);

                // Act
                var actual = service.CreateFromGitDiff(input).SingleOrDefault();
                
                // Assert
                Assert.Same(expected, actual);
            }
        }

        public class ParseFileDiff
        {
            [Fact]
            public void RequiresValidArguments()
            {
                ContractAssert.NotNullOrEmpty(s => CreateService().ParseFileDiff(s, "diff"), "fileName");
                ContractAssert.NotNullOrEmpty(s => CreateService().ParseFileDiff("fileName", s), "diff");
            }

            // Should test the rest... but I'm laaaaaazy
        }

        private static TestableDiffService CreateService()
        {
            return new TestableDiffService(
                new Mock<DiffReader>(),
                new Mock<DiffConverter>());
        }

        public class TestableDiffService : DiffService
        {
            public Mock<DiffReader> MockReader { get; private set; }
            public Mock<DiffConverter> MockConverter { get; private set; }

            public TestableDiffService(Mock<DiffReader> reader, Mock<DiffConverter> converter)
                : base(reader.Object, converter.Object)
            {
                MockReader = reader;
                MockConverter = converter;
            }
        }
    }
}
