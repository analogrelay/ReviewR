using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ReviewR.Web.Services
{
    internal static class Utils
    {
        public static string GetGravatarHash(string email)
        {
            MD5 m = new MD5Cng();
            return BitConverter.ToString(
                m.ComputeHash(Encoding.UTF8.GetBytes(email.Trim().ToLower())))
                .Replace("-", "")
                .ToLower();
        }
    }
}