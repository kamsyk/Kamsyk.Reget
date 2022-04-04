using Kamsyk.Reget.Model.ExtendedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.Repositories.Interfaces {
    public interface IUserRepository {
        Participants GetParticipantByUserName(string userName);
        Participants GetActiveParticipantByUserName(string userName);
        void SetParticipantLang(int participantId, string cultureName);
        void ChangeUserName(int userId, string userName);
        void DeleteUserFromApprovalMatrix(int participantId);
        bool DeleteUser(int participantId);
        void GetActiveManagerCg(
            int participantId, 
            out bool isActiveAppMan,
            out bool isActiveOrderer);
        List<Participants> GetAppAdmins();
        List<ParticipantsExtended> GetActiveParticipantsData(string rootUrl, List<int> adminCompIds, bool isUserNameDisplayed, int companyGroupId);
        bool IsAuthorized(int userId, int roleId, int officeId);
        
    }
}
