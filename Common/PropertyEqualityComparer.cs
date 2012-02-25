using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace VibrantUtils
{
    internal class PropertyEqualityComparer : IEqualityComparer<object>
    {
        public new bool Equals(object x, object y)
        {
            // Strings are enumerable, shortcut them
            if (x is IEnumerable && y is IEnumerable)
            {
                return Enumerable.SequenceEqual(((IEnumerable)x).OfType<object>(), ((IEnumerable)y).OfType<object>(), new PropertyEqualityComparer());
            }
            return (x == null && y == null) || (x != null && y != null && (Object.Equals(x, y) || MemberwiseEqual(x, y)));
        }

        public int GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        private bool MemberwiseEqual(object x, object y)
        {
            var xs = x.GetType()
                 .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                 .OrderBy(p => p.Name)
                 .Select(p => Tuple.Create(p.Name, p.GetValue(x)));
            var ys = y.GetType()
                 .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                 .OrderBy(p => p.Name)
                 .Select(p => Tuple.Create(p.Name, p.GetValue(y)));

            var xnames = xs.Select(t => t.Item1);
            var ynames = ys.Select(t => t.Item1);

            return Enumerable.SequenceEqual(xnames, ynames) &&
                   Enumerable.SequenceEqual(xs.Select(t => t.Item2), ys.Select(t => t.Item2), new PropertyEqualityComparer());
        }
    }
}
