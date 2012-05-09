using System;
using System.Collections.Generic;
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

namespace ReviewR.Web.Facts.Services
{
    public class CommentServiceFacts
    {
        public class Ctor
        {
            [Fact]
            public void InitializesValues()
            {
                // Arrange
                var data = new MockDataRepository();
                
                // Act
                CommentService auth = new CommentService(data);

                // Assert
                Assert.Same(data, auth.Data);
            }

            [Fact]
            public void RequiresNonNullArguments()
            {
                ContractAssert.NotNull(() => new CommentService(null), "data");
            }
        }

        public class CreateComment
        {
            [Fact]
            public void RequiresValidArguments()
            {
                ContractAssert.OutOfRange(() => CreateService().CreateComment(-1, 0, "b", 0), "changeId");
                ContractAssert.OutOfRange(() => CreateService().CreateComment(0, -1, "b", 0), "line");
                ContractAssert.NotNullOrEmpty(s => CreateService().CreateComment(0, 0, s, 0), "body");
                ContractAssert.OutOfRange(() => CreateService().CreateComment(0, 0, "b", -1), "userId");
            }

            [Fact]
            public void ReturnsNullIfChangeIdDoesNotExist()
            {
                // Arrange
                var comments = CreateService();
                var chg = new FileAddition() { FileName = "foo" };
                comments.MockData.Changes.Add(chg);
                comments.MockData.SaveChanges();

                // Act
                var comment = comments.CreateComment(chg.Id + 42, 0, "body", 0);

                // Assert
                Assert.Null(comment);
            }

            [Fact]
            public void AddsCommentIfChangeExists()
            {
                // Arrange
                DateTime start = DateTime.UtcNow;
                var comments = CreateService();
                var chg = new FileAddition() { FileName = "foo" };
                chg.Comments = new List<Comment>();
                comments.MockData.Changes.Add(chg);
                comments.MockData.SaveChanges();

                // Act
                var returned = comments.CreateComment(chg.Id, 42, "body", 24);

                // Assert
                Comment added = comments.MockData.Changes.Single().Comments.Single();
                Assert.Equal(added, returned, new PropertyEqualityComparer());
                Assert.Equal(added, new Comment()
                {
                    Content = "body",
                    DiffLineIndex = 42,
                    UserId = 24,
                    PostedOn = added.PostedOn // Don't care about exact PostedOn, so force it to pass this equality check.
                }, new PropertyEqualityComparer());

                // Do a sanity check on the posted on to make sure it's not MinValue or something like that.
                Assert.True(added.PostedOn <= DateTime.UtcNow && added.PostedOn >= start);
            }
        }

        public class DeleteComment
        {
            [Fact]
            public void RequiresValidArguments()
            {
                ContractAssert.OutOfRange(() => CreateService().DeleteComment(-1, 0), "id");
                ContractAssert.OutOfRange(() => CreateService().DeleteComment(0, -1), "userId");
            }

            [Fact]
            public void ReturnsObjectNotFoundIfCommentDoesNotExist()
            {
                // Arrange
                var comments = CreateService();
                var comment = new Comment() { Content = "Blarg" };
                comments.MockData.Comments.Add(comment);
                comments.MockData.SaveChanges();

                // Act
                var result = comments.DeleteComment(comment.Id + 42, 0);

                // Assert
                Assert.Equal(result, DatabaseActionOutcome.ObjectNotFound);
            }

            [Fact]
            public void ReturnsForbiddenIfProvidedUserIsNotAuthor()
            {
                // Arrange
                var comments = CreateService();
                var comment = new Comment() { Content = "Blarg", UserId = 42 };
                comments.MockData.Comments.Add(comment);
                comments.MockData.SaveChanges();

                // Act
                var result = comments.DeleteComment(comment.Id, 24);

                // Assert
                Assert.Equal(result, DatabaseActionOutcome.Forbidden);
            }

            [Fact]
            public void ReturnsSuccessAndRemovesCommentIfProvidedUserIsNotAuthor()
            {
                // Arrange
                var comments = CreateService();
                var comment1 = new Comment() { Content = "Blarg", UserId = 42 };
                var comment2 = new Comment() { Content = "Glarg", UserId = 42 };
                comments.MockData.Comments.Add(comment1);
                comments.MockData.Comments.Add(comment2);
                comments.MockData.SaveChanges();

                // Act
                var result = comments.DeleteComment(comment1.Id, 42);

                // Assert
                Assert.Equal(result, DatabaseActionOutcome.Success);
                Assert.DoesNotContain(comment1, comments.Data.Comments);
                Assert.Contains(comment2, comments.Data.Comments);
            }
        }

        private static TestableCommentService CreateService()
        {
            return new TestableCommentService();
        }

        private class TestableCommentService : CommentService
        {
            public MockDataRepository MockData { get; set; }

            public TestableCommentService()
            {
                Data = MockData = new MockDataRepository();
            }
        }
    }
}
