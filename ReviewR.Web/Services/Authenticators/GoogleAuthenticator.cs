using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Models;

namespace ReviewR.Web.Services.Authenticators
{
    public class GoogleAuthenticator : Authenticator
    {
        public override bool Active
        {
            get
            {
                return true;
            }
        }

        public override string Name
        {
            get { return "Google"; }
        }

        public override string Id
        {
            get { return "goog"; }
        }

        public override string DialogUrlFormat
        {
            get
            {
                return "https://accounts.google.com/o/oauth2/auth?" +
                    "response_type=token&" +
                    "client_id={0}&" +
                    "redirect_uri={1}&" +
                    "scope=" + HttpUtility.UrlEncode("https://www.googleapis.com/auth/userinfo.profile https://www.googleapis.com/auth/userinfo.email");
            }
        }

        protected internal override string FetchUserInfoBaseUrl
        {
            get
            {
                return "https://www.googleapis.com/oauth2/v1/userinfo";
            }
        }

        public override string GetAppId(ISettings appSettings)
        {
            return appSettings.Get("google:appid");
        }

        protected internal override UserInfo ParseResponse(string jsonResponse)
        {
            dynamic response = JObject.Parse(jsonResponse);
            string email = response.email;
            if(response.verified_email != true) {
                email = null;
            }
            return new UserInfo(
                provider: Name,
                identifier: "https://www.google.com/profiles/" + response.id,
                displayName: (string)response.name,
                email: email
           );
        }

        public override Task<string> VerifyToken(string appId, string accessToken)
        {
            HttpClient client = CreateHttpClient();
            return client.GetAsync("https://www.googleapis.com/oauth2/v1/tokeninfo?access_token=" + accessToken).Then(resp =>
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsStringAsync();
            }).Then(content =>
            {
                dynamic response = JObject.Parse(content);
                if (response.error == "invalid_token" ||
                    response.audience != appId)
                {
                    return (string)null;
                }
                else
                {
                    return accessToken;
                }
            });
        }
    }
}