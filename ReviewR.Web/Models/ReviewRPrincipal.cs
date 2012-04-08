using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Microsoft.Internal.Web.Utils;
using ReviewR.Web.Models.Data;
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

    public class ReviewRIdentity : IIdentity
    {
        private string _hash;
        private string _emailWhenHashGenerated;

        public int UserId { get; set; }
        public bool RememberMe { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public HashSet<string> Roles { get; set; }
        public string EmailHash
        {
            get
            {
                if (!String.Equals(_emailWhenHashGenerated, Email, StringComparison.Ordinal))
                {
                    _hash = Utils.GetGravatarHash(Email);
                    _emailWhenHashGenerated = Email;
                }
                return _hash;
            }
        }

        public static ReviewRIdentity FromUser(User u)
        {
            return new ReviewRIdentity()
            {
                UserId = u.Id,
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
                   UserId == other.UserId &&
                   RememberMe == other.RememberMe &&
                   String.Equals(DisplayName, other.DisplayName, StringComparison.Ordinal) &&
                   String.Equals(Email, other.Email, StringComparison.Ordinal) &&
                   Roles.SequenceEqual(other.Roles, StringComparer.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return HashCodeCombiner.Start()
                                   .Add(UserId)
                                   .Add(RememberMe)
                                   .Add(DisplayName)
                                   .Add(Email)
                                   .Add(Roles)
                                   .CombinedHash;
        }

        public override string ToString()
        {
            return String.Format("{{UserId = {0}, DisplayName = {1}, Email = {2}, RememberMe = {3}, Roles = {4}}}",
                UserId,
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