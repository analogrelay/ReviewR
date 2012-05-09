using System;
using System.Collections.Generic;
using System.IO;
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
using Xunit.Extensions;

namespace ReviewR.Web.Facts.Services
{
    public class TokenServiceFacts
    {
        public class UnprotectToken
        {
            [Fact]
            public void RequiresValidArguments()
            {
                ContractAssert.NotNullOrEmpty(s => CreateService().UnprotectToken(s, "porpoise?"), "token");
                ContractAssert.NotNullOrEmpty(s => CreateService().UnprotectToken("token", s), "purpose");
            }
        }

        public class ProtectToken
        {
            [Fact]
            public void RequiresValidArguments()
            {
                ContractAssert.NotNull(() => CreateService().ProtectToken(null, "porpoise?"), "token");
                ContractAssert.NotNullOrEmpty(s => CreateService().UnprotectToken("token", s), "purpose");
            }
        }

        // Difficult to test one method in isolation so we test the combination of them.
        // Still isolated to this 'unit' (TokenService) though so I don't really care ;)

        [Fact]
        public void CorrectlyUnprotectsProtectedTokenWithSamePurpose()
        {
            // Arrange
            var tokens = CreateService();
            var expires = DateTime.UtcNow;
            var token = new SessionToken(
                new ReviewRPrincipal(
                    new ReviewRIdentity()
                    {
                        Email = "bork@bork.bork",
                        DisplayName = "Swedish Chef",
                        Roles = new HashSet<string>()
                    }), expires);
            
            // Act
            string protectedToken = tokens.ProtectToken(token, "porpoise!");
            SessionToken unprotected = tokens.UnprotectToken(protectedToken, "porpoise!");

            // Assert
            Assert.Equal("Swedish Chef", unprotected.User.Identity.DisplayName);
            Assert.Equal("bork@bork.bork", unprotected.User.Identity.Email);
            Assert.Equal(expires, unprotected.Expires);
        }

        [Fact]
        public void FailsToUnprotectTokenIfPurposeIsDifferent()
        {
            // Arrange
            var tokens = CreateService();
            var expires = DateTime.UtcNow;
            var token = new SessionToken(
                new ReviewRPrincipal(
                    new ReviewRIdentity()
                    {
                        Email = "bork@bork.bork",
                        DisplayName = "Swedish Chef",
                        Roles = new HashSet<string>()
                    }), expires);
            
            // Act
            string protectedToken = tokens.ProtectToken(token, "porpoise!");
            Assert.Throws<InvalidDataException>(() => tokens.UnprotectToken(protectedToken, "notporpoise??!"));
        }

        [Fact]
        public void FailsToUnprotectTokenIfVersionIsDifferent()
        {
            // Arrange
            var tokens = CreateService();
            tokens.ProtectVersion = true;
            var expires = DateTime.UtcNow;
            var token = new SessionToken(
                new ReviewRPrincipal(
                    new ReviewRIdentity()
                    {
                        Email = "bork@bork.bork",
                        DisplayName = "Swedish Chef",
                        Roles = new HashSet<string>()
                    }), expires);
            
            // Act
            string protectedToken = tokens.ProtectToken(token, "porpoise!");
            Assert.Throws<NotSupportedException>(() => tokens.UnprotectToken(protectedToken, "notporpoise??!"));
        }

        private static TestableTokenService CreateService() {
            return new TestableTokenService();
        }

        private class TestableTokenService : TokenService
        {
            public bool ProtectVersion { get; set; }

            protected override string Protect(byte[] data, string purpose)
            {
                // High security, eh? ;)
                // Just to fiddle with the deserialization and our error handling, we'll leave the token version alone (first 4 bytes)
                if (ProtectVersion)
                {
                    return Convert.ToBase64String(data.Select(b => (byte)(((int)b + purpose.GetHashCode()) % 256)).ToArray());
                }
                else
                {
                    return Convert.ToBase64String(Enumerable.Concat(
                        data.Take(4),
                        data.Skip(4).Select(b => (byte)(((int)b + purpose.GetHashCode()) % 256))
                    ).ToArray());
                }
            }

            protected override byte[] Unprotect(string encoded, string purpose)
            {
                byte[] data = Convert.FromBase64String(encoded);
                if (ProtectVersion)
                {
                    return data.Select(b => (byte)(((int)b - purpose.GetHashCode()) % 256)).ToArray();
                }
                else
                {
                    return Enumerable.Concat(
                        data.Take(4),
                        data.Skip(4).Select(b => (byte)(((int)b - purpose.GetHashCode()) % 256))
                    ).ToArray();
                }
            }
        }
    }
}
