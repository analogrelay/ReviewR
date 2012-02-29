using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace ReviewR.Web.Infrastructure
{
    public class ReviewRPrincipal : IPrincipal
    {
        private ISet<string> _roles;

        public AuthTicket Ticket { get; private set; }
        public IIdentity Identity { get; private set; }

        public ReviewRPrincipal(IIdentity identity, AuthTicket ticket)
        {
            Identity = identity;
            Ticket = ticket;
            _roles = new HashSet<string>(ticket.Roles);
        }

        public bool IsInRole(string role)
        {
            return _roles.Contains(role);
        }
    }

    public static class ReviewRPrincipalMixin
    {
        public static ReviewRPrincipal CurrentUser(this HttpContextBase self)
        {
            return self.User.AsReviewRPrincipal();
        }

        public static ReviewRPrincipal AsReviewRPrincipal(this IPrincipal p)
        {
            return p as ReviewRPrincipal;
        }

        public static string DisplayName(this IPrincipal p)
        {
            return p.ReviewRProp(r => r.Ticket.DisplayName);
        }

        public static string Email(this IPrincipal p)
        {
            return p.ReviewRProp(r => r.Ticket.Email);
        }

        private static T ReviewRProp<T>(this IPrincipal p, Func<ReviewRPrincipal, T> prop)
        {
            var principal = p.AsReviewRPrincipal();
            if (principal == null)
            {
                throw new SecurityException("Expected that a user was currently logged in");
            }
            return prop(principal);
        }
    }
}
