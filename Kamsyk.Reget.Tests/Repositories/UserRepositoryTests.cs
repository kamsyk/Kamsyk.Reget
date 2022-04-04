using Kamsyk.Reget.Model.Repositories;
using Kamsyk.Reget.Model.Repositories.Interfaces;
using Moq;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kamsyk.Reget.Model.Repositories.Tests {
    
    public class UserRepositoryTests {
        [Fact]
        public void GetParticipantByUserNameTest() {
            ////Assign
            //var mockManager = MockRepository.GenerateMock<IUserRepository>();

            ////Act
            //Participants participant = mockManager.GetParticipantByUserName("syka");

            ////Assert
            //mockManager.AssertWasCalled(x => x.GetParticipantByUserName("syka"));

            //Arrange
            var id = 0;
            var user_name = "syka";
            var user = new Participants() { id = id, user_name = user_name };
            var mock = new Mock<IUserRepository>();
            mock.Setup(x => x.GetParticipantByUserName(user_name)).Returns(user);

            //Act
            var userRepository = new UserRepository(mock.Object);
            var actUser = userRepository.GetParticipantByUserName(user_name);

            //Assert
            Assert.Equal(id, actUser.id);
            Assert.Equal(user_name, actUser.user_name);
        }

        [Fact]
        public void GetActiveParticipantByUserNameTest() {
            //Assign
            var mockManager = Rhino.Mocks.MockRepository.GenerateMock<IUserRepository>();

            //Act
            Participants participant = mockManager.GetActiveParticipantByUserName("syka");

            //Assert
            mockManager.AssertWasCalled(x => x.GetActiveParticipantByUserName("syka"));
        }

        [Fact]
        public void SetParticipantLangTest() {
            //Assign
            var mockManager = Rhino.Mocks.MockRepository.GenerateMock<IUserRepository>();

            //Act
            mockManager.SetParticipantLang(0, "cs-CZ");

            //Assert
            mockManager.AssertWasCalled(x => x.SetParticipantLang(0, "cs-CZ"));
        }

        [Fact]
        public void ChangeUserNameTest() {
            //Assign
            var mockManager = Rhino.Mocks.MockRepository.GenerateMock<IUserRepository>();

            //Act
            mockManager.ChangeUserName(0, "xxx");

            //Assert
            mockManager.AssertWasCalled(x => x.ChangeUserName(0, "xxx"));
        }

        [Fact]
        public void DeleteUserTest() {
            //Assign
            var mockManager = Rhino.Mocks.MockRepository.GenerateMock<IUserRepository>();

            //Act
            mockManager.DeleteUser(0);

            //Assert
            mockManager.AssertWasCalled(x => x.DeleteUser(0));
            //Assert.Fail();
        }

        [Fact]
        public void IsAppManagerTest() {
            //Assign
            var mockManager = Rhino.Mocks.MockRepository.GenerateMock<IUserRepository>();

            //Act
            bool isActiveAppMan = false;
            bool isActiveOrderer = false;
            mockManager.GetActiveManagerCg(0, out isActiveAppMan, out isActiveOrderer);

            //Assert
            mockManager.AssertWasCalled(x => x.GetActiveManagerCg(0, out isActiveAppMan, out isActiveOrderer));
            //Assert.Fail();
        }

        
    }
}