using System.Data.Entity.Infrastructure;
using System.Web.DynamicData;
using System.Web.Routing;
using DynamicData.EFCodeFirstProvider;
using ReviewR.Web.Models.Data;

namespace DynamicData
{
    public class Registration
    {
        private static MetaModel s_defaultModel = new MetaModel();
        public static MetaModel DefaultModel
        {
            get
            {
                return s_defaultModel;
            }
        }

        public static void Register(RouteCollection routes)
        {
            DefaultModel.RegisterContext(
                () => ((IObjectContextAdapter)new ReviewRDbContext()).ObjectContext,
                new ContextConfiguration() { ScaffoldAllTables = true });
            DefaultModel.DynamicDataFolderVirtualPath = "~/DynamicData/DynamicData";

            // This route must come first to prevent some other route from the site to take over
            routes.Insert(0, new DynamicDataRoute("_dbadmin/{table}/{action}")
            {
                Constraints = new RouteValueDictionary(new { action = "List|Details|Edit|Insert" }),
                Model = DefaultModel
            });

            routes.MapPageRoute(
                "dd_default",
                "_dbadmin",
                "~/DynamicData/Default.aspx");
        }
    }
}