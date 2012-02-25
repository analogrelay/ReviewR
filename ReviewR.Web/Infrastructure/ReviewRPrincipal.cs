using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace ReviewR.Web.Infrastructure
{
    public class ReviewRPrincipal : IPrincipal
    {
        public ISet<string> Roles { get; private set; }
        public IIdentity Identity { get; private set; }

        public ReviewRPrincipal(IIdentity identity, ISet<string> roles)
        {
            Identity = identity;
            Roles = roles;
        }

        public bool IsInRole(string role)
        {
            return Roles.Contains(role);
        }
    }
}
