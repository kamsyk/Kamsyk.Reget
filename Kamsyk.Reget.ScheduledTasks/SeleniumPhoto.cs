using Kamsyk.Reget.Model;
using Kamsyk.Reget.Model.Repositories;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kamsyk.Reget.ScheduledTasks {
    public class SeleniumPhoto {
        #region Constants
        private const int WAIT_IN_SECONDS = 60;
        #endregion

        #region Properties
        private IWebDriver m_driver = null;
        private int m_ErrorCount = 0;
        private string m_GraphRequestId = null;
        #endregion

        #region Methods
        public byte[] GetUserPhotoGraph(string userMail) {
            if (m_ErrorCount > 5) {
                throw new TooMuchErrorsEx("User Photo Download crashed repeatedly!!!");
            }

            try {

                if (m_driver == null) {
                    DisplayLog("Logon");
                    SetChromeWebDriver();
                    LoginToGraph();
                    //m_driver.Navigate().GoToUrl("https://developer.microsoft.com/en-us/graph/graph-explorer?request=me%2Fphoto%2F%24value&version=v1.0");
                    Thread.Sleep(5000);
                }
                
                WebDriverWait webDriverWait = new WebDriverWait(m_driver, TimeSpan.FromSeconds(WAIT_IN_SECONDS));
                var inpTextField18 = GetElementById(webDriverWait, "TextField18");
                //inpTextField18.Clear();

                var strValue = inpTextField18.GetAttribute("value");
                for (int i = 0; i <= strValue.Length; i++) {
                    inpTextField18.SendKeys(Keys.Backspace);
                }

                string strGet = "https://graph.microsoft.com/v1.0/users/" + userMail + "/photo/$value";
                inpTextField18.SendKeys(strGet);
                                
                DisplayLog("Wait For Response");

                int repeatIndex = 0;
                bool isLoaded = false;
                while (!isLoaded) {
                    var buttons = GetElementsByTagName(webDriverWait, "button");
                    foreach (var button in buttons) {
                        var spans = button.FindElements(By.TagName("span"));
                        if (spans != null && spans.Count > 0) {
                            if (spans[0].GetAttribute("innerHTML") == "Run query") {
                                button.Click();
                                isLoaded = true;
                                break;
                            }
                        }
                    }
                    repeatIndex++;
                    if (repeatIndex > 30) {
                        throw new Exception("Button not found");
                    }
                    Thread.Sleep(1000);
                }

                //Wait unitl response is loaded
                var tabs = GetElementsByClassName(webDriverWait, "ms-TooltipHost");
                IWebElement tabPhoto = null;
                IWebElement tabRespHeader = null;
                foreach (var tab in tabs) {
                    string strInnerHtml = tab.GetAttribute("innerHTML");
                    if (strInnerHtml.IndexOf("Response preview") > -1) {
                        tabPhoto = tab;
                    }
                    if (strInnerHtml.IndexOf("Response headers") > -1) {
                        tabRespHeader = tab;
                    }
                }
                tabRespHeader.Click();
                Thread.Sleep(1000);

                isLoaded = false;
                repeatIndex = 0;
                while (!isLoaded) {
                    
                    var divResponses = GetElementsByTagName(webDriverWait, "div");
                    foreach (var divResponse in divResponses) {
                        if (isLoaded) break;
                        var respSpans = divResponse.FindElements(By.TagName("span"));
                        if (respSpans != null && respSpans.Count > 0) {
                            foreach (var respSpan in respSpans) {
                                if (isLoaded) break;
                                var subSpans = respSpan.FindElements(By.TagName("span"));
                                if (subSpans != null && subSpans.Count > 0) {
                                    foreach (var subSpan in subSpans) {
                                        string innerHtml = subSpan.GetAttribute("innerHTML");
                                        if (innerHtml == "\"request-id\"") {
                                            ReadOnlyCollection<IWebElement> spanReqs = respSpan.FindElements(By.ClassName("mtk5"));
                                            if (spanReqs != null && spanReqs.Count > 0) {
                                                string reqId = spanReqs[0].GetAttribute("innerHTML");
                                                if (reqId != m_GraphRequestId) {
                                                    m_GraphRequestId = reqId;
                                                    isLoaded = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //ReadOnlyCollection<IWebElement> spanResps = GetElementsByClassName(webDriverWait, "mtk5"); ;
                    //if (spanResps == null || spanResps.Count == 0) {
                    //    Thread.Sleep(1000);
                    //} else {

                    //}

                    repeatIndex++;
                    if (repeatIndex > 30) {
                        throw new Exception("Button not found");
                    }

                    Thread.Sleep(1000);
                }

                DisplayLog("Response Loaded");

                tabPhoto.Click();
                Thread.Sleep(1000);

                string strImgUrl = null;
                var divs = GetElementsByClassName(webDriverWait, "pivot-response");
                foreach (var div in divs) {
                    var imgs = div.FindElements(By.TagName("img"));
                    if (imgs != null && imgs.Count > 0) {
                        strImgUrl = imgs[0].GetAttribute("src");
                        break;
                    }
                }

                if (strImgUrl == null) {
                    m_ErrorCount = 0;
                    return null;
                }

                string currUrl = m_driver.Url;
                m_driver.Navigate().GoToUrl(strImgUrl);
                Thread.Sleep(2000);

                try {
                    var base64string = (m_driver as IJavaScriptExecutor).ExecuteScript(@"
                        var c = document.createElement('canvas');
                        var ctx = c.getContext('2d');
                        var imgs = document.getElementsByTagName('img');
                        var img = imgs[0];
                        c.height=img.naturalHeight;
                        c.width=img.naturalWidth;
                        ctx.drawImage(img, 0, 0,img.naturalWidth, img.naturalHeight);
                        var base64String = c.toDataURL();
                        return base64String;
                        ") as string;

                    var base64 = base64string.Split(',').Last();
                    using (var stream = new MemoryStream(Convert.FromBase64String(base64))) {
                        //using (var bitmap = new Bitmap(stream)) {
                        //    //var filepath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ImageName.png");
                        //    //bitmap.Save(filepath, ImageFormat.Png);
                        //}
                        byte[] imgByte = stream.ToArray();

                        if (imgByte != null && imgByte.Length < 128) {
                            imgByte = null;
                        }

                        m_ErrorCount = 0;

                        return imgByte;
                    }
                } catch (Exception ex) {
                    throw ex;
                } finally {
                    m_driver.Navigate().GoToUrl(currUrl);
                    Thread.Sleep(5000);
                }

                //return null;
            } catch (Exception ex) {
                m_ErrorCount++;
                throw ex;
            }
        }

        

        public byte[] GetUserPhoto(string userMail) {
            if (m_ErrorCount > 5) {
                throw new TooMuchErrorsEx("User Photo Download crashed repeatedly!!!");
            }

            try {
                if (m_driver == null) {
                    LoginToOffice();
                }

                m_driver.Navigate().GoToUrl("https://outlook.office.com/owa/service.svc/s/GetPersonaPhoto?email=" + userMail + "&UA=0&size=HR240x240");

                Thread.Sleep(2000);

                bool isExistError = IsElementExistByClassName(m_driver, "error-code");
                if (isExistError) {
                    return null;
                }

                var base64string = (m_driver as IJavaScriptExecutor).ExecuteScript(@"
                var c = document.createElement('canvas');
                var ctx = c.getContext('2d');
                var imgs = document.getElementsByTagName('img');
                var img = imgs[0];
                c.height=img.naturalHeight;
                c.width=img.naturalWidth;
                ctx.drawImage(img, 0, 0,img.naturalWidth, img.naturalHeight);
                var base64String = c.toDataURL();
                return base64String;
                ") as string;

                var base64 = base64string.Split(',').Last();
                using (var stream = new MemoryStream(Convert.FromBase64String(base64))) {
                    //using (var bitmap = new Bitmap(stream)) {
                    //    //var filepath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ImageName.png");
                    //    //bitmap.Save(filepath, ImageFormat.Png);
                    //}
                    byte[] imgByte = stream.ToArray();

                    if (imgByte != null && imgByte.Length < 128) {
                        imgByte = null;
                    }

                    m_ErrorCount = 0;

                    return imgByte;
                }

                //return null;
            } catch (Exception ex) {
                m_ErrorCount++;
                throw ex;
            }
        }

        private void SetChromeWebDriver() {
            ChromeOptions options = new ChromeOptions();

            //options.AddArguments("test-type");
            //options.AddArguments("no-sandbox");
            ////Fix for cannot get automation extension
            //options.AddArguments("disable-extensions");
            //options.AddArguments("start-maximized");
            //options.AddArguments("--js-flags=--expose-gc");
            //options.AddArguments("disable-plugins");
            //options.AddArguments("--enable-precise-memory-info");
            //options.AddArguments("--disable-popup-blocking");
            //options.AddArguments("--disable-default-apps");
            //options.AddArguments("test-type=browser");
            //options.AddArguments("disable-infobars");

            options.AddAdditionalCapability("useAutomationExtension", false);

            m_driver = new ChromeDriver(options);


        }

        //private void LoginToGraph() {
        //    string owaUserName = System.Configuration.ConfigurationManager.AppSettings["owaUserName"];
        //    string owaUserPwd = System.Configuration.ConfigurationManager.AppSettings["owaUserPwd"];

        //    WebDriverWait webDriverWait = new WebDriverWait(m_driver, TimeSpan.FromSeconds(WAIT_IN_SECONDS));

        //    m_driver.Navigate().GoToUrl("https://developer.microsoft.com/en-us/graph/graph-explorer?request=me%2Fphoto%2F%24value&version=v1.0");

        //    Thread.Sleep(5000);

        //    bool isGo = false;
        //    while (!isGo) {
        //        var buttons = GetElementsByTagName(webDriverWait, "button");
        //        if (buttons != null) {
        //            foreach (var button in buttons) {
        //                string ariaLabel = button.GetAttribute("aria-label");
        //                if (ariaLabel == "Sign in to Graph Explorer") {
        //                    button.Click();
        //                    isGo = true;
        //                    break;
        //                }
        //            }
        //        }

        //        Thread.Sleep(1000);
        //    }

        //    m_driver.Navigate().GoToUrl("https://www.microsoft.com");
        //    var mectrl_headerPicture = GetElementById(webDriverWait, "mectrl_headerPicture");
        //    mectrl_headerPicture.Click();
        //    Thread.Sleep(5000);

        //    var txtUserName = GetElementById(webDriverWait, "i0116");
        //    txtUserName.SendKeys(owaUserName);

        //    var btnNext = webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("idSIButton9")));
        //    btnNext.Click();
        //    Thread.Sleep(1000);

        //    var divUserAccount = GetElementById(webDriverWait, "aadTile");
        //    divUserAccount.Click();
        //    Thread.Sleep(1000);

        //    var txtPwd = GetElementById(webDriverWait, "i0118");
        //    txtPwd.SendKeys(owaUserPwd);

        //    var btnLoginUser = GetElementById(webDriverWait, "idSIButton9");
        //    btnLoginUser.Click();
        //    Thread.Sleep(1000);

        //    var btnNo = GetElementById(webDriverWait, "idBtn_Back");
        //    btnNo.Click();
        //    Thread.Sleep(1000);

        //    m_driver.Navigate().GoToUrl("https://developer.microsoft.com/en-us/graph/graph-explorer?request=me%2Fphoto%2F%24value&version=v1.0");
        //    Thread.Sleep(5000);
        //}

        private void LoginToGraph() {
            string owaUserName = System.Configuration.ConfigurationManager.AppSettings["owaUserName"];
            string owaUserPwd = System.Configuration.ConfigurationManager.AppSettings["owaUserPwd"];

            WebDriverWait webDriverWait = new WebDriverWait(m_driver, TimeSpan.FromSeconds(WAIT_IN_SECONDS));

            m_driver.Navigate().GoToUrl("https://developer.microsoft.com/en-us/graph/graph-explorer?request=me%2Fphoto%2F%24value&version=v1.0");

            Thread.Sleep(5000);

            bool isGo = false;
            while (!isGo) {
                var buttons = GetElementsByTagName(webDriverWait, "button");
                if (buttons != null) {
                    foreach (var button in buttons) {
                        string ariaLabel = button.GetAttribute("aria-label");
                        if (ariaLabel == "Sign in to Graph Explorer") {
                            button.Click();
                            isGo = true;
                            break;
                        }
                    }
                }

                Thread.Sleep(8000);
            }

            //var winHandles = m_driver.WindowHandles;
            //foreach (var winHandle in winHandles) {
            //    //if(winHandle)
            //}
            //m_driver.SwitchTo();

            IList<string> totWindowHandles = new List<string>(m_driver.WindowHandles);
            var parentWindowHandle = m_driver.WindowHandles[0];
            var childWindowHandle = m_driver.WindowHandles[1];
            var webDriverChild = m_driver.SwitchTo().Window(childWindowHandle);
            DisplayLog(webDriverChild.Title);

            Thread.Sleep(1000);

            var inputs = GetElementsByTagName(webDriverWait, "input");
            foreach (var input in inputs) {
                string strMsg = "id:" + input.GetAttribute("id")
                    + "," + "name:" + input.GetAttribute("name");
                DisplayLog(strMsg);
            }

            //var txtUserName = webDriverChild.FindElement(By.Id("i0116"));
            var txtUserName = GetElementByIdTmp(webDriverWait, "i0116");
            txtUserName.SendKeys(owaUserName);
            Thread.Sleep(1000);
                        
            //var btnNext = webDriverChild.FindElement(By.Id("idSIButton9"));
            var btnNext = webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("idSIButton9")));
            btnNext.Click();
            Thread.Sleep(1000);

            //var divUserAccount = webDriverChild.FindElement(By.Id("aadTile"));
            var divUserAccount = GetElementById(webDriverWait, "aadTile");
            divUserAccount.Click();
            Thread.Sleep(1000);

            //var txtPwd = webDriverChild.FindElement(By.Id("i0118"));
            var txtPwd = GetElementById(webDriverWait, "i0118");
            txtPwd.SendKeys(owaUserPwd);

            //var btnLoginUser = webDriverChild.FindElement(By.Id("idSIButton9"));
            var btnLoginUser = GetElementById(webDriverWait, "idSIButton9");
            btnLoginUser.Click();
            Thread.Sleep(1000);

            //var btnNo = webDriverChild.FindElement(By.Id("idBtn_Back"));
            var btnNo = GetElementById(webDriverWait, "idBtn_Back");
            btnNo.Click();
            Thread.Sleep(1000);

            m_driver.SwitchTo().Window(parentWindowHandle);
        }

        private void LoginToOffice() {
            string owaUserName = System.Configuration.ConfigurationManager.AppSettings["owaUserName"];
            string owaUserPwd = System.Configuration.ConfigurationManager.AppSettings["owaUserPwd"];

            ChromeOptions options = new ChromeOptions();

            //options.AddArguments("test-type");
            //options.AddArguments("no-sandbox");
            ////Fix for cannot get automation extension
            //options.AddArguments("disable-extensions");
            //options.AddArguments("start-maximized");
            //options.AddArguments("--js-flags=--expose-gc");
            //options.AddArguments("disable-plugins");
            //options.AddArguments("--enable-precise-memory-info");
            //options.AddArguments("--disable-popup-blocking");
            //options.AddArguments("--disable-default-apps");
            //options.AddArguments("test-type=browser");
            //options.AddArguments("disable-infobars");

            options.AddAdditionalCapability("useAutomationExtension", false);

            m_driver = new ChromeDriver(options);
            try {
                //using (IWebDriver m_driver = new ChromeDriver(options)) {
                m_driver.Url = "https://www.office.com/";

                WebDriverWait webDriverWait = new WebDriverWait(m_driver, TimeSpan.FromSeconds(WAIT_IN_SECONDS));

                var btnLogin = GetElementById(webDriverWait, "hero-banner-sign-in-to-office-365-link");
                btnLogin.Click();

                var txtUserName = GetElementById(webDriverWait, "i0116");
                txtUserName.SendKeys(owaUserName);

                var btnNext = webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("idSIButton9")));
                btnNext.Click();

                var divUserAccount = GetElementById(webDriverWait, "aadTile");
                divUserAccount.Click();

                var txtPwd = GetElementById(webDriverWait, "i0118");
                txtPwd.SendKeys(owaUserPwd);

                var btnLoginUser = GetElementById(webDriverWait, "idSIButton9");
                btnLoginUser.Click();

                var btnNo = GetElementById(webDriverWait, "idBtn_Back");
                btnNo.Click();

                var btnOutlook = GetElementById(webDriverWait, "ShellMail_link");
                btnOutlook.Click();
                Thread.Sleep(10000);
            } catch (Exception ex) {
                //m_ErrorCount++;
                DispatchSelenium();
                throw ex;
            }
        }

        private IWebElement GetElementById(WebDriverWait webDriverWait, string strId) {
            IWebElement iElement = null;
            int iCount = 0;

            while (iCount < 3) {
                try {
                    iElement = webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id(strId)));

                    return iElement;
                } catch (Exception ex) {
                    iCount++;
                    if (iCount == 3) {
                        throw ex;
                    }
                    Thread.Sleep(1000);
                }
            }

            return null;
        }

        private IWebElement GetElementByIdTmp(WebDriverWait webDriverWait, string strId) {
            IWebElement iElement = null;
            int iCount = 0;

            while (iCount < 3) {
                try {
                    DisplayLog("---------------------------------------------------");
                    m_driver.Navigate().Refresh();
                    Thread.Sleep(1000);
                    var inputs = GetElementsByTagName(webDriverWait, "input");
                    inputs[0].Click();
                    foreach (var input in inputs) {
                        
                        string strMsg = "id:" + input.GetAttribute("id")
                            + "," + "name:" + input.GetAttribute("name");
                        DisplayLog(strMsg);
                    }
                    

                    iElement = webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id(strId)));

                    return iElement;
                } catch (Exception ex) {
                    iCount++;
                    if (iCount == 3) {
                        throw ex;
                    }
                    Thread.Sleep(1000);
                }
            }

            return null;
        }

        private ReadOnlyCollection<IWebElement> GetElementsByTagName(WebDriverWait webDriverWait, string strTagName) {
            
            int iCount = 0;

            while (iCount < 3) {
                try {
                    var elements = webDriverWait.Until(c => c.FindElements(By.TagName(strTagName)));

                    return elements;
                } catch (Exception ex) {
                    iCount++;
                    if (iCount == 3) {
                        throw ex;
                    }
                    Thread.Sleep(1000);
                }
            }

            return null;
        }

        private ReadOnlyCollection<IWebElement> GetElementsByClassName(WebDriverWait webDriverWait, string strClassName) {

            int iCount = 0;

            while (iCount < 3) {
                try {
                    var elements = webDriverWait.Until(c => c.FindElements(By.ClassName(strClassName)));

                    return elements;
                } catch (Exception ex) {
                    iCount++;
                    if (iCount == 3) {
                        throw ex;
                    }
                    Thread.Sleep(1000);
                }
            }

            return null;
        }

        private bool IsElementExistByClassName(IWebDriver driver, string className) {

            var iElements = driver.FindElements(By.ClassName(className));

            bool isExist = (iElements != null && iElements.Count > 0);

            return isExist;
        }

        public void DispatchSelenium() {
            if (m_driver != null) {
                m_driver.Close();
                m_driver.Quit();
                m_driver = null;
            }
        }

        private void DisplayLog(string msg) {
            Console.WriteLine(msg);
        }

//        public void LoadUserPhotosxxx() {
//            //https://www.office.com
//            ChromeOptions options = new ChromeOptions();
           
//            //options.AddArguments("test-type");
//            //options.AddArguments("no-sandbox");
//            ////Fix for cannot get automation extension
//            //options.AddArguments("disable-extensions");
//            //options.AddArguments("start-maximized");
//            //options.AddArguments("--js-flags=--expose-gc");
//            //options.AddArguments("disable-plugins");
//            //options.AddArguments("--enable-precise-memory-info");
//            //options.AddArguments("--disable-popup-blocking");
//            //options.AddArguments("--disable-default-apps");
//            //options.AddArguments("test-type=browser");
//            //options.AddArguments("disable-infobars");
            
//            options.AddAdditionalCapability("useAutomationExtension", false);

//            using (IWebDriver driver = new ChromeDriver(options)) {
//                driver.Url = "https://www.office.com/";

//                WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_IN_SECONDS));

//                var btnLogin = webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("hero-banner-sign-in-to-office-365-link")));
//                //var btnLogin = driver.FindElement(By.Id("hero-banner-sign-in-to-office-365-link"));
//                btnLogin.Click();



//                //var txtUserName = driver.FindElement(By.Id("i0116"));
//                var txtUserName = webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("i0116")));
//                txtUserName.SendKeys("kamil.sykora@otis.com");

//                //var btnNext = driver.FindElement(By.Id("idSIButton9"));
//                var btnNext = webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("idSIButton9")));
//                btnNext.Click();

//                //var divUserAccount = driver.FindElement(By.Id("aadTileTitle"));
//                var divUserAccount = webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("aadTile")));
//                divUserAccount.Click();

//                var txtPwd = webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("i0118")));
//                txtPwd.SendKeys("Len6tini.");

//                var btnLoginUser = webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("idSIButton9")));
//                btnLoginUser.Click();

//                var btnNo = webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("idBtn_Back")));
//                btnNo.Click();


//                var btnOutlook = webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("ShellMail_link")));
//                btnOutlook.Click();

//                Thread.Sleep(3000);

//                //var btnNewMail = webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("Region_1")));

//                driver.Navigate().GoToUrl("https://outlook.office.com/owa/service.svc/s/GetPersonaPhoto?email=dusan.ondirko@otis.com&UA=0&size=HR240x240");

//                //driver.Url = "https://outlook.office.com/owa/service.svc/s/GetPersonaPhoto?email=kamil.sykora@otis.com&UA=0&size=HR240x240";
//                ////"loginfmt"
//                //var divOutlook = driver.FindElement(By.ClassName("webDriverWait"));
//                //divOutlook.Click();

//                //WebElement logo = driver.findElement(By.cssSelector(".image-logo"));

//                //String logoSRC = logo.getAttribute("src");

//                //Uri imageURL = new Uri(logoSRC);

//                //BufferedImage saveImage = ImageIO.read(imageURL);

//                //ImageIO.write(saveImage, "png", new File("logo-image.png"));


//                var base64string = (driver as IJavaScriptExecutor).ExecuteScript(@"
//    var c = document.createElement('canvas');
//    var ctx = c.getContext('2d');
//    var imgs = document.getElementsByTagName('img');
//var img = imgs[0];
//    c.height=img.naturalHeight;
//    c.width=img.naturalWidth;
//    ctx.drawImage(img, 0, 0,img.naturalWidth, img.naturalHeight);
//    var base64String = c.toDataURL();
//    return base64String;
//    ") as string;

//                var base64 = base64string.Split(',').Last();
//                using (var stream = new MemoryStream(Convert.FromBase64String(base64))) {
//                    using (var bitmap = new Bitmap(stream)) {
//                        var filepath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ImageName.png");
//                        bitmap.Save(filepath, ImageFormat.Png);
//                    }
//                }


//                //ITakesScreenshot ssdriver = driver as ITakesScreenshot;
//                //Screenshot screenshot = ssdriver.GetScreenshot();

//                //Screenshot tempImage = screenshot;

//                //tempImage.SaveAsFile(@"C:\temp\full.png", ScreenshotImageFormat.Png);

//                ////replace with the XPath of the image element
//                ////IWebElement my_image = driver.FindElement(By.XPath("//*[@id=\"hplogo\"]/canvas[1]"));
//                //var imgs = driver.FindElements(By.TagName("img"));
//                //IWebElement my_image = imgs[0];

//                //Point point = my_image.Location;
//                //int width = my_image.Size.Width;
//                //int height = my_image.Size.Height;

//                //Rectangle section = new Rectangle(point, new Size(width, height));
//                //Bitmap source = new Bitmap(@"C:\temp\full.png");
//                //Bitmap final_image = CropImage(source, section);

//                //final_image.Save(@"C:\temp\image.jpg");

//                //Thread.Sleep(5000);

//                /*
//                 * WebDriverWait wait = new WebDriverWait(getDriver(), timeOut);
//WebElement element = wait.until(ExpectedConditions.visibilityOfElementLocated(By.id("MainContent_lblLandmarkUPRN")));

//                 * */
//            }
//        }

//        public Bitmap CropImage(Bitmap source, Rectangle section) {
//            Bitmap bmp = new Bitmap(section.Width, section.Height);
//            Graphics g = Graphics.FromImage(bmp);
//            g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
//            return bmp;
//        }
        #endregion
    }

    public class TooMuchErrorsEx : Exception {
        public TooMuchErrorsEx(string errMsg) : base(errMsg) {
        }
    }
}
