using Kamsyk.Reget.Model.ExtendedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.Repositories.Interfaces {
    public interface ISubstitutionRepository {
        List<ParticipantsExtended> GetSubstitutedParticipantsData(string rootUrl, List<int> adminCompIds, int currentUserId);
    }
}
