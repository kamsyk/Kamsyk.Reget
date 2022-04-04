using OTISCZ.ActiveDirectory;
using Kamsyk.Reget.Controllers.Interface;
using Kamsyk.Reget.Controllers.RegetExceptions;
using Kamsyk.Reget.Model;
using Kamsyk.Reget.Model.Common;
using Kamsyk.Reget.Model.ExtendedModel;
using Kamsyk.Reget.Model.Repositories;
using Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using static Kamsyk.Reget.Model.Repositories.CentreRepository;
using static Kamsyk.Reget.Model.Repositories.UserRepository;
using Kamsyk.Reget.Model.ExtendedModel.HttpResult;

namespace Kamsyk.Reget.Controllers
{
    public class CentreController : BaseController {

        #region Virtual Properties
        public override string HeaderImgUrl {
            get {
                return GetRootUrl() + "/Content/Images/Controll/Centre.png";
            }
        }

        public override string HeaderTitle {
            get {
                return RequestResource.Centres;
            }
        }
        #endregion

        #region HttpGet
        [HttpGet]
        public ActionResult GetCentresAdminData(string filter, string sort, int pageSize, int currentPage) {
            try {
                string decFilter = DecodeUrl(filter);

                List<int> compIds = CurrentUser.ParticipantAdminCompanyIds;
                int rowCount;
                var centres = new CentreRepository().GetCentresAdminData(
                    compIds,
                    decFilter,
                    sort,
                    pageSize,
                    currentPage,
                    RequestResource.Always,
                    RequestResource.Never,
                    RequestResource.Optional,
                    out rowCount);

                if (centres != null) {
                    foreach (var centre in centres) {
                        centre.export_price_text = GetExportToPriceText(centre.export_price_to_order);

                    }
                }

                PartData<CentreAdminExtended> cd = new PartData<CentreAdminExtended>();
                cd.db_data = centres;
                cd.rows_count = rowCount;

                return GetJson(cd);
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);
            }
        }


        #endregion

        #region HttpPost
        [HttpPost]
        public ActionResult SaveCentreData(CentreAdminExtended centre) {
            try {
                //if (!CurrentUser.IsParticipantAppAdmin && !CurrentUser.IsParticipantCompanyAdmin) {
                //    throw new ExNotAuthorizedUpdateUser("Not authorized to edit Centres");
                //}

                CentreRepository centreRepository = new CentreRepository();
                int officeId = new CompanyRepository().GetCompanyIdByName(centre.company_name);
                if (officeId == DataNulls.INT_NULL) {
                    throw new ExNotAuthorizedUpdateUser("Unknown Company");
                }

                if (!IsUserInOfficeRole(UserRole.OfficeAdministrator, officeId)) {
                    throw new ExNotAuthorizedUpdateUser("Not authorized to update Address");
                }

                if (!IsCentreValid(centre)) {
                    throw new ExMissingMandatoryFields("Missing Mandatory Fields");
                }

                HttpResult httpResult = new HttpResult();

                List<string> msgItems;
                int centreId = new CentreRepository().SaveCentreData(
                    centre, 
                    CurrentUser.Participant.id, 
                    RequestResource.Always,
                    RequestResource.Never,
                    RequestResource.Optional,
                    out msgItems);
                
                
                httpResult.int_value = centreId;
                httpResult.string_value = GetErrorMsg(msgItems);

                return GetJson(httpResult);
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpPost]
        public ActionResult DeleteCentre(Centre centre) {
            try {
                //if (!CurrentUser.IsParticipantAppAdmin && !CurrentUser.IsParticipantCompanyAdmin) {
                //    throw new ExNotAuthorizedUpdateUser("Not authorized to edit Centres");
                //}

                CentreRepository centreRepository = new CentreRepository();

                if (centre.company_id == DataNulls.INT_NULL) {
                    throw new ExNotAuthorizedUpdateUser("Unknown Company");
                }

                if (!IsUserInOfficeRole(UserRole.OfficeAdministrator, centre.company_id)) {
                    throw new ExNotAuthorizedUpdateUser("Not authorized to update Centres");
                }

                HttpResult httpResult = new HttpResult();
                                
                if (new CentreRepository().DeleteCentre(centre.id)) {
                    return null;
                } else {
                    //return "disabled";
                    httpResult.string_value = "disabled";
                    return GetJson(httpResult);
                }


            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }
        #endregion

        #region Methods
        public ActionResult Centre() {
            return View("Index");
        }

        public static string GetExportToPriceText(int? exportPriceToOrder) {
            if (exportPriceToOrder == (int)ExportPrice.Always) {
                return RequestResource.Always;
            } else if (exportPriceToOrder == (int)ExportPrice.Optional) {
                return RequestResource.Optional;
            } else if (exportPriceToOrder == (int)ExportPrice.Never || exportPriceToOrder == null) {
                return RequestResource.Never;
            } else {
                throw new Exception("Export To Price was not found");
            }
        }

        private bool IsCentreValid(CentreAdminExtended centre) {
            if (String.IsNullOrEmpty(centre.name)) {
                return false;
            }

            if (String.IsNullOrEmpty(centre.company_name)) {
                return false;
            }

            if (String.IsNullOrEmpty(centre.export_price_text)) {
                return false;
            }

            return true;
        }
        #endregion
    }
}