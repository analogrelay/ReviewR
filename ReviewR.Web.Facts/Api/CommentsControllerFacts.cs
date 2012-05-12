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
    public class CommentsControllerFacts
    {
        public class Ctor
        {
            [Fact]
            public void RequiresNonNullArguments()
            {
                ContractAssert.NotNull(() => new CommentsController(null), "comments");
            }

            [Fact]
            public void InitializesServices()
            {
                // Arrange
                var comments = new Mock<CommentService>().Object;

                // Act
                var c = new CommentsController(comments);

                // Assert
                Assert.Same(c.Comments, comments);
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
            public void Returns404IfNoCommentWithId()
            {
                // Arrange
                var c = CreateController();
                var cmt = new Comment() { Content = "blarg" };
                c.MockData.Comments.Add(cmt);
                c.MockData.SaveChanges();

                // Act
                var result = c.Delete(cmt.Id + 42);

                // Assert
                Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
                Assert.Equal("blarg", c.MockData.Comments.Single().Content);
            }

            [Fact]
            public void Returns403IfCurrentUserNotCommentAuthor()
            {
                // Arrange
                var c = CreateController();
                var cmt = new Comment() { Content = "blarg", UserId = ApiTestData.NotLoggedInUser.Id };
                c.MockData.Comments.Add(cmt);
                c.MockData.SaveChanges();

                // Act
                var result = c.Delete(cmt.Id);

                // Assert
                Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
                Assert.Equal("blarg", c.MockData.Comments.Single().Content);
            }

            [Fact]
            public void Returns204AndDeletesCommentIfCurrentUserIsCommentAuthor()
            {
                // Arrange
                var c = CreateController();
                var cmt = new Comment() { Content = "blarg", UserId = ApiTestData.LoggedInUser.Id };
                c.MockData.Comments.Add(cmt);
                c.MockData.SaveChanges();

                // Act
                var result = c.Delete(cmt.Id);

                // Assert
                Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
                Assert.False(c.MockData.Comments.Any());
            }
        }

        public class Post
        {
            [Fact]
            public void RequiresValidArguments()
            {
                ContractAssert.OutOfRange(() => CreateController().Post(-1, 0, "body"), "changeId");
                ContractAssert.OutOfRange(() => CreateController().Post(0, -1, "body"), "line");
                ContractAssert.NotNullOrEmpty(s => CreateController().Post(0, 0, s), "body");
            }

            [Fact]
            public void Returns404IfNoChangeWithId()
            {
                // Arrange
                var c = CreateController();
                var chg = new FileAddition() { FileName = "blorg", Comments = new List<Comment>() };
                c.MockData.Changes.Add(chg);
                c.MockData.SaveChanges();

                // Act
                var result = c.Post(chg.Id + 42, 0, "blarg");

                // Assert
                Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
                Assert.False(chg.Comments.Any());
            }

            [Fact]
            public void Returns201AddsCommentAndReturnsCommentIfChangeExists()
            {
                // Arrange
                var c = CreateController();
                var chg = new FileAddition() { FileName = "blorg", Comments = new List<Comment>() };
                c.MockData.Changes.Add(chg);
                c.MockData.SaveChanges();

                // Act
                var result = c.Post(chg.Id, 0, "blarg");

                // Assert
                Assert.Equal(HttpStatusCode.Created, result.StatusCode);
                Assert.Equal(chg.Comments.Single().Content, "blarg");
                Assert.Equal(Assert.IsType<CommentModel>(result.GetObjectContent()).Body, "blarg");
            }
        }

        private static TestableCommentsController CreateController()
        {
            return CreateController(loggedIn: true);
        }

        private static TestableCommentsController CreateController(bool loggedIn)
        {
            var c = new TestableCommentsController();
            if (loggedIn)
            {
                c.User = ApiTestData.LoggedIn;
            }
            return c;
        }

        private class TestableCommentsController : CommentsController
        {
            public MockDataRepository MockData { get; set; }
            
            public TestableCommentsController()
            {
                MockData = new MockDataRepository();

                Comments = new CommentService(MockData);    
            }
        }
    }
}
