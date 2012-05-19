using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Moq;
using ReviewR.Web.Api;
using ReviewR.Web.Services;
using VibrantUtils;
using Xunit;

namespace ReviewR.Web.Facts.Api
{
    public class SessionsControllerFacts
    {
        public class Ctor
        {
            [Fact]
            public void RequiresNonNullArguments()
            {
                ContractAssert.NotNull(() => new SessionsController(null), "auth");
            }

            [Fact]
            public void InitializesServices()
            {
                // Arrange
                var auth = new Mock<AuthenticationService>().Object;

                // Act
                var c = new SessionsController(auth);

                // Assert
                Assert.Same(c.Auth, auth);
            }
        }

        public class Post
        {
            [Fact]
            public void RequiresValidArguments()
            {
                ContractAssert.NotNullOrEmpty(s => CreateController().Post(s, "token"), "id");
                ContractAssert.NotNullOrEmpty(s => CreateController().Post("id", s), "token");
            }

            [Fact]
            public Task ReturnsBadRequestOnMissingFields()
            {
                // Arrange
                var c = CreateController();
                c.MockAuth.Setup(a => a.AuthenticateWithProviderAsync("fb", "123abc"))
                          .Returns(TaskHelpers.FromResult(AuthenticationResult.MissingData(new[] { "email" })));

                // Act
                return c.Post("fb", "123abc").Then(result =>
                {
                    // Assert
                    Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
                    Assert.Equal(new { missingFields = new [] { "email" } }, result.GetObjectContent(), new PropertyEqualityComparer(typeEquality: false));
                });
            }

            [Fact]
            public Task Returns201OnRegistered()
            {
                // Arrange
                var c = CreateController();
                c.MockAuth.Setup(a => a.AuthenticateWithProviderAsync("fb", "123abc"))
                          .Returns(TaskHelpers.FromResult(AuthenticationResult.Registered(ApiTestData.LoggedInUser)));

                // Act
                return c.Post("fb", "123abc").Then(result =>
                {
                    // Assert
                    Assert.Equal(HttpStatusCode.Created, result.StatusCode);
                    Assert.Equal(c.User, ApiTestData.LoggedIn, new PropertyEqualityComparer());
                    Assert.Equal(new
                    { 
                        user = c.User.Identity,
                        token = c.SessionToken
                    }, result.GetObjectContent(), new PropertyEqualityComparer(typeEquality: false));
                });
            }

            [Fact]
            public Task Returns201OnAssociated()
            {
                // Arrange
                var c = CreateController();
                c.MockAuth.Setup(a => a.AuthenticateWithProviderAsync("fb", "123abc"))
                          .Returns(TaskHelpers.FromResult(AuthenticationResult.Associated(ApiTestData.LoggedInUser)));

                // Act
                return c.Post("fb", "123abc").Then(result =>
                {
                    // Assert
                    Assert.Equal(HttpStatusCode.Created, result.StatusCode);
                    Assert.Equal(c.User, ApiTestData.LoggedIn, new PropertyEqualityComparer());
                    Assert.Equal(new
                    {
                        user = c.User.Identity,
                        token = c.SessionToken
                    }, result.GetObjectContent(), new PropertyEqualityComparer(typeEquality: false));
                });
            }

            [Fact]
            public Task Returns201OnLoggedIn()
            {
                // Arrange
                var c = CreateController();
                c.MockAuth.Setup(a => a.AuthenticateWithProviderAsync("fb", "123abc"))
                          .Returns(TaskHelpers.FromResult(AuthenticationResult.LoggedIn(ApiTestData.LoggedInUser)));

                // Act
                return c.Post("fb", "123abc").Then(result =>
                {
                    // Assert
                    Assert.Equal(HttpStatusCode.Created, result.StatusCode);
                    Assert.Equal(c.User, ApiTestData.LoggedIn, new PropertyEqualityComparer());
                    Assert.Equal(new
                    {
                        user = c.User.Identity,
                        token = c.SessionToken
                    }, result.GetObjectContent(), new PropertyEqualityComparer(typeEquality: false));
                });
            }
        }

        private static TestableSessionsController CreateController() {
            return new TestableSessionsController();
        }

        private class TestableSessionsController : SessionsController
        {
            public Mock<AuthenticationService> MockAuth { get; private set; }

            public TestableSessionsController()
            {
                Auth = (MockAuth = new Mock<AuthenticationService>()).Object;
            }
        }
    }
}
