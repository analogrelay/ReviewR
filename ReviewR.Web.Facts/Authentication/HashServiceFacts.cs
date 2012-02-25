using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviewR.Web.Services;
using VibrantUtils;
using Xunit;

namespace ReviewR.Web.Facts.Authentication
{
    public class HashServiceFacts
    {
        public class GenerateHash
        {
            [Fact]
            public void RequiresNonNullOrEmptyValue()
            {
                ContractAssert.NotNullOrEmpty(s => new HashService().GenerateHash(s, "salt"), "value");
            }

            [Fact]
            public void RequiresNonNullOrEmptySalt()
            {
                ContractAssert.NotNullOrEmpty(s => new HashService().GenerateHash("value", s), "salt");
            }

            [Fact]
            public void GeneratesHMACSHA256HashOfInputsAsBase64()
            {
                // Pre-calculated
                const string expected = "syG5MRKK0ixBtoIFGOLvu3gmhwC0TAJ/0wWQ6V1iAHQ=";

                // Arrange
                var hasher = new HashService();
                var salt = Convert.ToBase64String(new byte[] { 1, 2, 3, 4, 5 });
                var value = "password";

                // Act
                var actual = hasher.GenerateHash(value, salt);

                // Assert
                Assert.Equal(expected, actual);
            }
        }

        public class GenerateSalt
        {
            [Fact]
            public void ReturnsRandomSaltString()
            {
                // Arrange
                var hasher = new HashService();

                // Act
                var salt1 = hasher.GenerateSalt();
                var salt2 = hasher.GenerateSalt();

                // Assert
                Assert.NotEmpty(salt1);
                Assert.NotEmpty(salt2);
                Assert.NotEqual(salt1, salt2);
            }
        }
    }
}
