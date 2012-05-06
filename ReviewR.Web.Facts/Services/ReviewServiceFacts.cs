using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using ReviewR.Web.Models;
using ReviewR.Web.Models.Data;
using ReviewR.Web.Services;
using VibrantUtils;
using Xunit;
using Xunit.Extensions;

namespace ReviewR.Web.Facts.Services
{
    public class ReviewServiceFacts
    {
        public class Ctor
        {
            [Fact]
            public void InitializesValues()
            {
                // Arrange
                var data = new MockDataRepository();

                // Act
                ReviewService auth = new ReviewService(data);

                // Assert
                Assert.Same(data, auth.Data);
            }

            [Fact]
            public void RequiresNonNullArguments()
            {
                ContractAssert.NotNull(() => new ReviewService(null), "data");
            }
        }

        public class CreateReview
        {
            [Fact]
            public void RequiresValidArguments()
            {
                ContractAssert.NotNullOrEmpty(s => CreateService().CreateReview(s, "desc", 0), "name");
                ContractAssert.NotNullOrEmpty(s => CreateService().CreateReview("name", s, 0), "description");
                ContractAssert.OutOfRange(() => CreateService().CreateReview("name", "desc", -1), "ownerId");
            }

            [Fact]
            public void CreatesExpectedReview()
            {
                // Arrange
                DateTime start = DateTime.UtcNow;
                var reviews = CreateService();

                // Act
                Review returned = reviews.CreateReview("name", "description", 42);

                // Assert
                Review db = reviews.MockData.Reviews.Single();
                Assert.Same(returned, db);
                Assert.Equal(new Review()
                {
                    Name = "name",
                    Description = "description",
                    UserId = 42,
                    Iterations = new List<Iteration>() {
                        new Iteration() { StartedOn = returned.Iterations.Single().StartedOn }
                    },
                    CreatedOn = returned.CreatedOn
                }, returned, new PropertyEqualityComparer());
                Assert.True(returned.Iterations.Single().StartedOn >= start && returned.Iterations.Single().StartedOn <= DateTime.UtcNow);
                Assert.True(returned.CreatedOn >= start && returned.CreatedOn <= DateTime.UtcNow);
            }
        }

        public class GetReviewsCreatedBy
        {
            [Fact]
            public void RequiresValidArguments()
            {
                ContractAssert.OutOfRange(() => CreateService().GetReviewsCreatedBy(-1), "userId");
            }

            [Fact]
            public void ReturnsEmptyListIfNoReviewsCreatedByUser()
            {
                // Arrange
                var reviews = CreateService();
                var review = new Review() { UserId = 42 };
                reviews.MockData.Reviews.Add(review);
                reviews.MockData.SaveChanges();

                // Act
                var result = reviews.GetReviewsCreatedBy(24);

                // Assert
                Assert.Empty(result);
            }

            [Fact]
            public void ReturnsOnlyReviewsCreatedByUser()
            {
                // Arrange
                var reviews = CreateService();
                var review1 = new Review() { UserId = 42 };
                var review2 = new Review() { UserId = 24 };
                reviews.MockData.Reviews.Add(review1);
                reviews.MockData.Reviews.Add(review2);
                reviews.MockData.SaveChanges();

                // Act
                var result = reviews.GetReviewsCreatedBy(42);

                // Assert
                Assert.Same(review1, result.Single());
            }
        }

        public class GetReviewsAssignedTo
        {
            [Fact]
            public void RequiresValidArguments()
            {
                ContractAssert.OutOfRange(() => CreateService().GetReviewsAssignedTo(-1), "userId");
            }

            [Fact]
            public void ReturnsEmptyListIfNoReviewsAssignedToUser()
            {
                // Arrange
                var reviews = CreateService();
                var review = new Review() { UserId = 24 };
                reviews.MockData.Reviews.Add(review);
                reviews.MockData.SaveChanges();

                // Act
                var result = reviews.GetReviewsAssignedTo(24);

                // Assert
                Assert.Empty(result);
            }

            [Fact]
            public void ReturnsOnlyReviewsAssignedToUser()
            {
                // Arrange
                var reviews = CreateService();
                var review1 = new Review() { UserId = 42 };
                var review2 = new Review() { UserId = 24 };
                reviews.MockData.Reviews.Add(review1);
                reviews.MockData.Reviews.Add(review2);
                reviews.MockData.SaveChanges();

                // Act
                var result = reviews.GetReviewsAssignedTo(42);

                // Assert
                Assert.Same(review2, result.Single());
            }
        }

        public class GetReview
        {
            [Fact]
            public void RequiresValidArguments()
            {
                ContractAssert.OutOfRange(() => CreateService().GetReview(-1), "id");
            }

            [Fact]
            public void ReturnsNullIfNoReviewWithId()
            {
                // Arrange
                var reviews = CreateService();
                var review = new Review();
                reviews.MockData.Reviews.Add(review);
                reviews.MockData.SaveChanges();

                // Act/Assert
                Assert.Null(reviews.GetReview(review.Id + 42));
            }

            [Fact]
            public void ReturnsIterationMatchingId()
            {
                // Arrange
                var reviews = CreateService();
                var review = new Review();
                reviews.MockData.Reviews.Add(review);
                reviews.MockData.SaveChanges();

                // Act/Assert
                Assert.Same(review, reviews.GetReview(review.Id));
            }
        }

        private static TestableReviewService CreateService()
        {
            return new TestableReviewService();
        }

        private class TestableReviewService : ReviewService
        {
            public MockDataRepository MockData { get; set; }

            public TestableReviewService()
            {
                Data = MockData = new MockDataRepository();
            }
        }
    }
}
