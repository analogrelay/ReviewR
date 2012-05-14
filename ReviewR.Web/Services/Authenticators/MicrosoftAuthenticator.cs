using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
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
                return false;
            }
        }

        public override string Name
        {
            get { return "Microsoft Account"; }
        }

        public override string Id
        {
            get { return "ms"; }
        }

        public override string GetAppId(ISettings appSettings)
        {
            return appSettings.Get("ms:appid");
        }

        public override Task<UserInfo> CompleteAuthentication(string accessToken)
        {
            throw new NotImplementedException();
        }
    }
}