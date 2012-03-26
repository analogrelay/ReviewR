using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ReviewR.Web.Models;

namespace ReviewR.Web.Infrastructure
{
    public class AuthTicket
    {
        public int UserId { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Roles { get; set; }

        public static AuthTicket FromUser(User u)
        {
            return new AuthTicket()
            {
                UserId = u.Id,
                DisplayName = u.DisplayName,
                Email = u.Email,
                Roles = u.Roles == null ? new string[0] : u.Roles.Select(r => r.RoleName).ToArray()
            };
        }
    }
}