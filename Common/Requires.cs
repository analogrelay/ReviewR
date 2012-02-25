using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace VibrantUtils
{
    internal static class Requires
    {
        public static void NotNullOrEmpty(string value, string paramName)
        {
            if (String.IsNullOrEmpty(value)) { 
                throw new ArgumentException(
                    String.Format(
                        CultureInfo.CurrentCulture, 
                        CommonResources.Argument_NotNullOrEmpty, 
                        paramName),
                    paramName); 
            }
        }
    }
}