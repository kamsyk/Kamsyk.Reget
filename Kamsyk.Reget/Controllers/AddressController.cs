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
using static Kamsyk.Reget.Model.Repositories.UserRepository;
using Kamsyk.Reget.Model.ExtendedModel.HttpResult;

namespace Kamsyk.Reget.Controllers
{
    public class AddressController : BaseController {
        #region Virtual Properties
        public override string HeaderImgUrl {
            get {
                return GetRootUrl() + "/Content/Images/Controll/Address.png";
            }
        }

        public override string HeaderTitle {
            get {
                return RequestResource.ShipToAddress;
            }
        }
        #endregion

        #region HttpGet
        [HttpGet]
        public ActionResult GetAddressesAdminData(string filter, string sort, int pageSize, int currentPage) {
           
            string decFilter = DecodeUrl(filter);

            List<int> compIds = CurrentUser.ParticipantAdminCompanyIds;

            if (compIds.Count == 0) {
                PartData<AddressAdminExtended> cdEmpty = new PartData<AddressAdminExtended>();
                cdEmpty.db_data = null;
                cdEmpty.rows_count = 0;
                return GetJson(cdEmpty);
            }

            int rowCount;
            var addresses = new AddressRepository().GetAddressAdminData(
                compIds,
                decFilter,
                sort,
                pageSize,
                currentPage,
                out rowCount);

            PartData<AddressAdminExtended> cd = new PartData<AddressAdminExtended>();
            cd.db_data = addresses;
            cd.rows_count = rowCount;

            return GetJson(cd);

        }

        [HttpGet]
        public ActionResult GetAddressesForCentresAdminData() {
            List<AgDropDown> addresses = new AddressRepository().GetAddressesDataByCompanyId(CurrentUser.ParticipantAdminCompanyIds);

            return GetJson(addresses.ToList());

        }

        [HttpGet]
        public ActionResult GetActiveAddressesDataByText(string addressText, string companyName) {
            try {
                var decName = DecodeUrl(addressText);
                var decCompName = DecodeUrl(companyName);

                List<AddressDbGrid> addresses = new AddressRepository().GetAddressDataByAddressText(
                    decName,
                    decCompName);
                                
                return GetJson(addresses.ToList());

            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }
        #endregion

        #region HttpPost
        [HttpPost]
        public ActionResult SaveAddressData(AddressAdminExtended address) {
            try {
                AddressRepository addressRepository = new AddressRepository();
                int officeId = new CompanyRepository().GetCompanyIdByName(address.company_name_text);
                if (officeId == DataNulls.INT_NULL) {
                    throw new ExNotAuthorizedUpdateUser("Unknown Company");
                }

                if (!IsUserInOfficeRole(UserRole.OfficeAdministrator, officeId)) {
                    throw new ExNotAuthorizedUpdateUser("Not authorized to update Address");
                }

                if (!IsAddressValid(address)) {
                    throw new ExMissingMandatoryFields("Missing Mandatory Fields");
                }

                HttpResult httpResult = new HttpResult();

                List<string> msgItems;
                int addressId = new AddressRepository().SaveAddressData(address, CurrentUser.Participant.id, out msgItems);

                httpResult.int_value = addressId;
                httpResult.string_value = GetErrorMsg(msgItems);

                return GetJson(httpResult);
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }
        #endregion

        #region Methods
        public ActionResult Address() {
            return View("Index");
        }

        private bool IsAddressValid(AddressAdminExtended address) {
            if (String.IsNullOrEmpty(address.company_name)) {
                return false;
            }

            if (String.IsNullOrEmpty(address.company_name_text)) {
                return false;
            }

            return true;
        }

        [HttpPost]
        public void DeleteAddress(AddressAdminExtended address) {
            try {
                AddressRepository addressRepository = new AddressRepository();
                int officeId = new CompanyRepository().GetCompanyIdByName(address.company_name_text);
                if (officeId == DataNulls.INT_NULL) {
                    throw new ExNotAuthorizedUpdateUser("Unknown Company");
                }

                if (!IsUserInOfficeRole(UserRole.OfficeAdministrator, officeId)) {
                    throw new ExNotAuthorizedUpdateUser("Not authorized to update Address");
                }

                if (!IsAddressValid(address)) {
                    throw new ExMissingMandatoryFields("Missing Mandatory Fields");
                }

                new AddressRepository().DeleteAddress(address.id);

                Response.StatusCode = (int)HttpStatusCode.OK;
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }
        #endregion
    }
}