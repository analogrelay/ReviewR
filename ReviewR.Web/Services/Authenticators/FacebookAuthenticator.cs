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

namespace ReviewR.Web.Services.Authenticators
{
    public class FacebookAuthenticator : Authenticator
    {
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

        public override string GetAppId(ISettings appSettings)
        {
            return appSettings.Get("facebook:appid");
        }

        protected override UserInfo ParseResponse(string jsonResponse)
        {
            dynamic response = JObject.Parse(jsonResponse);
            return new UserInfo(
                Name,
                (string)response.link,
                (string)response.name,
                (string)response.email);
        }
    }
}