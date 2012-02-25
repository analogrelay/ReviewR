using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviewR.Web.Controllers;
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
            public void ReturnsViewResultWithEmptyDashboardModelIfNotAuthenticated()
            {
                // Arrange
                var ctl = new HomeController();

                // Act
                var result = ctl.Index(isAuthenticated: false);

                // Assert
                ActionAssert.IsViewResult(result, new DashboardViewModel() { Reviews = new List<ReviewViewModel>() });
            }
        }
    }
}
