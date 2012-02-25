using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviewR.Web.ViewModels;
using VibrantUtils;
using ReviewR.Web.Controllers;
using Xunit;

namespace ReviewR.Web.Facts.Controllers
{
    public class ReviewControllerFacts
    {
        public class NewGet
        {
            [Fact]
            public void ReturnsView()
            {
                // Arrange
                var ctl = new ReviewController();

                // Act
                var result = ctl.New();

                // Assert
                ActionAssert.IsViewResult(result, new NewReviewViewModel());
            }
        }
    }
}
