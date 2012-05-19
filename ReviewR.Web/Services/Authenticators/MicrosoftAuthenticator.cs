using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Models;

namespace ReviewR.Web.Services.Authenticators
{
    public class MicrosoftAuthenticator : Authenticator
    {
        public override bool Active
        {
            get
            {
                return true;
            }
        }

        public override string DisplayName
        {
            get { return "Microsoft Account"; }
        }

        public override string Name
        {
            get { return "Microsoft"; }
        }

        public override string Id
        {
            get { return "ms"; }
        }

        protected internal override string FetchUserInfoBaseUrl
        {
            get
            {
                return "https://apis.live.net/v5.0/me";
            }
        }

        public override string DialogUrlFormat
        {
            get
            {
                return "https://login.live.com/oauth20_authorize.srf?" +
                    "client_id={0}&" +
                    "scope=wl.basic wl.emails wl.signin&" +
                    "response_type=token&" +
                    "redirect_uri={1}" +
                    "&popupui=1";
            }
        }

        public override string GetAppId(ISettings appSettings)
        {
            return appSettings.Get("ms:appid");
        }

        protected internal override UserInfo ParseResponse(string jsonResponse)
        {
            dynamic response = JObject.Parse(jsonResponse);
            string first = response.first_name;
            string last = response.last_name;
            string email = response.emails.preferred;
            string id = response.link;
            return new UserInfo(
                provider: Name,
                identifier: id,
                displayName: first + " " + last,
                email: email);
        }
    }
}