using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using Newtonsoft.Json;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Models;

namespace ReviewR.Web.Services
{
    public class TokenService
    {
        public TimeSpan Timeout { get; set; }
        public string CookieName { get; set; }

        public TokenService()
        {
            Timeout = TimeSpan.FromMinutes(30);
            CookieName = "AuthToken";
        }

        public virtual CookieHeaderValue CreateAuthCookie(ReviewRIdentity authTicket)
        {
            string authTicketStr = JsonConvert.SerializeObject(authTicket);
            string data = MachineKey.Encode(Encoding.UTF8.GetBytes(authTicketStr), MachineKeyProtection.All);

            var cookie = new CookieHeaderValue(CookieName, data)
            {
                HttpOnly = true,
                Path = "/"
            };
            if (authTicket.RememberMe)
            {
                cookie.Expires = DateTimeOffset.UtcNow.Add(Timeout);
            }
            return cookie;
        }

        public virtual ReviewRIdentity ReadAuthCookie(CookieHeaderValue currentCookie)
        {
            string encoded = currentCookie.Cookies.Single().Value;
            string data = Encoding.UTF8.GetString(MachineKey.Decode(encoded, MachineKeyProtection.All));
            return JsonConvert.DeserializeObject<ReviewRIdentity>(data);
        }

        public virtual CookieHeaderValue CreateSignoutCookie()
        {
            return new CookieHeaderValue(CookieName, String.Empty)
            {
                HttpOnly = true,
                Path = "/",
                Expires = new DateTimeOffset(new DateTime(1999, 10, 12))
            };
        }
    }
}