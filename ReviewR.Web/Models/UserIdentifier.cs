using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Web.Models
{
    public class UserInfo
    {
        public string Provider { get; set; }
        public string Identifier { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }

        public UserInfo(string provider, string identifier, string displayName, string email)
        {
            Provider = provider;
            Identifier = identifier;
            DisplayName = displayName;
            Email = email;
        }
    }
}
