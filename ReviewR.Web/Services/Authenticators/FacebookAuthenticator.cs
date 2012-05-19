using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Models;
using VibrantUtils;

namespace ReviewR.Web.Services.Authenticators
{
    public class FacebookAuthenticator : Authenticator
    {
        internal const string AppIdKey = "facebook:appid";

        public override string Name
        {
            get { return "Facebook"; }
        }

        public override string Id
        {
            get { return "fb"; }
        }

        protected internal override string FetchUserInfoBaseUrl
        {
            get
            {
                return "https://graph.facebook.com/me";
            }
        }

        public override string DialogUrlFormat
        {
            get { 
                return "https://www.facebook.com/dialog/oauth?" +
                    "client_id={0}&" +
                    "redirect_uri={1}&" +
                    "display=popup&" +
                    "scope=user_about_me,email&" +
                    "response_type=token";
            }
        }

        public override string GetAppId(ISettings appSettings)
        {
            Requires.NotNull(appSettings, "appSettings");

            return appSettings.Get(AppIdKey);
        }

        protected internal override UserInfo ParseResponse(string jsonResponse)
        {
            Requires.NotNullOrEmpty(jsonResponse, "jsonResponse");

            dynamic response = JObject.Parse(jsonResponse);
            return new UserInfo(
                DisplayName,
                (string)response.link,
                (string)response.name,
                (string)response.email);
        }
    }
}