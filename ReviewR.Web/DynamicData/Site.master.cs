using System;
using System.ComponentModel.DataAnnotations;
using System.Web.DynamicData;
using System.Web.UI.WebControls;

namespace DynamicData
{
    public partial class Site : System.Web.UI.MasterPage
    {
        public string CurrentTableName
        {
            get
            {
                var table = DynamicDataRouteHandler.GetRequestMetaTable(Context);
                return table == null ? String.Empty : table.Name;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            System.Collections.IList visibleTables = Registration.DefaultModel.VisibleTables;
            if (visibleTables.Count == 0)
            {
                throw new InvalidOperationException("There are no accessible tables. Make sure that at least one data model is registered in Global.asax and scaffolding is enabled or implement custom pages.");
            }
            Menu1.DataSource = visibleTables;
            Menu1.DataBind();
        }
    }
}
