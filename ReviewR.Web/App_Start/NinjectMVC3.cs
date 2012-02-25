[assembly: WebActivator.PreApplicationStartMethod(typeof(ReviewR.Web.App_Start.NinjectMVC3), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(ReviewR.Web.App_Start.NinjectMVC3), "Stop")]

namespace ReviewR.Web.App_Start
{
    using System.Reflection;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Mvc;
    using ReviewR.Web.Models;
    using ReviewR.Web.Services;

    public static class NinjectMVC3 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        public static Bootstrapper Bootstrapper { get { return bootstrapper; } }

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestModule));
            DynamicModuleUtility.RegisterModule(typeof(HttpApplicationInitializationModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            RegisterServices(kernel);
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            //kernel.Bind<IDataRepository>().To<DefaultDataRepository>();
            kernel.Bind<AuthenticationService>().ToSelf();
            kernel.Bind<AuthTokenService>().ToSelf();
            kernel.Bind<HashService>().ToSelf();
            kernel.Bind<UrlService>().ToSelf();
        }        
    }
}
