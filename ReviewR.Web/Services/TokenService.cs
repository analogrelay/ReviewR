using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using Newtonsoft.Json;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Models;
using ReviewR.Web.Models.Data;
using VibrantUtils;

namespace ReviewR.Web.Services
{
    public class TokenService
    {
        public virtual SessionToken UnprotectToken(string token, string purpose)
        {
            Requires.NotNullOrEmpty(token, "token");
            Requires.NotNullOrEmpty(purpose, "purpose");

            byte[] encodedToken = Unprotect(token, purpose);
            return SessionToken.FromEncodedToken(encodedToken);
        }

        public virtual string ProtectToken(SessionToken token, string purpose)
        {
            Requires.NotNull(token, "token");
            Requires.NotNullOrEmpty(purpose, "purpose");

            byte[] encoded = token.EncodeToken();
            return Protect(encoded, purpose);
        }

        protected virtual string Protect(byte[] data, string purpose)
        {
            return MachineKey.Encode(data, MachineKeyProtection.All);
        }

        protected virtual byte[] Unprotect(string data, string purpose)
        {
            return MachineKey.Decode(data, MachineKeyProtection.All);
        }
    }
}