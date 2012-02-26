using System;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Web.DynamicData;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DynamicData
{
    public partial class CodeField : System.Web.DynamicData.FieldTemplateUserControl
    {
        private const int MAX_DISPLAYLENGTH_IN_LIST = 25;

        public string FieldValueStringSummary
        {
            get
            {
                string value = base.FieldValueString;
                if (value != null && value.Length > MAX_DISPLAYLENGTH_IN_LIST)
                {
                    value = value.Substring(0, MAX_DISPLAYLENGTH_IN_LIST - 3) + "...";
                }
                return value;
            }
        }

        public override Control DataControl
        {
            get
            {
                return Literal1;
            }
        }

    }
}
