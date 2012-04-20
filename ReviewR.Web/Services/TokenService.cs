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

namespace ReviewR.Web.Services
{
    public class TokenService
    {
        public IDataRepository Data { get; set; }
        
        public TokenService(IDataRepository data)
        {
            Data = data;
        }

        public string EncryptToken(AuthenticationToken token, string purpose)
        {
            return Convert.ToBase64String(MachineKey.Protect(token.EncodeToken(), purpose));
        }

        public AuthenticationToken DecryptToken(string token, string purpose)
        {
            byte[] data = Convert.FromBase64String(token);
            byte[] encodedToken = MachineKey.Unprotect(data, purpose);
            return AuthenticationToken.FromEncodedToken(encodedToken);
        }
    }
}