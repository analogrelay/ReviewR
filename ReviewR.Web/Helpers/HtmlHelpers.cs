using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.WebPages;

namespace ReviewR.Web.Helpers
{
    public static class HtmlHelpers
    {
        public static HelperResult MenuLink(this HtmlHelper self, string title, string actionName, string controllerName, object routeValues)
        {
            RouteValueDictionary values = new RouteValueDictionary(routeValues);
            bool active = Enumerable.SequenceEqual(values, self.ViewContext.RouteData.Values);
            return new HelperResult(w =>
            {
                w.Write("<li");
                if (active)
                {
                    w.Write(" class=\"active\"");
                }
                w.Write(">");
                w.Write(self.ActionLink(title, actionName, controllerName, values, new Dictionary<string, object>()).ToHtmlString());
                w.Write("</li>");
            });
        }

        public static HelperResult CustomValidationSummary(this HtmlHelper helper, string errorMessage)
        {
            return new HelperResult(w =>
            {
                if (!helper.ViewData.ModelState.IsValid)
                {
                    w.Write("<div class=\"validation-summary-errors\">");
                    if (!String.IsNullOrEmpty(errorMessage))
                    {
                        w.Write("<span>");
                        w.Write(errorMessage);
                        w.Write("</span>");
                    }
                    w.Write("<ul>");
                    foreach (var message in helper.ViewData.ModelState.Values.SelectMany(m => m.Errors.Select(e => e.ErrorMessage)))
                    {
                        w.Write("<li class=\"alert alert-error\">");
                        w.Write(message);
                        w.Write("</li>");
                    }
                    w.Write("</ul>");
                    w.Write("</div>");
                }
            });
        }
    }
}