using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using ReviewR.Web.Infrastructure;

namespace ReviewR.Web.Services
{
    public class AuthTokenService
    {
        private JavaScriptSerializer _serializer = new JavaScriptSerializer();

        public HttpContextBase Context { get; private set; }

        protected AuthTokenService() { }

        public AuthTokenService(HttpContextBase context)
        {
            Context = context;
        }

        public virtual void SetAuthCookie(string userName, bool createPersistentCookie, AuthTicket authTicket)
        {
            string authTicketStr = _serializer.Serialize(authTicket);

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                version: 1,
                name: userName,
                issueDate: DateTime.UtcNow,
                expiration: DateTime.UtcNow.AddMinutes(30),
                isPersistent: createPersistentCookie,
                userData: authTicketStr);
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