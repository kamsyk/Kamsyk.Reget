using Kamsyk.Reget.Controllers;
using Kamsyk.Reget.Controllers.RegetExceptions;
using Kamsyk.Reget.Model;
using Kamsyk.Reget.Model.Common;
using Kamsyk.Reget.Model.ExtendedModel;
using Kamsyk.Reget.Model.ExtendedModel.HttpResult;
using Kamsyk.Reget.Model.Repositories;
using Kamsyk.Reget.TestsIntegration.BaseTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using static Kamsyk.Reget.Model.Repositories.RequestRepository;
using static Kamsyk.Reget.Model.Repositories.UserRepository;

namespace Kamsyk.Reget.TestsIntegration.Controllers {
    [TestClass]
    public class ParticipantSubstControllerTest : BaseTestIntegration {
        #region Constructor
        public ParticipantSubstControllerTest() {
            if (!IsTestDbConnected) {
                throw new Exception("Test DB is not connected");
            }
        }
        #endregion


        #region Test Methods
        [TestMethod]
        public void ZzInt_AddSubstitutionNormalUser_NoApproval() {
            try {
                //Arrange
                Participants substituted = GetAppManApprovalNotNeeded();
                Participants substitutee = GetOrdinarySubstitutee(substituted.id);
                int addedSubstId = -1;
                
                //Act
                bool isOk = true;

                Participant_Substitute substitution = null;
                #region Add Substitution
                try {
                    using (IWebDriver driver = GetWebDriver(substituted.id, "en-US")) {
                        string url = AppRootUrl + "Participant/UserSubstitution";
                        driver.Url = url;
                        WaitUntilLoadDialogIsClosed(driver);

                        isOk = AddSubstitution(driver, substituted, substitutee);
                        if (!isOk) {
                            throw new Exception("Add Substitution Failed");
                        }

                        //Check new substitution
                        ReadOnlyCollection<IWebElement> rows = driver.FindElements(By.ClassName("ui-grid-row"));
                        if (rows == null || rows.Count == 0) {
                            throw new Exception("Add Substitution Check 1 Failed");
                        }

                        //Check DB
                        substitution = CheckLastSubstitution(
                            substituted, 
                            substitutee, 
                            ApproveStatus.NotNeeded);
                        addedSubstId = substitution.id;
                        //substitution = new SubstitutionRepository().GetLastSubstitute();
                        //addedSubstId = substitution.id;
                        //if (substitution.substituted_user_id != substituted.id 
                        //    || substitution.substitute_user_id != substitutee.id) {
                        //    throw new Exception("Add Substitution Check 2 Failed");
                        //}
                        //if (substitution.approval_status != (int)ApproveStatus.NotNeeded) {
                        //    throw new Exception("Add Substitution Check 3 Failed");
                        //}
                        //if (substitution.author_id != substituted.id) {
                        //    throw new Exception("Add Substitution Check 4 Failed");
                        //}

                    }
                } catch (Exception ex) {
                    throw ex;
                } finally {
                    SwitchUserToDefaultTestUser();
                }
                #endregion

                #region Other User read Only
                try {
                    var participants = new UserRepository().GetActiveParticipantInRoleByCompanyId(substituted.company_id, (int)UserRole.ApprovalManager);
                    int delPartId = substituted.id;
                    Participants participant = null;
                    int iCount = 0;
                    while (delPartId == substituted.id && iCount < 50) {
                        int randomIndex = new Random().Next(0, participants.Count - 1);
                        participant = participants.ElementAt(randomIndex);
                        delPartId = participant.id;
                        iCount++;
                    }
                    WebDriver = null;
                    using (IWebDriver driver = GetWebDriver(participant.id, "en-US")) {
                        string url = AppRootUrl + "Participant/UserSubstitution";
                        driver.Url = url;
                        WaitUntilLoadDialogIsClosed(driver);

                        WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_IN_SECONDS));
                        if(IsElementExistByClassName(driver, "reget-grid-footer-button-last")) { 
                        //if (driver.FindElements(By.ClassName("reget-grid-footer-button-last")).Count() > 0) {
                            IWebElement btnLast = webDriverWait.Until(c => c.FindElement(By.ClassName("reget-grid-footer-button-last")));
                            btnLast.Click();
                        }

                        Thread.Sleep(2000);

                        ReadOnlyCollection<IWebElement> rows = driver.FindElements(By.ClassName("ui-grid-row"));
                        IWebElement lastRow = rows[rows.Count - 1];

                        //Try to Delete it from DB
                        UserSubstitutionExtended userSubstitutionExtended = new UserSubstitutionExtended();
                        ParticipantController participantController = new ParticipantController();
                        BaseRepository<Participant_Substitute>.SetValues(substitution, userSubstitutionExtended);
                        try {
                            RegetUser regetUser = new RegetUser();
                            regetUser.Participant = participant;
#if TEST
                            participantController.CurrentUser = regetUser;
#endif
                            var result = participantController.DeactivateUserSubstitution(userSubstitutionExtended);
                            if (result is JsonResult) {
                                JsonResult c = (JsonResult)result;
                                HttpResult httpRes = (HttpResult)c.Data;
                                if (httpRes.error_id != HttpResult.NOT_AUTHORIZED_ERROR) {
                                    throw new Exception("Substitution not authorized user delete1");
                                }
                                //if (c.Content != "Not authorized to update user substitution") {
                                //    throw new Exception("Substitution not authorized user delete1");
                                //} 
                            } else { 
                                Assert.Fail();
                                return;
                            }
                            
                        } catch (Exception ex) {
                            if (ex is ExNotAuthorizedUpdateUser) {
                                isOk = true;
                            } else {
                                throw new Exception("Substitution not authorized user delete2"); 
                            }
                        }
                    }
                } catch (Exception ex) {
                    throw ex;
                } finally {
                    SwitchUserToDefaultTestUser();
                }
#endregion

                #region Author Delete substitution
                try {
                    DeactivateSubstitution(substituted.id, addedSubstId);
                    //WebDriver = null;
                    //using (IWebDriver driver = GetWebDriver(substituted.id, "en-US")) {
                    //    string url = AppRootUrl + "Participant/UserSubstitution";
                    //    driver.Url = url;
                    //    WaitUntilLoadDialogIsClosed(driver);

                    //    WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_IN_SECONDS));
                    //    if (driver.FindElements(By.ClassName("reget-grid-footer-button-last")).Count() > 0) {
                    //        IWebElement btnLast = webDriverWait.Until(c => c.FindElement(By.ClassName("reget-grid-footer-button-last")));
                    //        btnLast.Click();
                    //    }

                    //    Thread.Sleep(2000);
                                               
                    //    ReadOnlyCollection<IWebElement> rows = driver.FindElements(By.ClassName("ui-grid-row"));
                    //    IWebElement lastRow = rows[rows.Count - 1];


                    //    IWebElement btnDel = lastRow.FindElement(By.ClassName("reget-btn-grid-delete"));
                    //    btnDel.Click();
                    //    Thread.Sleep(1000);

                    //    //Check confirm text
                    //    var mdDialogContent = driver.FindElement(By.ClassName("md-dialog-content"));
                    //    var ps = mdDialogContent.FindElements(By.TagName("p"));
                    //    if (String.IsNullOrWhiteSpace(ps[0].GetAttribute("innerHTML"))) {
                    //        throw new Exception("Delete button was not found");
                    //    }

                    //    IWebElement btnYes = driver.FindElement(By.ClassName("md-cancel-button"));
                    //    btnYes.Click();
                    //    Thread.Sleep(1000);

                    //    //Check DB
                    //    //record is deactivated only
                    //    substitution = new SubstitutionRepository().GetLastSubstitute();
                    //    if (substitution != null
                    //        && substitution.id == addedSubstId
                    //        && substitution.active == false) {
                    //        isOk = true;
                    //    } else {
                    //        throw new Exception("Substitution was not deactivated");
                    //    }

                    //}
                } catch (Exception ex) {
                    throw ex;
                } finally {
                    SwitchUserToDefaultTestUser();
                }
                #endregion

                //Assert
                Assert.IsTrue(isOk);
            } catch (Exception ex) {
                new TestFailRepository().SaveTestFail("ZzInt_AddSubstitutionNormalUser_WithApproval", ex.ToString());
                Assert.Fail();
            }
        }


        [TestMethod]
        public void ZzInt_AddSubstitutionNormalUser_WithApprovalApprove() {
            try {
                //Arrange
                Participants substituted = GetAppManApprovalNeeded();
                Participants substitutee = GetOrdinarySubstitutee(substituted.id);
                int addedSubstId = -1;
                Participant_Substitute substitution = null;

                //Act
                bool isOk = true;
                #region Create Substitution
                try {
                    using (IWebDriver driver = GetWebDriver(substituted.id, "en-US")) {
                        string url = AppRootUrl + "Participant/UserSubstitution";
                        driver.Url = url;
                        WaitUntilLoadDialogIsClosed(driver);

                        isOk = AddSubstitution(driver, substituted, substitutee);
                        
                        //Check new substitution
                        ReadOnlyCollection<IWebElement> rows = driver.FindElements(By.ClassName("ui-grid-row"));
                        if (rows == null || rows.Count == 0) {
                            throw new Exception("Substitution row was not created");
                        }

                        //Check DB
                        substitution = CheckLastSubstitution(
                            substituted,
                            substitutee,
                            ApproveStatus.WaitForApproval);
                        addedSubstId = substitution.id;

                        //Cannot approve it
                        var lastRow = GetLastRow(driver);
                        var appElements = lastRow.FindElements(By.ClassName("reget-btn-grid-approve"));
                        if (appElements != null && appElements.Count > 0) {
                            throw new Exception("Substitued user can approve substitution");
                        }

                        ParticipantController participantController = new ParticipantController();
                        RegetUser regetUser = new RegetUser();
                        regetUser.Participant = substituted;
#if TEST
                        participantController.CurrentUser = regetUser;
#endif

                        var result = participantController.ApproveSubstitution(addedSubstId);
                        if (result is JsonResult) {
                            JsonResult c = (JsonResult)result;
                            if (((HttpResult)c.Data).string_value != "NotAuthorized") {
                                throw new Exception("Substitution not authorized to be approved");
                            }
                        } else {
                            Assert.Fail();
                            return;
                        }

                    }
                } catch (Exception ex) {
                    throw ex;
                } finally {
                    SwitchUserToDefaultTestUser();
                }
                #endregion

                #region Approve it
                //Get App Man
                var appMen = new UserRepository().GetActiveParticipantInRoleByCompanyId(substitution.SubstitutedUser.company_id, (int)UserRole.SubstitutionApproveManager);
                var appMan = appMen.ElementAt(0);
                try {
                    WebDriver = null;
                    using (IWebDriver driver = GetWebDriver(appMan.id, "en-US")) {
                        string url = AppRootUrl + "Participant/UserSubstitution";
                        driver.Url = url;
                        WaitUntilLoadDialogIsClosed(driver);

                        var lastRow = GetLastRow(driver);
                        var appElements = lastRow.FindElements(By.ClassName("reget-btn-grid-approve"));
                        appElements[0].Click();
                        Thread.Sleep(1000);

                        //Check confirm text
                        var mdDialogContent = driver.FindElement(By.ClassName("md-dialog-content"));
                        var ps = mdDialogContent.FindElements(By.TagName("p"));
                        if (String.IsNullOrWhiteSpace(ps[0].GetAttribute("innerHTML"))) {
                            throw new Exception("Confirm button was not found");
                        }

                        IWebElement btnYes = driver.FindElement(By.Id("btnPerform"));
                        btnYes.Click();
                        Thread.Sleep(1000);

                        //Check Status
                        var subst = new SubstitutionRepository().GetSubstitutionById(addedSubstId);
                        if (subst.approval_status != (int)ApproveStatus.Approved) {
                            throw new Exception("Substitution was not approved");
                        }

                    }
                } catch (Exception ex) {
                    throw ex;
                } finally {
                    SwitchUserToDefaultTestUser();
                }
                #endregion

                #region Deactivate Substitution
                try {
                    DeactivateSubstitution(substituted.id, addedSubstId);
          
                } catch (Exception ex) {
                    throw ex;
                } finally {
                    SwitchUserToDefaultTestUser();
                }
                #endregion

                //Assert
                Assert.IsTrue(isOk);
            } catch (Exception ex) {
                new TestFailRepository().SaveTestFail("ZzInt_AddSubstitutionNormalUser_WithApproval", ex.ToString());
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ZzInt_AddSubstitutionNormalUser_WithApprovalReject() {
            try {
                //Arrange
                Participants substituted = GetAppManApprovalNeeded();
                Participants substitutee = GetOrdinarySubstitutee(substituted.id);
                int addedSubstId = -1;
                Participant_Substitute substitution = null;

                //Act
                bool isOk = true;
                #region Create Substitution
                try {
                    using (IWebDriver driver = GetWebDriver(substituted.id, "en-US")) {
                        string url = AppRootUrl + "Participant/UserSubstitution";
                        driver.Url = url;
                        WaitUntilLoadDialogIsClosed(driver);

                        isOk = AddSubstitution(driver, substituted, substitutee);

                        //Check new substitution
                        ReadOnlyCollection<IWebElement> rows = driver.FindElements(By.ClassName("ui-grid-row"));
                        if (rows == null || rows.Count == 0) {
                            throw new Exception("Substitution row was not created");
                        }

                        //Check DB
                        substitution = CheckLastSubstitution(
                            substituted,
                            substitutee,
                            ApproveStatus.WaitForApproval);
                        addedSubstId = substitution.id;

                        //Cannot approve it
                        var lastRow = GetLastRow(driver);
                        var appElements = lastRow.FindElements(By.ClassName("reget-btn-grid-approve"));
                        if (appElements != null && appElements.Count > 0) {
                            throw new Exception("Substitued user can approve substitution");
                        }

                        ParticipantController participantController = new ParticipantController();
                        RegetUser regetUser = new RegetUser();
                        regetUser.Participant = substituted;
#if TEST
                        participantController.CurrentUser = regetUser;
#endif

                        var result = participantController.ApproveSubstitution(addedSubstId);
                        if (result is JsonResult) {
                            JsonResult c = (JsonResult)result;
                            if (((HttpResult)c.Data).string_value != "NotAuthorized") {
                                throw new Exception("Substitution not authorized to be approved");
                            }
                        } else {
                            Assert.Fail();
                            return;
                        }

                    }
                } catch (Exception ex) {
                    throw ex;
                } finally {
                    SwitchUserToDefaultTestUser();
                }
                #endregion

                #region Reject it
                //Get App Man
                var appMen = new UserRepository().GetActiveParticipantInRoleByCompanyId(substitution.SubstitutedUser.company_id, (int)UserRole.SubstitutionApproveManager);
                var appMan = appMen.ElementAt(0);
                try {
                    WebDriver = null;
                    using (IWebDriver driver = GetWebDriver(appMan.id, "en-US")) {
                        string url = AppRootUrl + "Participant/UserSubstitution";
                        driver.Url = url;
                        WaitUntilLoadDialogIsClosed(driver);

                        var lastRow = GetLastRow(driver);
                        var appElements = lastRow.FindElements(By.ClassName("reget-btn-grid-reject"));
                        appElements[0].Click();
                        Thread.Sleep(1000);

                        //Check confirm text
                        var mdDialogContent = driver.FindElement(By.ClassName("md-dialog-content"));
                        var ps = mdDialogContent.FindElements(By.TagName("p"));
                        if (String.IsNullOrWhiteSpace(ps[0].GetAttribute("innerHTML"))) {
                            throw new Exception("Confirm button was not found");
                        }

                        IWebElement btnYes = driver.FindElement(By.Id("btnPerform"));
                        btnYes.Click();
                        Thread.Sleep(1000);

                        //Check Status
                        var subst = new SubstitutionRepository().GetSubstitutionById(addedSubstId);
                        if (subst.approval_status != (int)ApproveStatus.Rejected) {
                            throw new Exception("Substitution was not rejected");
                        }
                        if (IsElementExistByClassName(driver, "reget-btn-grid-reject", lastRow)) {
                            throw new Exception("Reject button is available after reject");
                        }
                    }
                } catch (Exception ex) {
                    throw ex;
                } finally {
                    SwitchUserToDefaultTestUser();
                }
                #endregion

                //Assert
                Assert.IsTrue(isOk);
            } catch (Exception ex) {
                new TestFailRepository().SaveTestFail("ZzInt_AddSubstitutionNormalUser_WithApproval", ex.ToString());
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ZzInt_ReadOnlyUser_AddComment() {
            try {
                //Arrange
                var substs = new SubstitutionRepository().GetActiveSubstitutions();
                int iIndex = new Random().Next(0, substs.Count - 1);
                var subst = substs.ElementAt(iIndex);
                int companyId = subst.SubstitutedUser.company_id;
                var parts = new UserRepository().GetActiveParticipantNotInRoleByCompanyId(companyId, (int)UserRole.ApprovalManager);
                int iIndexPart = new Random().Next(0, parts.Count - 1);
                var part = parts.ElementAt(iIndexPart);

                //Act
                try {
                    WebDriver = null;
                    using (IWebDriver driver = GetWebDriver(part.id, "en-US")) {
                        string url = AppRootUrl + "Participant/UserSubstitution";
                        driver.Url = url;
                        WaitUntilLoadDialogIsClosed(driver);

                        var lastRow = GetLastRow(driver);
                        var appElements = lastRow.FindElements(By.ClassName("reget-btn-grid-three-dots"));
                        if (appElements.Count == 0) {
                            new TestFailRepository().SaveTestFail("ZzInt_AddSubstitutionNormalUser_WithApproval", "No Substitution Found");
                            Assert.Fail();
                        }
                        appElements[0].Click();
                        Thread.Sleep(2000);

                        WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));
                        var txtSubstRemark = FindElementByName(webDriverWait, "txtSubstRemark");
                        string strRemark = "Remark " + DateTime.Now; 
                        txtSubstRemark.SendKeys(strRemark);

                        var btnAddRemark = FindElementById(webDriverWait, "btnAddRemark");
                        btnAddRemark.Click();
                        Thread.Sleep(2000);

                        //Assert
                        var disc = new DiscussionRepository().GetLastDiscussion();
                        if (disc.App_Text_Store.text_content == strRemark) {
                            Assert.IsTrue(true);
                            return;
                        } else {
                            new TestFailRepository().SaveTestFail("ZzInt_AddSubstitutionNormalUser_WithApproval", "Remark does not match");
                            Assert.Fail();
                            return;
                        }
                    }
                } catch (Exception ex) {
                    new TestFailRepository().SaveTestFail("ZzInt_AddSubstitutionNormalUser_WithApproval", ex.ToString());
                    Assert.Fail();
                } finally {
                    SwitchUserToDefaultTestUser();
                }

                //Assert
                Assert.Fail();
            } catch (Exception ex) {
                new TestFailRepository().SaveTestFail("ZzInt_AddSubstitutionNormalUser_WithApproval", ex.ToString());
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ZzInt_Substitution_ReadOnly_RowCannotBeEdit() {
            SubstitutionRepository substitutionRepository = new SubstitutionRepository();
            int substId = -1;
            try {
                //Arrange
                int userId = 0;
                UserSubstitutionExtended subst = new UserSubstitutionExtended();
                subst.id = -1;
                subst.substituted_user_id = 10;
                subst.substitute_user_id = 20;
                subst.substitute_start_date = DateTime.Now;
                subst.substitute_end_date = DateTime.Now.AddDays(10);
                subst.approval_status = (int)ApproveStatus.Approved;
                //subst.companies_ids = ",0,";
                subst.active = true;

                List<string> msg = null;
                substId = substitutionRepository.SaveSubstitutionData(subst, userId, out msg);

                //Act
                WebDriver = null;
                using (IWebDriver driver = GetWebDriver(userId, "en-US")) {
                    string url = AppRootUrl + "Participant/UserSubstitution";
                    driver.Url = url;
                    WaitUntilLoadDialogIsClosed(driver);

                    var elements = driver.FindElements(By.ClassName("ui-grid-header-cell-primary-focus"));
                    foreach (var el in elements) {
                        string strHtml = el.GetAttribute("innerHTML");
                        if (strHtml.ToLower().IndexOf("last modification date") > -1) {

                            Actions actions = new Actions(driver);
                            actions.MoveToElement(el).Click().Perform();
                            Thread.Sleep(1000);

                            el.Click();
                            Thread.Sleep(2000);
                            var sortEl = el.FindElements(By.ClassName("ui-grid-icon-up-dir"));
                            int iCount = 0;
                            while ((sortEl == null || sortEl.Count == 0) && iCount < 6) {
                                el.Click();
                                Thread.Sleep(2000);
                                sortEl = el.FindElements(By.ClassName("ui-grid-icon-up-dir"));
                                iCount++;
                            }
                            //if (sortEl == null || sortEl.Count == 0) {
                            //    el.Click();
                            //    Thread.Sleep(2000);
                            //}

                            sortEl = el.FindElements(By.ClassName("ui-grid-icon-up-dir"));
                            if (sortEl == null || sortEl.Count == 0) {
                                throw new Exception("Not Sorted");
                            }

                            var lastRow = GetLastRow(driver);
                            var cells = lastRow.FindElements(By.ClassName("ui-grid-cell"));
                            cells[4].Click();

                            var btnSave = lastRow.FindElements(By.ClassName("reget-btn-save"));
                            if (btnSave == null || btnSave.Count == 0) {
                                Assert.IsTrue(true);
                                return;
                            } else {
                                new TestFailRepository().SaveTestFail("Substitution_ReadOnly_RowCannotBeEdit","Row is Editable");
                                Assert.Fail();
                                return;
                            }
                                                        
                        }
                    }
                }

                //Assert
                Assert.IsTrue(false);
            } catch (Exception ex) {
                new TestFailRepository().SaveTestFail("Substitution_ReadOnly_RowCannotBeEdit", ex.ToString());
                Assert.Fail();
            } finally {
                substitutionRepository.DeleteUserSubstitutionById(substId);
            }
        }

        [TestMethod]
        public void ZzInt_Substitution_UserCanSetsubsForHimselfOnly_FormIsLoadedWoError() {
            //Assert
            Assert.IsTrue(false);
        }
        #endregion

        #region Methods
        private Participants GetParticipantNotInRoles(int companyId, List<UserRole> userRoles) {
            UserRepository userRepository = new UserRepository();
            var users = userRepository.GetActiveParticipantNotInRoleByCompanyId(companyId, (int)userRoles.ElementAt(0));
            int iAppManIndex = new Random().Next(0, users.Count - 1);
            for (int i= iAppManIndex; i>=0; i--) {
                
                for (int j = 1; j < userRoles.Count; j++) {
                    bool isOutOfRole = IsUserOutOfRole(users.ElementAt(i), (int)userRoles[j]);
                    if (isOutOfRole) {
                        return users.ElementAt(i);
                    }
                                        
                }
               
            }

            for (int i = iAppManIndex; i < users.Count; i++) {
                //bool isOk = true;
                for (int j = 1; j < userRoles.Count; j++) {
                    bool isOutOfRole = IsUserOutOfRole(users.ElementAt(i), (int)userRoles[j]);
                    if (!isOutOfRole) {
                        return users.ElementAt(i);
                    }
                    
                }
           
            }

            return null;
        }

        private bool IsUserOutOfRole(Participants user, int userRoleId) {
            var roles = (from roleDb in user.ParticipantRole_CentreGroup
                         where roleDb.role_id == userRoleId
                         select roleDb).FirstOrDefault();
            if (roles != null) {
                return false;
            }


            var compRoles = (from roleDb in user.Participant_Office_Role
                             where roleDb.role_id == userRoleId
                             select roleDb).FirstOrDefault();
            if (compRoles != null) {
                return false;
            }

            return true;
        }

        private Participants GetAppManApprovalNeeded() {
            CompanyRepository companyRepository = new CompanyRepository();
            var companies = companyRepository.GetActiveCompaniesWithSubstApproval();
            int compCount = companies.Count();
            int iCompIndex = new Random().Next(0, compCount - 1);
            int selCompId = companies.ElementAt(iCompIndex);

            UserRepository userRepository = new UserRepository();
            var appMen = userRepository.GetActiveParticipantInRoleByCompanyId(selCompId, (int)UserRole.ApprovalManager);
            int appMenCount = appMen.Count();
            int iAppManIndex = new Random().Next(0, appMenCount - 1);
            var appMan = appMen.ElementAt(iAppManIndex);

            return appMan;
        }

        private Participants GetAppManApprovalNotNeeded() {
            CompanyRepository companyRepository = new CompanyRepository();
            var companies = companyRepository.GetActiveCompaniesWithoutSubstApproval();
            int compCount = companies.Count();
            int iCompIndex = new Random().Next(0, compCount - 1);
            int selCompId = companies.ElementAt(iCompIndex);

            UserRepository userRepository = new UserRepository();
            var appMen = userRepository.GetActiveParticipantInRoleByCompanyId(selCompId, (int)UserRole.ApprovalManager);
            int failCount = 0;
            while (appMen.Count() == 0 || failCount > 10) {
                iCompIndex++;
                if (iCompIndex >= companies.Count()) {
                    iCompIndex = 0;
                }
                selCompId = companies.ElementAt(iCompIndex);
                appMen = userRepository.GetActiveParticipantInRoleByCompanyId(selCompId, (int)UserRole.ApprovalManager);
                failCount++;
            }
            int appMenCount = appMen.Count();
            int iAppManIndex = new Random().Next(0, appMenCount - 1);
            var appMan = appMen.ElementAt(iAppManIndex);

            return appMan;
        }

        private Participants GetOrdinarySubstitutee(int substitutedId) {
            var participants = new UserRepository().GetActiveParticipants();
            int partCount = participants.Count();
            int iPartIndex = new Random().Next(0, partCount - 1);

            for (int i = iPartIndex; i >= 0; i--) {
                if (participants.ElementAt(i).id == substitutedId) {
                    continue;
                }

                var part = participants.ElementAt(i);

                var compRoles = (from compRolesDb in part.ParticipantRole_CentreGroup
                                 where compRolesDb.role_id == (int)UserRole.ApplicationAdmin
                                 || compRolesDb.role_id == (int)UserRole.OfficeAdministrator
                                 || compRolesDb.role_id == (int)UserRole.SubstituteCompanyManager
                                 || compRolesDb.role_id == (int)UserRole.SubstitutionApproveManager
                                 select compRolesDb).FirstOrDefault();

                if (compRoles == null) {
                    return part;
                }
            }


            for (int i = iPartIndex; i < participants.Count - 1; i++) {
                var part = participants.ElementAt(i);

                var compRoles = (from compRolesDb in part.ParticipantRole_CentreGroup
                                 where compRolesDb.role_id == (int)UserRole.ApplicationAdmin
                                 || compRolesDb.role_id == (int)UserRole.OfficeAdministrator
                                 || compRolesDb.role_id == (int)UserRole.SubstituteCompanyManager
                                 || compRolesDb.role_id == (int)UserRole.SubstitutionApproveManager
                                 select compRolesDb).FirstOrDefault();

                if (compRoles == null) {
                    return part;
                }
            }

            return null;
        }

        private bool AddSubstitution(
            IWebDriver driver, 
            Participants substituted,
            Participants substitutee) {

            bool isOk = false;


            WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitInSeconds));

            var txtUserSubstitutee = FindElementById(webDriverWait, "txtUserSubstitutee");
            txtUserSubstitutee.SendKeys(substitutee.surname + " " + substitutee.first_name);
            Thread.Sleep(2000);

            var li = FindautoCompleteOptionsElements(driver, webDriverWait, "txtUserSubstitutee");
            var js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].click();", li[0]);
            Thread.Sleep(2000);

            var ctdtTo = FindElementById(webDriverWait, "ctdtTo");
            var dtInputs = ctdtTo.FindElements(By.TagName("input"));
            dtInputs[0].SendKeys(DateTime.Now.AddMonths(1).ToString("MM/dd/yyyy"));

            //Remark
            var txtSubstRemark = FindElementById(webDriverWait, "txtSubstRemark");
            txtSubstRemark.SendKeys("Substituted remark.");

            var btnAddSubst = FindElementById(webDriverWait, "btnAddSubst");
            btnAddSubst.Click();

            Thread.Sleep(3000);

            var divErrMsg = FindElementById(webDriverWait, "divErrMsg");
            if (String.IsNullOrWhiteSpace(divErrMsg.GetAttribute("innerHTML"))) {
                isOk = true;
            } else {
                isOk = false;
            }

            return isOk;

        }

        private Participant_Substitute CheckLastSubstitution(
            Participants substituted,
            Participants substitutee,
            ApproveStatus requestedStatus) {

            var substitution = new SubstitutionRepository().GetLastSubstitute();
            int addedSubstId = substitution.id;
            if (substitution.substituted_user_id != substituted.id
                || substitution.substitute_user_id != substitutee.id) {
                throw new Exception("Add Substitution Check 2 Failed");
            }
            if (substitution.approval_status != (int)requestedStatus) {
                throw new Exception("Add Substitution Check 3 Failed");
            }
            if (substitution.author_id != substituted.id) {
                throw new Exception("Add Substitution Check 4 Failed");
            }

            return substitution;
        }

        private IWebElement GetLastRow(IWebDriver driver) {
            WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_IN_SECONDS));
            if (driver.FindElements(By.ClassName("reget-grid-footer-button-last")).Count() > 0) {
                IWebElement btnLast = webDriverWait.Until(c => c.FindElement(By.ClassName("reget-grid-footer-button-last")));
                btnLast.Click();
            }

            Thread.Sleep(2000);

            ReadOnlyCollection<IWebElement> rows = driver.FindElements(By.ClassName("ui-grid-row"));
            IWebElement lastRow = rows[rows.Count - 1];

            return lastRow;
        }

        private void DeactivateSubstitution(int substitutedId, int substitutionId) {
            WebDriver = null;
            using (IWebDriver driver = GetWebDriver(substitutedId, "en-US")) {
                string url = AppRootUrl + "Participant/UserSubstitution";
                driver.Url = url;
                WaitUntilLoadDialogIsClosed(driver);

                WebDriverWait webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_IN_SECONDS));
                if (driver.FindElements(By.ClassName("reget-grid-footer-button-last")).Count() > 0) {
                    IWebElement btnLast = webDriverWait.Until(c => c.FindElement(By.ClassName("reget-grid-footer-button-last")));
                    btnLast.Click();
                }

                Thread.Sleep(2000);

                //ReadOnlyCollection<IWebElement> rows = driver.FindElements(By.ClassName("ui-grid-row"));
                //IWebElement lastRow = rows[rows.Count - 1];
                IWebElement lastRow = GetLastRow(driver);


                IWebElement btnDel = lastRow.FindElement(By.ClassName("reget-btn-grid-delete"));
                btnDel.Click();
                Thread.Sleep(1000);

                //Check confirm text
                var mdDialogContent = driver.FindElement(By.ClassName("md-dialog-content"));
                var ps = mdDialogContent.FindElements(By.TagName("p"));
                if (String.IsNullOrWhiteSpace(ps[0].GetAttribute("innerHTML"))) {
                    throw new Exception("Delete button was not found");
                }

                IWebElement btnYes = driver.FindElement(By.Id("btnPerform"));
                btnYes.Click();
                Thread.Sleep(1000);

                //Check DB
                //record is deactivated only
                var substitution = new SubstitutionRepository().GetLastSubstitute();
                if (substitution != null
                    && substitution.id == substitutionId
                    && substitution.active == false) {

                } else {
                    throw new Exception("Substitution was not deactivated");
                }
            }
        }

#endregion
    }
}
