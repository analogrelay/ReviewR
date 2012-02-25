using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReviewR.Web.Services;

namespace ReviewR.Web.Facts.Controllers
{
    public class TestAuthTokenService : AuthTokenService
    {
        public string UserName { get; set; }
        public bool Persistent { get; set; }
        public IEnumerable<string> Roles { get; set; }

        public override void SetAuthCookie(string userName, bool createPersistentCookie, IEnumerable<string> roles)
        {
            UserName = userName;
            Persistent = createPersistentCookie;
            Roles = roles;
        }
    }
}
