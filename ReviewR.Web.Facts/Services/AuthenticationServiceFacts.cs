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
    public class AuthenticationServiceFacts
    {
        public class Ctor
        {
            [Fact]
            public void InitializesValues()
            {
                // Arrange
                var data = new MockDataRepository();
                var tokens = new TokenService();
                var settings = new MockSettings();

                // Act
                AuthenticationService auth = new AuthenticationService(data, tokens, settings);

                // Assert
                Assert.Same(data, auth.Data);
                Assert.Same(tokens, auth.Tokens);
                Assert.Same(settings, auth.Settings);
                Assert.Equal(TimeSpan.FromMinutes(30), auth.Timeout);
            }

            [Fact]
            public void RequiresNonNullArguments()
            {
                ContractAssert.NotNull(() => new AuthenticationService(null, new MockTokenService(), new MockSettings()), "data");
                ContractAssert.NotNull(() => new AuthenticationService(new MockDataRepository(), null, new MockSettings()), "tokens");
                ContractAssert.NotNull(() => new AuthenticationService(new MockDataRepository(), new MockTokenService(), null), "settings");
            }
        }

        public class GetUserByEmail
        {
            [Fact]
            public void RequiresNonNullOrEmptyEmail()
            {
                ContractAssert.NotNullOrEmpty(s => CreateService().GetUserByEmail(s), "email");
            }

            [Fact]
            public void ReturnsNullForNonExistantEmail()
            {
                Assert.Null(CreateService().GetUserByEmail("bork@bork.bork"));
            }

            [Fact]
            public void ReturnsUserMatchingEmailIfOneExists()
            {
                // Arrange
                var auth = CreateService();
                auth.MockData.Users.Add(new User() { Email = "bork@bork.bork", DisplayName = "Swedish Chef" });
                auth.MockData.SaveChanges();

                // Act
                var actual = auth.GetUserByEmail("bork@bork.bork");

                // Assert
                Assert.Equal("Swedish Chef", actual.DisplayName);
            }
        }

        public class ResolveAuthTokenAsync
        {
            [Fact]
            public void RequiresNonNullOrEmptyAuthToken()
            {
                ContractAssert.NotNullOrEmpty(s => CreateService().ResolveAuthTokenAsync(s).Wait(), "authenticationToken");
            }

            [Fact]
            public void BubblesHttpExceptions()
            {
                AggregateException agg = Assert.Throws<AggregateException>(() => CreateService().ResolveAuthTokenAsync("abc123").Result);
                Assert.Equal(1, agg.InnerExceptions.Count);
                Assert.IsType<HttpRequestException>(agg.InnerExceptions[0]);
            }

            [Fact]
            public Task ReturnsNullOnFailureErrorCode()
            {
                // Arrange
                const string token = "abc123";
                var auth = CreateService();
                var msg = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                auth.TokenExchanges[token] = msg;

                // Act/Assert
                return auth.ResolveAuthTokenAsync(token).ContinueWith(t =>
                {
                    Assert.Null(t.Result);
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
            }

            [Fact]
            public void BubblesJsonParseExceptions()
            {
                // Arrange
                const string token = "abc123";
                var auth = CreateService();
                var msg = new HttpResponseMessage(HttpStatusCode.OK);
                msg.Content = new StringContent("<glorb>");
                auth.TokenExchanges[token] = msg;

                // Act/Assert
                AggregateException agg = Assert.Throws<AggregateException>(() => auth.ResolveAuthTokenAsync(token).Wait());
                Assert.Equal(1, agg.InnerExceptions.Count);
                Assert.IsType<JsonReaderException>(agg.InnerExceptions[0]);
            }

            [Fact]
            public Task ReturnsNullIfProfileMissing()
            {
                // Arrange
                const string token = "abc123";
                var auth = CreateService();
                var msg = new HttpResponseMessage(HttpStatusCode.OK);
                msg.Content = new StringContent("{ glorb: 'glorp' }");
                auth.TokenExchanges[token] = msg;

                // Act/Assert
                return auth.ResolveAuthTokenAsync(token).ContinueWith(t => {
                    Assert.Null(t.Result);
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
            }

            [Fact]
            public Task ReturnsDataFromProfile()
            {
                // Arrange
                const string token = "abc123";
                var auth = CreateService();
                var msg = new HttpResponseMessage(HttpStatusCode.OK);
                msg.Content = new StringContent(@"{ 
                    profile: {
                        providerName: 'bacefook',
                        identifier: 'http://bacefook/bork',
                        displayName: 'Swedish Chef',
                        verifiedEmail: 'bork@bork.bork'
                    }
                }");
                auth.TokenExchanges[token] = msg;

                // Act
                return auth.ResolveAuthTokenAsync(token).ContinueWith(t =>
                {
                    var info = t.Result;

                    // Assert
                    Assert.Equal("bacefook", info.Provider);
                    Assert.Equal("http://bacefook/bork", info.Identifier);
                    Assert.Equal("Swedish Chef", info.DisplayName);
                    Assert.Equal("bork@bork.bork", info.Email);
                });
            }

            [Fact]
            public Task UsesFormattedNameAsDisplayNameIfPresent()
            {
                // Arrange
                const string token = "abc123";
                var auth = CreateService();
                var msg = new HttpResponseMessage(HttpStatusCode.OK);
                msg.Content = new StringContent(@"{ 
                    profile: {
                        name: {
                            formatted: 'Swedish Chef'
                        },
                        providerName: 'bacefook',
                        identifier: 'http://bacefook/bork',
                        displayName: 'swbork',
                        verifiedEmail: 'bork@bork.bork'
                    }
                }");
                auth.TokenExchanges[token] = msg;

                // Act
                return auth.ResolveAuthTokenAsync(token).ContinueWith(t =>
                {
                    var info = t.Result;

                    // Assert
                    Assert.Equal("bacefook", info.Provider);
                    Assert.Equal("http://bacefook/bork", info.Identifier);
                    Assert.Equal("Swedish Chef", info.DisplayName);
                    Assert.Equal("bork@bork.bork", info.Email);
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
            }
        }

        public class Login
        {
            [Fact]
            public void RequiresNonNullOrEmptyArguments()
            {
                ContractAssert.NotNullOrEmpty(s => CreateService().Login(s, "foo"), "provider");
                ContractAssert.NotNullOrEmpty(s => CreateService().Login("foo", s), "identifier");
            }

            [Fact]
            public void ReturnsNullIfNoCredentialMatches()
            {
                // Arrange
                var auth = CreateService();
                var user = new User() { DisplayName = "Swedish Chef" };
                var cred = new Credential() { Provider = "flitter", Identifier = "#swbork", User = user };
                user.Credentials = new List<Credential>() { cred };
                auth.MockData.Users.Add(user);
                auth.MockData.Credentials.Add(cred);
                auth.MockData.SaveChanges();

                // Act
                var actual = auth.Login("bacefook", "http://bacefook/bork");

                // Assert
                Assert.Null(actual);
            }

            [Fact]
            public void ReturnsUserMatchingCredential()
            {
                // Arrange
                var auth = CreateService();
                var expected = new User() { DisplayName = "Swedish Chef" };
                var notexpected = new User() { DisplayName = "Beaker" };
                var cred = new Credential() { Provider = "bacefook", Identifier = "http://bacefook/bork", User = expected };
                expected.Credentials = new List<Credential>() { cred };
                auth.MockData.Users.Add(expected);
                auth.MockData.Users.Add(notexpected);
                auth.MockData.Credentials.Add(cred);
                auth.MockData.SaveChanges();

                // Act
                var actual = auth.Login("bacefook", "http://bacefook/bork");

                // Assert
                Assert.Equal(expected.DisplayName, actual.DisplayName);
            }

            [Fact]
            public void BothProviderAndIdMustMatch()
            {
                // Arrange
                var auth = CreateService();
                var expected = new User() { DisplayName = "Swedish Chef" };
                var cred = new Credential() { Provider = "flitter", Identifier = "http://bacefook/bork", User = expected };
                expected.Credentials = new List<Credential>() { cred };
                auth.MockData.Users.Add(expected);
                auth.MockData.Credentials.Add(cred);
                auth.MockData.SaveChanges();

                // Act
                var actual = auth.Login("bacefook", "http://bacefook/bork");

                // Assert
                Assert.Null(actual);
            }
        }

        public class Register
        {
            [Fact]
            public void RequiresNonNullOrEmptyArguments()
            {
                ContractAssert.NotNullOrEmpty(s => CreateService().Register(s, "id", "bar", "foo"), "provider");
                ContractAssert.NotNullOrEmpty(s => CreateService().Register("p", s, "bar", "foo"), "identifier");
                ContractAssert.NotNullOrEmpty(s => CreateService().Register("p", "i", s, "foo"), "email");
                ContractAssert.NotNullOrEmpty(s => CreateService().Register("p", "i", "foo", s), "displayName");
            }

            [Fact]
            public void ReturnsCreatedUserAndCredential()
            {
                // Arrange
                var auth = CreateService();

                // Act
                User ret = auth.Register("provider", "identifier", "email", "displayName");

                // Assert
                User db = auth.Data.Users.Single();
                Assert.Equal(ret.Email, db.Email);
                Assert.Equal(ret.DisplayName, db.DisplayName);
                Assert.Equal(ret.Id, db.Id);
                Credential retCred = ret.Credentials.Single();
                Credential dbCred = db.Credentials.Single();
                Assert.Equal(retCred.Provider, dbCred.Provider);
                Assert.Equal(retCred.Identifier, dbCred.Identifier);
            }

            [Fact]
            public void AddsUserAndCredentialToDatabase()
            {
                // Arrange
                var auth = CreateService();

                // Act
                auth.Register("provider", "identifier", "email", "displayName");

                // Assert
                User db = auth.Data.Users.Single();
                Assert.Equal("email", db.Email);
                Assert.Equal("displayName", db.DisplayName);
                Assert.Equal(auth.MockData.LastId, db.Id);
                Credential dbCred = db.Credentials.Single();
                Assert.Equal("provider", dbCred.Provider);
                Assert.Equal("identifier", dbCred.Identifier);
            }
        }

        public class AddCredential
        {
            [Fact]
            public void RequiresNonNullOrEmptyArguments()
            {
                ContractAssert.OutOfRange(() => CreateService().AddCredential(-1, "p", "i"), "userId");
                ContractAssert.NotNullOrEmpty(s => CreateService().AddCredential(1, s, "i"), "provider");
                ContractAssert.NotNullOrEmpty(s => CreateService().AddCredential(1, "p", s), "identifier");
            }

            [Fact]
            public void AddsCredentialToDatabase()
            {
                // Arrange
                var auth = CreateService();

                // Act
                auth.AddCredential(42, "provider", "identifier");

                // Assert
                Credential cred = auth.MockData.Credentials.Single();
                Assert.Equal(42, cred.UserId);
                Assert.Equal("provider", cred.Provider);
                Assert.Equal("identifier", cred.Identifier);
            }
        }

        private static TestableAuthenticationService CreateService()
        {
            return new TestableAuthenticationService();
        }

        private class TestableAuthenticationService : AuthenticationService
        {
            public MockDataRepository MockData { get; set; }
            public MockTokenService MockTokens { get; set; }
            public MockSettings MockSettings { get; set; }
            public IDictionary<string, HttpResponseMessage> TokenExchanges { get; private set; }

            public TestableAuthenticationService()
            {
                Data = MockData = new MockDataRepository();
                Tokens = MockTokens = new MockTokenService();
                Settings = MockSettings = new MockSettings();
                TokenExchanges = new Dictionary<string, HttpResponseMessage>();
            }

            protected internal override Task<HttpResponseMessage> ExchangeToken(string token)
            {
                var tcs = new TaskCompletionSource<HttpResponseMessage>();
                HttpResponseMessage msg;
                if (!TokenExchanges.TryGetValue(token, out msg))
                {
                    tcs.TrySetException(new HttpRequestException("ruh roh!"));
                }
                else
                {
                    tcs.TrySetResult(msg);
                }
                return tcs.Task;
            }
        }
    }
}
