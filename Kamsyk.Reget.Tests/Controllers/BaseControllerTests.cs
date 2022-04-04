using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kamsyk.Reget.Controllers;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Kamsyk.Reget.AgControls;

namespace Kamsyk.Reget.Controllers.Tests {
    [TestClass()]
    public class BaseControllerTests {
        [TestMethod()]
        public void CreateTest_BaseController_CultureEN() {
            //Arrange
            BaseController baseController = new BaseController();

            var mocks = new MockRepository();
            var mockedhttpContext = mocks.DynamicMock<HttpContextBase>();
            var mockedHttpRequest = mocks.DynamicMock<HttpRequestBase>();
            SetupResult.For(mockedhttpContext.Request).Return(mockedHttpRequest);

            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            //Act
            baseController.Create(mockedhttpContext.Request.RequestContext, typeof(RequestController));

            //Assert
            Assert.IsTrue(Thread.CurrentThread.CurrentCulture.Name == "en-US");
        }

        [TestMethod()]
        public void GetAngularTextBoxTest() {
            //Arange
            BaseController bc = new BaseController();

            //Act
            MdInputContainer mdInputContainer = new MdInputContainer();
            mdInputContainer.FormName = "formName";
            mdInputContainer.NgModel = "test";
            string strTextBox = mdInputContainer.RenderControlHtml();
                
            //Assert
            Assert.IsTrue(strTextBox != null);
        }


        [TestMethod()]
        public void GetNavigationTextTest() {
            //Arange
            BaseController bc = new BaseController();


            var mocks = new MockRepository();
            var mockedhttpContext = mocks.DynamicMock<HttpContextBase>();
            var mockedHttpRequest = mocks.DynamicMock<HttpRequestBase>();
            SetupResult.For(mockedhttpContext.Request).Return(mockedHttpRequest);

            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            //Act
            bc.Create(mockedhttpContext.Request.RequestContext, typeof(RequestController));
            string strNavText = bc.GetNavigationText();

            //Assert
            Assert.IsNotNull(strNavText);
        }

        [TestMethod()]
        public void GetAngularDropdownBoxTest() {
            //Arange
            MdSelect mdSelect = new MdSelect();

            //Act
            mdSelect.FormName = "formName";
            mdSelect.AgSourceList = "agSourceList";
            mdSelect.AgModel = "agModel";
            mdSelect.AgIdItem = "idItem";
            mdSelect.AgTextItem = "textItem";

            string strDropdownBox = mdSelect.RenderControlHtml();

            //Assert
            Assert.IsTrue(strDropdownBox != null);
        }

        [TestMethod()]
        public void GetAngularTextAreaTest() {
            //Arange
            MdTextArea mdTextArea = new MdTextArea();

            //Act
            mdTextArea.FormName = "frm";
            mdTextArea.NgModel = "model";
            var strTextAtea = mdTextArea.RenderControlHtml();

            //Assert
            Assert.IsTrue(strTextAtea != null && strTextAtea != null);
        }
    }
}