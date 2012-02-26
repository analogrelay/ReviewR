using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using ReviewR.Web.Controllers;
using ReviewR.Web.Services;
using ReviewR.Web.ViewModels;
using VibrantUtils;
using Xunit;

namespace ReviewR.Web.Facts.Controllers
{
    public class HomeControllerFacts
    {
        public class Index
        {
            [Fact]
            public void ReturnsViewResult()
            {
                // Arrange
                var ctl = new HomeController();

                // Act
                var result = ctl.Index();

                // Assert
                ActionAssert.IsViewResult(result);
            }
        }
    }
}
