﻿using System;
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

        public static void NotNull(object value, string paramName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        public static void InRange(bool condition, string paramName)
        {
            if (!condition)
            {
                throw new ArgumentOutOfRangeException(paramName);
            }
        }
    }
}