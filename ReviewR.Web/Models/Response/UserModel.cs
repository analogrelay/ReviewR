using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Internal.Web.Utils;
using ReviewR.Web.Models.Data;
using ReviewR.Web.Services;

namespace ReviewR.Web.Models.Response
{
    public class UserModel
    {
        private string _hash;
        private string _emailWhenHashGenerated;

        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
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

        public static UserModel FromUser(User u)
        {
            return new UserModel()
            {
                Id = u.Id,
                DisplayName = u.DisplayName,
                Email = u.Email
            };
        }

        public override bool Equals(object obj)
        {
            UserModel other = obj as UserModel;
            return other != null &&
                   Id == other.Id &&
                   String.Equals(DisplayName, other.DisplayName, StringComparison.Ordinal) &&
                   String.Equals(Email, other.Email, StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            return HashCodeCombiner.Start()
                                   .Add(Id)
                                   .Add(DisplayName)
                                   .Add(Email)
                                   .CombinedHash;
        }

        public override string ToString()
        {
            return String.Format("{{UserId = {0}, DisplayName = {1}, Email = {2}}}",
                Id,
                DisplayName,
                Email);
        }
    }
}
