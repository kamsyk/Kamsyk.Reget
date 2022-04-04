using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.Repositories.Interfaces {
    public interface IRequestRepository {
        bool IsRequestorAuthorized(int participantId, int centreId);
        bool GetIsRevertable(Request_Event requestEvent, int userId);
    }
}
