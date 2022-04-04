using Kamsyk.Reget.Controllers;
using Kamsyk.Reget.Model;
using Kamsyk.Reget.Model.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Controllers.Tests {
    [TestClass()]
    public class RequestUserTest {
        [TestMethod()]
        public void ParticipantNullFirstNameSurname_GetUserName() {
            // Arrange
            RegetUser regetUser = new RegetUser();
            Participants participant = new Participants();

            // Act
            bool isOk = true;
            try {
                participant.first_name = null;
                participant.surname = null;
                regetUser.Participant = participant;
                string userName = regetUser.ParticipantName;

                participant.first_name = null;
                participant.surname = "fafasfas";
                regetUser.Participant = participant;
                userName = regetUser.ParticipantName;

                participant.first_name = "dfssda sfd f";
                participant.surname = null;
                regetUser.Participant = participant;
                userName = regetUser.ParticipantName;

                participant.first_name = "fsdfasdfd";
                participant.surname = "fasfsfsad";
                regetUser.Participant = participant;
                userName = regetUser.ParticipantName;
            } catch {
                isOk = false;
            }

            // Assert
            Assert.IsTrue(isOk);

        }
       
    }
}
