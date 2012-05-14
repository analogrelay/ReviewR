using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Models;
using ReviewR.Web.Services.Authenticators;
using Xunit;

namespace ReviewR.Web.Facts.Services.Authenticators
{
    public class AuthenticatorFacts
    {
        [Fact]
        public void ActiveDefaultsToTrue()
        {
            Assert.True(CreateAuthenticator().Active);
        }

        [Fact]
        public void AccessTokenNameHasDefault()
        {
            Assert.Equal("access_token", CreateAuthenticator().AccessTokenQueryParameterName);
        }

        public class CompleteAuthentication
        {
            [Fact]
            public Task ThrowsIfBaseUrlNotSet()
            {
                // Arrange
                var a = CreateAuthenticator();

                // Act
                return a.CompleteAuthentication("abc").ShouldThrow<NotImplementedException>();
            }

            [Fact]
            public Task ThrowsIfParseResponseNotOverridden()
            {
                // Arrange
                var a = CreateAuthenticator(baseUrl: "http://foo.bar");
                a.TestMessageHandler.NextResult =
                    TaskHelpers.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{ blarg }") });

                // Act
                return a.CompleteAuthentication("abc").ShouldThrow<NotImplementedException>();
            }

            [Fact]
            public Task ThrowsIfResponseNotSuccessful()
            {
                // Arrange
                var a = CreateAuthenticator(baseUrl: "http://foo.bar");
                a.TestMessageHandler.NextResult =
                    TaskHelpers.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError));

                // Act
                return a.CompleteAuthentication("abc").ShouldThrow<HttpRequestException>();
            }

            [Fact]
            public Task ReturnsParserResultIfSuccessful()
            {
                // Arrange
                var a = CreateAuthenticator(
                    baseUrl: "http://foo.bar", 
                    parser: s => {
                        if(!String.Equals("glarb", s, StringComparison.Ordinal)) {
                            throw new InvalidOperationException("Unexpected input: " + s);
                        }
                        return new UserInfo("test", "t", "Test User", "test@test.test");
                    });
                a.TestMessageHandler.NextResult =
                    TaskHelpers.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("glarb") });

                // Act
                return a.CompleteAuthentication("abc").Then(u => {
                    // Assert
                    Assert.Equal("test", u.Provider);
                    Assert.Equal("t", u.Identifier);
                    Assert.Equal("Test User", u.DisplayName);
                    Assert.Equal("test@test.test", u.Email);
                });
            }

            [Fact]
            public Task SendsGetRequestToBaseUrlWithAccessTokenInQueryStringUsingAccessTokenName()
            {
                // Arrange
                var a = CreateAuthenticator(
                    baseUrl: "http://foo.bar",
                    accessTokenName: "flarb",
                    parser: s =>
                    {
                        if (!String.Equals("glarb", s, StringComparison.Ordinal))
                        {
                            throw new InvalidOperationException("Unexpected input: " + s);
                        }
                        return new UserInfo("test", "t", "Test User", "test@test.test");
                    });
                a.TestMessageHandler.NextResult =
                    TaskHelpers.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("glarb") });

                // Act
                return a.CompleteAuthentication("blerg").Then(u =>
                {
                    Assert.Equal("http://foo.bar/?flarb=blerg", a.TestMessageHandler.LastCall.Item1.RequestUri.AbsoluteUri);
                    Assert.Equal(HttpMethod.Get, a.TestMessageHandler.LastCall.Item1.Method);
                });
            }
        }

        private static TestableAuthenticator CreateAuthenticator()
        {
            return new TestableAuthenticator();
        }

        private static TestableAuthenticator CreateAuthenticator(string baseUrl = null, string accessTokenName = null, bool? active = null, Func<dynamic, UserInfo> parser = null)
        {
            return new TestableAuthenticator(baseUrl, accessTokenName, active, parser);
        }

        public class TestableAuthenticator : Authenticator
        {
            private string _baseUrl;
            private string _accessTokenName;
            private bool? _active;
            private Func<dynamic, UserInfo> _parser;

            public TestableMessageHandler TestMessageHandler { get; set; }

            public TestableAuthenticator()
            {
                TestMessageHandler = new TestableMessageHandler();
            }

            public TestableAuthenticator(string baseUrl, string accessTokenName, bool? active, Func<dynamic, UserInfo> parser)
            {
                TestMessageHandler = new TestableMessageHandler();
                _baseUrl = baseUrl;
                _accessTokenName = accessTokenName;
                _active = active;
                _parser = parser;
            }

            protected override UserInfo ParseResponse(string jsonResponse)
            {
                return _parser == null ? base.ParseResponse(jsonResponse) : _parser(jsonResponse);
            }

            protected internal override HttpClient CreateHttpClient()
            {
                return new HttpClient(TestMessageHandler);
            }

            protected internal override string AccessTokenQueryParameterName
            {
                get
                {
                    return String.IsNullOrEmpty(_accessTokenName) ? base.AccessTokenQueryParameterName : _accessTokenName;
                }
            }

            protected internal override string FetchUserInfoBaseUrl
            {
                get
                {
                    return String.IsNullOrEmpty(_baseUrl) ? base.FetchUserInfoBaseUrl : _baseUrl;
                }
            }

            public override bool Active
            {
                get
                {
                    return _active ?? base.Active;
                }
            }

            public override string Name
            {
                get { return "Test"; }
            }

            public override string Id
            {
                get { return "t"; }
            }

            public override string GetAppId(ISettings appSettings)
            {
                return appSettings.Get("t:appid");
            }
        }

        public class TestableMessageHandler : HttpMessageHandler
        {
            public Tuple<HttpRequestMessage, CancellationToken> LastCall { get; set; }
            public Task<HttpResponseMessage> NextResult { get; set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastCall = Tuple.Create(request, cancellationToken);
                return NextResult;
            }
        }
    }
}
