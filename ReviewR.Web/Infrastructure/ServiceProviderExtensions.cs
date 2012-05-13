using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReviewR.Web.Infrastructure
{
    public static class ServiceProviderExtensions
    {
        public static T GetService<T>(this IServiceProvider self) where T : class
        {
            return self.GetService(typeof(T)) as T;
        }
    }
}