using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReviewR.Web.Infrastructure
{
    public class RequestDataValueProviderFactory : ValueProviderFactory
    {    
        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            return new DictionaryValueProvider<object>(new Dictionary<string, object>()
            {
                { "IsAjaxRequest", controllerContext.RequestContext.HttpContext.Request.IsAjaxRequest() }
            }, CultureInfo.InvariantCulture);
        }
    }
}