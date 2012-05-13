using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Models;

namespace ReviewR.Web.Services.Authenticators
{
    public abstract class Authenticator
    {
        public virtual bool Active { get { return true; } }
        public abstract string Name { get; }
        public abstract string Id { get; }
        public abstract string GetAppId(ISettings appSettings);
        public abstract string GetSecret(ISettings settings);
        public abstract Task<UserInfo> CompleteAuthentication(string accessToken);
    }
}