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

        public static AuthenticationResult Success(User user)
        {
            return new AuthenticationResult(AuthenticationOutcome.Success, user, new List<string>());
        }

        public static AuthenticationResult MissingData(IList<string> missingFields)
        {
            return new AuthenticationResult(AuthenticationOutcome.MissingFields, null, missingFields);
        }
    }

    public enum AuthenticationOutcome
    {
        Success,
        MissingFields,
    }
}
