using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReviewR.Web.Services;

namespace ReviewR.Web.Facts
{
    public class TestUrlService : UrlService
    {
        public TestUrlService() : base() { }

        public override string Action(string actionName, string controllerName)
        {
            return String.Concat("/app/", controllerName, "/", actionName);
        }

        public override bool IsLocalUrl(string returnUrl)
        {
            return !returnUrl.StartsWith("http");
        }
    }
}
