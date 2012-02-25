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
                ActionAssert.IsRedirectResult(result, new { controller = "Review", action = "View", id = ctl.TestData.LastId });
                Assert.Contains(new Review() {
                    Id = ctl.TestData.LastId,
                    UserId = 42,
                    Name = "123",
                    Files = changes.ToList()
                }, ctl.Data.Reviews, new PropertyEqualityComparer());
            }
        }

        private static TestableReviewController CreateController()
        {
            return new TestableReviewController(
                new Mock<DiffService>(),
                new Mock<AuthenticationService>(),
                new TestDataRepository());
        }

        private class TestableReviewController : ReviewController
        {
            public Mock<DiffService> MockDiff { get; set; }
            public Mock<AuthenticationService> MockAuth { get; set; }
            public TestDataRepository TestData { get; set; }

            public TestableReviewController(Mock<DiffService> mockDiff, Mock<AuthenticationService> mockAuth, TestDataRepository data)
                : base(mockDiff.Object, mockAuth.Object, data)
            {
                MockDiff = mockDiff;
                MockAuth = mockAuth;
                TestData = data;
            }
        }
    }
}
