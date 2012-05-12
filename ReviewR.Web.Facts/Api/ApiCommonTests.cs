using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using ReviewR.Web.Infrastructure;
using Xunit;

namespace ReviewR.Web.Facts.Api
{
    public static class ApiCommonTests
    {
        public static void GetReturns404WhenNoObjectWithId<T>(ITestableController c) where T : class, new()
        {
            GetReturns404WhenNoObjectWithId(c, new T());
        }

        public static void GetReturns404WhenNoObjectWithId<T>(ITestableController c, T item) where T : class
        {
            // Arrange
            var id = AddToEntitySet(c.MockData, item);

            // Act
            var result = ((dynamic)c).Get(id + 42);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
            Assert.Null(result.Content);
        }

        private static int AddToEntitySet<T>(MockDataRepository mockDataRepository, T item) where T : class
        {
            var setType = typeof(IEntitySet<>).MakeGenericType(typeof(T));
            PropertyInfo prop = mockDataRepository.GetType()
                                                  .GetProperties()
                                                  .Where(p => setType.IsAssignableFrom(p.PropertyType))
                                                  .SingleOrDefault();
            Assert.NotNull(prop);
            IEntitySet<T> set = prop.GetValue(mockDataRepository, new object[0]) as IEntitySet<T>;
            Assert.NotNull(set);
            set.Add(item);
            mockDataRepository.SaveChanges();

            PropertyInfo idProp = typeof(T).GetProperties()
                                           .Where(p => p.Name == "Id" && p.PropertyType == typeof(int))
                                           .SingleOrDefault();
            Assert.NotNull(idProp);
            return (int)idProp.GetValue(item, new object[0]);
        }
    }
}
