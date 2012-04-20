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
    }
}