using Kamsyk.Reget.Controllers;
using Kamsyk.Reget.Controllers.RegetExceptions;
using Kamsyk.Reget.Model;
using Kamsyk.Reget.Model.ExtendedModel;
using Kamsyk.Reget.Model.Repositories;
using Kamsyk.Reget.Model.Repositories.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Kamsyk.Reget.Model.Repositories.RequestRepository;
using static Kamsyk.Reget.Model.Repositories.UserRepository;

namespace Kamsyk.Reget.Controllers.Tests {

    public class ParticipantControllerTests {
        [Fact]
        public void DeleteUserTest_NotAuthorized() {
            //Arrange
            int userId = 25;
            int userRole = (int)UserRole.OfficeAdministrator;
            int companyId = 0;
            string officeName = null;

            var mockCompanyRep = new Mock<ICompanyRepository>();
            mockCompanyRep.Setup(x => x.GetCompanyIdByName(officeName)).Returns(0);

            var mockUserRep = new Mock<IUserRepository>();
            mockUserRep.Setup(x => x.IsAuthorized(userId, userRole, companyId)).Returns(false);

            ParticipantsExtended part = new ParticipantsExtended();
            try {
                new ParticipantController(mockCompanyRep.Object, mockUserRep.Object).DeleteUser(part);
            } catch (Exception ex) {
                if (ex is ExNotAuthorizedUpdateUser) {
                    Assert.True(true);
                    return;
                }
            }


            Assert.True(false);
        }

        [Fact()]
        public void SaveUserSubstitution_ApprovedRejectedNotAuthorSubstedUser_CannotBeSaved() {
            //Arrange
            ParticipantController participantController = new ParticipantController();
            RegetUser currUser = new RegetUser();
            Participants participant = new Participants();
            participant.id = 0;
            currUser.Participant = participant;
#if TEST
            participantController.CurrentUser = currUser;
#endif
            
            //Act
            try {
                UserSubstitutionExtended subst = new UserSubstitutionExtended();
                subst.approval_status = (int)ApproveStatus.Approved;
                subst.author_id = 0;

                participantController.SaveUserSubstitution(subst);

                Assert.True(false);
            } catch (Exception ex) {
                if (!(ex is ExNotAuthorizedUpdateUser)) {
                    new TestFailRepository().SaveTestFail("SaveUserSubstitution_ApprovedRejectedNotAuthorSubstedUSer_CannotBeSaved", "Was Saved");
                    Assert.True(false, "Was Saved");
                }
            }

            try {
                UserSubstitutionExtended subst = new UserSubstitutionExtended();
                subst.approval_status = (int)ApproveStatus.Rejected;
                subst.author_id = 0;

                participantController.SaveUserSubstitution(subst);

                Assert.True(false);
            } catch (Exception ex) {
                if (!(ex is ExNotAuthorizedUpdateUser)) {
                    new TestFailRepository().SaveTestFail("SaveUserSubstitution_ApprovedRejectedNotAuthorSubstedUSer_CannotBeSaved", "Was Saved");
                    Assert.True(false, "Was Saved");
                }
            }

            try {
                UserSubstitutionExtended subst = new UserSubstitutionExtended();
                subst.approval_status = (int)ApproveStatus.NotNeeded;
                subst.substituted_user_id = 1;
                subst.author_id = 1;

                participantController.SaveUserSubstitution(subst);

                Assert.True(false);
            } catch (Exception ex) {
                if (!(ex is ExNotAuthorizedUpdateUser)) {
                    new TestFailRepository().SaveTestFail("SaveUserSubstitution_ApprovedRejectedNotAuthorSubstedUSer_CannotBeSaved", "Was Saved");
                    Assert.True(false, "Was Saved");
                }
            }

            //Assert
            Assert.True(true);
        }
    }
}