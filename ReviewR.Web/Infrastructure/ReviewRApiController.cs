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
using Ninject;
using ReviewR.Web.Models;
using ReviewR.Web.Services;

namespace ReviewR.Web.Infrastructure
{
    public class ReviewRApiController : ApiController
    {
        private ReviewRPrincipal _sessionUser;
        private bool _sessionUserSet = false;

        [Inject]
        public TokenService Tokens { get; set; }
        
        public ReviewRPrincipal User {
            get { return _sessionUserSet ? _sessionUser : (Request.GetUserPrincipal() as ReviewRPrincipal); }
            set { _sessionUser = value; _sessionUserSet = true; }
        }

        public override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken)
        {
            return base.ExecuteAsync(controllerContext, cancellationToken).ContinueWith(t => PostProcessMessage(t));
        }

        private HttpResponseMessage PostProcessMessage(Task<HttpResponseMessage> task)
        {
            if (!task.IsCompleted)
            {
                return task.Result;
            }
            
            HttpResponseMessage resp = task.Result;
            var currentCookie =
                Request.Headers.GetCookies().Where(
                    c => c.Cookies.Where(
                        s => String.Equals(
                            s.Name,
                            Tokens.CookieName,
                            StringComparison.OrdinalIgnoreCase)).Any()).FirstOrDefault();

            if (resp.Headers.CacheControl == null)
            {
                resp.Headers.CacheControl = new CacheControlHeaderValue();
            }
            resp.Headers.CacheControl.NoCacheHeaders.Add("Set-Cookie");
            if (User != null)
            {
                // Refresh the cookie
                resp.Headers.AddCookies(new[] {
                    Tokens.CreateAuthCookie(User.Identity)
                });
            }
            else if(currentCookie != null)
            {
                // We need to clear the existing cookie, so we set an already expired empty cookie in it's place
                resp.Headers.AddCookies(new[] {
                    Tokens.CreateSignoutCookie()
                });
            }
            return resp;
        }

        public HttpResponseMessage<string[]> ValidationErrors()
        {
            return BadRequest(
                ModelState.SelectMany(
                    p => p.Value.Errors.Select(e => e.ErrorMessage)).ToArray());
        }

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
    }
}