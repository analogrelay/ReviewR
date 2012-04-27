using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Models;

namespace ReviewR.Web.Services
{
    public static class Utils
    {
        private static TokenService _tokens;

        public static TokenService Tokens
        {
            get { return _tokens ?? (_tokens = ReviewRApplication.Services.GetService(typeof(TokenService)) as TokenService); }
            set { _tokens = value; }
        }

        public static string GetGravatarHash(string email)
        {
            MD5 m = new MD5Cng();
            return BitConverter.ToString(
                m.ComputeHash(Encoding.UTF8.GetBytes(email.Trim().ToLower())))
                .Replace("-", "")
                .ToLower();
        }

        public static string DecodeCookieToJson(HttpContextBase context)
        {
            HttpCookie cookie = context.Request.Cookies[ReviewRApiController.CookieName];
            if (cookie != null)
            {
                SessionToken token;
                try
                {
                    token = Tokens.UnprotectToken(HttpUtility.UrlDecode(cookie.Value), ReviewRApiController.Purpose);
                }
                catch (NotSupportedException)
                {
                    return null;
                }
                return JsonConvert.SerializeObject(token.User.Identity, new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            }
            return null;
        }
    }
}