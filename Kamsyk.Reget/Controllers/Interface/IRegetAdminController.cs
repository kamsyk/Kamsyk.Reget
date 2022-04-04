using Kamsyk.Reget.Model;
using Kamsyk.Reget.Model.ExtendedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Kamsyk.Reget.Controllers.Interface {
    public interface IRegetAdminController {
        ActionResult GetCgAdminRoles(int cgId, int userId);
        ActionResult GetCgActiveCentres(int cgId);
        ActionResult SaveCentreGroupData(CentreGroupExtended cg);
        ActionResult GetPurchaseGroupsByCgId(int cgId, int indexFrom, bool? isDeativatedLoaded, int? pgRequestor);
        ActionResult GetActiveParticipantsData();
        ActionResult SavePurchaseGroupData(PurchaseGroupExtended pg);
    }
}
