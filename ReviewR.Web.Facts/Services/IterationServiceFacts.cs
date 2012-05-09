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
    public class IterationServiceFacts
    {
        public class Ctor
        {
            [Fact]
            public void InitializesValues()
            {
                // Arrange
                var data = new MockDataRepository();
                var diff = new Mock<DiffService>().Object;

                // Act
                IterationService auth = new IterationService(data, diff);

                // Assert
                Assert.Same(data, auth.Data);
                Assert.Same(diff, auth.Diff);
            }

            [Fact]
            public void RequiresNonNullArguments()
            {
                ContractAssert.NotNull(() => new IterationService(null, new Mock<DiffService>().Object), "data");
                ContractAssert.NotNull(() => new IterationService(new MockDataRepository(), null), "diff");
            }
        }

        public class AddIteration
        {
            [Fact]
            public void RequiresValidArguments()
            {
                ContractAssert.OutOfRange(() => CreateService().AddIteration(-1, 0), "reviewId");
                ContractAssert.OutOfRange(() => CreateService().AddIteration(0, -1), "currentUserId");
            }

            [Fact]
            public void ReturnsNotFoundIfNoReviewWithSpecifiedId()
            {
                // Arrange
                var iters = CreateService();
                var review = new Review() { Name = "NotForYou", UserId = 42 };
                iters.MockData.Reviews.Add(review);
                iters.MockData.SaveChanges();

                // Act
                Assert.Equal(DatabaseActionOutcome.ObjectNotFound, iters.AddIteration(review.Id + 42, 42).Outcome);
            }

            [Fact]
            public void ReturnsForbiddenIfSpecifiedUserIsNotReviewAuthor()
            {
                // Arrange
                var iters = CreateService();
                var review = new Review() { Name = "NotForYou", UserId = 42 };
                iters.MockData.Reviews.Add(review);
                iters.MockData.SaveChanges();

                // Act
                Assert.Equal(DatabaseActionOutcome.Forbidden, iters.AddIteration(review.Id, 24).Outcome);
            }

            [Fact]
            public void ReturnsCreatedIterationIfSuccessful()
            {
                // Arrange
                DateTime start = DateTime.UtcNow;
                var iters = CreateService();
                var review = new Review() { Name = "NotForYou", UserId = 42, Iterations = new List<Iteration>() };
                iters.MockData.Reviews.Add(review);
                iters.MockData.SaveChanges();

                // Act
                var result = iters.AddIteration(review.Id, 42);

                // Assert
                Assert.Equal(DatabaseActionOutcome.Success, result.Outcome);
                Assert.Equal(new Iteration()
                {
                    Id = result.Object.Id,
                    Published = false,
                    ReviewId = review.Id,
                    StartedOn = result.Object.StartedOn
                }, result.Object, new PropertyEqualityComparer());
                Assert.True(result.Object.StartedOn >= start && result.Object.StartedOn <= DateTime.UtcNow);
            }
        }

        public class DeleteIteration
        {
            [Fact]
            public void RequiresValidArguments()
            {
                ContractAssert.OutOfRange(() => CreateService().DeleteIteration(-1, 0), "iterationId");
                ContractAssert.OutOfRange(() => CreateService().DeleteIteration(0, -1), "currentUserId");
            }

            [Fact]
            public void ReturnsNotFoundIfNoIterationWithSpecifiedId()
            {
                // Arrange
                var iters = CreateService();
                var iter = new Iteration();
                iters.MockData.Iterations.Add(iter);
                iters.MockData.SaveChanges();

                // Act
                Assert.Equal(DatabaseActionOutcome.ObjectNotFound, iters.DeleteIteration(iter.Id + 42, 42));
            }

            [Fact]
            public void ReturnsForbiddenIfSpecifiedUserIsNotReviewAuthor()
            {
                // Arrange
                var iters = CreateService();
                var iter = new Iteration() { Review = new Review() { UserId = 42 } };
                iters.MockData.Iterations.Add(iter);
                iters.MockData.SaveChanges();

                // Act
                Assert.Equal(DatabaseActionOutcome.Forbidden, iters.DeleteIteration(iter.Id, 24));
            }

            [Fact]
            public void ReturnsSuccessAndRemovesIterationIfUserCorrectAndIterationExists()
            {
                // Arrange
                var iters = CreateService();
                var iter1 = new Iteration() { Review = new Review() { UserId = 42 } };
                var iter2 = new Iteration() { Review = new Review() { UserId = 42 } };
                iters.MockData.Iterations.Add(iter1);
                iters.MockData.Iterations.Add(iter2);
                iters.MockData.SaveChanges();

                // Act
                var result = iters.DeleteIteration(iter1.Id, 42);

                // Assert
                Assert.Equal(DatabaseActionOutcome.Success, result);
                Assert.DoesNotContain(iter1, iters.MockData.Iterations);
                Assert.Contains(iter2, iters.MockData.Iterations);
            }
        }

        public class AddDiffToIteration
        {
            [Fact]
            public void RequiresValidArguments()
            {
                ContractAssert.OutOfRange(() => CreateService().AddDiffToIteration(-1, "diff", 0), "id");
                ContractAssert.NotNullOrEmpty(s => CreateService().AddDiffToIteration(0, s, 0), "diff");
                ContractAssert.OutOfRange(() => CreateService().AddDiffToIteration(0, "diff", -1), "currentUserId");
            }

            [Fact]
            public void ReturnsNotFoundIfNoIterationWithSpecifiedId()
            {
                // Arrange
                var iters = CreateService();
                var iter = new Iteration();
                iters.MockData.Iterations.Add(iter);
                iters.MockData.SaveChanges();

                // Act
                Assert.Equal(DatabaseActionOutcome.ObjectNotFound, iters.AddDiffToIteration(iter.Id + 42, "diff", 42));
            }

            [Fact]
            public void ReturnsForbiddenIfSpecifiedUserIsNotReviewAuthor()
            {
                // Arrange
                var iters = CreateService();
                var iter = new Iteration() { Review = new Review() { UserId = 42 } };
                iters.MockData.Iterations.Add(iter);
                iters.MockData.SaveChanges();

                // Act
                Assert.Equal(DatabaseActionOutcome.Forbidden, iters.AddDiffToIteration(iter.Id, "diff", 24));
            }

            [Fact]
            public void AddsFilesInDiffToIterationIfSuccessful()
            {
                // Arrange
                var iters = CreateService();
                var iter = new Iteration() { Review = new Review() { UserId = 42 }, Files = new List<FileChange>() };
                iters.MockData.Iterations.Add(iter);
                iters.MockData.SaveChanges();

                var expectedChanges = new List<FileChange>() { new FileAddition() };
                iters.MockDiff.Setup(d => d.CreateFromGitDiff(It.Is<TextReader>(tr => tr.ReadToEnd() == "thediff")))
                              .Returns(expectedChanges)
                              .Verifiable();

                // Act
                var result = iters.AddDiffToIteration(iter.Id, "thediff", 42);

                // Assert
                Assert.Equal(DatabaseActionOutcome.Success, result);
                Assert.Equal(1, iter.Files.Count);
                Assert.Same(iter.Files.Single(), expectedChanges.Single());
                iters.MockDiff.Verify();
            }
        }

        public class GetIteration
        {
            [Fact]
            public void RequiresValidArguments()
            {
                ContractAssert.OutOfRange(() => CreateService().GetIteration(-1), "iterationId");
            }

            [Fact]
            public void ReturnsNullIfNoIterationWithId()
            {
                // Arrange
                var iters = CreateService();
                var iter = new Iteration();
                iters.MockData.Iterations.Add(iter);
                iters.MockData.SaveChanges();

                // Act/Assert
                Assert.Null(iters.GetIteration(iter.Id + 42));
            }

            [Fact]
            public void ReturnsIterationMatchingId()
            {
                // Arrange
                var iters = CreateService();
                var iter = new Iteration();
                iters.MockData.Iterations.Add(iter);
                iters.MockData.SaveChanges();

                // Act/Assert
                Assert.Same(iter, iters.GetIteration(iter.Id));
            }
        }

        public class SetIterationPublished
        {
            [Fact]
            public void RequiresValidArguments()
            {
                ContractAssert.OutOfRange(() => CreateService().SetIterationPublished(-1, true, 0), "id");
                ContractAssert.OutOfRange(() => CreateService().SetIterationPublished(0, true, -1), "userId");
            }

            [Fact]
            public void ReturnsNotFoundIfNoIterationWithSpecifiedId()
            {
                // Arrange
                var iters = CreateService();
                var iter = new Iteration();
                iters.MockData.Iterations.Add(iter);
                iters.MockData.SaveChanges();

                // Act
                Assert.Equal(DatabaseActionOutcome.ObjectNotFound, iters.SetIterationPublished(iter.Id + 42, true, 42));
            }

            [Fact]
            public void ReturnsForbiddenIfSpecifiedUserIsNotReviewAuthor()
            {
                // Arrange
                var iters = CreateService();
                var iter = new Iteration() { Review = new Review() { UserId = 42 } };
                iters.MockData.Iterations.Add(iter);
                iters.MockData.SaveChanges();

                // Act
                Assert.Equal(DatabaseActionOutcome.Forbidden, iters.SetIterationPublished(iter.Id, true, 24));
            }

            [Fact]
            public void SetsIterationToSpecifiedPublishStateIfSuccessful()
            {
                // Arrange
                var iters = CreateService();
                var iter = new Iteration() { Review = new Review() { UserId = 42 } };
                iters.MockData.Iterations.Add(iter);
                iters.MockData.SaveChanges();

                // Act
                var result = iters.SetIterationPublished(iter.Id, true, 42);

                // Assert
                Assert.Equal(DatabaseActionOutcome.Success, result);
                Assert.True(iter.Published);
            }
        }

        private static TestableIterationService CreateService()
        {
            return new TestableIterationService();
        }

        private class TestableIterationService : IterationService
        {
            public MockDataRepository MockData { get; set; }
            public Mock<DiffService> MockDiff { get; set; }

            public TestableIterationService()
            {
                Data = MockData = new MockDataRepository();
                Diff = (MockDiff = new Mock<DiffService>()).Object;

            }
        }
    }
}
