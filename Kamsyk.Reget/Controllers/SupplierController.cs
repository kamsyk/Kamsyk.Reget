using Kamsyk.Reget.Controllers.Interface;
using Kamsyk.Reget.Controllers.RegetExceptions;
using Kamsyk.Reget.Model;
using Kamsyk.Reget.Model.Common;
using Kamsyk.Reget.Model.ExtendedModel;
using Kamsyk.Reget.Model.ExtendedModel.HttpResult;
using Kamsyk.Reget.Model.ExtendedModel.Supplier;
using Kamsyk.Reget.Model.Repositories;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using static Kamsyk.Reget.Model.Repositories.UserRepository;

namespace Kamsyk.Reget.Controllers {
    public class SupplierController : BaseController {
        #region Virtual Properties
        public override string HeaderImgUrl {
            get {
                return GetRootUrl() + "/Content/Images/Controll/supplier.png";
            }
        }

        public override string HeaderTitle {
            get {
                return RequestResource.Suppliers;
            }
        }

        
        #endregion

        #region HttpGet
        [HttpGet]
        public ActionResult GetSupplierData(int companyId, string filter, string sort, int pageSize, int currentPage) {
            string decFilter = DecodeUrl(filter);

            int rowCount;
            var suppliers = new SupplierRepository().GetSupplierData(
                companyId,
                decFilter,
                sort,
                pageSize,
                currentPage,
                out rowCount);

            PartData<SupplierGridData> sd = new PartData<SupplierGridData>();
            sd.db_data = suppliers;
            sd.rows_count = rowCount;

            return GetJson(sd);

        }

        [HttpGet]
        public ActionResult GetSuppliersAdmins(int companyId) {
            
            var suppliers = new UserRepository().GetSupplierAdminJs(companyId);

            return GetJson(suppliers);

        }

        [HttpGet]
        public ActionResult GetUserOfficeData() {

            CompanyRepository companyRepository = new CompanyRepository();

            foreach (var comp in CurrentUser.UserCompanies) {
                comp.is_supplier_maintenance_allowed = companyRepository.IsSupplierMaintenanceAllowed(comp.id);
            }

            return GetJson(CurrentUser.UserCompanies);

        }

        [HttpGet]
        public ActionResult ImportSuppliers(int companyId) {
            Company comp = new CompanyRepository().GetCompanyById(companyId);
            if (comp.is_supplier_maintenance_allowed == true) {
                throw new Exception(comp.country_code + " is maintaned manually");
            }

#if DEBUG
            WsSupplierDebug.Supplier wsSupplier = new WsSupplierDebug.Supplier();
            //WsSupplier.Supplier wsSupplier = new WsSupplier.Supplier();
#else
            WsSupplier.Supplier wsSupplier = new WsSupplier.Supplier();
#endif
#if !TEST
            wsSupplier.Timeout = 900000;
            wsSupplier.ImportSupplier(companyId, true);
#endif

            PartData<SupplierGridData> sd = new PartData<SupplierGridData>();

            return GetJson(sd);
        }

        [HttpGet]
        public ActionResult GetLastCompanySupplierUpload(int companyId) {
            try {
                DateTime lastCompSuppUploadDate = new CompanyRepository().GetLastSupplierUploadDate(companyId);
                if (lastCompSuppUploadDate != DataNulls.DATETIME_NULL) {
                    string strDate = ConvertData.ToStringFromDateTimeLocal(lastCompSuppUploadDate);
                    string isOK = (lastCompSuppUploadDate.AddHours(26) > DateTime.Now) ? "1" : "0";
                    string strResult = strDate + "|" + isOK;
                    return GetJson(strResult);
                } else {
                    return GetJson(null);
                }
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);
            }
        }

        [HttpGet]
        public ActionResult GetActiveParticipantsData() {
            try {

                List<ParticipantsExtended> participants = new UserRepository().GetActiveParticipantsData(GetRootUrl(), false, CurrentUser.Participant.Company_Group.id);
                return GetJson(participants.ToList());
               
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpGet]
        public ActionResult GetActiveSuppliersByNameIdJs(string searchText, int centreId) {
            try {
                if (IsSuppGroupIsAllowed(centreId)) {
                    throw new ExNotAuthorizedUser();
                }

                var suppNameId = DecodeUrl(searchText);

                var centre = new CentreRepository().GetCentreById(centreId);
                int suppGroupId = (int)centre.Company.supplier_group_id;
               
                List<SupplierSimpleExtended> suppliers = new SupplierRepository().GetActiveSuppliersByNameId(
                    searchText,
                    suppGroupId);

                return GetJson(suppliers.ToList());

            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpGet]
        public ActionResult GetActiveSupplierByIdJs(int supplierId) {
            try {
                //if (IsSuppGroupIsAllowed(centreId)) {
                //    throw new ExNotAuthorizedUser();
                //}

                //var suppNameId = DecodeUrl(searchText);

                //var centre = new CentreRepository().GetCentreById(centreId);
                //int suppGroupId = (int)centre.Company.supplier_group_id;

                SupplierSimpleExtended suppliers = new SupplierRepository().GetActiveSuppliersById(supplierId);

                if (suppliers == null) {
                    return null;
                }

                return GetJson(suppliers);

            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpGet]
        public ActionResult GetSupplierById(int id) {
            try {
                
                Supplier supplier = new SupplierRepository().GetSuppliersById(id);

                if (supplier == null) {
                    return null;
                }

                return GetJson(supplier);

            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        public ActionResult GetContactsData(int supplierId, string filter, string sort, int pageSize, int currentPage) {
            try {
                string decFilter = DecodeUrl(filter);

                
                int rowCount;
                var supplierContacts = new SupplierRepository().GetContactData(
                    supplierId,
                    decFilter,
                    sort,
                    pageSize,
                    currentPage,
                    out rowCount);
                                

                PartData<SupplierContactGridData> scd = new PartData<SupplierContactGridData>();
                scd.db_data = supplierContacts;
                scd.rows_count = rowCount;

                return GetJson(scd);
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);
            }
        }
        #endregion

        #region HttpPost
        [HttpPost]
        public ActionResult SaveSupplierMaintenance(int companyId, bool isManualSuppMaintenance) {
            try {
                CompanyRepository companyRepository = new CompanyRepository();
                string eventErrMsg = null;
                companyRepository.SaveSuppMaintenance(
                    companyId, 
                    isManualSuppMaintenance, 
                    CurrentUser.ParticipantId,
                    out eventErrMsg);

                if (!String.IsNullOrEmpty(eventErrMsg)) {
                    HandleError(new Exception(eventErrMsg));
                }

                
                HttpResult httpResult = new HttpResult();
                //httpResult.int_value = userId;
                //httpResult.string_value = GetErrorMsg(msgItems);

                return GetJson(httpResult);
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }
                
        [HttpPost]
        public ActionResult SaveSupplierData(SupplierGridData supplier) {
            try {
                int companyId = supplier.company_id;

                UserRepository userRepository = new UserRepository();
                if (!userRepository.IsAuthorized(CurrentUser.ParticipantId, (int)UserRepository.UserRole.OfficeAdministrator, companyId) &&
                    !userRepository.IsAuthorized(CurrentUser.ParticipantId, (int)UserRepository.UserRole.SupplierAdmin, companyId)) {
                    throw new ExNotAuthorizedUpdateUser("Not authorized to update Supplier");
                }
                
                HttpResult httpResult = new HttpResult();

                List<string> msgItems;
                int supplierId = new SupplierRepository().SaveSupplier(supplier, CurrentUser.Participant.id, companyId, false, out msgItems);

                httpResult.int_value = supplierId;
                httpResult.string_value = GetErrorMsg(msgItems);

                return GetJson(httpResult);
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpPost]
        public ActionResult DeleteSupplier(SupplierGridData supplier) {
            try {
                int companyId = supplier.company_id;
                SupplierRepository supplierRepository = new SupplierRepository();
                
                if (companyId == DataNulls.INT_NULL) {
                    throw new ExNotAuthorizedUpdateUser("Unknown Company");
                }

                if (!IsUserInOfficeRole(UserRole.OfficeAdministrator, companyId) &&
                    !IsUserInOfficeRole(UserRole.SupplierAdmin, companyId)) {
                    throw new ExNotAuthorizedUpdateUser("Not authorized to update Suppliers");
                }

                HttpResult httpResult = new HttpResult();

                //bool isSuppUsed = IsSupplierUsed(supplier.id);

               
                    if (new SupplierRepository().DeleteSupplier(supplier.id)) {
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

        [HttpPost]
        public ActionResult SaveNewSuppAdmin(int adminId, int companyId) {
            try {
                if (!IsUpdateSuppAdminAllowed(UserRole.OfficeAdministrator, companyId)) {
                    throw new ExNotAuthorizedUpdateUser("Not authorized to update Suppliers Administrators");
                }

                new SupplierRepository().AddNewSuppAdmin(adminId, companyId);

                HttpResult httpResult = new HttpResult();

                httpResult.string_value = "";

                return GetJson(httpResult);
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);
            }
        }

        [HttpPost]
        public ActionResult DeleteSuppAdmin(int adminId, int companyId) {
            try {
                if (!IsUpdateSuppAdminAllowed(UserRole.OfficeAdministrator, companyId)) {
                    throw new ExNotAuthorizedUpdateUser("Not authorized to update Suppliers Administrators");
                }

                new SupplierRepository().DeleteSuppAdmin(adminId, companyId);

                HttpResult httpResult = new HttpResult();

                httpResult.string_value = "";

                return GetJson(httpResult);
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);
            }
        }

        [HttpPost]
        public ActionResult SaveContactData(SupplierContactGridData contact) {
            try {

                //SupplierRepository supplierRepository = new SupplierRepository();
                //Supplier supplier = new SupplierRepository().GetSupplierDataById(contact.supplier_id);
                //int officeId = supplier.SupplierGroup.Company.ElementAt(0).id;
                //if (officeId == DataNulls.INT_NULL) {
                //    throw new ExNotAuthorizedUpdateUser("Unknown Company");
                //}

                //if (!IsUserInOfficeRole(UserRole.OfficeAdministrator, officeId)
                //    && !IsUserInOfficeRole(UserRole.Orderer, officeId)) {
                //    throw new ExNotAuthorizedUpdateUser("Not authorized to update Address");
                //}
                if (!IsUserAuthorizedToEdit(contact.supplier_id)) {
                    throw new ExNotAuthorizedUpdateUser("Not authorized to update Contact");
                }

                if (!IsContactValid(contact)) {
                    throw new ExMissingMandatoryFields("Missing Mandatory Fields");
                }

                HttpResult httpResult = new HttpResult();

                List<string> msgItems;
                int contactId = new SupplierContactRepository().SaveSupplierContact(
                    contact,
                    CurrentUser.Participant.id,
                    out msgItems);


                httpResult.int_value = contactId;
                httpResult.string_value = GetErrorMsg(msgItems);

                return GetJson(httpResult);
            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }

        [HttpPost]
        public ActionResult DeleteSupplierContact(SupplierContactGridData suppContact) {
            try {
                if (!IsUserAuthorizedToEdit(suppContact.supplier_id)) {
                    throw new ExNotAuthorizedUpdateUser("Not authorized to Delete Contact");
                }

                SupplierContactRepository scRepository = new SupplierContactRepository();
                scRepository.DeleteSupplierContact(suppContact.id);

                HttpResult httpResult = new HttpResult();
                httpResult.string_value = null;

                return GetJson(httpResult);

            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);

            }
        }
        #endregion

        #region Methods
        public ActionResult Detail() {
            ViewBag.IsNew = true;

            ViewBag.HeaderImgUrl = GetRootUrl() + "/Content/Images/Menu/List16.png";
            ViewBag.HeaderTitle = RequestResource.Supplier + " - " + "{{angCtrl.supplier.supp_name}}";

            return View("Detail");
        }

        private bool IsContactValid(SupplierContactGridData contact) {
            if (String.IsNullOrEmpty(contact.email)) {
                return false;
            }
                        
            return true;
        }

        public bool IsUserAuthorizedToEdit(int supplierId) {
            SupplierRepository supplierRepository = new SupplierRepository();
            Supplier supplier = new SupplierRepository().GetSupplierDataById(supplierId);

            int officeId = supplier.SupplierGroup.Company.ElementAt(0).id;
            if (officeId == DataNulls.INT_NULL) {
                throw new ExNotAuthorizedUpdateUser("Unknown Company");
            }

            if (!IsUserInOfficeRole(UserRole.OfficeAdministrator, officeId)
                && !IsUserInOfficeRole(UserRole.Orderer, officeId)) {
                throw new ExNotAuthorizedUpdateUser("Not authorized to update Address");
            }

            return true;
        }

        
        #endregion
    }
}