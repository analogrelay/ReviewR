using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Web.Facts.Api
{
    public interface ITestableController
    {
        MockDataRepository MockData { get; set; }
    }
}
