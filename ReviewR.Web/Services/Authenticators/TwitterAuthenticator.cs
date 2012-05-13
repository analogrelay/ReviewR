using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Models;

namespace ReviewR.Web.Services.Authenticators
{
    public class TwitterAuthenticator : Authenticator
    {
        public override bool Active
        {
            get
            {
                return false;
            }
        }

        public override string Name
        {
            get { return "Twitter"; }
        }

        public override string Id
        {
            get { return "twt"; }
        }

        public override string GetAppId(ISettings appSettings)
        {
            return appSettings.Get("google:appid");
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