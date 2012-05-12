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
    public class MyControllerFacts
    {
        public class Ctor
        {
            [Fact]
            public void RequiresNonNullArguments()
            {
                ContractAssert.NotNull(() => new MyController(null), "reviews");
            }

            [Fact]
            public void InitializesServices()
            {
                // Arrange
                var reviews = new Mock<ReviewService>().Object;

                // Act
                var c = new MyController(reviews);

                // Assert
                Assert.Same(c.Reviews, reviews);
            }
        }

        public class GetReviews
        {
            [Fact]
            public void Returns200WithEmptyListsIfNoReviews()
            {
                // Arrange
                var c = CreateController();

                // Act
                var result = c.GetReviews();

                // Assert
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                var content = Assert.IsType<DashboardModel>(result.GetObjectContent());
                Assert.Empty(content.Assigned);
                Assert.Empty(content.Created);
            }

            [Fact]
            public void Returns200WithCorrectReviewDistribution()
            {
                // Arrange
                var c = CreateController();
                var created = new Review() { Name = "Created", Creator = ApiTestData.LoggedInUser, UserId = ApiTestData.LoggedInUser.Id };
                var assigned = new Review() { Name = "Assigned", Creator = ApiTestData.NotLoggedInUser, UserId = ApiTestData.NotLoggedInUser.Id };
                c.MockData.Reviews.Add(created);
                c.MockData.Reviews.Add(assigned);
                c.MockData.SaveChanges();

                // Act
                var result = c.GetReviews();

                // Assert
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                var content = Assert.IsType<DashboardModel>(result.GetObjectContent());
                Assert.Equal(new ReviewModel()
                {
                    Id = assigned.Id,
                    Title = assigned.Name,
                    Author = new UserModel()
                    {
                        Id = assigned.Creator.Id,
                        Email = assigned.Creator.Email,
                        DisplayName = assigned.Creator.DisplayName
                    }
                }, content.Assigned.Single(), new PropertyEqualityComparer());
                Assert.Equal(new ReviewModel()
                {
                    Id = created.Id,
                    Title = created.Name,
                    Author = new UserModel()
                    {
                        Id = created.Creator.Id,
                        Email = created.Creator.Email,
                        DisplayName = created.Creator.DisplayName
                    }
                }, content.Created.Single(), new PropertyEqualityComparer());
            }
        }

        private static TestableMyController CreateController()
        {
            return CreateController(loggedIn: true);
        }

        private static TestableMyController CreateController(bool loggedIn)
        {
            var c = new TestableMyController();
            if (loggedIn)
            {
                c.User = ApiTestData.LoggedIn;
            }
            return c;
        }

        private class TestableMyController : MyController, ITestableController
        {
            public MockDataRepository MockData { get; set; }

            public TestableMyController()
            {
                MockData = new MockDataRepository();
                Reviews = new ReviewService(MockData);
            }
        }
    }
}
