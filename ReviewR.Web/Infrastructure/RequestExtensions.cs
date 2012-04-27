using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using ReviewR.Web.Models;

namespace ReviewR.Web.Infrastructure
{
    public static class RequestExtensions
    {
        public static string GetAuthHeader(this HttpRequestHeaders self)
        {
            var authHeader = self.Authorization;
            if (authHeader != null && String.Equals(authHeader.Scheme, "Basic", StringComparison.OrdinalIgnoreCase))
            {
                return authHeader.Parameter;
            }
            return null;
        }

        public static string GetAuthCookie(this HttpRequestHeaders self)
        {
            return self.GetCookies()
                       .Select(cs => cs.Cookies.Where(c => String.Equals(c.Name, ReviewRApiController.CookieName)).SingleOrDefault())
                       .Where(c => c != null)
                       .Select(c => c.Value)
                       .SingleOrDefault();
        }

        public static void SetAuthCookie(this HttpResponseHeaders self, string token, string path, DateTimeOffset expires)
        {
            CookieHeaderValue cookie = new CookieHeaderValue(ReviewRApiController.CookieName, token)
            {
                Expires = expires,
                HttpOnly = true,
                Path = path
            };
            self.AddCookies(new[] { cookie });
        }

        public static void ClearCookie(this HttpResponseHeaders self, string name, string path)
        {
            CookieHeaderValue cookie = new CookieHeaderValue(name, String.Empty)
            {
                Expires = DateTimeOffset.MinValue,
                Path = path,
                HttpOnly = true
            };
            self.AddCookies(new[] { cookie });
        }
    }
}