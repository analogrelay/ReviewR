using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Moq;
using ReviewR.Web.Models;
using ReviewR.Web.Services;
using VibrantUtils;
using Xunit;

namespace ReviewR.Web.Facts.Authentication
{
    public class AuthenticationServiceFacts
    {
        public class LogIn
        {
            [Fact]
            public void RequiresNonNullOrEmptyEmail()
            {
                ContractAssert.NotNullOrEmpty(c => CreateService().LogIn(c, "password"), "email");          
            }

            [Fact]
            public void RequiresNonNullOrEmptyPassword()
            {
                ContractAssert.NotNullOrEmpty(c => CreateService().LogIn("foo", c), "password");
            }

            [Fact]
            public void ReturnsNullIfNoUserWithEmail()
            {
                // Arrange
                var svc = CreateService();

                // Act
                var actual = svc.LogIn("non@existant.com", "password");

                // Actual
                Assert.Null(actual);
            }

            [Fact]
            public void ReturnsNullIfUserPasswordDoesNotMatchSpecifiedPassword()
            {
                // Arrange
                var svc = CreateService();
                var expected = new User() { Email = "real@user.com", Password = "drowssap|tlas", PasswordSalt = "salt" };
                svc.Data.Users.Add(expected);
                svc.Data.SaveChanges();

                // Act
                var actual = svc.LogIn("real@user.com", "abc123");

                // Assert
                Assert.Null(actual);
            }

            [Fact]
            public void ReturnsUserObjectIfPasswordDoesMatchSpecifiedPassword()
            {
                // Arrange
                var svc = CreateService();
                var expected = new User() { Email = "real@user.com",  Password = "drowssap|tlas", PasswordSalt="salt"};
                svc.Data.Users.Add(expected);
                svc.Data.SaveChanges();

                // Act
                var actual = svc.LogIn("real@user.com", "password");

                // Assert
                Assert.Equal(expected, actual);
            }
        }

        public class CreateUser
        {
            [Fact]
            public void RequiresNonNullOrEmptyEmail()
            {
                ContractAssert.NotNullOrEmpty(s => CreateService().CreateUser(s, "displayName", "password"), "email");
            }

            [Fact]
            public void RequiresNonNullOrEmptyDisplayName()
            {
                ContractAssert.NotNullOrEmpty(s => CreateService().CreateUser("email", s, "password"), "displayName");
            }

            [Fact]
            public void RequiresNonNullOrEmptyPassword()
            {
                ContractAssert.NotNullOrEmpty(s => CreateService().CreateUser("email", "displayName", s), "password");
            }

            [Fact]
            public void UserIsCreatedWithHashedPasswordAndOtherProvidedData()
            {
                // Arrange
                var svc = CreateService();

                // Act
                CreateUserResult result = svc.CreateUser(email: "new@user.com", displayName: "New User", password: "password");

                // Assert
                Assert.Equal(CreateUserResult.Success, result);
                User user = svc.Data.Users.SingleOrDefault();
                Assert.NotNull(user);
                Assert.Equal("new@user.com", user.Email);
                Assert.Equal("New User", user.DisplayName);
                Assert.Equal("salt", user.PasswordSalt);
                Assert.Equal("drowssap|tlas", user.Password);
            }

            [Fact]
            public void EmailAlreadyExistsReturnedIfUserWithSpecifiedEmailAlreadyExists()
            {
                // Arrange
                var svc = CreateService();
                svc.Data.Users.Add(new User() { Email = "new@user.com" });
                svc.Data.SaveChanges();

                // Act
                CreateUserResult result = svc.CreateUser(email: "new@user.com", displayName: "New User", password: "password");

                // Assert
                Assert.Equal(CreateUserResult.EmailTaken, result);
                Assert.Equal(1, svc.Data.Users.Count());
            }
        }

        private static AuthenticationService CreateService()
        {
            return new AuthenticationService(new TestDataRepository(), new TestHashService(), new Mock<HttpContextBase>().Object);
        }
    }
}
