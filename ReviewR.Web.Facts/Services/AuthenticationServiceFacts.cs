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
using ReviewR.Web.Facts.Api;
using ReviewR.Web.Models;
using ReviewR.Web.Models.Data;
using ReviewR.Web.Services;
using ReviewR.Web.Services.Authenticators;
using VibrantUtils;
using Xunit;

namespace ReviewR.Web.Facts.Services
{
    public class AuthenticationServiceFacts
    {
        public class Ctor
        {
            [Fact]
            public void InitializesValues()
            {
                // Arrange
                var data = new MockDataRepository();
                var auths = new List<Authenticator>() { new FacebookAuthenticator() };
                var settings = new MockSettings();

                // Act
                AuthenticationService auth = new AuthenticationService(auths, settings, data);

                // Assert
                Assert.Same(data, auth.Data);
                Assert.Equal(1, auth.Authenticators.Count);
                Assert.Same(auths[0], auth.Authenticators["fb"]);
                Assert.Same(settings, auth.Settings);
            }

            [Fact]
            public void RequiresNonNullArguments()
            {
                ContractAssert.NotNull(() => new AuthenticationService(null, new MockSettings(), new MockDataRepository()), "authenticators");
                ContractAssert.NotNull(() => new AuthenticationService(new List<Authenticator>(), null, new MockDataRepository()), "settings");
                ContractAssert.NotNull(() => new AuthenticationService(new List<Authenticator>(), new MockSettings(), null), "data");
            }
        }

        public class GetClientIds
        {
            [Fact]
            public void ReturnsDictionaryOfClientIds()
            {
                // Arrange
                var s = CreateService();
                s.MockSettings.Values["facebook:appid"] = "12345";

                // Act/Assert
                Assert.Equal(new[] { new KeyValuePair<string, string>("fb", "12345") }, s.GetClientIds().ToArray());
            }
        }

        public class AuthenticateWithProviderAsync
        {
            [Fact]
            public void RequiresValidArguments()
            {
                // Don't need to wait on the result because we should have thrown before creating the task
                ContractAssert.NotNullOrEmpty(s => CreateService().AuthenticateWithProviderAsync(s, "fb"), "type", ignoreTrace: true);
                ContractAssert.NotNullOrEmpty(s => CreateService().AuthenticateWithProviderAsync("fb", s), "accessToken", ignoreTrace: true);
            }

            [Fact]
            public void ThrowsIfNoAuthenticatorWithId()
            {
                CreateService().AuthenticateWithProviderAsync("glarb", "blarg").ShouldThrow<NotSupportedException>(ex =>
                {
                    Assert.Equal("No such authentication provider: glarb", ex.Message);
                });
            }

            [Fact]
            public void BubblesHttpExceptions()
            {
                CreateService().AuthenticateWithProviderAsync("fb", "abc123").ShouldThrow<HttpRequestException>();
            }

            [Fact]
            public Task ReturnsLoggedInIfMatchingCredentialFound()
            {
                // Arrange
                var s = CreateService();
                var user = ApiTestData.CreateLoggedInUser();
                s.MockData.Credentials.Add(new Credential() { Provider = "Facebook", Identifier = "foobar", User = user });
                s.MockData.SaveChanges();
                s.AddTokenExchange("glarb", new UserInfo("Facebook", "foobar", "Swedish Chef", "bork@bork.bork"));

                // Act
                return s.AuthenticateWithProviderAsync("fb", "glarb").Then(r =>
                {
                    // Assert
                    Assert.Equal(AuthenticationOutcome.LoggedIn, r.Outcome);
                    Assert.Same(user, r.User);
                    Assert.Empty(r.MissingFields);
                });
            }

            [Fact]
            public Task ReturnsAssociatedIfUserWithSameEmailFound()
            {
                // Arrange
                var s = CreateService();
                var user = ApiTestData.CreateLoggedInUser();
                s.MockData.Users.Add(user);
                s.MockData.Credentials.Add(new Credential() { Provider = "Google", Identifier = "glarg", User = user });
                s.MockData.SaveChanges();
                s.AddTokenExchange("glarb", new UserInfo("Facebook", "foobar", "Swedish Chef", "bork@bork.bork"));

                // Act
                return s.AuthenticateWithProviderAsync("fb", "glarb").Then(r =>
                {
                    // Assert
                    Assert.Equal(AuthenticationOutcome.Associated, r.Outcome);
                    Assert.Same(user, r.User);
                    Assert.Empty(r.MissingFields);

                    // Credential is added to user, the other credential should still exist.
                    Assert.Equal("Facebook", user.Credentials.Single().Provider);
                    Assert.Equal("foobar", user.Credentials.Single().Identifier);
                });
            }

            [Fact]
            public Task ReturnsMissingDataIfEmailNotProvided()
            {
                // Arrange
                var s = CreateService();
                s.AddTokenExchange("glarb", new UserInfo("Facebook", "foobar", "Swedish Chef", email: null));

                // Act
                return s.AuthenticateWithProviderAsync("fb", "glarb").Then(r =>
                {
                    // Assert
                    Assert.Equal(AuthenticationOutcome.MissingFields, r.Outcome);
                    Assert.Equal(new [] { "email" }, r.MissingFields.ToArray());
                });
            }

            [Fact]
            public Task ReturnsMissingDataIfDisplayNameNotProvided()
            {
                // Arrange
                var s = CreateService();
                s.AddTokenExchange("glarb", new UserInfo("Facebook", "foobar", displayName: null, email: "bork@bork.bork"));

                // Act
                return s.AuthenticateWithProviderAsync("fb", "glarb").Then(r =>
                {
                    // Assert
                    Assert.Equal(AuthenticationOutcome.MissingFields, r.Outcome);
                    Assert.Equal(new[] { "displayName" }, r.MissingFields.ToArray());
                });
            }

            [Fact]
            public Task ReturnsMissingDataIfDisplayNameAndEmailNotProvided()
            {
                // Arrange
                var s = CreateService();
                s.AddTokenExchange("glarb", new UserInfo("Facebook", "foobar", displayName: null, email: null));

                // Act
                return s.AuthenticateWithProviderAsync("fb", "glarb").Then(r =>
                {
                    // Assert
                    Assert.Equal(AuthenticationOutcome.MissingFields, r.Outcome);
                    Assert.Equal(new[] { "email", "displayName" }, r.MissingFields.ToArray());
                });
            }

            [Fact]
            public Task RegistersAndReturnsNewUserWithCredentialIfAllDataProviderAndNoCurrentUser()
            {
                // Arrange
                var s = CreateService();
                var user = ApiTestData.CreateLoggedInUser();
                s.MockData.Users.Add(user);
                s.MockData.Credentials.Add(new Credential() { Provider = "Google", Identifier = "glarg", User = user });
                s.MockData.SaveChanges();
                s.AddTokenExchange("glarb", new UserInfo("Facebook", "foobar", "Beeker", "meep@meep.meep"));

                // Act
                return s.AuthenticateWithProviderAsync("fb", "glarb").Then(r =>
                {
                    // Assert
                    Assert.Equal(AuthenticationOutcome.Registered, r.Outcome);
                    Assert.Same(s.MockData.Users.Where(u => !ReferenceEquals(u, user)).Single(), r.User);
                    Assert.Empty(r.MissingFields);
                    Assert.Equal("Beeker", r.User.DisplayName);
                    Assert.Equal("meep@meep.meep", r.User.Email);
                    Assert.Equal("Facebook", r.User.Credentials.Single().Provider);
                    Assert.Equal("foobar", r.User.Credentials.Single().Identifier);
                });
            }
        }

        private static TestableAuthenticationService CreateService()
        {
            return new TestableAuthenticationService();
        }

        private class TestableAuthenticationService : AuthenticationService
        {
            public MockDataRepository MockData { get; set; }
            public MockSettings MockSettings { get; set; }
            private IDictionary<string, UserInfo> TokenExchanges { get; set; }

            public TestableAuthenticationService()
            {
                Authenticators = new Dictionary<string, Authenticator>() {
                    {"fb", new FacebookAuthenticator()}
                };
                Data = MockData = new MockDataRepository();
                Settings = MockSettings = new MockSettings();
                TokenExchanges = new Dictionary<string, UserInfo>();
            }

            public void AddTokenExchange(string token, UserInfo response)
            {
                TokenExchanges["fb|" + token] = response;
            }

            protected internal override Task<UserInfo> ExchangeToken(string token, Authenticator auth)
            {
                UserInfo usr;
                if (!TokenExchanges.TryGetValue(auth.Id + "|" + token, out usr))
                {
                    return TaskHelpers.FromError<UserInfo>(new HttpRequestException("ruh roh!"));
                }
                else
                {
                    return TaskHelpers.FromResult(usr);
                }
            }
        }
    }
}
