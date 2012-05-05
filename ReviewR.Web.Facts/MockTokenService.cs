using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using ReviewR.Web.Models;
using ReviewR.Web.Services;

namespace ReviewR.Web.Facts
{
    public class MockTokenService : TokenService
    {
        public override string ProtectToken(SessionToken token, string purpose)
        {
            byte[] encoded = token.EncodeToken();
            return purpose + "|" + Convert.ToBase64String(encoded);
        }

        public override SessionToken UnprotectToken(string token, string purpose)
        {
            string[] splitted = token.Split('|');
            if (!String.Equals(splitted[0], purpose, StringComparison.Ordinal))
            {
                return null;
            }
            byte[] data = Convert.FromBase64String(splitted[1]);
            return SessionToken.FromEncodedToken(data);
        }
    }
}
