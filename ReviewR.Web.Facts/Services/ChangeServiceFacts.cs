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
    public class ChangeServiceFacts
    {
        public class Ctor
        {
            [Fact]
            public void InitializesValues()
            {
                // Arrange
                var data = new MockDataRepository();
                
                // Act
                ChangeService auth = new ChangeService(data);

                // Assert
                Assert.Same(data, auth.Data);
            }

            [Fact]
            public void RequiresNonNullArguments()
            {
                ContractAssert.NotNull(() => new ChangeService(null), "data");
            }
        }

        public class GetChange
        {
            [Fact]
            public void RequiresNonNegativeId()
            {
                ContractAssert.OutOfRange(() => CreateService().GetChange(-1), "id");
            }

            [Fact]
            public void ReturnsNullIfNoSuchChange()
            {
                // Arrange
                var changes = CreateService();
                var added = new FileChange() { FileName = "florb" };
                changes.MockData.Changes.Add(added);
                changes.MockData.SaveChanges();

                // Act
                FileChange fetched = changes.GetChange(added.Id + 42);

                // Assert
                Assert.Null(fetched);
            }

            [Fact]
            public void ReturnsChangeWithSpecifiedIdIfPresent()
            {
                // Arrange
                var changes = CreateService();
                var added = new FileChange() { FileName = "florb" };
                changes.MockData.Changes.Add(added);
                changes.MockData.SaveChanges();

                // Act
                FileChange fetched = changes.GetChange(added.Id);

                // Assert
                Assert.Equal(added, fetched, new PropertyEqualityComparer());
            }
        }

        private static TestableChangeService CreateService()
        {
            return new TestableChangeService();
        }

        private class TestableChangeService : ChangeService
        {
            public MockDataRepository MockData { get; set; }
            
            public TestableChangeService()
            {
                Data = MockData = new MockDataRepository();
            }
        }
    }
}
