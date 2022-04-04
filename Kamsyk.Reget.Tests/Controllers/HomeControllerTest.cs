using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kamsyk.Reget;
using Kamsyk.Reget.Controllers;
using Xunit;

namespace Kamsyk.Reget.Controllers.Tests {
    //[TestClass]
    //public class HomeControllerTest {
    //    [TestMethod]
    //    public void Index_Redirect_ToRequest() {
    //        // Arrange
    //        HomeController controller = new HomeController();

    //        // Act
    //        var result = controller.Index() as ViewResult;

    //        // Assert
    //        Assert.IsNotNull(result);
    //    }

    //    [TestMethod]
    //    public void HomeControllert_AboutHelo() {
    //        // Arrange
    //        HomeController controller = new HomeController();

    //        // Act
    //        ViewResult result = controller.AboutHelp() as ViewResult;

    //        // Assert
    //        Assert.IsInstanceOfType(result, typeof(ViewResult));
    //    }
    //}
    public class HomeControllerTest {
        [Fact]
        public void Index_Redirect_ToRequest() {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            var result = controller.Index(null) as ViewResult;

            // Assert
            Assert.True(result != null);
        }

        [Fact]
        public void HomeControllert_AboutHelo() {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            var result = controller.AboutHelp() as ViewResult;

            // Assert
            Assert.True(result != null);
        }
    }
}
