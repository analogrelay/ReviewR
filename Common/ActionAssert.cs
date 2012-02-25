using System;
using System.Web.Mvc;
using System.Web.Routing;
using Xunit;

namespace VibrantUtils
{
    internal static class ActionAssert
    {
        public static void IsViewResult(ActionResult result)
        {
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            Assert.True(String.IsNullOrEmpty(viewResult.MasterName));
            VerifyViewResult(viewResult, null, null);
        }

        public static void IsViewResult(ActionResult result, object model)
        {
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            Assert.True(String.IsNullOrEmpty(viewResult.MasterName));
            VerifyViewResult(viewResult, null, model);
        }

        public static void IsPartialViewResult(ActionResult result)
        {
            PartialViewResult viewResult = Assert.IsType<PartialViewResult>(result);
            VerifyViewResult(viewResult, null, null);
        }

        public static void IsPartialViewResult(ActionResult result, object model)
        {
            PartialViewResult viewResult = Assert.IsType<PartialViewResult>(result);
            VerifyViewResult(viewResult, null, model);
        }

        public static void IsRedirectResult(ActionResult result, string url, bool permanent = false)
        {
            RedirectResult redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal(url, redirectResult.Url);
            Assert.Equal(permanent, redirectResult.Permanent);
        }

        public static void IsRedirectResult(ActionResult result, object routeValues)
        {
            RedirectToRouteResult redirectResult = Assert.IsType<RedirectToRouteResult>(result);
            RouteValueDictionary dict = new RouteValueDictionary(routeValues);
            foreach (var pair in dict)
            {
                Assert.True(redirectResult.RouteValues.ContainsKey(pair.Key));
                Assert.Equal(pair.Value, redirectResult.RouteValues[pair.Key]);
            }
        }

        public static void IsJsonResult(ActionResult result, object data)
        {
            JsonResult jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(data, jsonResult.Data, new PropertyEqualityComparer(typeEquality: false));
        }

        private static void VerifyViewResult(ViewResultBase viewResultBase, string viewName, object model)
        {
            Assert.Equal(viewName ?? String.Empty, viewResultBase.ViewName);
            Assert.Equal(model, viewResultBase.Model, new PropertyEqualityComparer());
        }
    }
}
