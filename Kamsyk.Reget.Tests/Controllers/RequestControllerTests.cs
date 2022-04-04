using Xunit;
using Kamsyk.Reget.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kamsyk.Reget.Model.ExtendedModel;
using Kamsyk.Reget.Model;
using Kamsyk.Reget.Model.Repositories;
using static Kamsyk.Reget.Model.Repositories.UserRepository;
using Kamsyk.Reget.Model.Repositories.Interfaces;
using Moq;
using System.Web.Mvc;
using Kamsyk.Reget.Model.ExtendedModel.HttpResult;
using Kamsyk.Reget.Model.Request.ExtendedModel;
using Kamsyk.Reget.Model.ExtendedModel.Request;
using static Kamsyk.Reget.Model.Repositories.RequestRepository;

namespace Kamsyk.Reget.Controllers.Tests {
    public class RequestControllerTests {
        [Fact()]
        public void SendForApproval_NotAuthorizedUser() {
            //Arrange
            //int userId = 124;
            //int centreId = 12;
            //int userRole = (int)UserRole.OfficeAdministrator;
            //int companyId = 0;

            //var mockUserRep = new Mock<IUserRepository>();
            //mockUserRep.Setup(x => x.IsAuthorized(userId, userRole, companyId)).Returns(false);

            var mockReqRep = new Mock<IRequestRepository>();
            mockReqRep.Setup(x => x.IsRequestorAuthorized(It.IsAny<int>(), It.IsAny<int>())).Returns(false);


            RegetUser currUser = new RegetUser();
            Participants participant = new Participants();
            participant.id = 0;
            currUser.Participant = participant;

            //Act
            RequestController requestController = new RequestController(mockReqRep.Object, null, currUser);
            RequestEventExtended requestExtended = new RequestEventExtended();
            JsonResult httpResult = requestController.SendForApproval(requestExtended) as JsonResult;

            //Assert
            HttpResult httpRes = httpResult.Data as HttpResult;
            Assert.True(httpRes.string_value == requestController.MSG_KEY_NOT_AUTHORIZED);
        }

        [Fact()]
        public void SendForApproval_MissingRequestorMandatoryItem() {
            //Arrange
            RequestController requestController = GetRequestControllerForMandatoryCheck();
            RequestEventExtended requestExtended = GetValidRequest();

            //Act
            requestExtended.requestor = null;
            JsonResult httpResult = requestController.SendForApproval(requestExtended) as JsonResult;

            //Assert
            HttpResult httpRes = httpResult.Data as HttpResult;
            Assert.True(httpRes.string_value == requestController.MSG_KEY_MISSING_MANDATORY_ITEM);
        }

        [Fact()]
        public void SendForApproval_MissingCentreIdMandatoryItem() {
            //Arrange
            RequestController requestController = GetRequestControllerForMandatoryCheck();
            RequestEventExtended requestExtended = GetValidRequest();

            //Act
            requestExtended.request_centre_id = null;
            JsonResult httpResult = requestController.SendForApproval(requestExtended) as JsonResult;

            //Assert
            HttpResult httpRes = httpResult.Data as HttpResult;
            Assert.True(httpRes.string_value == requestController.MSG_KEY_MISSING_MANDATORY_ITEM);
        }

        [Fact()]
        public void SendForApproval_MissingPgMandatoryItem() {
            //Arrange
            RequestController requestController = GetRequestControllerForMandatoryCheck();
            RequestEventExtended requestExtended = GetValidRequest();

            //Act
            requestExtended.purchase_group_id = null;
            JsonResult httpResult = requestController.SendForApproval(requestExtended) as JsonResult;

            //Assert
            HttpResult httpRes = httpResult.Data as HttpResult;
            Assert.True(httpRes.string_value == requestController.MSG_KEY_MISSING_MANDATORY_ITEM);
        }

        [Fact()]
        public void SendForApproval_MissingDeliveryDateMandatoryItem() {
            //Arrange
            RequestController requestController = GetRequestControllerForMandatoryCheck();
            RequestEventExtended requestExtended = GetValidRequest();

            //Act
            requestExtended.lead_time = null;
            JsonResult httpResult = requestController.SendForApproval(requestExtended) as JsonResult;

            //Assert
            HttpResult httpRes = httpResult.Data as HttpResult;
            Assert.True(httpRes.string_value == requestController.MSG_KEY_MISSING_MANDATORY_ITEM);
        }

        [Fact()]
        public void SendForApproval_MissingEstPriceMandatoryItem() {
            //Arrange
            RequestController requestController = GetRequestControllerForMandatoryCheck();
            RequestEventExtended requestExtended = GetValidRequest();

            //Act
            requestExtended.estimated_price = null;
            JsonResult httpResult = requestController.SendForApproval(requestExtended) as JsonResult;

            //Assert
            HttpResult httpRes = httpResult.Data as HttpResult;
            Assert.True(httpRes.string_value == requestController.MSG_KEY_MISSING_MANDATORY_ITEM);
        }

        [Fact()]
        public void SendForApproval_MissingCurrencyMandatoryItem() {
            //Arrange
            RequestController requestController = GetRequestControllerForMandatoryCheck();
            RequestEventExtended requestExtended = GetValidRequest();

            //Act
            requestExtended.currency_id = null;
            JsonResult httpResult = requestController.SendForApproval(requestExtended) as JsonResult;

            //Assert
            HttpResult httpRes = httpResult.Data as HttpResult;
            Assert.True(httpRes.string_value == requestController.MSG_KEY_MISSING_MANDATORY_ITEM);
        }

        [Fact()]
        public void SendForApproval_MissingAppMenMandatoryItem() {
            //Arrange
            RequestController requestController = GetRequestControllerForMandatoryCheck();
            RequestEventExtended requestExtended = GetValidRequest();

            //Act
            requestExtended.request_event_approval = null;
            JsonResult httpResult = requestController.SendForApproval(requestExtended) as JsonResult;

            //Assert
            HttpResult httpRes = httpResult.Data as HttpResult;
            Assert.True(httpRes.string_value == requestController.MSG_KEY_MISSING_MANDATORY_ITEM);
        }

        [Fact()]
        public void SendForApproval_MissingOrdererMandatoryItem() {
            //Arrange
            RequestController requestController = GetRequestControllerForMandatoryCheck();
            RequestEventExtended requestExtended = GetValidRequest();

            //Act
            requestExtended.orderer_id = null;
            JsonResult httpResult = requestController.SendForApproval(requestExtended) as JsonResult;

            //Assert
            HttpResult httpRes = httpResult.Data as HttpResult;
            Assert.True(httpRes.string_value == requestController.MSG_KEY_MISSING_MANDATORY_ITEM);
        }

        [Fact()]
        public void SendForApproval_MissingPrivacyMandatoryItem() {
            //Arrange
            RequestController requestController = GetRequestControllerForMandatoryCheck();
            RequestEventExtended requestExtended = GetValidRequest();

            //Act
            requestExtended.privacy_id = null;
            JsonResult httpResult = requestController.SendForApproval(requestExtended) as JsonResult;

            //Assert
            HttpResult httpRes = httpResult.Data as HttpResult;
            Assert.True(httpRes.string_value == requestController.MSG_KEY_MISSING_MANDATORY_ITEM);
        }

        [Fact()]
        public void SendForApproval_MultiOrderer_OK() {
            //Arrange
            RequestController requestController = GetRequestControllerForMandatoryCheck(true);
            RequestEventExtended requestExtended = GetValidRequest();

            //Act
            requestExtended.orderer_id = null;
            JsonResult httpResult = requestController.SendForApproval(requestExtended) as JsonResult;

            //Assert
            HttpResult httpRes = httpResult.Data as HttpResult;
            Assert.True(httpRes.string_value == null);

        }

        #region Methods
        private RequestController GetRequestControllerForMandatoryCheck() {
            return GetRequestControllerForMandatoryCheck(false);
        }

        private RequestController GetRequestControllerForMandatoryCheck(bool isMultiOrder) {
            var mockReqRep = new Mock<IRequestRepository>();
            mockReqRep.Setup(x => x.IsRequestorAuthorized(It.IsAny<int>(), It.IsAny<int>())).Returns(true);

            var mockPgRep = new Mock<IPgRepository>();
            Purchase_Group pg = new Purchase_Group();
            pg.is_approval_needed = true;
            pg.is_order_needed = true;
            pg.is_multi_orderer = isMultiOrder;
            mockPgRep.Setup(x => x.GetPgById(It.IsAny<int>())).Returns(pg);

            RegetUser currUser = new RegetUser();
            Participants participant = new Participants();
            participant.id = 0;
            currUser.Participant = participant;

            RequestController requestController = new RequestController(mockReqRep.Object, mockPgRep.Object, currUser);

            return requestController;
        }

        private RequestEventExtended GetValidRequest() {
            RequestEventExtended requestExtended = new RequestEventExtended();
            requestExtended.requestor = 0;
            requestExtended.request_centre_id = 0;
            requestExtended.purchase_group_id = 0;
            requestExtended.lead_time = DateTime.Now;
            requestExtended.estimated_price = 1;
            requestExtended.currency_id = 0;
            requestExtended.request_text = "request text";
            requestExtended.request_event_approval = new List<RequestEventApprovalExtended>();
            RequestEventApprovalExtended rep = new RequestEventApprovalExtended();
            requestExtended.request_event_approval.Add(rep);
            requestExtended.orderer_id = 0;
            requestExtended.privacy_id = 0;

            return requestExtended;
        }
        #endregion

        [Fact()]
        public void IsOrderAuthorizedOrderer_Yes() {
            //Arrange
            int userId = 56;
            Request_Event request = new Request_Event();
            request.orderer_id = userId;
            request.privacy_id = (int)PrivacyRepository.RequestPrivacy.Public;
            request.request_status = (int)RequestStatus.Ordered;

            //Act
            bool isAuthorized = new RequestController().IsOrderAuthorized(request, userId);

            //Assert
            Assert.True(isAuthorized);
        }

        [Fact()]
        public void IsOrderAuthorizedApproved_Yes() {
            //Arrange
            int userId = 56;
            Request_Event request = new Request_Event();
            request.orderer_id = userId;
            request.privacy_id = (int)PrivacyRepository.RequestPrivacy.Public;
            request.request_status = (int)RequestStatus.Approved;

            //Act
            bool isAuthorized = new RequestController().IsOrderAuthorized(request, userId);

            //Assert
            Assert.True(isAuthorized);
        }

        [Fact()]
        public void IsOrderAuthorizedWaitForApproval_No() {
            //Arrange
            int userId = 56;
            Request_Event request = new Request_Event();
            request.orderer_id = userId;
            request.privacy_id = (int)PrivacyRepository.RequestPrivacy.Public;
            request.request_status = (int)RequestStatus.WaitForApproval;

            //Act
            bool isAuthorized = new RequestController().IsOrderAuthorized(request, userId);

            //Assert
            Assert.False(isAuthorized);
        }
    }
}