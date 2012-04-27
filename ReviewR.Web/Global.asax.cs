using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace ReviewR.Web
{
    public class ReviewRApplication : HttpApplication
    {
        public static ReviewRApplication Instance
        {
            get
            {
                var context = HttpContext.Current;
                if (context == null)
                {
                    return null;
                }
                return context.ApplicationInstance as ReviewRApplication;
            }
        }

        public static IServiceProvider Services { get; set; }
    }
}