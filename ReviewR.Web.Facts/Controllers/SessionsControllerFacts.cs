using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using ReviewR.Web.Api;
using ReviewR.Web.Models;
using ReviewR.Web.Models.Data;
using ReviewR.Web.Models.Request;
using ReviewR.Web.Services;
using Xunit;

namespace ReviewR.Web.Facts.Controllers
{
    public class SessionsControllerFacts
    {
        public class Delete
        {
            [Fact]
            public void SetsCurrentUserToNull()
            {
                // Arrange
                var c = CreateController();

                // Act
                c.Delete();

                // Assert
                Assert.Null(c.User);
            }

            [Fact]
            public void ReturnsOK()
            {
                // Arrange
                var c = CreateController();

                // Act/Assert
                Assert.Equal(HttpStatusCode.OK, c.Delete().StatusCode);
            }
        }

        public class Post
        {
            [Fact]
            public void WithInvalidModelReturnsBadRequest()
            {
                // Arrange
                var c = CreateController();
                c.ModelState.AddModelError("", "Test");
                var model = new CreateSessionRequestModel();

                // Act
                var result = c.Post(model);

                // Assert
                Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            }

            [Fact]
            public void WithValidModelAndUserInfoSetsAuthToken()
            {
                // Arrange
                var model = new CreateSessionRequestModel() { Email = "real@user.com", Password = "password" };
                var c = CreateController();
                c.Auth.CreateUser("real@user.com", "Real User", "password");
                var user = c.Auth.Data.Users.Where(u => u.Email == "real@user.com").Single();
                user.Roles = new List<Role>() {
                    new Role() { RoleName = "Role1" },
                    new Role() { RoleName = "Role2" },
                    new Role() { RoleName = "Role3" }
                };

                // Act
                var result = c.Post(model);

                // Assert
                var expectedId = new ReviewRIdentity()
                {
                    Email = "real@user.com",
                    Id = user.Id,
                    DisplayName = "Real User",
                    Roles = new HashSet<string>(new [] { "Role1", "Role2", "Role3" }),
                    RememberMe = false
                };
                Assert.Equal(HttpStatusCode.Created, result.StatusCode);
                Assert.Equal(expectedId, c.User.Identity);
                Assert.Equal(expectedId, result.GetObjectContent());
            }

            [Fact]
            public void WithValidModelAndUserInfoSetsPersistentAuthTokenIfRememberMeSet()
            {
                // Arrange
                var model = new CreateSessionRequestModel() { Email = "real@user.com", Password = "password", RememberMe = true };
                var c = CreateController();
                c.Auth.CreateUser("real@user.com", "Real User", "password");

                // Act
                var result = c.Post(model);

                // Assert
                Assert.Equal(HttpStatusCode.Created, result.StatusCode);
                Assert.True(c.User.Identity.RememberMe);
                Assert.True(((ReviewRIdentity)result.GetObjectContent()).RememberMe);
            }

            [Fact]
            public void WithInvalidUserNameReturnsForbidden()
            {
                // Arrange
                var model = new CreateSessionRequestModel() { Email = "real@user.com", Password = "password" };
                var c = CreateController();

                // Act
                var result = c.Post(model);

                // Assert
                Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
            }

            [Fact]
            public void WithInvalidPasswordReturnsForbidden()
            {
                // Arrange
                var model = new CreateSessionRequestModel() { Email = "real@user.com", Password = "password" };
                var c = CreateController();
                c.Auth.CreateUser("real@user.com", "Real User", "123456");

                // Act
                var result = c.Post(model);

                // Assert
                Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
            }
        }

        private static SessionsController CreateController()
        {
            return new SessionsController(
                new AuthenticationService(
                    new MockDataRepository(),
                    new MockHashService()))
            {
                Request = new HttpRequestMessage()
            };
        }
    }
}
