using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using ReviewR.Web.Controllers;
using VibrantUtils;
using System.Net;
using ReviewR.Web.Models;
using ReviewR.Web.Services;
using ReviewR.Web.ViewModels;
using Moq;

namespace ReviewR.Web.Facts.Controllers
{
    public class CommentsControllerFacts
    {
        public class NewGet
        {
            [Fact]
            public void Returns404IfChangeDoesNotExist()
            {
                // Arrange
                var ctl = CreateController();

                // Act
                var result = ctl.New(42, line: 1);

                // Assert
                ActionAssert.IsHttpStatusResult(result, HttpStatusCode.NotFound);
            }

            [Fact]
            public void Returns404IfLineNegative()
            {
                // Arrange
                var ctl = CreateController();
                FileChange chg = ctl.Reviews.Data.Changes.Add(new FileAddition()
                {
                    Id = 42
                });
                ctl.Reviews.Data.SaveChanges();

                // Act
                var result = ctl.New(42, line: -12);

                // Assert
                ActionAssert.IsHttpStatusResult(result, HttpStatusCode.NotFound);
            }

            [Fact]
            public void Returns404IfLineNotSpecified()
            {
                // Arrange
                var ctl = CreateController();
                FileChange chg = ctl.Reviews.Data.Changes.Add(new FileAddition()
                {
                    Id = 42
                });
                ctl.Reviews.Data.SaveChanges();

                // Act
                var result = ctl.New(42, line: null);

                // Assert
                ActionAssert.IsHttpStatusResult(result, HttpStatusCode.NotFound);
            }

            [Fact]
            public void Returns404IfLineTooHigh()
            {
                // Arrange
                var ctl = CreateController();
                FileChange chg = ctl.Reviews.Data.Changes.Add(new FileAddition()
                {
                    Diff = "@@ -1,1 +1,1 @@"
                });
                ctl.Reviews.Data.SaveChanges();

                // Act
                var result = ctl.New(chg.Id, line: 2);

                // Assert
                ActionAssert.IsHttpStatusResult(result, HttpStatusCode.NotFound);
            }

            [Fact]
            public void ReturnsViewWithPreparedNewCommentViewModel()
            {
                // Arrange
                var ctl = CreateController();
                FileChange chg = ctl.Reviews.Data.Changes.Add(new FileAddition()
                {
                    Diff = "@@ -1,1 +1,1 @@"
                });
                ctl.Reviews.Data.SaveChanges();

                // Act
                var result = ctl.New(chg.Id, line: 0);

                // Assert
                ActionAssert.IsViewResult(result, new NewCommentViewModel());
            }
        }

        public class NewPost
        {
            [Fact]
            public void Returns404IfChangeDoesNotExist()
            {
                // Arrange
                var ctl = CreateController();

                // Act
                var result = ctl.New(42, 1, new NewCommentViewModel());

                // Assert
                ActionAssert.IsHttpStatusResult(result, HttpStatusCode.NotFound);
            }

            [Fact]
            public void Returns404IfLineNegative()
            {
                // Arrange
                var ctl = CreateController();
                FileChange chg = ctl.Reviews.Data.Changes.Add(new FileAddition()
                {
                    Id = 42
                });
                ctl.Reviews.Data.SaveChanges();

                // Act
                var result = ctl.New(42, -12, new NewCommentViewModel());

                // Assert
                ActionAssert.IsHttpStatusResult(result, HttpStatusCode.NotFound);
            }

            [Fact]
            public void Returns404IfLineNotSpecified()
            {
                // Arrange
                var ctl = CreateController();
                FileChange chg = ctl.Reviews.Data.Changes.Add(new FileAddition()
                {
                    Id = 42
                });
                ctl.Reviews.Data.SaveChanges();

                // Act
                var result = ctl.New(42, null, new NewCommentViewModel());

                // Assert
                ActionAssert.IsHttpStatusResult(result, HttpStatusCode.NotFound);
            }

            [Fact]
            public void Returns404IfLineTooHigh()
            {
                // Arrange
                var ctl = CreateController();
                FileChange chg = ctl.Reviews.Data.Changes.Add(new FileAddition()
                {
                    Diff = "@@ -1,1 +1,1 @@"
                });
                ctl.Reviews.Data.SaveChanges();

                // Act
                var result = ctl.New(chg.Id, 2, new NewCommentViewModel());

                // Assert
                ActionAssert.IsHttpStatusResult(result, HttpStatusCode.NotFound);
            }

            [Fact]
            public void ReturnsViewIfModelNotValid()
            {
                // Arrange
                var ctl = CreateController();
                FileChange chg = ctl.Reviews.Data.Changes.Add(new FileAddition()
                {
                    Diff = "@@ -1,1 +1,1 @@"
                });
                ctl.Reviews.Data.SaveChanges();
                ctl.ModelState.AddModelError("", "Test");

                // Act
                var result = ctl.New(chg.Id, 0, new NewCommentViewModel() { Body = "Foo" });

                // Assert
                ActionAssert.IsViewResult(result, new NewCommentViewModel() { Body = "Foo" });
            }

            [Fact]
            public void RedirectsToChangeViewAfterAddingComment()
            {
                // Arrange
                var ctl = CreateController();
                FileChange chg = ctl.Reviews.Data.Changes.Add(new FileAddition()
                {
                    Diff = "@@ -1,1 +1,1 @@"
                });
                ctl.Reviews.Data.SaveChanges();
                ctl.MockAuth.Setup(a => a.GetCurrentUserId()).Returns(42);

                // Act
                var result = ctl.New(chg.Id, 0, new NewCommentViewModel() { Body = "Foo" });

                // Assert
                ActionAssert.IsRedirectResult(result, new { controller = "Changes", action = "View", id = chg.Id });
                Assert.Contains(
                    new { Id = ctl.TestData.LastId, Content = "Foo", UserId = 42, FileId = chg.Id, DiffLineIndex = 0 },
                    ctl.Reviews.Data.Comments.Select(c => new { c.Id, c.Content, c.UserId, c.FileId, c.DiffLineIndex }),
                    new PropertyEqualityComparer(typeEquality: false));
            }
        }

        private static TestableCommentsController CreateController()
        {
            return new TestableCommentsController(new TestDataRepository(), new Mock<AuthenticationService>());
        }

        public class TestableCommentsController : CommentsController
        {
            public TestDataRepository TestData { get; set; }
            public Mock<AuthenticationService> MockAuth { get; set; }

            public TestableCommentsController(TestDataRepository data, Mock<AuthenticationService> mockAuth)
                : base(new ReviewService(data), mockAuth.Object)
            {
                TestData = data;
                MockAuth = mockAuth;
            }
        }
    }
}
