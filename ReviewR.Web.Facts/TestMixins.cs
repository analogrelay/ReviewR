using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.ModelBinding;
using ReviewR.Web.Models.Data;
using Xunit;

namespace ReviewR.Web.Facts
{
    public static class TestMixins
    {
        public static IEnumerable<string> AllErrors(this ModelStateDictionary self)
        {
            return self.Values.SelectMany(m => m.Errors.Select(e => e.ErrorMessage));
        }

        public static object GetObjectContent(this HttpResponseMessage self)
        {
            ObjectContent c = self.Content as ObjectContent;
            return c == null ? null : c.ReadAsync().Result;
        }

        public static int GetLastId(this IDataRepository self)
        {
            MockDataRepository d = self as MockDataRepository;
            Assert.NotNull(d);
            return d.LastId;
        }

        public static Task ShouldThrow<T>(this Task self) where T : Exception
        {
            return ShouldThrow<T>(self, _ => { });
        }
        public static Task ShouldThrow<T>(this Task self, Action<T> checker) where T : Exception
        {
            return self.ContinueWith(t =>
            {
                Assert.True(t.IsFaulted);
                AggregateException aggex = Assert.IsType<AggregateException>(t.Exception);
                var ex = Assert.IsType<T>(aggex.Flatten().InnerException);
                checker(ex);
            });
        }
    }
}
