using Kamsyk.Reget.Model;
using Kamsyk.Reget.Model.ExtendedModel;
using Kamsyk.Reget.Model.Repositories;
using Kamsyk.Reget.TestsIntegration.BaseTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Kamsyk.Reget.Model.Repositories.UserRepository;

namespace Kamsyk.Reget.TestsIntegration.Controllers {
    [TestClass]
    public class ParticipantControllerTest : BaseTestIntegration {
        #region Constructor
        public ParticipantControllerTest() {
            if (!IsTestDbConnected) {
                throw new Exception("Test DB is not connected");
            }
        }
        #endregion

        #region Test Methods
        [TestMethod]
        public void ZzInt_NewUser_IsSerachKeySet() {
            int partId = -1;
            try {
                //Assign
                string userName = "arnostrehorarnostek";
                UserRepository userRepository = new UserRepository();
                var tmpParticipant = userRepository.GetParticipantByUserName(userName);
                if (tmpParticipant != null) {
                    userRepository.DeleteUser(tmpParticipant.id);
                }

                //Act
                var company = new CompanyRepository().GetCompanyById(0);

                ParticipantsExtended participant = new ParticipantsExtended();
                participant.id = -1;
                //participant.office_id = 0;
                participant.office_name = company.country_code;
                participant.first_name = "Arnošt";
                participant.surname = "Řehoř";
                participant.user_name = userName;
                participant.email = userName;
                List<string> msg;
                int errId = 0;
                partId = userRepository.SaveParticipantData(participant, 0, out msg, out errId);

                //Assert
                var checkPart = userRepository.GetParticipantById(partId);
                bool isOK = true;
                if (checkPart.first_name_search_key != "arnost") {
                    isOK = false;
                }
                if (checkPart.surname_search_key != "rehor") {
                    isOK = false;
                }
                if (checkPart.user_search_key != "rehor arnost") {
                    isOK = false;
                }

                Assert.IsTrue(isOK);
            } catch {
                Assert.Fail();
            } finally {
                new UserRepository().DeleteUser(partId);
            }
        }

        [TestMethod]
        public void ZzInt_ReplaceUser() {
            try {
                //Arrange
                int companyId = 0;
                UserRepository userRepository = new UserRepository();
                List<Participants> requestors = userRepository.GetActiveParticipantInRoleByCompanyId(companyId, (int)UserRole.Requestor);
                List<Participants> targetRequestors = userRepository.GetActiveParticipantByCompanyId(companyId);
                int reqCount = requestors.Count;
                Random r = new Random();
                int iIndexFrom = r.Next(reqCount - 1);
                Participants partFrom = requestors.ElementAt(iIndexFrom);

                //var userRole = partFrom.ParticipantRole_CentreGroup.Select(
                //    x => x.role_id == (int)UserRole.Requestor ||
                //    x.role_id == (int)UserRole.Orderer ||
                //    x.role_id == (int)UserRole.ApprovalManager).ToList();

                //while (userRole == null || userRole.Count == 0) {
                //    userRole = partFrom.ParticipantRole_CentreGroup.Select(
                //        x => x.role_id == (int)UserRole.Requestor ||
                //        x.role_id == (int)UserRole.Orderer ||
                //        x.role_id == (int)UserRole.ApprovalManager).ToList();
                //}

                int iIndexTo = -1;
                while (iIndexTo < 0 || iIndexTo == iIndexFrom) {
                    iIndexTo = r.Next(targetRequestors.Count - 1);
                }
                Participants partTo = targetRequestors.ElementAt(iIndexTo);

                //Act
                using (IWebDriver driver = GetWebDriver(0, "en-US")) {
                    //Arrange
                    string url = AppRootUrl + "Participant/ReplaceUser";
                    driver.Url = url;
                    WaitUntilLoadDialogIsClosed(driver);

                    WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                    IWebElement txtUserToBeReplaced = FindElementById(webDriverWait, "txtUserToBeReplaced");
                    txtUserToBeReplaced.SendKeys(partFrom.surname);
                    Thread.Sleep(1000);
                    txtUserToBeReplaced.SendKeys(" " + partFrom.first_name);
                    Thread.Sleep(2000);

                    var li = FindautoCompleteOptionsElements(driver, webDriverWait, "txtUserToBeReplaced");
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    js = (IJavaScriptExecutor)driver;
                    js.ExecuteScript("arguments[0].click();", li[0]);
                    Thread.Sleep(3000);

                    IWebElement divReplace_divRequestor = FindElementById(webDriverWait, "divReplace_divRequestor");
                    var ckbs = divReplace_divRequestor.FindElements(By.TagName("md-checkbox"));
                    int cgId = Convert.ToInt16(ckbs.ElementAt(1).GetAttribute("id").Replace("cgreq_", ""));
                    ckbs.ElementAt(1).Click();

                    IWebElement txtReplaceRequestor = FindElementById(webDriverWait, "txtReplaceRequestor");
                    txtReplaceRequestor.SendKeys(partTo.surname);
                    Thread.Sleep(1000);
                    txtReplaceRequestor.SendKeys(" " + partTo.first_name);
                    Thread.Sleep(2000);

                    li = FindautoCompleteOptionsElements(driver, webDriverWait, "txtReplaceRequestor");
                    js = (IJavaScriptExecutor)driver;
                    js.ExecuteScript("arguments[0].click();", li[0]);
                    Thread.Sleep(3000);

                    IWebElement btnReplaceRequestor = FindElementById(webDriverWait, "btnReplaceRequestor");
                    btnReplaceRequestor.Click();
                    Thread.Sleep(3000);

                    //Check Role
                    var partFromCheck = userRepository.GetParticipantById(partFrom.id);
                    var roleCheck = (from prCg in partFromCheck.ParticipantRole_CentreGroup
                                     where prCg.role_id == (int)UserRole.Requestor &&
                                     prCg.centre_group_id == cgId
                                     select prCg).FirstOrDefault();
                    if (roleCheck != null) {
                        Assert.Fail();
                        return;
                    }

                    var partToCheck = userRepository.GetParticipantById(partTo.id);
                    roleCheck = (from prCg in partToCheck.ParticipantRole_CentreGroup
                                 where prCg.role_id == (int)UserRole.Requestor &&
                                 prCg.centre_group_id == cgId
                                 select prCg).FirstOrDefault();
                    if (roleCheck == null) {
                        Assert.Fail();
                        return;
                    }

                    //Revert
                    txtUserToBeReplaced.Clear();
                    txtUserToBeReplaced.SendKeys(partTo.surname);
                    Thread.Sleep(1000);
                    txtUserToBeReplaced.SendKeys(" " + partTo.first_name);
                    Thread.Sleep(2000);

                    li = FindautoCompleteOptionsElements(driver, webDriverWait, "txtUserToBeReplaced");
                    js = (IJavaScriptExecutor)driver;
                    js.ExecuteScript("arguments[0].click();", li[0]);
                    Thread.Sleep(3000);

                    txtReplaceRequestor.SendKeys(partFrom.surname);
                    Thread.Sleep(1000);
                    txtReplaceRequestor.SendKeys(" " + partFrom.first_name);
                    Thread.Sleep(2000);

                    li = FindautoCompleteOptionsElements(driver, webDriverWait, "txtReplaceRequestor");
                    js = (IJavaScriptExecutor)driver;
                    js.ExecuteScript("arguments[0].click();", li[0]);
                    Thread.Sleep(3000);

                    divReplace_divRequestor = FindElementById(webDriverWait, "divReplace_divRequestor");
                    ckbs = divReplace_divRequestor.FindElements(By.TagName("md-checkbox"));
                    foreach (var ckb in ckbs) {
                        if (ckb.GetAttribute("id") == "cgreq_" + cgId) {
                            ckb.Click();
                            break;
                        }
                    }
                    Thread.Sleep(2000);

                    btnReplaceRequestor.Click();
                }

                //Assert
                Assert.IsTrue(true);
            } catch (Exception ex) {
                new TestFailRepository().SaveTestFail("ZzInt_CentreGroupAddNew_CgAlreadyExists", ex.ToString());
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ZzInt_DeactivateUser_UserIsAppManInNonActiveCommodity_UserDeleted() {
            int userId = -1;
            //int origAppManId = -1;
            int cgId = -1;
            int pgLimitId = -1;
            

            Manager_Role origAppMan = null;

            try {
                //Arange
                PgRepository pgRepository = new PgRepository();
                var deactPg = pgRepository.GetRandomDeactivatedPgwithCgAppMan();
                
                if (deactPg == null) {
                    int deactPgId = pgRepository.DeactivatePg();
                    deactPg = pgRepository.GetPgById(deactPgId);
                }

                UserRepository userRepository = new UserRepository();
                var participantsNotAppMan = userRepository.GetActiveParticipantNotInRole((int)UserRole.ApprovalManager);
                var participantsNotOrderer = userRepository.GetActiveParticipantNotInRole((int)UserRole.Orderer);

                Participants selNotOrd = null;
                Participants appMan = null;
                int iIndex = 0;
               
                //int randIndex = new Random().Next(participantsNotAppMan.Count - 10, participantsNotAppMan.Count - 1);
                int randIndex = new Random().Next(0, participantsNotAppMan.Count - 1);
                iIndex = randIndex;
                do {
                    appMan = participantsNotAppMan.ElementAt(iIndex);
                    selNotOrd = (from participantsNotOrdererDb in participantsNotOrderer
                                 where participantsNotOrdererDb.id == appMan.id
                                 select participantsNotOrdererDb).FirstOrDefault();

                    iIndex++;
                } while (selNotOrd == null && iIndex < participantsNotAppMan.Count);

                if (selNotOrd == null) {
                    iIndex = randIndex;
                    do {
                        appMan = participantsNotAppMan.ElementAt(iIndex);
                        selNotOrd = (from participantsNotOrdererDb in participantsNotOrderer
                                     where participantsNotOrdererDb.id == appMan.id
                                     select participantsNotOrdererDb).FirstOrDefault();

                        iIndex--;
                    } while (selNotOrd == null && iIndex >= 0);
                }

                //appMan = new UserRepository().GetParticipantById(756);

                userId = appMan.id;
                
                cgId = deactPg.Centre_Group.ElementAt(0).id;
                foreach (var limit in deactPg.Purchase_Group_Limit) {
                    if (limit.Manager_Role != null && limit.Manager_Role.Count > 0) {
                        pgLimitId = deactPg.Purchase_Group_Limit.ElementAt(0).limit_id;
                        break;
                    }
                }

                origAppMan = new ManagerRoleRepository().SetApprovalManager(userId, cgId, pgLimitId);


                //Act
                int currUserId = 0;
                using (IWebDriver driver = GetWebDriver(currUserId, "en-US")) {
                    //Arrange
                    string url = AppRootUrl + "Participant";
                    driver.Url = url;
                    WaitUntilLoadDialogIsClosed(driver);

                    WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                    var txtFilters = driver.FindElements(By.ClassName("ui-grid-filter-input"));
                    var txtFilterSurname = txtFilters.ElementAt(0);
                    var txtFilterFirstname = txtFilters.ElementAt(1);

                    txtFilterSurname.SendKeys(appMan.surname);
                    txtFilterFirstname.SendKeys(appMan.first_name);
                    Thread.Sleep(1000);

                    var btnEditActions = driver.FindElements(By.ClassName("reget-btn-grid-edit"));
                    btnEditActions.ElementAt(0).Click();
                    Thread.Sleep(1000);

                    var btnCkbs = driver.FindElements(By.ClassName("md-checked"));
                    btnCkbs.ElementAt(0).Click();

                    var btnSaveActions = driver.FindElements(By.ClassName("reget-btn-grid-save"));
                    btnSaveActions.ElementAt(0).Click();
                    Thread.Sleep(1000);

                    
                }


                //Assert
                bool isOk = true;
                var part = new UserRepository().GetParticipantById(userId);
                if (part.active == true) {
                    Assert.Fail();
                    return;
                }

                var pg = pgRepository.GetPgById(deactPg.id);
                if (pg.active == true) {
                    Assert.Fail();
                    return;
                }

                var cg = new CentreGroupRepository().GetCentreGroupFullById(cgId);
                var prCg = (from prCgDb in cg.ParticipantRole_CentreGroup
                            where prCgDb.participant_id == userId
                            && prCgDb.role_id == (int)UserRole.ApprovalManager
                            select prCgDb).FirstOrDefault();
                if (prCg != null) {
                    Assert.Fail();
                    return;
                }


                var manRole = new ManagerRoleRepository().GetManagerRoleByLimitIdUserId(pgLimitId, userId);
                if (manRole != null) {
                    Assert.Fail();
                    return;
                }

                Assert.IsTrue(isOk);
            } catch (Exception ex) {
                throw ex;
            } finally {
                //Revert Changes
                new UserRepository().ActivateUser(userId);
                new ManagerRoleRepository().RemoveApprovalManager(userId, origAppMan, cgId, pgLimitId);
            }
        }

        [TestMethod]
        public void ZzInt_DeactivateUser_UserIsAppManInActiveCommodityNonActiveCg() {
            int userId = -1;
            //int origAppManId = -1;
            int cgId = -1;
            int pgLimitId = -1;
            int pgId = -1;

            Manager_Role origAppMan = null;

            try {
                //Arange
                PgRepository pgRepository = new PgRepository();
                var activePg = pgRepository.GetActivePgInDeactivatedCg();
                pgId = activePg.id;
                
                UserRepository userRepository = new UserRepository();
                var participants = userRepository.GetActiveParticipantNotInRole((int)UserRole.ApprovalManager);

                int randIndex = new Random().Next(participants.Count - 10, participants.Count - 1);

                Participants appMan = participants.ElementAt(randIndex);
                userId = appMan.id;


                cgId = activePg.Centre_Group.ElementAt(0).id;
                foreach(var limit in activePg.Purchase_Group_Limit) {
                    if (limit.Manager_Role != null && limit.Manager_Role.Count > 0) {
                        pgLimitId = limit.limit_id;
                        break;
                    }
                }
                
                origAppMan = new ManagerRoleRepository().SetApprovalManager(appMan.id, cgId, pgLimitId);


                //Act
                int currUserId = 0;
                using (IWebDriver driver = GetWebDriver(currUserId, "en-US")) {
                    //Arrange
                    string url = AppRootUrl + "Participant";
                    driver.Url = url;
                    WaitUntilLoadDialogIsClosed(driver);

                    WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                    var txtFilters = driver.FindElements(By.ClassName("ui-grid-filter-input"));
                    var txtFilterSurname = txtFilters.ElementAt(0);
                    var txtFilterFirstname = txtFilters.ElementAt(1);

                    txtFilterSurname.SendKeys(appMan.surname);
                    txtFilterFirstname.SendKeys(appMan.first_name);
                    Thread.Sleep(1000);

                    var btnEditActions = driver.FindElements(By.ClassName("reget-btn-grid-edit"));
                    btnEditActions.ElementAt(0).Click();
                    Thread.Sleep(1000);

                    var btnCkbs = driver.FindElements(By.ClassName("md-checked"));
                    btnCkbs.ElementAt(0).Click();

                    var btnSaveActions = driver.FindElements(By.ClassName("reget-btn-grid-save"));
                    btnSaveActions.ElementAt(0).Click();
                    Thread.Sleep(1000);

                    Thread.Sleep(1000);
                }


                //Assert
                bool isOk = true;
                var part = new UserRepository().GetParticipantById(userId);
                if (part.active == true) {
                    isOk = false;
                }

                var pg = new PgRepository().GetPgById(activePg.id);
                if (pg.active == true) {
                    isOk = false;
                }

                var cg = new CentreGroupRepository().GetCentreGroupFullById(cgId);
                var prCg = (from prCgDb in cg.ParticipantRole_CentreGroup
                            where prCgDb.participant_id == userId
                            && prCgDb.role_id == (int)UserRole.ApprovalManager
                            select prCgDb).FirstOrDefault();
                if (prCg != null) {
                    isOk = false;
                }


                var manRole = new ManagerRoleRepository().GetManagerRoleByLimitIdUserId(pgLimitId, userId);
                if (manRole != null) {
                    isOk = false;
                }

                Assert.IsTrue(isOk);
            } catch (Exception ex) {
                throw ex;
            } finally {
                //Revert Changes
                new UserRepository().ActivateUser(userId);
                new ManagerRoleRepository().RemoveApprovalManager(userId, origAppMan, cgId, pgLimitId);
                new PgRepository().ActivatePg(pgId);
            }
        }

        [TestMethod()]
        public void LoadUserPhoto() {
            ParticipantPhoto pp = new ParticipantPhoto();
            pp.participant_id = 0;
            pp.modif_date = DateTime.Now;
            byte[] img = File.ReadAllBytes(@"c:\Temp\RegetPhoto1.jpg");
            pp.user_picture_240 = img;

            UserPhotoRepository userPhotoRepository = new UserPhotoRepository();

            userPhotoRepository.SaveParticipantPhotoData(pp);

            // Assert
            Assert.IsTrue(true);
        }
        #endregion
    }
}
