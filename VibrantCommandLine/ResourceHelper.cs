using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Globalization;

namespace VibrantCommandLine
{
    public static class ResourceHelper
    {
        private static Dictionary<Tuple<Type, string>, string> _cachedResourceStrings;

        public static string GetLocalizedString(Type resourceType, string resourceName)
        {
            if (String.IsNullOrEmpty(resourceName))
            {
                throw new ArgumentException(Resources.Argument_Cannot_Be_Null_Or_Empty, "resourceName");
            }

            if (resourceType == null)
            {
                throw new ArgumentNullException("resourceType");
            }

            if (_cachedResourceStrings == null)
            {
                _cachedResourceStrings = new Dictionary<Tuple<Type, string>, string>();
            }

            var key = Tuple.Create(resourceType, resourceName);
            string resourceValue;

            if (!_cachedResourceStrings.TryGetValue(key, out resourceValue))
            {
                PropertyInfo property = resourceType.GetProperty(resourceName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

                if (property == null)
                {
                    throw new InvalidOperationException(
                        String.Format(CultureInfo.CurrentCulture, Resources.ResourceTypeDoesNotHaveProperty, resourceType, resourceName));
                }

                if (property.PropertyType != typeof(string))
                {
                    throw new InvalidOperationException(
                        String.Format(CultureInfo.CurrentCulture, Resources.ResourcePropertyNotStringType, resourceName, resourceType));
                }

                MethodInfo getMethod = property.GetGetMethod(true);
                if ((getMethod == null) || (!getMethod.IsAssembly && !getMethod.IsPublic))
                {
                    throw new InvalidOperationException(
                        String.Format(CultureInfo.CurrentCulture, Resources.ResourcePropertyDoesNotHaveAccessibleGet, resourceType, resourceName));
                }
                resourceValue = (string)property.GetValue(null, null);
                _cachedResourceStrings[key] = resourceValue;
            }

            return resourceValue;
        }
    }
}
