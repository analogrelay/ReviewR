using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviewR.Web.Models.Data;

namespace ReviewR.Web.Services
{
    public class AuthenticationResult
    {
        public AuthenticationOutcome Outcome { get; set; }
        public User User { get; set; }
        public IList<string> MissingFields { get; private set; }

        public AuthenticationResult(AuthenticationOutcome outcome, User user, IList<string> missingFields)
        {
            Outcome = outcome;
            User = user;
            MissingFields = missingFields;
        }

        public static AuthenticationResult LoggedIn(User user)
        {
            return new AuthenticationResult(AuthenticationOutcome.LoggedIn, user, new List<string>());
        }

        public static AuthenticationResult Registered(User user)
        {
            return new AuthenticationResult(AuthenticationOutcome.Registered, user, new List<string>());
        }

        public static AuthenticationResult Associated(User user)
        {
            return new AuthenticationResult(AuthenticationOutcome.Associated, user, new List<string>());
        }

        public static AuthenticationResult MissingData(IEnumerable<string> missingFields)
        {
            return new AuthenticationResult(AuthenticationOutcome.MissingFields, null, new List<string>(missingFields));
        }
    }

    public enum AuthenticationOutcome
    {
        LoggedIn,
        Registered,
        Associated,
        MissingFields,
    }
}
