using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Moq;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Models;
using ReviewR.Web.Services;
using VibrantUtils;
using Xunit;

namespace ReviewR.Web.Facts.Services
{
    public class UtilsFacts
    {
        public class GetGravatarHash
        {
            [Fact]
            public void RequiresValidArguments()
            {
                ContractAssert.NotNullOrEmpty(s => Utils.GetGravatarHash(s), "email");
            }

            [Fact]
            public void CorrectlyHashesValue()
            {
                Assert.Equal("67614c787f39a22a54b7dcd169a01477", Utils.GetGravatarHash("bork@bork.bork"));
            }
        }

        public class GetActiveSessionToken
        {
            [Fact]
            public void RequiresValidArguments()
            {
                ContractAssert.NotNull(() => Utils.GetActiveSessionToken(null), "context");
            }

            [Fact]
            public void ReturnsNullIfNoCookieWithExpectedName()
            {
                // Arrange
                var context = new Mock<HttpContextBase>();
                context.Setup(c => c.Request.Cookies)
                       .Returns(() => new HttpCookieCollection());
                
                // Act
                Assert.Null(Utils.GetActiveSessionToken(context.Object));
            }

            [Fact]
            public void ReturnsNullIfCookieIsUnsupportedVersion()
            {
                // Arrange
                var context = new Mock<HttpContextBase>();
                var tokens = new Mock<TokenService>();
                var utils = new Utils.UtilWorker(tokens.Object);
                context.Setup(c => c.Request.Cookies)
                       .Returns(() => new HttpCookieCollection() { 
                           new HttpCookie(ReviewRApiController.CookieName, "COOK%20IE!?") 
                       });
                tokens.Setup(t => t.UnprotectToken("COOK IE!?", ReviewRApiController.Purpose))
                      .Throws<NotSupportedException>();

                // Act
                Assert.Null(utils.GetActiveSessionToken(context.Object));
            }

            [Fact]
            public void ReturnsNullIfCookieIsInvalid()
            {
                // Arrange
                var context = new Mock<HttpContextBase>();
                var tokens = new Mock<TokenService>();
                var utils = new Utils.UtilWorker(tokens.Object);
                context.Setup(c => c.Request.Cookies)
                       .Returns(() => new HttpCookieCollection() { 
                           new HttpCookie(ReviewRApiController.CookieName, "COOK%20IE!?") 
                       });
                tokens.Setup(t => t.UnprotectToken("COOK IE!?", ReviewRApiController.Purpose))
                      .Throws<InvalidDataException>();

                // Act
                Assert.Null(utils.GetActiveSessionToken(context.Object));
            }
        }

        public class DecodeCookieToJson
        {
            [Fact]
            public void RequiresValidArguments()
            {
                ContractAssert.NotNull(() => Utils.DecodeCookieToJson(null), "context");
            }

            [Fact]
            public void ReturnsNullIfCookieIsUnsupportedVersion()
            {
                // Arrange
                var context = new Mock<HttpContextBase>();
                var tokens = new Mock<TokenService>();
                var utils = new Utils.UtilWorker(tokens.Object);
                context.Setup(c => c.Request.Cookies)
                       .Returns(() => new HttpCookieCollection() { 
                           new HttpCookie(ReviewRApiController.CookieName, "COOK%20IE!?") 
                       });
                tokens.Setup(t => t.UnprotectToken("COOK IE!?", ReviewRApiController.Purpose))
                      .Throws<NotSupportedException>();

                // Act
                Assert.Null(utils.DecodeCookieToJson(context.Object));
            }

            [Fact]
            public void ReturnsNullIfCookieIsInvalid()
            {
                // Arrange
                var context = new Mock<HttpContextBase>();
                var tokens = new Mock<TokenService>();
                var utils = new Utils.UtilWorker(tokens.Object);
                context.Setup(c => c.Request.Cookies)
                       .Returns(() => new HttpCookieCollection() { 
                           new HttpCookie(ReviewRApiController.CookieName, "COOK%20IE!?") 
                       });
                tokens.Setup(t => t.UnprotectToken("COOK IE!?", ReviewRApiController.Purpose))
                      .Throws<InvalidDataException>();

                // Act
                Assert.Null(utils.DecodeCookieToJson(context.Object));
            }

            [Fact]
            public void ReturnsJsonOfTokenIfCookieValid()
            {
                // Arrange
                var context = new Mock<HttpContextBase>();
                var tokens = new Mock<TokenService>();
                var utils = new Utils.UtilWorker(tokens.Object);
                context.Setup(c => c.Request.Cookies)
                       .Returns(() => new HttpCookieCollection() { 
                           new HttpCookie(ReviewRApiController.CookieName, "COOK%20IE!?") 
                       });
                tokens.Setup(t => t.UnprotectToken("COOK IE!?", ReviewRApiController.Purpose))
                      .Returns(
                        new SessionToken(
                            new ReviewRPrincipal(
                                new ReviewRIdentity() {
                                    Email = "bork@bork.bork",
                                    DisplayName = "Swedish Chef"
                                }), DateTime.UtcNow));

                // Act
                Assert.Equal(
                    "{\"userId\":0,\"displayName\":\"Swedish Chef\",\"email\":\"bork@bork.bork\",\"emailHash\":\"67614c787f39a22a54b7dcd169a01477\",\"roles\":null}", 
                    utils.DecodeCookieToJson(context.Object));
            }
        }
    }
}
