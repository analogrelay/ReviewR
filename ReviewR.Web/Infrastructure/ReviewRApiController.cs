using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using Ninject;
using ReviewR.Web.Models;
using ReviewR.Web.Services;

namespace ReviewR.Web.Infrastructure
{
    public class ReviewRApiController : ApiController
    {
        internal static readonly string CookieName = "ReviewRAuth";
        internal static readonly string Purpose = "session";

        private string _sessionTokenString;
        private SessionToken _sessionToken;

        public string SessionToken
        {
            get { return _sessionTokenString; }
        }

        public ReviewRPrincipal User { get; set; }

        [Inject]
        public TokenService Tokens { get; set; }

        public HttpResponseMessage<string[]> ValidationErrors()
        {
            return BadRequest(
                ModelState.SelectMany(
                    p => p.Value.Errors.Select(e => e.ErrorMessage)).ToArray());
        }

        public async override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken)
        {
            // Check for an auth token
            string authCookie = controllerContext.Request.Headers.GetAuthCookie();
            _sessionTokenString = authCookie;
            if (!String.IsNullOrEmpty(_sessionTokenString) ||
                !String.IsNullOrEmpty(_sessionTokenString = controllerContext.Request.Headers.GetAuthHeader()))
            {
                // Parse authVal
                SessionToken token = null;
                try
                {
                    token = Tokens.UnprotectToken(_sessionTokenString, Purpose);
                }
                catch (Exception)
                {
                    // Token is invalid, just clear it
                }

                if (token != null)
                {
                    _sessionToken = token;
                    User = _sessionToken.User;
                    controllerContext.Request.Properties[HttpPropertyKeys.UserPrincipalKey] = User;
                }
            }
            
            // Run the action
            HttpResponseMessage resp = await base.ExecuteAsync(controllerContext, cancellationToken);
            
            // If we had a request token in the cookie but don't now...
            string path = GetSiteRoot(controllerContext.Request.RequestUri);
            if (!String.IsNullOrEmpty(authCookie) && User == null)
            {
                // Clear the cookie
                resp.Headers.ClearCookie(CookieName, path);
            }
            // Otherwise if we have a user now...
            else if (User != null)
            {
                // Issue a new token as an Auth cookie
                var tokenStr = IssueSessionToken();
                if (_sessionToken != null && !String.IsNullOrEmpty(tokenStr))
                {
                    resp.Headers.SetAuthCookie(tokenStr, path, _sessionToken.Expires);
                }
            }
            
            return resp;
        }

        private string GetSiteRoot(Uri uri)
        {
            string path = uri.AbsolutePath;
            int apiRootPos = path.IndexOf("api");
            return path.Substring(0, apiRootPos);
        }

        private string IssueSessionToken()
        {
            if (User != null)
            {
                _sessionToken = new SessionToken(User, DateTime.UtcNow.AddDays(30));
                return _sessionTokenString = Tokens.ProtectToken(_sessionToken, Purpose);
            }
            return null;
        }

        #region Status Code Helpers
        protected HttpResponseMessage BadRequest() { return R(HttpStatusCode.BadRequest); }
        protected HttpResponseMessage<T> BadRequest<T>(T val) { return R(val, HttpStatusCode.BadRequest); }
        protected HttpResponseMessage Forbidden() { return R(HttpStatusCode.Forbidden); }
        protected HttpResponseMessage<T> Forbidden<T>(T val) { return R(val, HttpStatusCode.Forbidden); }
        protected HttpResponseMessage NotFound() { return R(HttpStatusCode.NotFound); }
        protected HttpResponseMessage<T> NotFound<T>(T val) { return R(val, HttpStatusCode.NotFound); }
        protected HttpResponseMessage Conflict() { return R(HttpStatusCode.Conflict); }
        protected HttpResponseMessage<T> Conflict<T>(T val) { return R(val, HttpStatusCode.Conflict); }
        protected HttpResponseMessage Ok() { return R(HttpStatusCode.OK); }
        protected HttpResponseMessage<T> Ok<T>(T val) { return R(val, HttpStatusCode.OK); }
        protected HttpResponseMessage NoContent() { return R(HttpStatusCode.NoContent); }
        protected HttpResponseMessage<T> NoContent<T>(T val) { return R(val, HttpStatusCode.NoContent); }
        protected HttpResponseMessage Created() { return R(HttpStatusCode.Created); }
        protected HttpResponseMessage<T> Created<T>(T val) { return R(val, HttpStatusCode.Created); }

        private HttpResponseMessage R(HttpStatusCode code)
        {
            return new HttpResponseMessage(code);
        }

        private HttpResponseMessage<T> R<T>(T val, HttpStatusCode code)
        {
            return new HttpResponseMessage<T>(val, code);
        }
        #endregion
    }
}