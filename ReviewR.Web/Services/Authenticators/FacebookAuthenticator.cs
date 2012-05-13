using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
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

        public override string GetAppId(ISettings appSettings)
        {
            return appSettings.Get("facebook:appid");
        }

        public override string GetSecret(ISettings settings)
        {
            throw new NotImplementedException();
        }

        public override Task<UserInfo> CompleteAuthentication(string accessToken)
        {
            throw new NotImplementedException();
        }
    }
}