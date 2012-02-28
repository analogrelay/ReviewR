using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviewR.Web.ViewModels;
using VibrantUtils;
using ReviewR.Web.Controllers;
using Xunit;
using ReviewR.Web.Services;
using Moq;
using System.Web;
using System.IO;
using ReviewR.Web.Models;
using ReviewR.Web.Facts.Authentication;
using System.Net;

namespace ReviewR.Web.Facts.Controllers
{
    public class ReviewControllerFacts
    {
        public class NewGet
        {
            [Fact]
            public void ReturnsView()
            {
                // Arrange
                var ctl = CreateController();

                // Act
                var result = ctl.New();

                // Assert
                ActionAssert.IsViewResult(result, new NewReviewViewModel());
            }
        }

        public class NewPost
        {
            [Fact]
            public void ReturnsViewIfModelInvalid()
            {
                // Arrange
                var ctl = CreateController();
                var model = new NewReviewViewModel() { Name = "123" };
                ctl.ModelState.AddModelError("", "Test");

                // Act
                var result = ctl.New(model);

                // Assert
                ActionAssert.IsViewResult(result, model);
            }

            [Fact]
            public void ReturnsErrorIfDiffServiceThrows()
            {
                // Arrange
                var ctl = CreateController();
                var model = new NewReviewViewModel() { Name = "123", Diff = new TestPostedFile("Test") };
                ctl.MockDiff
                   .Setup(s => s.CreateFromGitDiff(It.Is<StreamReader>(r => ReferenceEquals(r.BaseStream, model.Diff.InputStream))))
                   .Throws(new NotSupportedException("Borked!"));


                // Act
                var result = ctl.New(model);

                // Assert
                ActionAssert.IsViewResult(result, model);
                Assert.Contains("An error occurred reading the diff: Borked!", ctl.ModelState["Diff"].Errors.Select(e => e.ErrorMessage).ToArray());
            }

            [Fact]
            public void CreatesReviewWithCurrentUserAsOwnerFromDiff()
            {
                // Arrange
                var userId = 42;
                var ctl = CreateController();
                var model = new NewReviewViewModel() { Name = "123", Diff = new TestPostedFile("Test") };
                var changes = new FileChange[] { new FileAddition(), new FileDeletion(), new FileModification() };
                ctl.MockDiff
                   .Setup(s => s.CreateFromGitDiff(It.Is<StreamReader>(r => ReferenceEquals(r.BaseStream, model.Diff.InputStream))))
                   .Returns(changes);
                ctl.MockAuth
                   .Setup(s => s.GetCurrentUserId())
                   .Returns(userId);

                // Act
                var result = ctl.New(model);

                // Assert
                ActionAssert.IsRedirectResult(result, new { controller = "Reviews", action = "View", id = ctl.TestData.LastId });
                Assert.Contains(new Review() {
                    Id = ctl.TestData.LastId,
                    UserId = 42,
                    Name = "123",
                    Files = changes.ToList()
                }, ctl.Reviews.Data.Reviews, new PropertyEqualityComparer());
            }
        }

        public class IndexGet
        {
            [Fact]
            public void ReturnsViewResultWithEmptyDashboardViewModelIfNoReviewsForCurrentUser()
            {
                // Arrange
                var ctl = CreateController();
                User u = ctl.Reviews.Data.Users.Add(new User() { Reviews = new List<Review>() });
                ctl.Reviews.Data.SaveChanges();
                ctl.MockAuth.Setup(s => s.GetCurrentUserId()).Returns(u.Id);
                
                // Act
                var result = ctl.Index();

                // Assert
                ActionAssert.IsViewResult(result, new DashboardViewModel() { CreatedReviews = new List<ReviewSummaryViewModel>(), AssignedReviews = new List<ReviewSummaryViewModel>() });
            }

            [Fact]
            public void ReturnsViewResultWithCreatedAndAssignedReviews()
            {
                // Arrange
                var ctl = CreateController();
                Review r1 = ctl.Reviews.CreateReview("Review1", new List<FileChange>(), 42);
                Review r2 = ctl.Reviews.CreateReview("Review2", new List<FileChange>(), 42);
                Review r3 = ctl.Reviews.CreateReview("Review3", new List<FileChange>(), 12);
                Participant p = new Participant() { Review = r3, UserId = 42 };
                ctl.Reviews.Data.Participants.Add(p);
                ctl.Reviews.Data.SaveChanges();
                ctl.MockAuth.Setup(a => a.GetCurrentUserId()).Returns(42);
                
                // Act
                var result = ctl.Index();

                // Assert
                ActionAssert.IsViewResult(result,
                    new DashboardViewModel()
                    {
                        CreatedReviews = new List<ReviewSummaryViewModel>()
                        {
                            new ReviewSummaryViewModel() { Id = r1.Id, Name = r1.Name },
                            new ReviewSummaryViewModel() { Id = r2.Id, Name = r2.Name }
                        },
                        AssignedReviews = new List<ReviewSummaryViewModel>()
                        {
                            new ReviewSummaryViewModel() { Id = r3.Id, Name = r3.Name }
                        }
                    });
            }
        }

        public class ViewGet
        {
            [Fact]
            public void Returns404IfNoReviewWithId()
            {
                // Arrange
                var ctl = CreateController();

                // Assume
                Assert.DoesNotContain(42, ctl.Reviews.Data.Reviews.Select(r => r.Id));

                // Act
                var result = ctl.View(42);

                // Assert
                ActionAssert.IsHttpStatusResult(result, HttpStatusCode.NotFound);
            }

            [Fact]
            public void ProcessesReviewIntoFileStructure()
            {
                // Arrange
                var ctl = CreateController();
                Review r = ctl.Reviews.CreateReview("Foo", new List<FileChange>()
                {
                    new FileAddition() { FileName = "/Foo/Bar/Baz" },
                    new FileDeletion() { FileName = "/Foo/Bar/Biz" },
                    new FileModification() { FileName = "/Foo/Boz", NewFileName = "/Foo/Baz" },
                    new FileAddition() { FileName = "/Biz" }
                }, 42);
                User u = ctl.Reviews.Data.Users.Add(new User() { Reviews = new List<Review>() { r } });
                ctl.Reviews.Data.SaveChanges();
                ctl.MockAuth.Setup(s => s.GetCurrentUserId()).Returns(u.Id);
                r.UserId = u.Id;
                
                // Act
                var result = ctl.View(r.Id);

                // Assert
                ActionAssert.IsViewResult(result, new ReviewDetailViewModel()
                {
                    Id = r.Id,
                    Name = "Foo",
                    Folders = new List<FolderChangeViewModel>()
                    {
                        new FolderChangeViewModel() { Name = "/", Files = new List<FileChangeViewModel>() {
                            new FileChangeViewModel() { ChangeType = FileChangeType.Added, FileName = "Biz" }
                        } },
                        new FolderChangeViewModel() { Name = "/Foo", Files = new List<FileChangeViewModel>() {
                            new FileChangeViewModel() { ChangeType = FileChangeType.Modified, FileName = "Boz" }
                        } },
                        new FolderChangeViewModel() { Name = "/Foo/Bar", Files = new List<FileChangeViewModel>() {
                            new FileChangeViewModel() { ChangeType = FileChangeType.Added, FileName = "Baz" },
                            new FileChangeViewModel() { ChangeType = FileChangeType.Removed, FileName = "Biz" }
                        } }
                    }
                });
            }
        }

        private static TestableReviewController CreateController()
        {
            TestableReviewController c = new TestableReviewController(
                new Mock<DiffService>(),
                new Mock<AuthenticationService>(),
                new TestDataRepository());
            return c;
        }

        private class TestableReviewController : ReviewsController
        {
            public Mock<DiffService> MockDiff { get; set; }
            public Mock<AuthenticationService> MockAuth { get; set; }
            public TestDataRepository TestData { get; set; }

            public TestableReviewController(Mock<DiffService> mockDiff, Mock<AuthenticationService> mockAuth, TestDataRepository data)
                : base(mockDiff.Object, mockAuth.Object, new ReviewService(data))
            {
                MockDiff = mockDiff;
                MockAuth = mockAuth;
                TestData = data;
            }
        }
    }
}
