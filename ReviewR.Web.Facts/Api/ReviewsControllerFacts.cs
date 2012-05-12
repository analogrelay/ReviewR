using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Moq;
using ReviewR.Web.Api;
using ReviewR.Web.Models;
using ReviewR.Web.Models.Data;
using ReviewR.Web.Models.Request;
using ReviewR.Web.Models.Response;
using ReviewR.Web.Services;
using VibrantUtils;
using Xunit;

namespace ReviewR.Web.Facts.Api
{
    public class ReviewsControllerFacts
    {
        public class Ctor
        {
            [Fact]
            public void RequiresNonNullArguments()
            {
                ContractAssert.NotNull(() => new ReviewsController(null), "reviews");
            }

            [Fact]
            public void InitializesServices()
            {
                // Arrange
                var reviews = new Mock<ReviewService>().Object;

                // Act
                var c = new ReviewsController(reviews);

                // Assert
                Assert.Same(c.Reviews, reviews);
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
            public void Returns404IfNoReviewWithSpecifiedId()
            {
                ApiCommonTests.GetReturns404WhenNoObjectWithId<Review>(CreateController());
            }

            [Fact]
            public void Returns200WithReviewData()
            {
                // Arrange
                var c = CreateController();
                var rev = new Review()
                {
                    Name = "Review",
                    Creator = ApiTestData.LoggedInUser,
                    UserId = ApiTestData.LoggedInUser.Id,
                    Description = "Description",
                    Iterations = new List<Iteration>()
                };
                c.MockData.Reviews.Add(rev);
                c.MockData.SaveChanges();

                // Act
                var result = c.Get(rev.Id);

                // Assert
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.Equal(new ReviewDetailModel()
                {
                    Id = rev.Id,
                    Title = "Review",
                    Description = "Description",
                    Owner = true,
                    Author = new UserModel()
                    {
                        Id = ApiTestData.LoggedInUser.Id,
                        Email = ApiTestData.LoggedInUser.Email,
                        DisplayName = ApiTestData.LoggedInUser.DisplayName
                    },
                    Iterations = new List<IterationModel>()
                }, result.GetObjectContent(), new PropertyEqualityComparer());
            }

            [Fact]
            public void Returns200WithAllIterationsIfUserIsOwner()
            {
                // Arrange
                var c = CreateController();
                var rev = new Review()
                {
                    Name = "Review",
                    Creator = ApiTestData.LoggedInUser,
                    UserId = ApiTestData.LoggedInUser.Id,
                    Description = "Description",
                    Iterations = new List<Iteration>()
                    {
                        new Iteration() { Description = "Iter1", Published = true },
                        new Iteration() { Description = "Iter2", Published = false },
                        new Iteration() { Description = "Iter3", Published = true }
                    }
                };
                c.MockData.Reviews.Add(rev);
                c.MockData.SaveChanges();

                // Act
                var result = c.Get(rev.Id);

                // Assert
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.Equal(new ReviewDetailModel()
                {
                    Id = rev.Id,
                    Title = "Review",
                    Description = "Description",
                    Owner = true,
                    Author = new UserModel()
                    {
                        Id = ApiTestData.LoggedInUser.Id,
                        Email = ApiTestData.LoggedInUser.Email,
                        DisplayName = ApiTestData.LoggedInUser.DisplayName
                    },
                    Iterations = new List<IterationModel>()
                    {
                        new IterationModel() { Id = rev.Iterations.ElementAt(0).Id, Order = 0, Published = true, Description = "Iter1" },
                        new IterationModel() { Id = rev.Iterations.ElementAt(1).Id, Order = 1, Published = false, Description = "Iter2" },
                        new IterationModel() { Id = rev.Iterations.ElementAt(2).Id, Order = 2, Published = true, Description = "Iter3" },
                    }
                }, result.GetObjectContent(), new PropertyEqualityComparer());
            }

            [Fact]
            public void Returns200WithPublishedIterationsIfUserIsNotOwner()
            {
                // Arrange
                var c = CreateController();
                var rev = new Review()
                {
                    Name = "Review",
                    Creator = ApiTestData.NotLoggedInUser,
                    UserId = ApiTestData.NotLoggedInUser.Id,
                    Description = "Description",
                    Iterations = new List<Iteration>()
                    {
                        new Iteration() { Description = "Iter1", Published = true },
                        new Iteration() { Description = "Iter2", Published = false },
                        new Iteration() { Description = "Iter3", Published = true }
                    }
                };
                c.MockData.Reviews.Add(rev);
                c.MockData.SaveChanges();

                // Act
                var result = c.Get(rev.Id);

                // Assert
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.Equal(new ReviewDetailModel()
                {
                    Id = rev.Id,
                    Title = "Review",
                    Description = "Description",
                    Owner = false,
                    Author = new UserModel()
                    {
                        Id = ApiTestData.NotLoggedInUser.Id,
                        Email = ApiTestData.NotLoggedInUser.Email,
                        DisplayName = ApiTestData.NotLoggedInUser.DisplayName
                    },
                    Iterations = new List<IterationModel>()
                    {
                        new IterationModel() { Id = rev.Iterations.ElementAt(0).Id, Order = 0, Published = true, Description = "Iter1" },
                        new IterationModel() { Id = rev.Iterations.ElementAt(2).Id, Order = 1, Published = true, Description = "Iter3" },
                    }
                }, result.GetObjectContent(), new PropertyEqualityComparer());
            }
        }

        public class Post
        {
            [Fact]
            public void RequiresValidArguments()
            {
                ContractAssert.NotNull(() => CreateController().Post(null), "model");
            }

            [Fact]
            public void Returns400IfModelStateInvalid()
            {
                // Arrange
                var c = CreateController();
                c.ModelState.AddModelError("Title", "ERROR'D!!");

                // Act
                var result = c.Post(new ReviewRequestModel());

                // Assert
                Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
                Assert.Equal(new string[] { "ERROR'D!!" }, result.GetObjectContent());
            }

            [Fact]
            public void Returns201AndCreatesReviewIfModelValid()
            {
                // Arrange
                var c = CreateController();
                
                // Act
                var result = c.Post(new ReviewRequestModel()
                {
                    Title = "New Review",
                    Description = "A new review!"
                });

                // Assert
                Assert.Equal(HttpStatusCode.Created, result.StatusCode);
                Assert.Equal(new ReviewModel()
                {
                    Id = c.MockData.LastId,
                    Title = "New Review",
                    Author = new UserModel()
                    {
                        Id = ApiTestData.LoggedInUser.Id,
                        DisplayName = ApiTestData.LoggedInUser.DisplayName,
                        Email = ApiTestData.LoggedInUser.Email
                    }
                }, result.GetObjectContent(), new PropertyEqualityComparer());
            }
        }

        private static TestableReviewsController CreateController()
        {
            return CreateController(loggedIn: true);
        }

        private static TestableReviewsController CreateController(bool loggedIn)
        {
            var c = new TestableReviewsController();
            if (loggedIn)
            {
                c.User = ApiTestData.LoggedIn;
            }
            return c;
        }

        private class TestableReviewsController : ReviewsController, ITestableController
        {
            public MockDataRepository MockData { get; set; }

            public TestableReviewsController()
            {
                MockData = new MockDataRepository();
                Reviews = new ReviewService(MockData);
            }
        }
    }
}
