using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using Moq;
using ReviewR.Diff;
using ReviewR.Web.Controllers;
using ReviewR.Web.Models;
using ReviewR.Web.Services;
using ReviewR.Web.ViewModels;
using VibrantUtils;
using Xunit;

namespace ReviewR.Web.Facts.Controllers
{
    public class ChangesControllerFacts
    {
        public class ViewGet
        {
            [Fact]
            public void Returns404IfNoChangeWithId()
            {
                // Arrange
                var ctl = CreateController();

                // Assume
                Assert.DoesNotContain(42, ctl.Reviews.Data.Changes.Select(r => r.Id));

                // Act
                var result = ctl.View(42);

                // Assert
                ActionAssert.IsHttpStatusResult(result, HttpStatusCode.NotFound);
            }

            [Fact]
            public void Returns404IfNoReviewAttachedToChange()
            {
                // Arrange
                var ctl = CreateController();
                ctl.Reviews.Data.Changes.Add(new FileModification()
                {
                    Id = 42
                });

                // Act
                var result = ctl.View(42);

                // Assert
                ActionAssert.IsHttpStatusResult(result, HttpStatusCode.NotFound);
            }

            [Fact]
            public void ReturnsViewResultWithViewModelIfFileExists()
            {
                // Arrange
                var ctl = CreateController();
                Review r = ctl.Reviews.Data.Reviews.Add(new Review()
                {
                    Name = "Test"
                });
                ctl.Reviews.Data.SaveChanges();
                FileChange c = ctl.Reviews.Data.Changes.Add(new FileModification()
                {
                    FileName = "/Foo/Bar.txt",
                    Diff = @"@@ -1,2 +3,4 @@
+Foo",
                    ReviewId = r.Id,
                    Review = r,
                    Comments = new List<Comment>()
                });
                ctl.Reviews.Data.SaveChanges();
                r.Files = new List<FileChange>() { c };

                // Act
                var result = ctl.View(c.Id);

                // Assert
                var file = new FileChangeViewModel() { ChangeType = FileChangeType.Modified, FileName = "Bar.txt", Id = c.Id };
                ActionAssert.IsViewResult(result, new ChangeDetailViewModel()
                {
                    Review = new ReviewDetailViewModel()
                    {
                        Id = r.Id,
                        Name = "Test",
                        Folders = new List<FolderChangeViewModel>()
                        {
                            new FolderChangeViewModel() { Name = "/Foo", Files = new List<FileChangeViewModel>() {
                                file
                            } }
                        },
                        Selected = file
                    },
                    Diff = new DiffFileViewModel()
                    {
                        Id = c.Id,
                        Insertions = 1,
                        Deletions = 0,
                        FileName = "/Foo/Bar.txt",
                        DiffLines = new List<DiffLineViewModel>() {
                            new DiffLineViewModel() {
                                Index = 0,
                                Text = "@@ -1,2 +3,4 @@",
                                Type = LineDiffType.HunkHeader,
                                Comments = new List<LineCommentViewModel>()
                            },
                            new DiffLineViewModel() {
                                Index = 1,
                                RightLine = 3,
                                Text = "Foo",
                                Type = LineDiffType.Added,
                                Comments = new List<LineCommentViewModel>()
                            }
                        }
                    }
                });
            }

            [Fact]
            public void ReturnsViewResultWithCommentsAttachedIfTheyExist()
            {
                // Ugh... this is the biggest test and really needs simplification, but I'm lazy ;P

                // Arrange
                var ctl = CreateController();
                Review r = ctl.Reviews.Data.Reviews.Add(new Review()
                {
                    Name = "Test"
                });
                ctl.Reviews.Data.SaveChanges();
                FileChange c = ctl.Reviews.Data.Changes.Add(new FileModification()
                {
                    FileName = "/Foo/Bar.txt",
                    Diff = @"@@ -1,2 +3,4 @@
+Foo",
                    ReviewId = r.Id,
                    Review = r,
                    Comments = new List<Comment>()
                });
                ctl.Reviews.Data.SaveChanges();
                Comment cmt1 = ctl.Reviews.Data.Comments.Add(new Comment()
                {
                    DiffLineIndex = 0,
                    File = c,
                    FileId = c.Id,
                    Content = "Comment#1",
                    PostedOn = new DateTime(1867, 07, 01),
                    User = new User() { DisplayName = "Test", Email = "test" }
                });
                Comment cmt2 = ctl.Reviews.Data.Comments.Add(new Comment()
                {
                    DiffLineIndex = 1,
                    File = c,
                    FileId = c.Id,
                    Content = "Comment#2",
                    PostedOn = new DateTime(1867, 07, 01, 01, 00, 00),
                    User = new User() { DisplayName = "Test", Email = "test" }
                });
                Comment cmt3 = ctl.Reviews.Data.Comments.Add(new Comment()
                {
                    DiffLineIndex = 1,
                    File = c,
                    FileId = c.Id,
                    Content = "Comment#3",
                    PostedOn = new DateTime(1867, 07, 01, 02, 00, 00),
                    User = new User() { DisplayName = "Test", Email = "test" }
                });
                ctl.Reviews.Data.SaveChanges();

                r.Files = new List<FileChange>() { c };
                c.Comments.Add(cmt1);
                c.Comments.Add(cmt2);
                c.Comments.Add(cmt3);

                // Act
                var result = ctl.View(c.Id);

                // Assert
                ViewResult viewResult = Assert.IsType<ViewResult>(result);
                ChangeDetailViewModel model = Assert.IsType<ChangeDetailViewModel>(viewResult.Model);
                Assert.Contains(
                    new LineCommentViewModel() { Id = cmt1.Id, AuthorEmail = "test", AuthorName = "test", Body = "Comment#1", PostedOn = cmt1.PostedOn },
                    model.Diff.DiffLines[0].Comments,
                    new PropertyEqualityComparer());
                Assert.Contains(
                    new LineCommentViewModel() { Id = cmt2.Id, AuthorEmail = "test", AuthorName = "test", Body = "Comment#2", PostedOn = cmt2.PostedOn },
                    model.Diff.DiffLines[1].Comments,
                    new PropertyEqualityComparer());
                Assert.Contains(    
                    new LineCommentViewModel() { Id = cmt3.Id, AuthorEmail = "test", AuthorName = "test", Body = "Comment#3", PostedOn = cmt3.PostedOn },
                    model.Diff.DiffLines[1].Comments,
                    new PropertyEqualityComparer());
                    
            }
        }

        private static ChangesController CreateController()
        {
            TestDataRepository data = new TestDataRepository();
            return new TestableChangesController(
                new ReviewService(data),
                new Mock<AuthenticationService>(),
                new DiffService(new DiffReader(), new DiffConverter()));
        }

        private class TestableChangesController : ChangesController
        {
            public Mock<AuthenticationService> MockAuth { get; set; }

            public TestableChangesController(ReviewService reviews, Mock<AuthenticationService> auth, DiffService diff)
                : base(reviews, auth.Object, diff)
            {
                MockAuth = auth;
            }
        }
    }
}
