using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;
using System.Web.WebPages;

namespace ReviewR.Web.Infrastructure
{
    public class WebPageRoute : RouteBase, IRouteHandler
    {
        public string TargetPagePath { get; private set; }

        public WebPageRoute(string targetPagePath)
        {
            TargetPagePath = targetPagePath;
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            return new RouteData(this, this);
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            // Not intended to be used
            return new VirtualPathData(this, "~/");
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return WebPageHttpHandler.CreateFromVirtualPath(TargetPagePath);
        }
    }
}
