using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Moq;
using ReviewR.Web.Api;
using ReviewR.Web.Models;
using ReviewR.Web.Models.Data;
using ReviewR.Web.Models.Response;
using ReviewR.Web.Services;
using VibrantUtils;
using Xunit;

namespace ReviewR.Web.Facts.Api
{
    public class ChangesControllerFacts
    {
        public class Ctor
        {
            [Fact]
            public void RequiresNonNullArguments()
            {
                ContractAssert.NotNull(() => new ChangesController(null, new Mock<DiffService>().Object), "changes");
                ContractAssert.NotNull(() => new ChangesController(new ChangeService(new MockDataRepository()), null), "diff");
            }

            [Fact]
            public void InitializesServices()
            {
                // Arrange
                var diff = new Mock<DiffService>().Object;
                var changes = new Mock<ChangeService>().Object;

                // Act
                var c = new ChangesController(changes, diff);

                // Assert
                Assert.Same(c.Diff, diff);
                Assert.Same(c.Changes, changes);
            }
        }

        public class Get
        {
            [Fact]
            public void RequiresValidArguments()
            {
                ContractAssert.OutOfRange(() => CreateController().Get(-1), "id");
            }

            [Fact]
            public void Returns404AndNoContentWhenNoChangeWithId()
            {
                ApiCommonTests.GetReturns404WhenNoObjectWithId<FileChange>(CreateController(), new FileAddition());
            }

            [Fact]
            public void ReturnsOkWithFileDiffModelWithNoCommentsIfNoCommentsFoundForChange()
            {
                // Arrange
                var c = CreateController();
                var add = new FileAddition() { FileName = "blorg", Diff = "blarg" };
                var expected = new FileDiffModel();
                c.MockData.Changes.Add(add);
                c.MockData.SaveChanges();
                c.MockDiff.Setup(d => d.ParseFileDiff("blorg", "blarg"))
                          .Returns(expected);

                // Act
                var result = c.Get(add.Id);

                // Assert
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.Same(expected, result.GetObjectContent());
            }

            [Fact]
            public void ReturnsOkWithFileDiffModelWithCommentsIfCommentsFoundForChange()
            {
                // Arrange
                var c = CreateController();
                var add = new FileAddition()
                {
                    FileName = "blorg",
                    Diff = "blarg",
                    Comments = new List<Comment>()
                    {
                        new Comment() { DiffLineIndex = 0, Content = "blorg", UserId = ApiTestData.LoggedInUser.Id, User = ApiTestData.LoggedInUser },
                        new Comment() { DiffLineIndex = 2, Content = "blarg", UserId = ApiTestData.NotLoggedInUser.Id, User = ApiTestData.NotLoggedInUser }
                    }
                };
                var expected = new FileDiffModel()
                {
                    Lines = new List<LineDiffModel>()
                    {
                        new LineDiffModel() { Index = 0, Comments = new List<CommentModel>() },
                        new LineDiffModel() { Index = 1, Comments = new List<CommentModel>() },
                        new LineDiffModel() { Index = 2, Comments = new List<CommentModel>() },
                    }
                };
                c.MockData.Changes.Add(add);
                c.MockData.SaveChanges();
                c.MockDiff.Setup(d => d.ParseFileDiff("blorg", "blarg"))
                          .Returns(expected);

                // Act
                var result = c.Get(add.Id);

                // Assert
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                var actual = Assert.IsType<FileDiffModel>(result.GetObjectContent());
                Assert.Same(expected, actual);
                Assert.Equal(actual.Lines[0].Comments.Single().Body, "blorg");
                Assert.True(actual.Lines[0].Comments.Single().IsAuthor);
                Assert.False(actual.Lines[1].Comments.Any());
                Assert.Equal(actual.Lines[2].Comments.Single().Body, "blarg");
                Assert.False(actual.Lines[2].Comments.Single().IsAuthor);
            }

            [Fact]
            public void SilentlyIgnoresCommentsOnIncorrectLineIndicies()
            {
                // Arrange
                var c = CreateController();
                var add = new FileAddition()
                {
                    FileName = "blorg",
                    Diff = "blarg",
                    Comments = new List<Comment>()
                    {
                        new Comment() { DiffLineIndex = 42, Content = "blorg", UserId = ApiTestData.LoggedInUser.Id, User = ApiTestData.LoggedInUser },
                    }
                };
                var expected = new FileDiffModel()
                {
                    Lines = new List<LineDiffModel>()
                    {
                        new LineDiffModel() { Index = 0, Comments = new List<CommentModel>() },
                        new LineDiffModel() { Index = 1, Comments = new List<CommentModel>() },
                        new LineDiffModel() { Index = 2, Comments = new List<CommentModel>() },
                    }
                };
                c.MockData.Changes.Add(add);
                c.MockData.SaveChanges();
                c.MockDiff.Setup(d => d.ParseFileDiff("blorg", "blarg"))
                          .Returns(expected);

                // Act
                var result = c.Get(add.Id);

                // Assert
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                var actual = Assert.IsType<FileDiffModel>(result.GetObjectContent());
                Assert.Same(expected, actual);
                Assert.False(actual.Lines[0].Comments.Any());
                Assert.False(actual.Lines[1].Comments.Any());
                Assert.False(actual.Lines[2].Comments.Any());
            }
        }

        private static TestableChangesController CreateController()
        {
            return CreateController(loggedIn: true);
        }

        private static TestableChangesController CreateController(bool loggedIn)
        {
            var c = new TestableChangesController();
            if (loggedIn)
            {
                c.User = ApiTestData.LoggedIn;
            }
            return c;
        }

        private class TestableChangesController : ChangesController, ITestableController
        {
            public MockDataRepository MockData { get; set; }
            public Mock<DiffService> MockDiff { get; set; }

            public TestableChangesController()
            {
                MockData = new MockDataRepository();

                Changes = new ChangeService(MockData);
                Diff = (MockDiff = new Mock<DiffService>()).Object;
            }
        }
    }
}
