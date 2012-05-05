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
        public bool TypeEquality { get; set; }

        public PropertyEqualityComparer() : this(typeEquality: true) { }
        public PropertyEqualityComparer(bool typeEquality)
        {
            TypeEquality = typeEquality;
        }

        public new bool Equals(object x, object y)
        {
            if (x is string && y is string)
            {
                return String.Equals((string)x, (string)y, StringComparison.Ordinal);
            }
            if (x is IEnumerable && y is IEnumerable)
            {
                return Enumerable.SequenceEqual(((IEnumerable)x).OfType<object>(), ((IEnumerable)y).OfType<object>(), new PropertyEqualityComparer());
            }
            if (x is ValueType && y is ValueType)
            {
                return Object.Equals(x, y);
            }
            return (x == null && y == null) || (x != null && y != null && (!TypeEquality || x.GetType().IsAssignableFrom(y.GetType())) && (Object.Equals(x, y) || MemberwiseEqual(x, y)));
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
                 .Select(p => Tuple.Create(p.Name, p.GetValue(x, new object[0])));
            var ys = y.GetType()
                 .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                 .OrderBy(p => p.Name)
                 .Select(p => Tuple.Create(p.Name, p.GetValue(y, new object[0])));

            var xnames = xs.Select(t => t.Item1);
            var ynames = ys.Select(t => t.Item1);

            bool equal = Enumerable.SequenceEqual(xnames, ynames) &&
                Enumerable.SequenceEqual(xs.Select(t => t.Item2), ys.Select(t => t.Item2), new PropertyEqualityComparer());
            return equal;
        }
    }
}
