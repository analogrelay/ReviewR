using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;

namespace ReviewR.Web.Infrastructure
{
    public interface ISettings
    {
        string Get(string name);
    }

    public partial class WebSettings : ISettings
    {
        private static ISettings _instance = new WebSettings();
        public static ISettings Instance { get { return _instance; } }
        
        private Dictionary<string, string> _overrideKeys = new Dictionary<string, string>();
        
        partial void AddDeveloperSettings(Dictionary<string, string> overrideKeys);

        public WebSettings()
        {
            AddDeveloperSettings(_overrideKeys);
        }

        public string Get(string name)
        {
            string val = WebConfigurationManager.AppSettings[name];
            if (String.IsNullOrEmpty(val) && !_overrideKeys.TryGetValue(name, out val))
            {
                return String.Empty;
            }
            return val;
        }
    }
}
