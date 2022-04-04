using Xunit;
using Kamsyk.Reget.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Kamsyk.Reget.Model.Repositories.Interfaces;
using static Kamsyk.Reget.Model.Repositories.RequestRepository;

namespace Kamsyk.Reget.Model.Repositories.Tests {
    public class RequestRepositoryTests {
        [Fact()]
        public void GetIsRevertableTest_WaitForFirstApproval_Yes() {
            //Arrange
            var requestEvent = new Request_Event();
            var currentUserId = 0;
            requestEvent.request_status = (int)RequestStatus.WaitForApproval;
            requestEvent.requestor = currentUserId;
            requestEvent.Request_Event_Approval = new List<Request_Event_Approval>();
            Request_Event_Approval app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.WaitForApproval;
            app.app_level_id = 0;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            //Act
            bool isRevertable = new RequestRepository().GetIsRevertable(requestEvent, currentUserId);

            //Assert
            Assert.True(isRevertable);
        }

        [Fact()]
        public void GetIsRevertableTest_WaitForFirstApproval_DifferentRequestor() {
            //Arrange
            var requestEvent = new Request_Event();
            var currentUserId = 0;
            requestEvent.request_status = (int)RequestStatus.WaitForApproval;
            requestEvent.requestor = 15;
            requestEvent.Request_Event_Approval = new List<Request_Event_Approval>();
            Request_Event_Approval app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.WaitForApproval;
            app.app_level_id = 0;
            requestEvent.Request_Event_Approval.Add(app);

            //Act
            bool isRevertable = new RequestRepository().GetIsRevertable(requestEvent, currentUserId);

            //Assert
            Assert.False(isRevertable);
        }

        [Fact()]
        public void GetIsRevertableTest_NotWaitForFirstApproval() {
            //Arrange
            var requestEvent = new Request_Event();
            var currentUserId = 0;
            requestEvent.request_status = (int)RequestStatus.Ordered;
            requestEvent.requestor = currentUserId;
            requestEvent.Request_Event_Approval = new List<Request_Event_Approval>();
            Request_Event_Approval app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.WaitForApproval;
            app.app_level_id = 0;
            requestEvent.Request_Event_Approval.Add(app);

            //Act
            bool isRevertable = new RequestRepository().GetIsRevertable(requestEvent, currentUserId);

            //Assert
            Assert.False(isRevertable);
        }

        [Fact()]
        public void GetIsRevertableTest_NotWaitForFirstApproval1() {
            //Arrange
            var requestEvent = new Request_Event();
            var currentUserId = 1;
            requestEvent.request_status = (int)RequestStatus.WaitForApproval;
            requestEvent.requestor = currentUserId;
            requestEvent.Request_Event_Approval = new List<Request_Event_Approval>();
            Request_Event_Approval app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.Rejected;
            app.app_man_id = 54;
            app.app_level_id = 0;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            //Act
            bool isRevertable = new RequestRepository().GetIsRevertable(requestEvent, currentUserId);

            //Assert
            Assert.False(isRevertable);
        }

        [Fact()]
        public void GetPreviousStatus_Draft() {
            var requestEvent = new Request_Event();
            requestEvent.request_status = (int)RequestStatus.WaitForApproval;
            requestEvent.Request_Event_Approval = new List<Request_Event_Approval>();
            Request_Event_Approval app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.WaitForApproval;
            app.app_level_id = 0;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            var prevStatus = new RequestRepository().GetPreviousStatus(requestEvent, 0);
            Assert.True(prevStatus == RequestStatus.Draft);
        }

        [Fact()]
        public void GetPreviousStatus_FromApproved_WaitForApproval() {
            var requestEvent = new Request_Event();
            requestEvent.request_status = (int)RequestStatus.Approved;
            requestEvent.Request_Event_Approval = new List<Request_Event_Approval>();
            Request_Event_Approval app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.Approved;
            app.app_level_id = 0;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            var prevStatus = new RequestRepository().GetPreviousStatus(requestEvent, 0);
            Assert.True(prevStatus == RequestStatus.WaitForApproval);
        }

        [Fact()]
        public void GetPreviousStatus_FromRejected_WaitForApproval() {
            var requestEvent = new Request_Event();
            requestEvent.request_status = (int)RequestStatus.WaitForApproval;
            requestEvent.Request_Event_Approval = new List<Request_Event_Approval>();
            Request_Event_Approval app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.Rejected;
            app.app_level_id = 0;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            var prevStatus = new RequestRepository().GetPreviousStatus(requestEvent, 0);
            Assert.True(prevStatus == RequestStatus.WaitForApproval);
        }

        [Fact()]
        public void GetPreviousStatus_Approved_To_WaitForApproval_2AppInLevel() {
            //Asset
            int userId = 1;
            Request_Event requestEvent = new Request_Event();
            requestEvent.request_status = (int)RequestStatus.WaitForApproval;
            requestEvent.requestor = 53;

            requestEvent.Request_Event_Approval = new List<Request_Event_Approval>();
            Request_Event_Approval app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.Approved;
            app.app_level_id = 0;
            app.app_man_id = userId;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.WaitForApproval;
            app.app_level_id = 0;
            app.app_man_id = userId;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            //Act
            var prevStatus = new RequestRepository().GetPreviousStatus(requestEvent, userId);

            //Assert
            Assert.True(prevStatus == RequestStatus.WaitForApproval);
        }

        [Fact()]
        public void GetPreviousStatus_FromOrdered_Approved() {
            var requestEvent = new Request_Event();
            requestEvent.request_status = (int)RequestStatus.Ordered;
            requestEvent.Request_Event_Approval = new List<Request_Event_Approval>();
            Request_Event_Approval app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.Approved;
            app.app_level_id = 0;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            var prevStatus = new RequestRepository().GetPreviousStatus(requestEvent, 0);
            Assert.True(prevStatus == RequestStatus.Approved);
        }

        [Fact()]
        public void GetIsApprovable_Closed_False() {
            //Arrange
            int userId = 1;
            Request_Event requestEvent = new Request_Event();
            requestEvent.Request_Event_Approval = new List<Request_Event_Approval>();
            requestEvent.request_status = (int)RequestStatus.Closed;

            //Act
            bool isApprovable = new RequestRepository().GetIsApprovable(requestEvent, userId);

            //Assert
            Assert.False(isApprovable);
        }

        [Fact()]
        public void GetIsApprovable_1AppLevel_1AppMan_True() {
            //Arrange
            int userId = 1;
            Request_Event requestEvent = new Request_Event();
            requestEvent.request_status = (int)RequestStatus.WaitForApproval;

            requestEvent.Request_Event_Approval = new List<Request_Event_Approval>();
            Request_Event_Approval app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.WaitForApproval;
            app.app_level_id = 0;
            app.app_man_id = userId;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            //Act
            bool isApprovable = new RequestRepository().GetIsApprovable(requestEvent, userId);

            //Assert
            Assert.True(isApprovable);
        }

        [Fact()]
        public void GetIsApprovable_1AppLevel_1AppMan_False() {
            //Arrange
            int userId = 1;
            Request_Event requestEvent = new Request_Event();
            requestEvent.request_status = (int)RequestStatus.WaitForApproval;

            requestEvent.Request_Event_Approval = new List<Request_Event_Approval>();
            Request_Event_Approval app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.WaitForApproval;
            app.app_level_id = 0;
            app.app_man_id = 25;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            //Act
            bool isApprovable = new RequestRepository().GetIsApprovable(requestEvent, userId);

            //Assert
            Assert.False(isApprovable);
        }

        [Fact()]
        public void GetIsApprovable_5AppLevel_2ndAppMan_True() {
            //Arrange
            int userId = 1;
            Request_Event requestEvent = new Request_Event();
            requestEvent.request_status = (int)RequestStatus.WaitForApproval;

            requestEvent.Request_Event_Approval = new List<Request_Event_Approval>();
            Request_Event_Approval app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.Approved;
            app.app_level_id = 0;
            app.app_man_id = 24;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.WaitForApproval;
            app.app_level_id = 1;
            app.app_man_id = userId;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.WaitForApproval;
            app.app_level_id = 2;
            app.app_man_id = 15;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.WaitForApproval;
            app.app_level_id = 3;
            app.app_man_id = 15;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.WaitForApproval;
            app.app_level_id = 4;
            app.app_man_id = 15;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            //Act
            bool isApprovable = new RequestRepository().GetIsApprovable(requestEvent, userId);

            //Assert
            Assert.True(isApprovable);
        }

        [Fact()]
        public void GetIsApprovable_5AppLevel_2ndAppMan_False() {
            //Arrange
            int userId = 1;
            Request_Event requestEvent = new Request_Event();
            requestEvent.request_status = (int)RequestStatus.WaitForApproval;

            requestEvent.Request_Event_Approval = new List<Request_Event_Approval>();
            Request_Event_Approval app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.Approved;
            app.app_level_id = 0;
            app.app_man_id = 24;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.WaitForApproval;
            app.app_level_id = 1;
            app.app_man_id = 35;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.WaitForApproval;
            app.app_level_id = userId;
            app.app_man_id = 15;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.WaitForApproval;
            app.app_level_id = 3;
            app.app_man_id = 15;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.WaitForApproval;
            app.app_level_id = 4;
            app.app_man_id = 15;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            //Act
            bool isApprovable = new RequestRepository().GetIsApprovable(requestEvent, userId);

            //Assert
            Assert.False(isApprovable);
        }

        [Fact()]
        public void GetIsApprovable_5AppLevel_2ndAppManMultiApp_True() {
            //Arrange
            int userId = 1;
            Request_Event requestEvent = new Request_Event();
            requestEvent.request_status = (int)RequestStatus.WaitForApproval;

            requestEvent.Request_Event_Approval = new List<Request_Event_Approval>();
            Request_Event_Approval app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.Approved;
            app.app_level_id = 0;
            app.app_man_id = 24;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.Approved;
            app.app_level_id = 1;
            app.app_man_id = 6;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.WaitForApproval;
            app.app_level_id = 1;
            app.app_man_id = userId;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.WaitForApproval;
            app.app_level_id = 2;
            app.app_man_id = 15;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.WaitForApproval;
            app.app_level_id = 3;
            app.app_man_id = 15;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.WaitForApproval;
            app.app_level_id = 4;
            app.app_man_id = 15;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            //Act
            bool isApprovable = new RequestRepository().GetIsApprovable(requestEvent, userId);

            //Assert
            Assert.True(isApprovable);
        }

        [Fact()]
        public void GetIsApprovable_5AppLevel_2ndAppManMultiNotApp_True() {
            //Arrange
            int userId = 1;
            Request_Event requestEvent = new Request_Event();
            requestEvent.request_status = (int)RequestStatus.WaitForApproval;

            requestEvent.Request_Event_Approval = new List<Request_Event_Approval>();
            Request_Event_Approval app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.Approved;
            app.app_level_id = 0;
            app.app_man_id = 24;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.WaitForApproval;
            app.app_level_id = 1;
            app.app_man_id = 6;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.WaitForApproval;
            app.app_level_id = 1;
            app.app_man_id = userId;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.WaitForApproval;
            app.app_level_id = 2;
            app.app_man_id = 15;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.WaitForApproval;
            app.app_level_id = 3;
            app.app_man_id = 15;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.WaitForApproval;
            app.app_level_id = 4;
            app.app_man_id = 15;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            //Act
            bool isApprovable = new RequestRepository().GetIsApprovable(requestEvent, userId);

            //Assert
            Assert.True(isApprovable);
        }

        [Fact()]
        public void GetIsRevertable_AppMan_Yes() {
            int userId = 1;

            Request_Event requestEvent = new Request_Event();
            requestEvent.request_status = (int)RequestStatus.WaitForApproval;
            requestEvent.requestor = 53;

            requestEvent.Request_Event_Approval = new List<Request_Event_Approval>();
            Request_Event_Approval app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.Approved;
            app.app_level_id = 0;
            app.app_man_id = userId;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            bool isRevertable = new RequestRepository().GetIsRevertable(requestEvent, userId);
            Assert.True(isRevertable);
        }

        [Fact()]
        public void GetIsRevertable_AppMan3Level_2LevelApproved_No() {
            int userId = 1;

            Request_Event requestEvent = new Request_Event();
            requestEvent.request_status = (int)RequestStatus.WaitForApproval;
            requestEvent.requestor = 42;

            requestEvent.Request_Event_Approval = new List<Request_Event_Approval>();
            Request_Event_Approval app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.Approved;
            app.app_level_id = 0;
            app.app_man_id = 16;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            requestEvent.Request_Event_Approval = new List<Request_Event_Approval>();
            app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.Approved;
            app.app_level_id = 1;
            app.app_man_id = 74;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            requestEvent.Request_Event_Approval = new List<Request_Event_Approval>();
            app = new Request_Event_Approval();
            app.approve_status = (int)ApproveStatus.WaitForApproval;
            app.app_level_id = 2;
            app.app_man_id = userId;
            app.is_last_version = true;
            requestEvent.Request_Event_Approval.Add(app);

            bool isRevertable = new RequestRepository().GetIsRevertable(requestEvent, userId);
            Assert.False(isRevertable);
        }

        [Fact()]
        public void GetIsOrderAvailable_Yes() {
            int userId = 16;
            Request_Event requestEvent = new Request_Event();
            requestEvent.request_status = (int)RequestStatus.Approved;
            requestEvent.is_order_needed = true;
            requestEvent.orderer_id = userId;

            bool isOrderAvailable = new RequestRepository().GetIsOrderAvailable(requestEvent, 16);
            Assert.True(isOrderAvailable);
        }
    }
}