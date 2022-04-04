using Kamsyk.Reget.Model.ExtendedModel;
using System.Collections.Generic;

namespace Kamsyk.Reget.Model.Repositories.Interfaces {
    public interface ICentreGroupRepository {
        List<CentreGroupAdminList> GetActiveCentreGroupsByUserId(int userId, string rootUrl);
        CentreGroupExtended GetCentreGroupDataById(int cgId, int currentUserId);
        Centre_Group GetCentreGroupAllDataById(int cgId);
        int SaveCentreGroupData(CentreGroupExtended cg);
        CgAdminRoles GetCgAdminRoles(int cgId, int userId);
        List<CurrencyExtended> GetCurrencyData(int cgId, int currentCurrencyId, int? companyId);
        List<CentreExtended> GetActiveCentreData();
        List<Centre> GetCgActiveCentreData(int cgId);
        List<Currency> GetCgCurrencies(int cgId);
        //List<ParticipantsExtended> GetActiveParticipantsData(string rootUrl, List<int> adminCompIds, bool isUserNameDisplayed);
        bool IsAuthorized(int userId, int roleId, int cgId);
        bool DeletePurchaseGroup(int pgId, int cgId);
    }
}
