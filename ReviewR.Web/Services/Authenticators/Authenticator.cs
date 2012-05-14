using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Models;

namespace ReviewR.Web.Services.Authenticators
{
    public abstract class Authenticator
    {
        public virtual bool Active { get { return true; } }
        public abstract string Name { get; }
        public abstract string Id { get; }

        protected internal virtual string FetchUserInfoBaseUrl
        {
            get { throw new NotImplementedException("Authenticator must implement FetchUserInfoBaseUrl or CompleteAuthentication(string)"); }
        }
        protected internal virtual string AccessTokenQueryParameterName { get { return "access_token"; } }

        public abstract string GetAppId(ISettings appSettings);
        
        protected virtual UserInfo ParseResponse(string jsonResponse)
        {
            throw new NotImplementedException("Authenticator must implement ParseResponse(dynamic) or CompleteAuthentication(string)");
        }

        public virtual Task<UserInfo> CompleteAuthentication(string accessToken)
        {
            try
            {
                // Build the URL
                UriBuilder meUrl = new UriBuilder(FetchUserInfoBaseUrl);
                meUrl.Query = AccessTokenQueryParameterName + "=" + accessToken;

                // Fetch the user's record from the service
                HttpClient client = CreateHttpClient();
                return client.GetAsync(meUrl.Uri).Then(resp =>
                {
                    resp.EnsureSuccessStatusCode();
                    return resp.Content.ReadAsStringAsync();
                }).Then(json =>
                {
                    return ParseResponse(json);
                });
            }
            catch (Exception ex)
            {
                return TaskHelpers.FromError<UserInfo>(ex);
            }
        }

        protected internal virtual HttpClient CreateHttpClient()
        {
            return new HttpClient();
        }
    }
}