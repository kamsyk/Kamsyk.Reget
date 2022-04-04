using Kamsyk.Reget.Model.ExtendedModel;
using Kamsyk.Reget.Model.Repositories;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kamsyk.Reget.Controllers {
    public class EventViewerController : BaseController {
        #region Virtual Properties
        public override string HeaderImgUrl {
            get {
                return GetRootUrl() + "/Content/Images/Controll/Event.png";
            }
        }

        public override string HeaderTitle {
            get {
                return RequestResource.EventViewer;
            }
        }
        #endregion

        #region HttpGet
        [HttpGet]
        public ActionResult GetEventData(int companyId, string filter, string sort, int pageSize, int currentPage) {
            string decFilter = DecodeUrl(filter);

            int rowCount;
            var events = new EventRepository().GetEventData(
                companyId,
                decFilter,
                sort,
                pageSize,
                currentPage,
                CurrentUser.ParticipantId,
                out rowCount);

            PartData<EventGridData> ed = new PartData<EventGridData>();
            ed.db_data = events;
            ed.rows_count = rowCount;

            return GetJson(ed);

        }
        #endregion
    }
}