[assembly: WebActivator.PreApplicationStartMethod(typeof(ReviewR.Web.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(ReviewR.Web.App_Start.NinjectWebCommon), "Stop")]

namespace ReviewR.Web.App_Start
{
    using System;
    using System.Net.Http;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Common;
    using System.Web.Http.Dispatcher;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Common;
    using ReviewR.Web.Infrastructure;
    using ReviewR.Web.Models.Data;
    using ReviewR.Web.Services;
    using ReviewR.Web.Services.Authenticators;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);

            GlobalConfiguration.Configuration.ServiceResolver.SetResolver(
                t => bootstrapper.Kernel.TryGet(t),
                t => bootstrapper.Kernel.GetAll(t));

            foreach (var handler in bootstrapper.Kernel.GetAll<DelegatingHandler>())
            {
                GlobalConfiguration.Configuration.MessageHandlers.Add(handler);
            }

            ReviewRApplication.Services = bootstrapper.Kernel;
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
            ReviewRApplication.Services = kernel;
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
            
            RegisterServices(kernel);

            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<TokenService>().ToSelf();
            kernel.Bind<AuthenticationService>().ToSelf();
            kernel.Bind<IDataRepository>().To<DefaultDataRepository>();
            kernel.Bind<DiffService>().ToSelf();
            kernel.Bind<ISettings>().To<WebSettings>();
            kernel.Bind<Authenticator>()
                  .To<FacebookAuthenticator>();
            kernel.Bind<Authenticator>()
                  .To<GoogleAuthenticator>();
            kernel.Bind<Authenticator>()
                  .To<MicrosoftAuthenticator>();
        }        
    }
}
