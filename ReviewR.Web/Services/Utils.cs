using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Models;
using VibrantUtils;

namespace ReviewR.Web.Services
{
    public static class Utils
    {
        // The pattern used here allows tests to create their own UtilWorkers to allow for parallel test execution while
        // still gaining the benefit of a static.

        private static UtilWorker _worker = new UtilWorker();

        public static string GetGravatarHash(string email) { return _worker.GetGravatarHash(email); }
        public static SessionToken GetActiveSessionToken(HttpContextBase context) { return _worker.GetActiveSessionToken(context); }
        public static string DecodeCookieToJson(HttpContextBase context) { return _worker.DecodeCookieToJson(context); }

        internal class UtilWorker
        {
            private TokenService _tokens;

            private TokenService Tokens
            {
                get { return _tokens ?? (_tokens = ReviewRApplication.Services.GetService(typeof(TokenService)) as TokenService); }
                set { _tokens = value; }
            }

            public UtilWorker()
            {
            }

            public UtilWorker(TokenService tokens)
            {
                Tokens = tokens;
            }

            public string GetGravatarHash(string email)
            {
                Requires.NotNullOrEmpty(email, "email");

                MD5 m = new MD5Cng();
                return BitConverter.ToString(
                    m.ComputeHash(Encoding.UTF8.GetBytes(email.Trim().ToLower())))
                    .Replace("-", "")
                    .ToLower();
            }

            public SessionToken GetActiveSessionToken(HttpContextBase context)
            {
                Requires.NotNull(context, "context");

                HttpCookie cookie = context.Request.Cookies[ReviewRApiController.CookieName];
                SessionToken token = null;
                if (cookie != null)
                {
                    try
                    {
                        token = Tokens.UnprotectToken(HttpUtility.UrlDecode(cookie.Value), ReviewRApiController.Purpose);
                    }
                    catch (NotSupportedException) { return null; }
                    catch (InvalidDataException) { return null; }
                }
                return token;
            }

            public string DecodeCookieToJson(HttpContextBase context)
            {
                Requires.NotNull(context, "context");

                SessionToken token = GetActiveSessionToken(context);
                if (token != null)
                {
                    return JsonConvert.SerializeObject(token.User.Identity, new JsonSerializerSettings()
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    });
                }
                return null;
            }
        }
    }
}