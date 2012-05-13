using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Models;
using ReviewR.Web.Models.Data;
using ReviewR.Web.Services.Authenticators;
using VibrantUtils;

namespace ReviewR.Web.Services
{
    public class AuthenticationService
    {
        public ISettings Settings { get; set; }
        public IEnumerable<Authenticator> Authenticators { get; set; }

        protected AuthenticationService() { }

        public AuthenticationService(IEnumerable<Authenticator> authenticators, ISettings settings)
        {
            Requires.NotNull(authenticators, "authenticators");
            Requires.NotNull(settings, "settings");

            Authenticators = authenticators;
            Settings = settings;
        }

        public virtual IDictionary<string, string> GetClientIds()
        {
            return Authenticators.ToDictionary(a => a.Id, a => a.GetAppId(Settings));
        }
    }
}
