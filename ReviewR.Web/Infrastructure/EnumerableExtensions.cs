using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReviewR.Web.Infrastructure
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<Tuple<T, T>> Pairwise<T>(this IEnumerable<T> self)
        {
            // Use a bool to indicate nullity so we can use value types or reference types
            bool haveLast = false;
            T last = default(T);
            foreach (T cur in self)
            {
                if (haveLast)
                {
                    haveLast = false;
                    yield return Tuple.Create(last, cur);
                }
                else
                {
                    last = cur;
                    haveLast = true;
                }
            }
            if (haveLast)
            {
                yield return Tuple.Create(last, default(T));
            }
        }
    }
}