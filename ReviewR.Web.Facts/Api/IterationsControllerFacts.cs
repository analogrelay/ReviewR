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
                ContractAssert.NotNull(() => new IterationsController(null), "iterations");
            }

            [Fact]
            public void InitializesServices()
            {
                // Arrange
                var iterations = new Mock<IterationService>().Object;

                // Act
                var c = new IterationsController(iterations);

                // Assert
                Assert.Same(c.Iterations, iterations);
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

        public class Post
        {
            [Fact]
            public void RequiresValidArguments()
            {
                ContractAssert.OutOfRange(() => CreateController().Post(-1), "reviewId");
            }

            [Fact]
            public void Returns404IfNoReviewWithSpecifiedId()
            {
                // Arrange
                var c = CreateController();
                var rev = new Review() { Name = "Blarg", Iterations = new List<Iteration>() };
                c.MockData.Reviews.Add(rev);
                c.MockData.SaveChanges();

                // Act
                var result = c.Post(rev.Id + 42);

                // Assert
                Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
                Assert.False(rev.Iterations.Any());
            }

            [Fact]
            public void Returns403IfCurrentUserNotOwnerOfSpecifiedReview()
            {
                // Arrange
                var c = CreateController();
                var rev = new Review() { Name = "Blarg", Iterations = new List<Iteration>(), UserId = ApiTestData.NotLoggedInUser.Id };
                c.MockData.Reviews.Add(rev);
                c.MockData.SaveChanges();

                // Act
                var result = c.Post(rev.Id);

                // Assert
                Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
                Assert.False(rev.Iterations.Any());
            }

            [Fact]
            public void Returns201WithCreatedIterationAndAddsIterationIfReviewExistsAndCurrentUserIsOwner()
            {
                // Arrange
                var c = CreateController();
                var rev = new Review() { Name = "Blarg", Iterations = new List<Iteration>(), UserId = ApiTestData.LoggedInUser.Id };
                c.MockData.Reviews.Add(rev);
                c.MockData.SaveChanges();

                // Act
                var result = c.Post(rev.Id);

                // Assert
                Assert.Equal(HttpStatusCode.Created, result.StatusCode);
                Assert.False(rev.Iterations.Single().Published);
            }
        }

        public class Delete
        {
            [Fact]
            public void RequiresValidArguments()
            {
                ContractAssert.OutOfRange(() => CreateController().Delete(-1), "id");
            }

            [Fact]
            public void Returns404IfNoIterationWithSpecifiedId()
            {
                // Arrange
                var c = CreateController();
                var iter = new Iteration() { Description = "blorg" };
                c.MockData.Iterations.Add(iter);
                c.MockData.SaveChanges();

                // Act
                var result = c.Delete(iter.Id + 42);

                // Assert
                Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
                Assert.Equal("blorg", c.MockData.Iterations.Single().Description);
            }

            [Fact]
            public void Returns403IfCurrentUserNotOwnerOfSpecifiedReview()
            {
                // Arrange
                var c = CreateController();
                var iter = new Iteration() { Description = "blorg", Review = new Review() { UserId = ApiTestData.NotLoggedInUser.Id } };
                c.MockData.Iterations.Add(iter);
                c.MockData.SaveChanges();

                // Act
                var result = c.Delete(iter.Id);

                // Assert
                Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
                Assert.Equal("blorg", c.MockData.Iterations.Single().Description);
            }

            [Fact]
            public void Returns204AndRemovesIterationIfIterationExistsAndCurrentUserIsOwner()
            {
                // Arrange
                var c = CreateController();
                var iter = new Iteration() { Description = "blorg", Review = new Review() { UserId = ApiTestData.LoggedInUser.Id } };
                c.MockData.Iterations.Add(iter);
                c.MockData.SaveChanges();

                // Act
                var result = c.Delete(iter.Id);

                // Assert
                Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
                Assert.False(c.MockData.Iterations.Any());
            }
        }

        public class PutPublished
        {
            [Fact]
            public void RequiresValidArguments()
            {
                ContractAssert.OutOfRange(() => CreateController().PutPublished(-1, false), "id");
            }

            [Fact]
            public void Returns404IfNoIterationWithSpecifiedId()
            {
                // Arrange
                var c = CreateController();
                var iter = new Iteration() { Description = "blorg", Published = false };
                c.MockData.Iterations.Add(iter);
                c.MockData.SaveChanges();

                // Act
                var result = c.PutPublished(iter.Id + 42, published: true);

                // Assert
                Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
                Assert.False(c.MockData.Iterations.Single().Published);
            }

            [Fact]
            public void Returns403IfCurrentUserNotOwnerOfSpecifiedReview()
            {
                // Arrange
                var c = CreateController();
                var iter = new Iteration() { Published = false, Review = new Review() { UserId = ApiTestData.NotLoggedInUser.Id } };
                c.MockData.Iterations.Add(iter);
                c.MockData.SaveChanges();

                // Act
                var result = c.PutPublished(iter.Id, published: true);

                // Assert
                Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
                Assert.False(c.MockData.Iterations.Single().Published);
            }

            [Fact]
            public void Returns200WithModifiedIterationAndModifiesIterationIfExistsAndUserIsOwner()
            {
                // Arrange
                var c = CreateController();
                var iter = new Iteration() { Published = false, Review = new Review() { UserId = ApiTestData.LoggedInUser.Id } };
                c.MockData.Iterations.Add(iter);
                c.MockData.SaveChanges();

                // Act
                var result = c.PutPublished(iter.Id, published: true);

                // Assert
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.True(c.MockData.Iterations.Single().Published);
            }
        }

        public class PutDiff
        {
            [Fact]
            public void RequiresValidArguments()
            {
                ContractAssert.OutOfRange(() => CreateController().PutDiff(-1, "foo"), "id");
                ContractAssert.NotNullOrEmpty(s => CreateController().PutDiff(0, s), "diff");
            }

            [Fact]
            public void Returns404IfNoIterationWithSpecifiedId()
            {
                // Arrange
                var c = CreateController();
                var iter = new Iteration() { Files = new List<FileChange>() };
                c.MockData.Iterations.Add(iter);
                c.MockData.SaveChanges();

                // Act
                var result = c.PutDiff(iter.Id + 42, "glarb");

                // Assert
                Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
                Assert.False(c.MockData.Iterations.Single().Files.Any());
            }

            [Fact]
            public void Returns403IfCurrentUserNotOwnerOfSpecifiedReview()
            {
                // Arrange
                var c = CreateController();
                var iter = new Iteration() { Files = new List<FileChange>(), Review = new Review() { UserId = ApiTestData.NotLoggedInUser.Id } };
                c.MockData.Iterations.Add(iter);
                c.MockData.SaveChanges();

                // Act
                var result = c.PutDiff(iter.Id, "glarb");

                // Assert
                Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
                Assert.False(c.MockData.Iterations.Single().Files.Any());
            }

            [Fact]
            public void Returns200WithModifiedIterationAndModifiesIterationIfExistsAndUserIsOwner()
            {
                // Arrange
                var c = CreateController(mockIterationsService: true);
                c.MockIterations.Setup(i => i.AddDiffToIteration(123, "glarb", ApiTestData.LoggedInUser.Id))
                                .Returns(DatabaseActionResult<Iteration>.Success(new Iteration() {
                                    Id = 123,
                                    Description = "blarg",
                                    Published = true
                                }))
                                .Verifiable();

                // Act
                var result = c.PutDiff(123, "glarb");

                // Assert
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.Equal(new IterationModel() {
                    Id = 123,
                    Description = "blarg",
                    Published = true,
                    Order = null
                }, result.GetObjectContent(), new PropertyEqualityComparer());
                c.MockIterations.VerifyAll();
            }
        }

        private static TestableIterationsController CreateController()
        {
            return CreateController(mockIterationsService: false, loggedIn: true);
        }

        private static TestableIterationsController CreateController(bool mockIterationsService)
        {
            return CreateController(mockIterationsService, loggedIn: true);
        }

        private static TestableIterationsController CreateController(bool mockIterationsService, bool loggedIn)
        {
            var c = mockIterationsService ? new TestableIterationsController(new Mock<IterationService>()) : new TestableIterationsController();
            if (loggedIn)
            {
                c.User = ApiTestData.LoggedIn;
            }
            return c;
        }

        private class TestableIterationsController : IterationsController, ITestableController
        {
            public MockDataRepository MockData { get; set; }
            public Mock<IterationService> MockIterations { get; set; }

            public TestableIterationsController()
            {
                MockData = new MockDataRepository();
                Iterations = new IterationService(MockData, new Mock<DiffService>().Object);
            }

            public TestableIterationsController(Mock<IterationService> mockIterations)
            {
                MockData = new MockDataRepository();
                Iterations = (MockIterations = mockIterations).Object;
            }
        }
    }
}
