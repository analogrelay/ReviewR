using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using ReviewR.Web.Controllers;
using ReviewR.Web.Facts.Authentication;
using ReviewR.Web.Models;
using ReviewR.Web.Services;
using ReviewR.Web.ViewModels;
using VibrantUtils;
using Xunit;

namespace ReviewR.Web.Facts.Controllers
{
    public class AccountControllerFacts
    {
        public class LoginGet
        {
            [Fact]
            public void WithoutContentFlagReturnsLoginView()
            {
                // Arrange
                var c = CreateController();

                // Act
                var result = c.Login(content: null);

                // Assert
                ActionAssert.IsViewResult(result, new LoginViewModel());
            }

            [Fact]
            public void WithContentFlagReturnsLoginPartialView()
            {
                // Arrange
                var c = CreateController();

                // Act
                var result = c.Login(content: "1");

                // Assert
                ActionAssert.IsPartialViewResult(result, new LoginViewModel());
            }
        }

        public class LoginPost
        {
            [Fact]
            public void WithInvalidModelReturnsError()
            {
                // Arrange
                var c = CreateController();
                c.ModelState.AddModelError("", "Test");
                var model = new LoginViewModel();

                // Act
                var result = c.Login(model, returnUrl: null, isAjaxRequest: false);

                // Assert
                ActionAssert.IsViewResult(result, model);
                Assert.Contains("Test", c.ModelState.AllErrors());
            }

            [Fact]
            public void WithInvalidModelReturnsError_Ajax()
            {
                // Arrange
                var c = CreateController();
                c.ModelState.AddModelError("", "Test");
                var model = new LoginViewModel();

                // Act
                var result = c.Login(model, returnUrl: null, isAjaxRequest: true);

                // Assert
                ActionAssert.IsJsonResult(result, new { success = false, errors = new[] { "Test" } });
                Assert.Contains("Test", c.ModelState.AllErrors());
            }

            [Fact]
            public void WithValidModelAndUserInfoSetsAuthToken()
            {
                // Arrange
                var model = new LoginViewModel() { Email = "real@user.com", Password = "password" };
                var c = CreateController();
                c.AuthService.CreateUser("real@user.com", "Real User", "password");
                var user = c.AuthService.Data.Users.Where(u => u.Email == "real@user.com").Single();
                user.Roles = new List<Role>() {
                    new Role() { RoleName = "Role1" },
                    new Role() { RoleName = "Role2" },
                    new Role() { RoleName = "Role3" }
                };

                // Act
                var result = c.Login(model, returnUrl: null, isAjaxRequest: false);

                // Assert
                ActionAssert.IsRedirectResult(result, "/app/Home/Index");
                Assert.Equal("real@user.com", c.TestTokenService.UserName);
                Assert.False(c.TestTokenService.Persistent);
                Assert.Equal(new[] { "Role1", "Role2", "Role3" }, c.TestTokenService.Roles.ToArray());
            }

            [Fact]
            public void WithValidModelAndUserInfoSetsPersistentAuthTokenIfRememberMeSet()
            {
                // Arrange
                var model = new LoginViewModel() { Email = "real@user.com", Password = "password", RememberMe = true };
                var c = CreateController();
                c.AuthService.CreateUser("real@user.com", "Real User", "password");

                // Act
                var result = c.Login(model, returnUrl: null, isAjaxRequest: false);

                // Assert
                ActionAssert.IsRedirectResult(result, "/app/Home/Index");
                Assert.Equal("real@user.com", c.TestTokenService.UserName);
                Assert.True(c.TestTokenService.Persistent);
            }

            [Fact]
            public void WithValidModelAndUserInfoReturnsRedirectToHomeIfNoReturnUrl()
            {
                // Arrange
                var model = new LoginViewModel() { Email = "real@user.com", Password = "password" };
                var c = CreateController();
                c.AuthService.CreateUser("real@user.com", "Real User", "password");

                // Act
                var result = c.Login(model, returnUrl: null, isAjaxRequest: false);

                // Assert
                ActionAssert.IsRedirectResult(result, "/app/Home/Index");
            }

            [Fact]
            public void WithValidModelAndUserInfoReturnsRedirectToHomeIfNoReturnUrl_Ajax()
            {
                // Arrange
                var model = new LoginViewModel() { Email = "real@user.com", Password = "password" };
                var c = CreateController();
                c.AuthService.CreateUser("real@user.com", "Real User", "password");

                // Act
                var result = c.Login(model, returnUrl: null, isAjaxRequest: true);

                // Assert
                ActionAssert.IsJsonResult(result, new { success = true, redirect = "/app/Home/Index" });
            }

            [Fact]
            public void WithValidModelAndUserInfoReturnsRedirectToHomeIfReturnUrlNotLocal()
            {
                // Arrange
                var model = new LoginViewModel() { Email = "real@user.com", Password = "password" };
                var c = CreateController();
                c.AuthService.CreateUser("real@user.com", "Real User", "password");

                // Act
                var result = c.Login(model, returnUrl: "http://www.microsoft.com", isAjaxRequest: false);

                // Assert
                ActionAssert.IsRedirectResult(result, "/app/Home/Index");
            }

            [Fact]
            public void WithValidModelAndUserInfoReturnsRedirectToHomeIfReturnUrlNotLocal_Ajax()
            {
                // Arrange
                var model = new LoginViewModel() { Email = "real@user.com", Password = "password" };
                var c = CreateController();
                c.AuthService.CreateUser("real@user.com", "Real User", "password");

                // Act
                var result = c.Login(model, returnUrl: "http://www.microsoft.com", isAjaxRequest: true);

                // Assert
                ActionAssert.IsJsonResult(result, new { success = true, redirect = "/app/Home/Index" });
            }

            [Fact]
            public void WithValidModelAndUserInfoReturnsRedirectToReturnUrlIfLocal()
            {
                // Arrange
                var model = new LoginViewModel() { Email = "real@user.com", Password = "password" };
                var c = CreateController();
                c.AuthService.CreateUser("real@user.com", "Real User", "password");

                // Act
                var result = c.Login(model, returnUrl: "/app/Foo/Bar", isAjaxRequest: false);

                // Assert
                ActionAssert.IsRedirectResult(result, "/app/Foo/Bar");
            }

            [Fact]
            public void WithValidModelAndUserInfoReturnsRedirectToReturnUrlIfLocal_Ajax()
            {
                // Arrange
                var model = new LoginViewModel() { Email = "real@user.com", Password = "password" };
                var c = CreateController();
                c.AuthService.CreateUser("real@user.com", "Real User", "password");

                // Act
                var result = c.Login(model, returnUrl: "/app/Foo/Bar", isAjaxRequest: true);

                // Assert
                ActionAssert.IsJsonResult(result, new { success = true, redirect = "/app/Foo/Bar" });
            }

            [Fact]
            public void WithInvalidUserNameReturnsError()
            {
                // Arrange
                var model = new LoginViewModel() { Email = "real@user.com", Password = "password" };
                var c = CreateController();
                
                // Act
                var result = c.Login(model, returnUrl: null, isAjaxRequest: false);

                // Assert
                ActionAssert.IsViewResult(result, model);
                Assert.Contains("Either there is no user with that email address, or you entered the wrong password.", c.ModelState.AllErrors());
            }

            [Fact]
            public void WithInvalidUserNameReturnsError_Ajax()
            {
                // Arrange
                var model = new LoginViewModel() { Email = "real@user.com", Password = "password" };
                var c = CreateController();

                // Act
                var result = c.Login(model, returnUrl: null, isAjaxRequest: true);

                // Assert
                ActionAssert.IsJsonResult(result, new { success = false, errors = new[] { "Either there is no user with that email address, or you entered the wrong password." } });
                Assert.Contains("Either there is no user with that email address, or you entered the wrong password.", c.ModelState.AllErrors());
            }

            [Fact]
            public void WithInvalidPasswordReturnsError()
            {
                // Arrange
                var model = new LoginViewModel() { Email = "real@user.com", Password = "password" };
                var c = CreateController();
                c.AuthService.CreateUser("real@user.com", "Real User", "123456");

                // Act
                var result = c.Login(model, returnUrl: null, isAjaxRequest: false);

                // Assert
                ActionAssert.IsViewResult(result, model);
                Assert.Contains("Either there is no user with that email address, or you entered the wrong password.", c.ModelState.AllErrors());
            }

            [Fact]
            public void WithInvalidPasswordReturnsError_Ajax()
            {
                // Arrange
                var model = new LoginViewModel() { Email = "real@user.com", Password = "password" };
                var c = CreateController();
                c.AuthService.CreateUser("real@user.com", "Real User", "123456");

                // Act
                var result = c.Login(model, returnUrl: null, isAjaxRequest: true);

                // Assert
                ActionAssert.IsJsonResult(result, new { success = false, errors = new[] { "Either there is no user with that email address, or you entered the wrong password." } });
                Assert.Contains("Either there is no user with that email address, or you entered the wrong password.", c.ModelState.AllErrors());
            }
        }

        public class RegisterGet
        {
            [Fact]
            public void WithoutContentFlagReturnsLoginView()
            {
                // Arrange
                var c = CreateController();

                // Act
                var result = c.Register(content: null);

                // Assert
                ActionAssert.IsViewResult(result, new RegisterViewModel());
            }

            [Fact]
            public void WithContentFlagReturnsLoginPartialView()
            {
                // Arrange
                var c = CreateController();

                // Act
                var result = c.Register(content: "1");

                // Assert
                ActionAssert.IsPartialViewResult(result, new RegisterViewModel());
            }
        }

        public class RegisterPost
        {
            [Fact]
            public void WithInvalidModelReturnsError()
            {
                // Arrange
                var c = CreateController();
                c.ModelState.AddModelError("", "Test");
                var model = new RegisterViewModel();

                // Act
                var result = c.Register(model, returnUrl: null, isAjaxRequest: false);

                // Assert
                ActionAssert.IsViewResult(result, model);
                Assert.Contains("Test", c.ModelState.AllErrors());
            }

            [Fact]
            public void WithInvalidModelReturnsError_Ajax()
            {
                // Arrange
                var c = CreateController();
                c.ModelState.AddModelError("", "Test");
                var model = new RegisterViewModel();

                // Act
                var result = c.Register(model, returnUrl: null, isAjaxRequest: true);

                // Assert
                ActionAssert.IsJsonResult(result, new { success = false, errors = new[] { "Test" } });
                Assert.Contains("Test", c.ModelState.AllErrors());
            }

            [Fact]
            public void WithDuplicateEmailReturnsError()
            {
                // Arrange
                var model = new RegisterViewModel() { Email = "real@user.com", DisplayName = "Real User", Password = "password" };
                var c = CreateController();
                c.AuthService.CreateUser("real@user.com", "Real User", "123456");

                // Act
                var result = c.Register(model, returnUrl: null, isAjaxRequest: false);

                // Assert
                ActionAssert.IsViewResult(result, model);
                Assert.Contains("There is already a user with that email address.", c.ModelState.AllErrors());
            }

            [Fact]
            public void WithDuplicateEmailReturnsError_Ajax()
            {
                // Arrange
                var model = new RegisterViewModel() { Email = "real@user.com", DisplayName = "Real User", Password = "password" };
                var c = CreateController();
                c.AuthService.CreateUser("real@user.com", "Real User", "123456");

                // Act
                var result = c.Register(model, returnUrl: null, isAjaxRequest: true);

                // Assert
                ActionAssert.IsJsonResult(result, new { success = false, errors = new[] { "There is already a user with that email address." } });
                Assert.Contains("There is already a user with that email address.", c.ModelState.AllErrors());
            }

            [Fact]
            public void WithValidModelAndUniqueEmailReturnsRedirectToHomeIfNoReturnUrl()
            {
                // Arrange
                var model = new RegisterViewModel() { Email = "real@user.com", DisplayName = "Real User", Password = "password" };
                var c = CreateController();
                c.AuthService.CreateUser("other@user.com", "Other User", "password");

                // Act
                var result = c.Register(model, returnUrl: null, isAjaxRequest: false);

                // Assert
                ActionAssert.IsRedirectResult(result, "/app/Home/Index");
                Assert.True(c.AuthService.Data.Users.Where(u => u.Email == "real@user.com").Any());
            }

            [Fact]
            public void WithValidModelAndUserInfoReturnsRedirectToHomeIfNoReturnUrl_Ajax()
            {
                // Arrange
                var model = new RegisterViewModel() { Email = "real@user.com", DisplayName = "Real User", Password = "password" };
                var c = CreateController();
                c.AuthService.CreateUser("other@user.com", "Other User", "password");

                // Act
                var result = c.Register(model, returnUrl: null, isAjaxRequest: true);

                // Assert
                ActionAssert.IsJsonResult(result, new { success = true, redirect = "/app/Home/Index" });
                Assert.True(c.AuthService.Data.Users.Where(u => u.Email == "real@user.com").Any());
            }

            [Fact]
            public void WithValidModelAndUserInfoReturnsRedirectToHomeIfReturnUrlNotLocal()
            {
                // Arrange
                var model = new RegisterViewModel() { Email = "real@user.com", DisplayName = "Real User", Password = "password" };
                var c = CreateController();
                c.AuthService.CreateUser("other@user.com", "Other User", "password");

                // Act
                var result = c.Register(model, returnUrl: "http://www.microsoft.com", isAjaxRequest: false);

                // Assert
                ActionAssert.IsRedirectResult(result, "/app/Home/Index");
                Assert.True(c.AuthService.Data.Users.Where(u => u.Email == "real@user.com").Any());
            }

            [Fact]
            public void WithValidModelAndUserInfoReturnsRedirectToHomeIfReturnUrlNotLocal_Ajax()
            {
                // Arrange
                var model = new RegisterViewModel() { Email = "real@user.com", DisplayName = "Real User", Password = "password" };
                var c = CreateController();
                c.AuthService.CreateUser("other@user.com", "Other User", "password");

                // Act
                var result = c.Register(model, returnUrl: "http://www.microsoft.com", isAjaxRequest: true);

                // Assert
                ActionAssert.IsJsonResult(result, new { success = true, redirect = "/app/Home/Index" });
                Assert.True(c.AuthService.Data.Users.Where(u => u.Email == "real@user.com").Any());
            }

            [Fact]
            public void WithValidModelAndUserInfoReturnsRedirectToReturnUrlIfLocal()
            {
                // Arrange
                var model = new RegisterViewModel() { Email = "real@user.com", DisplayName = "Real User", Password = "password" };
                var c = CreateController();
                c.AuthService.CreateUser("other@user.com", "Other User", "password");

                // Act
                var result = c.Register(model, returnUrl: "/app/Foo/Bar", isAjaxRequest: false);

                // Assert
                ActionAssert.IsRedirectResult(result, "/app/Foo/Bar");
                Assert.True(c.AuthService.Data.Users.Where(u => u.Email == "real@user.com").Any());
            }

            [Fact]
            public void WithValidModelAndUserInfoReturnsRedirectToReturnUrlIfLocal_Ajax()
            {
                // Arrange
                var model = new RegisterViewModel() { Email = "real@user.com", DisplayName = "Real User", Password = "password" };
                var c = CreateController();
                c.AuthService.CreateUser("other@user.com", "Other User", "password");

                // Act
                var result = c.Register(model, returnUrl: "/app/Foo/Bar", isAjaxRequest: true);

                // Assert
                ActionAssert.IsJsonResult(result, new { success = true, redirect = "/app/Foo/Bar" });
                Assert.True(c.AuthService.Data.Users.Where(u => u.Email == "real@user.com").Any());
            }
        }

        private static TestableAccountController CreateController()
        {
            var c = new TestableAccountController(
                new AuthenticationService(
                    new TestDataRepository(), 
                    new TestHashService()), 
                new TestAuthTokenService(),
                new TestUrlService());
            return c;
        }

        public class TestableAccountController : AccountController
        {
            public TestAuthTokenService TestTokenService { get; set; }

            public TestableAccountController(AuthenticationService authService, TestAuthTokenService tokenService, UrlService urlService)
                : base(authService, tokenService, urlService)
            {
                TestTokenService = tokenService;
            }
        }
    }
}
