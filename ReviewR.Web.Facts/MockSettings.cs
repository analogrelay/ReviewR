using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviewR.Web.Infrastructure;

namespace ReviewR.Web.Facts
{
    public class MockSettings : ISettings
    {
        public IDictionary<string, string> Values { get; private set; }

        public MockSettings()
        {
            Values = new Dictionary<string, string>();
        }

        public string Get(string name)
        {
            string val;
            if (!Values.TryGetValue(name, out val))
            {
                return null;
            }
            return val;
        }
    }
}
