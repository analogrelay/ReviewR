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

            byte[] data = Convert.FromBase64String(token);
            byte[] encodedToken = Unprotect(data, purpose);
            return SessionToken.FromEncodedToken(encodedToken);
        }

        public virtual string ProtectToken(SessionToken token, string purpose)
        {
            Requires.NotNull(token, "token");
            Requires.NotNullOrEmpty(purpose, "purpose");

            byte[] encoded = token.EncodeToken();
            byte[] encrypted = Protect(encoded, purpose);
            return Convert.ToBase64String(encrypted);
        }

        protected virtual byte[] Protect(byte[] data, string purpose)
        {
            return MachineKey.Protect(data, purpose);
        }

        protected virtual byte[] Unprotect(byte[] data, string purpose)
        {
            return MachineKey.Unprotect(data, purpose);
        }
    }
}