using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReviewR.Web.Services
{
    public class UrlService
    {
        private UrlHelper _url;

        protected UrlService()
        {
        }

        public UrlService(UrlHelper url)
        {
            _url = url;
        }

        public virtual string Action(string actionName, string controllerName)
        {
            return _url.Action(actionName, controllerName);
        }

        public virtual bool IsLocalUrl(string returnUrl)
        {
            return _url.IsLocalUrl(returnUrl);
        }
    }
}