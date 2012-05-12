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
    public class IterationsControllerFacts
    {
        public class Ctor
        {
            [Fact]
            public void RequiresNonNullArguments()
            {
                ContractAssert.NotNull(() => new IterationsController(null, new Mock<DiffService>().Object), "iterations");
                ContractAssert.NotNull(() => new IterationsController(new Mock<IterationService>().Object, null), "diff");
            }

            [Fact]
            public void InitializesServices()
            {
                // Arrange
                var iterations = new Mock<IterationService>().Object;
                var diff = new Mock<DiffService>().Object;

                // Act
                var c = new IterationsController(iterations, diff);

                // Assert
                Assert.Same(c.Iterations, iterations);
                Assert.Same(c.Diff, diff);
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
            public void Returns404AndNoContentWhenNoIterationWithId()
            {
                ApiCommonTests.GetReturns404WhenNoObjectWithId<Iteration>(CreateController());
            }

            [Fact]
            public void ReturnsForbiddenIfUserNotCreatorAndIterationNotPublished()
            {
                // Arrange
                var c = CreateController();
                var iter = new Iteration() { Review = new Review() { UserId = ApiTestData.NotLoggedInUser.Id }, Published = false };
                c.MockData.Iterations.Add(iter);
                c.MockData.SaveChanges();

                // Act
                var result = c.Get(iter.Id);

                // Assert
                Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
                Assert.Null(result.Content);
            }

            [Fact]
            public void ReturnsOkIfUserCreatorAndIterationNotPublished()
            {
                // Arrange
                var c = CreateController();
                var iter = new Iteration() { Review = new Review() { UserId = ApiTestData.LoggedInUser.Id }, Published = false, Files = new List<FileChange>() };
                c.MockData.Iterations.Add(iter);
                c.MockData.SaveChanges();

                // Act
                var result = c.Get(iter.Id);

                // Assert
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            }

            [Fact]
            public void ReturnsOkIfUserNotCreatorAndIterationPublished()
            {
                // Arrange
                var c = CreateController();
                var iter = new Iteration() { Review = new Review() { UserId = ApiTestData.NotLoggedInUser.Id }, Published = true, Files = new List<FileChange>() };
                c.MockData.Iterations.Add(iter);
                c.MockData.SaveChanges();

                // Act
                var result = c.Get(iter.Id);

                // Assert
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            }

            [Fact]
            public void ReturnsFilesGroupedByFolder()
            {
                // Arrange
                var c = CreateController();
                var iter = new Iteration()
                {
                    Review = new Review() { UserId = ApiTestData.NotLoggedInUser.Id },
                    Published = true,
                    Files = new List<FileChange>()
                    {
                        new FileAddition() { Id = 1, FileName = "Foo/Bar/Baz" },
                        new FileRemoval() { Id = 2, FileName = "Foo/Bar/Boz" },
                        new FileAddition() { Id = 3, FileName = "Biz/Qux" },
                        new FileRemoval() { Id = 4, FileName = "Foo/Tux" },
                        new FileAddition() { Id = 5, FileName = "Box" }
                    }
                };
                c.MockData.Iterations.Add(iter);
                c.MockData.SaveChanges();

                // Act
                var result = c.Get(iter.Id);

                // Assert
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.Equal(new[] {
                    new FolderModel {
                        Name = "Foo/Bar",
                        Files = new [] {
                            new FileModel { Id = 1, FileName = "Baz", FullPath = "Foo/Bar/Baz", ChangeType = FileChangeType.Added },
                            new FileModel { Id = 2, FileName = "Boz", FullPath = "Foo/Bar/Boz", ChangeType = FileChangeType.Removed }
                        }
                    },
                    new FolderModel {
                        Name = "Biz",
                        Files = new [] {
                            new FileModel { Id = 3, FileName = "Qux", FullPath = "Biz/Qux", ChangeType = FileChangeType.Added },
                        }
                    },
                    new FolderModel {
                        Name = "Foo",
                        Files = new [] {
                            new FileModel { Id = 4, FileName = "Tux", FullPath = "Foo/Tux", ChangeType = FileChangeType.Removed },
                        }
                    },
                    new FolderModel {
                        Name = "/",
                        Files = new [] {
                            new FileModel { Id = 5, FileName = "Box", FullPath = "Box", ChangeType = FileChangeType.Added },
                        }
                    },
                }, result.GetObjectContent(), new PropertyEqualityComparer(typeEquality: false));
            }
        }

        private static TestableIterationsController CreateController()
        {
            return CreateController(loggedIn: true);
        }

        private static TestableIterationsController CreateController(bool loggedIn)
        {
            var c = new TestableIterationsController();
            if (loggedIn)
            {
                c.User = ApiTestData.LoggedIn;
            }
            return c;
        }

        private class TestableIterationsController : IterationsController, ITestableController
        {
            public MockDataRepository MockData { get; set; }
            public Mock<DiffService> MockDiff { get; set; }

            public TestableIterationsController()
            {
                MockData = new MockDataRepository();

                Diff = (MockDiff = new Mock<DiffService>()).Object;
                Iterations = new IterationService(MockData, Diff);
            }
        }
    }
}
