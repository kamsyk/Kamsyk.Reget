using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kamsyk.Reget.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.Repositories.Tests {
    [TestClass()]
    public class UserRepositoryTests {
        [TestMethod()]
        public void ZzInt_RemoveDiacriticsUserName() {
            //Assign
            UserRepository userRepository = new UserRepository();
            Participants p = userRepository.GetParticipantById(3);

            //Act
            p.first_name = "ěščřžýáíéĚŠČŘŽ%ŤŁł3";
            userRepository.SaveParticipantData(p);

            //Assert
            p = userRepository.GetParticipantById(3);
            Assert.IsTrue(p.first_name_search_key == "escrzyaieESCRZ%TLl3".ToLower());
        }
    }
}