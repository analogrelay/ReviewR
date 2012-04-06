using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using ReviewR.Web.Api;
using ReviewR.Web.Models;
using ReviewR.Web.Models.Request;
using ReviewR.Web.Services;
using Xunit;

namespace ReviewR.Web.Facts.Controllers
{
    public class UsersControllerFacts
    {
        public class Post
        {
            [Fact]
            public void WithInvalidModelReturnsBadRequest()
            {
                // Arrange
                var c = CreateController();
                c.ModelState.AddModelError("", "Test");
                var model = new CreateUserRequestModel();

                // Act
                var result = c.Post(model);

                // Assert
                Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            }

            [Fact]
            public void WithDuplicateEmailReturnsConflict()
            {
                // Arrange
                var model = new CreateUserRequestModel() { Email = "real@user.com", DisplayName = "Real User", Password = "password" };
                var c = CreateController();
                c.Auth.CreateUser("real@user.com", "Real User", "123456");

                // Act
                var result = c.Post(model);

                // Assert
                Assert.Equal(HttpStatusCode.Conflict, result.StatusCode);
            }

            [Fact]
            public void WithValidModelAndUniqueEmailReturnsCreatedResponseWithNewUserIdentity()
            {
                // Arrange
                var model = new CreateUserRequestModel() { Email = "real@user.com", DisplayName = "Real User", Password = "password" };
                var c = CreateController();
                c.Auth.CreateUser("other@user.com", "Other User", "password");

                // Act
                var result = c.Post(model);

                // Assert
                Assert.Equal(HttpStatusCode.Created, result.StatusCode);
                Assert.Equal(new ReviewRIdentity()
                {
                    DisplayName = "Real User",
                    Email = "real@user.com",
                    RememberMe = false,
                    Roles = new HashSet<string>(),
                    UserId = c.Auth.Data.GetLastId()
                }, result.GetObjectContent());
            }
        }

        private static UsersController CreateController()
        {
            var c = new UsersController(
                new AuthenticationService(
                    new MockDataRepository(),
                    new MockHashService()))
                    {
                        Request = new HttpRequestMessage()
                    };
            return c;
        }

    }
}
