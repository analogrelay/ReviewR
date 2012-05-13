using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;

namespace ReviewR.Web.Infrastructure
{
    public abstract class PageBase : WebPage
    {
        private IServiceProvider _services;

        public IServiceProvider Services
        {
            get { return _services ?? (_services = ReviewRApplication.Services); }
            set { _services = value; }
        }
    }
}