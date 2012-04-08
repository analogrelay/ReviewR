using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Microsoft.Internal.Web.Utils;
using ReviewR.Web.Models.Data;
using ReviewR.Web.Models.Response;
using ReviewR.Web.Services;

namespace ReviewR.Web.Models
{
    public class ReviewRPrincipal : IPrincipal
    {
        public ReviewRIdentity Identity { get; private set; }
        IIdentity IPrincipal.Identity { get { return Identity; } }

        public ReviewRPrincipal(User u) : this(ReviewRIdentity.FromUser(u)) { }
        public ReviewRPrincipal(ReviewRIdentity id)
        {
            Identity = id;
        }

        public bool IsInRole(string role)
        {
            return Identity.Roles.Contains(role);
        }
    }

    public class ReviewRIdentity : UserModel, IIdentity
    {
        public bool RememberMe { get; set; }
        public HashSet<string> Roles { get; set; }
        
        public static ReviewRIdentity FromUser(User u)
        {
            return new ReviewRIdentity()
            {
                Id = u.Id,
                DisplayName = u.DisplayName,
                Email = u.Email,
                Roles = u.Roles == null ?
                    new HashSet<string>() :
                    new HashSet<string>(u.Roles.Select(r => r.RoleName))
            };
        }

        public override bool Equals(object obj)
        {
            ReviewRIdentity other = obj as ReviewRIdentity;
            return other != null &&
                   base.Equals(other) &&
                   RememberMe == other.RememberMe &&
                   Roles.SequenceEqual(other.Roles, StringComparer.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return HashCodeCombiner.Start()
                                   .Add(base.GetHashCode())
                                   .Add(RememberMe)
                                   .Add(Roles)
                                   .CombinedHash;
        }

        public override string ToString()
        {
            return String.Format("{{UserId = {0}, DisplayName = {1}, Email = {2}, RememberMe = {3}, Roles = {4}}}",
                Id,
                DisplayName,
                Email,
                RememberMe,
                String.Join(",", Roles));
        }

        string IIdentity.AuthenticationType
        {
            get { return "ReviewR"; }
        }

        bool IIdentity.IsAuthenticated
        {
            get { return true; }
        }

        string IIdentity.Name
        {
            get { return Email; }
        }
    }
}