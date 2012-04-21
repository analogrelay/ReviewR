using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace ReviewR.Web.Infrastructure
{
    public class ExpiresAttribute : ActionFilterAttribute
    {
        public DateTimeOffset Expires { get; set; }

        public ExpiresAttribute(TimeSpan expiresIn) : this(DateTimeOffset.UtcNow + expiresIn) { }
        public ExpiresAttribute(DateTimeOffset expires)
        {
            Expires = expires;
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.Result.Content.Headers.Expires = Expires;
            base.OnActionExecuted(actionExecutedContext);
        }
    }
}