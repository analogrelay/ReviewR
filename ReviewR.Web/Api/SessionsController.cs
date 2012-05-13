using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Models;
using ReviewR.Web.Models.Data;
using ReviewR.Web.Models.Response;
using ReviewR.Web.Services;

namespace ReviewR.Web.Api
{
    public class SessionsController : ReviewRApiController
    {
        //public AuthenticationService Auth { get; set; }

        //public SessionsController(AuthenticationService auth)
        //{
        //    Auth = auth;
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //public void Facebook(string access_token, int expires_in)
        //{
            
        //}

        //public void Delete()
        //{
        //    User = null;
        //}

        //private Tuple<User, IList<string>> GetOrCreateUser(UserInfo id, string email, string displayName)
        //{
        //    bool emailFromId = !String.IsNullOrEmpty(id.Email);
        //    email = email ?? id.Email;
        //    displayName = displayName ?? id.DisplayName;

        //    IList<string> missingFields = new List<string>();
        //    User user = Auth.Login(id.Provider, id.Identifier);
        //    if (user == null)
        //    {
        //        // Can we find a user with that email?
        //        if (!String.IsNullOrEmpty(email))
        //        {
        //            user = Auth.GetUserByEmail(email);
        //            if (user != null)
        //            {
        //                if (!emailFromId)
        //                {
        //                    // We can't just associate them based on the email they typed...
        //                    return null;
        //                }

        //                // Associate this credential with the user
        //                Auth.AddCredential(user.Id, id.Provider, id.Identifier);
        //                return Tuple.Create(user, (IList<string>)null);
        //            }
        //        }
        //        else
        //        {
        //            // Need an email address
        //            missingFields.Add("email");
        //        }

        //        if (String.IsNullOrEmpty(displayName))
        //        {
        //            missingFields.Add("displayName");
        //        }

        //        if (missingFields.Count > 0)
        //        {
        //            return Tuple.Create((User)null, missingFields);
        //        }

        //        // Nothing is missing, so we can just create the user
        //        user = Auth.Register(id.Provider, id.Identifier, email, displayName);
        //    }
        //    return Tuple.Create(user, (IList<string>)null);
        //}
    }
}