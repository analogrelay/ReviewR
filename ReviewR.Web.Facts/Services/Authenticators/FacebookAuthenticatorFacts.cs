using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReviewR.Web.Models;
using ReviewR.Web.Services.Authenticators;
using VibrantUtils;
using Xunit;

namespace ReviewR.Web.Facts.Services.Authenticators
{
    public class FacebookAuthenticatorFacts
    {
        [Fact]
        public void BasicDataVerification()
        {
            // Arrange
            FacebookAuthenticator auth = new FacebookAuthenticator();

            // Assert
            Assert.Equal("Facebook", auth.Name);
            Assert.Equal("fb", auth.Id);
            Assert.Equal("https://graph.facebook.com/me", auth.FetchUserInfoBaseUrl);
        }

        public class GetAppId
        {
            [Fact]
            public void RequiresNonNullSettings()
            {
                ContractAssert.NotNull(() => new FacebookAuthenticator().GetAppId(null), "appSettings");
            }

            [Fact]
            public void ReturnsNullIfNotRegistered()
            {
                Assert.Null(new FacebookAuthenticator().GetAppId(new MockSettings()));
            }

            [Fact]
            public void ReturnsAppIDValueIfInSettings()
            {
                // Arrange
                const string expected = "123456";
                var settings = new MockSettings();
                settings.Values[FacebookAuthenticator.AppIdKey] = expected;

                // Act/Assert
                Assert.Equal(expected, new FacebookAuthenticator().GetAppId(settings));
            }
        }

        public class ParseResponse
        {
            [Fact]
            public void RequiresNonNullOrEmptyJson()
            {
                ContractAssert.NotNullOrEmpty(s => new FacebookAuthenticator().ParseResponse(s), "jsonResponse");
            }

            [Fact]
            public void ParsesDataOutOfFBResponse()
            {
                // Arrange
                const string json = "{ link: 'glarb', name: 'blarb', email: 'flarb' }";
                
                // Act/Assert
                Assert.Equal(new UserInfo(
                    provider: "Facebook",
                    identifier: "glarb",
                    displayName: "blarb",
                    email: "flarb"
                ), new FacebookAuthenticator().ParseResponse(json), new PropertyEqualityComparer());
            }

            [Fact]
            public void IgnoresExtraData()
            {
                // Arrange
                const string json = "{ link: 'glarb', flink: 12, name: 'blarb', shmame: [], email: 'flarb', sbemail: {} }";

                // Act/Assert
                Assert.Equal(new UserInfo(
                    provider: "Facebook",
                    identifier: "glarb",
                    displayName: "blarb",
                    email: "flarb"
                ), new FacebookAuthenticator().ParseResponse(json), new PropertyEqualityComparer());
            }

            [Fact]
            public void InsertsNullForMissingData()
            {
                // Arrange
                const string json = "{ flink: 12, shmame: [], sbemail: {} }";

                // Act/Assert
                Assert.Equal(new UserInfo(
                    provider: "Facebook",
                    identifier: null,
                    displayName: null,
                    email: null
                ), new FacebookAuthenticator().ParseResponse(json), new PropertyEqualityComparer());
            }
        }
    }
}
