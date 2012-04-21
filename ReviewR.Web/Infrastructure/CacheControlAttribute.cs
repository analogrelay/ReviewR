using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http.Filters;

namespace ReviewR.Web.Infrastructure
{
    public class CacheControlAttribute : ActionFilterAttribute
    {
        public CacheabilityValue Cacheability { get; set; }
        public bool NoStore { get; set; }
        public TimeSpan? SharedMaxAge { get; set; }
        public TimeSpan? MaxAge { get; set; }
        public TimeSpan? MinFresh { get; set; }
        public bool MaxStale { get; set; }
        public bool OnlyIfCached { get; set; }
        public bool MustRevalidate { get; set; }
        public bool ProxyRevalidate { get; set; }
        public bool NoTransform { get; set; }

        public CacheControlAttribute(CacheabilityValue cacheability)
        {
            Cacheability = cacheability;
        }

        public CacheControlAttribute(CacheabilityValue cacheability, bool noStore) : this(cacheability)
        {
            NoStore = noStore;
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var cacheControl = actionExecutedContext.Result.Headers.CacheControl;
            if (cacheControl == null)
            {
                cacheControl = actionExecutedContext.Result.Headers.CacheControl = new CacheControlHeaderValue();
            }
            switch (Cacheability)
            {
                case CacheabilityValue.Public:
                    cacheControl.Public = true;
                    break;
                case CacheabilityValue.Private:
                    cacheControl.Private = true;
                    break;
                default:
                    cacheControl.NoCache = true;
                    break;
            }
            cacheControl.NoStore = NoStore;
            cacheControl.SharedMaxAge = SharedMaxAge;
            cacheControl.MaxAge = MaxAge;
            cacheControl.MinFresh = MinFresh;
            cacheControl.MaxStale = MaxStale;
            cacheControl.OnlyIfCached = OnlyIfCached;
            cacheControl.MustRevalidate = MustRevalidate;
            cacheControl.ProxyRevalidate = ProxyRevalidate;
            cacheControl.NoTransform = NoTransform;
            base.OnActionExecuted(actionExecutedContext);
        }
    }

    public enum CacheabilityValue
    {
        Public,
        Private,
        NoCache
    }
}