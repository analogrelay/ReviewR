using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Http.Routing;

namespace ReviewR.Web.Infrastructure
{
    public static class UrlExtensions
    {
        private static MethodInfo TupleCreateMethod;
        private static Dictionary<Type, Func<object, Tuple<string, int>>> _routeDataExtractors = new Dictionary<Type, Func<object, Tuple<string, int>>>();

        static UrlExtensions()
        {
            TupleCreateMethod = typeof(Tuple).GetMethods()
                                             .Where(m => m.Name.Equals("Create") && m.IsGenericMethod && m.GetGenericArguments().Length == 2)
                                             .Select(m => m.MakeGenericMethod(typeof(string), typeof(int)))
                                             .Single();
        }

        public static string Resource(this UrlHelper helper, object o)
        {
            Func<object, Tuple<string, int>> extractor;
            if (!_routeDataExtractors.TryGetValue(o.GetType(), out extractor))
            {
                extractor = _routeDataExtractors[o.GetType()] = CreateExtractor(o.GetType());
            }
            var tup = extractor(o);
            return helper.Route("DefaultApi", new
            {
                controller = Pluralizer.ToPlural(tup.Item1).ToLower(),
                id = tup.Item2
            });
        }

        private static Func<object, Tuple<string, int>> CreateExtractor(Type type)
        {
            string name = type.Name;
            PropertyInfo idProp = type.GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
            ParameterExpression input = Expression.Parameter(typeof(object));
            return Expression.Lambda<Func<object, Tuple<string, int>>>(
                Expression.Call(TupleCreateMethod,
                    Expression.Constant(name),
                    Expression.Property(Expression.Convert(input, type), idProp)),
                input).Compile();
        }
    }
}