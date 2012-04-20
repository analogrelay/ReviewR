using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Hosting;
using System.Web.Security;
using Newtonsoft.Json;
using ReviewR.Web.Models;
using ReviewR.Web.Models.Response;
using ReviewR.Web.Services;

namespace ReviewR.Web.Infrastructure
{
    public class AuthenticationHandler : DelegatingHandler
    {
    }
}