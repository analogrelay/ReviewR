using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Web.Models
{
    public class UserInfo
    {
        public string Provider { get; set; }
        public string Value { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }

        public UserInfo(string provider, string value, string displayName, string email)
        {
            Provider = provider;
            Value = value;
            DisplayName = displayName;
            Email = email;
        }
    }
}
