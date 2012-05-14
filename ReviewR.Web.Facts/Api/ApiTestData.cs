using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReviewR.Web.Models;
using ReviewR.Web.Models.Data;

namespace ReviewR.Web.Facts.Api
{
    public static class ApiTestData
    {
        public static User LoggedInUser = CreateLoggedInUser();

        public static User NotLoggedInUser = new User()
        {
            Id = 24,
            DisplayName = "Beeker",
            Email = "meep@meep.meep",
            Roles = new List<Role>(),
            Reviews = new List<Review>(),
            Credentials = new List<Credential>()
        };

        public static ReviewRPrincipal LoggedIn = new ReviewRPrincipal(ReviewRIdentity.FromUser(LoggedInUser));
        public static ReviewRPrincipal NotLoggedIn = new ReviewRPrincipal(ReviewRIdentity.FromUser(NotLoggedInUser));

        public static User CreateLoggedInUser()
        {
            return new User()
            {
                Id = 42,
                DisplayName = "Swedish Chef",
                Email = "bork@bork.bork",
                Roles = new List<Role>(),
                Reviews = new List<Review>(),
                Credentials = new List<Credential>()
            };
        }
    }
}
