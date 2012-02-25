using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace ReviewR.Web.Services
{
    public class AuthTokenService
    {
        public HttpContextBase Context { get; private set; }

        protected AuthTokenService() { }

        public AuthTokenService(HttpContextBase context)
        {
            Context = context;
        }

        public virtual void SetAuthCookie(string userName, bool createPersistentCookie, IEnumerable<string> roles)
        {
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                version: 1,
                name: userName,
                issueDate: DateTime.UtcNow,
                expiration: DateTime.UtcNow.AddMinutes(30),
                isPersistent: createPersistentCookie,
                userData: String.Join("|", roles.ToArray()));
            string encryptedTicket = FormsAuthentication.Encrypt(ticket);
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            Context.Response.Cookies.Add(cookie);
        }

        public virtual void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
}