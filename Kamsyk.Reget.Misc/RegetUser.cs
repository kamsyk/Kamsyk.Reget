using Kamsyk.Reget.Model;
using Kamsyk.Reget.Model.ExtendedModel;
using Kamsyk.Reget.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Misc {
    public class RegetUser {
        public void SetUserSearchKey() {
            UserRepository userRepository = new UserRepository();
            List<Participants> participants = userRepository.GetAllParticipants();

            foreach (var participant in participants) {
                
                userRepository.SaveParticipantData(participant);
                Console.WriteLine(participant.surname);
            }
        }
    }
}
