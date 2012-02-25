using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ReviewR.Web.Facts
{
    public static class TestMixins
    {
        public static IEnumerable<string> AllErrors(this ModelStateDictionary self)
        {
            return self.Values.SelectMany(m => m.Errors.Select(e => e.ErrorMessage));
        }
    }
}
