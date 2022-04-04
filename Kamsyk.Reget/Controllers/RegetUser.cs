using Kamsyk.Reget.Model;
using Kamsyk.Reget.Model.Common;
using Kamsyk.Reget.Model.ExtendedModel;
using Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Kamsyk.Reget.Model.Repositories.UserRepository;

namespace Kamsyk.Reget.Controllers {
    public class RegetUser {
        #region Properties
        private Participants m_Participant = null;
        public Participants Participant {
            get { return m_Participant; }
            set { m_Participant = value; }
        }

        public string ParticipantName {
            get {
                if (m_Participant == null) {
                    return "Not Allowed User";
                }
                return m_Participant.first_name + " " + m_Participant.surname;
            }
        }

        public string ParticipantNameSurnameFirst {
            get {
                if (m_Participant == null) {
                    return "Not Allowed User";
                }
                return m_Participant.surname + " " + m_Participant.first_name;
            }
        }

        public string ParticipantNameFirstNameFirst {
            get {
                return ParticipantName;
            }
        }

        public string UserName {
            get {
                if (m_Participant == null) {
                    return null;
                }
                return m_Participant.user_name;
            }
        }

        public List<string> ParticipantRolesText {
            get { return GetCurrentUserRoles(); }
        }

        private List<int> m_ParticipantAdminCompanyIds = null;
        public List<int> ParticipantAdminCompanyIds {
            get {
                if (m_Participant == null) {
                    return null;
                }

                if (m_ParticipantAdminCompanyIds == null) {
                    m_ParticipantAdminCompanyIds = new List<int>();
                    var compAdminList = (from compAdminListDb in m_Participant.Participant_Office_Role
                                         where compAdminListDb.role_id == (int)UserRole.OfficeAdministrator
                                         select compAdminListDb).ToList();

                    foreach (var compAdmin in compAdminList) {
                        m_ParticipantAdminCompanyIds.Add(compAdmin.office_id);
                    }

                    //if (m_ParticipantAdminCompanyIds == null || m_ParticipantAdminCompanyIds.Count == 0) {
                    //    m_ParticipantAdminCompanyIds = new List<int> { Participant.company_id };
                    //}
                }

                return m_ParticipantAdminCompanyIds;
            }
        }

        private List<int> m_ParticipantSubstManCompanyIds = null;
        public List<int> ParticipantSubstManCompanyIds {
            get {
                if (m_Participant == null) {
                    return null;
                }

                if (m_ParticipantSubstManCompanyIds == null) {
                    m_ParticipantSubstManCompanyIds = new List<int>();
                    var compAdminList = (from compAdminListDb in m_Participant.Participant_Office_Role
                                         where compAdminListDb.role_id == (int)UserRole.SubstituteCompanyManager
                                         select compAdminListDb).ToList();

                    foreach (var compAdmin in compAdminList) {
                        m_ParticipantSubstManCompanyIds.Add(compAdmin.office_id);
                    }

                }

                return m_ParticipantSubstManCompanyIds;
            }
        }

        private List<int> m_ParticipantCompanyIds = null;
        public List<int> ParticipantCompanyIds {
            get {
                if (m_Participant == null) {
                    return null;
                }

                if (m_ParticipantCompanyIds == null) {
                    m_ParticipantCompanyIds = new List<int>();
                    var userRoles = (from compAdminListDb in m_Participant.ParticipantRole_CentreGroup
                                        select compAdminListDb).ToList();

                    foreach (var userRole in userRoles) {
                        if (!m_ParticipantCompanyIds.Contains(userRole.Centre_Group.company_id)) {
                            m_ParticipantCompanyIds.Add(userRole.Centre_Group.company_id);
                        }
                    }

                    
                }

                if (ParticipantAdminCompanyIds != null) {
                    foreach (var compId in ParticipantAdminCompanyIds) {
                        if (!m_ParticipantCompanyIds.Contains(compId)) {
                            m_ParticipantCompanyIds.Add(compId);
                        }
                    }
                }

                foreach (var compRole in m_Participant.Participant_Office_Role) {
                    if (!m_ParticipantCompanyIds.Contains(compRole.office_id)) {
                        m_ParticipantCompanyIds.Add(compRole.office_id);
                    }
                }

                return m_ParticipantCompanyIds;
            }
        }

        //private List<Company> m_SupplierAdminCompanies = null;
        //public List<Company> SupplierAdminCompanies {
        //    get {
        //        if (m_SupplierAdminCompanies == null) {
        //            m_SupplierAdminCompanies = GetSupplierAdminCompanies();
        //        }
        //        return m_SupplierAdminCompanies;
        //    }
        //}

        private List<CompanyExtended> m_UserCompanies = null;
        public List<CompanyExtended> UserCompanies {
            get {
                if (m_UserCompanies == null) {
                    m_UserCompanies = GetUserCompanies();
                }
                return m_UserCompanies;
            }
        }

        private List<int> m_UserCompaniesIds = null;
        public List<int> UserCompaniesIds {
            get {
                if (m_UserCompaniesIds == null) {
                    if (UserCompanies != null && UserCompanies.Count > 0) {
                        m_UserCompaniesIds = new List<int>();
                        foreach (var userComp in UserCompanies) {
                            m_UserCompaniesIds.Add(userComp.id);
                        }
                    }
                }

                return m_UserCompaniesIds;
            }
        }

        public int ParticipantId {
            get {
                if (m_Participant == null) {
                    return DataNulls.INT_NULL;
                }
                return m_Participant.id;
            }
        }

        
        public bool IsParticipantAreaPropAdmin {
            get {
                if (m_Participant == null) {
                    return false;
                }

                var areaAdmin = from c in m_Participant.ParticipantRole_CentreGroup
                                where c.role_id == (int)UserRole.CentreGroupPropAdmin
                                select c;

                return ((areaAdmin != null) && (areaAdmin.ToList().Count > 0));
            }
        }

        public bool IsParticipantMatrixAdmin {
            get {
                if (m_Participant == null) {
                    return false;
                }

                var areaAdmin = from c in m_Participant.ParticipantRole_CentreGroup
                                where c.role_id == (int)UserRole.ApproveMatrixAdmin
                                select c;

                return ((areaAdmin != null) && (areaAdmin.ToList().Count > 0));
            }
        }

        public bool IsParticipantOrderAdmin {
            get {
                if (m_Participant == null) {
                    return false;
                }

                var areaAdmin = from c in m_Participant.ParticipantRole_CentreGroup
                                where c.role_id == (int)UserRole.OrdererAdmin
                                select c;

                return ((areaAdmin != null) && (areaAdmin.ToList().Count > 0));
            }
        }

        public bool IsParticipantRequestorAdmin {
            get {
                if (m_Participant == null) {
                    return false;
                }

                var areaAdmin = from c in m_Participant.ParticipantRole_CentreGroup
                                where c.role_id == (int)UserRole.RequestorAdmin
                                select c;

                return ((areaAdmin != null) && (areaAdmin.ToList().Count > 0));
            }
        }

        public bool IsParticipantAreaAdmin {
            get {
                return (IsParticipantAreaPropAdmin || IsParticipantMatrixAdmin || IsParticipantOrderAdmin || IsParticipantRequestorAdmin);
            }
        }

        public bool IsParticipantRequestor {
            get {
                if (m_Participant == null) {
                    return false;
                }

                var part = from c in m_Participant.ParticipantRole_CentreGroup
                                where c.role_id == (int)UserRole.Requestor
                                select c;

                return ((part != null) && (part.ToList().Count > 0));
            }
        }

        public bool IsParticipantOrderer {
            get {
                if (m_Participant == null) {
                    return false;
                }

                var orderer = from c in m_Participant.ParticipantRole_CentreGroup
                                where c.role_id == (int)UserRole.Orderer
                                select c;

                return ((orderer != null) && (orderer.ToList().Count > 0));
            }
        }

        public bool IsParticipantAppAdmin {
            get {
                if (m_Participant == null) {
                    return false;
                }

                if (m_Participant.is_app_admin == true) {
                    return true;
                }

                return false;
            }
        }

        public bool IsParticipantCompanyAdmin {
            get {
                if (m_Participant == null) {
                    return false;
                }

                var oficeAdmin = from c in m_Participant.Participant_Office_Role
                               where c.role_id == (int)UserRole.OfficeAdministrator
                               select c;

                return ((oficeAdmin != null) && (oficeAdmin.ToList().Count > 0));
            }
        }

        public bool IsParticipantAppMan {
            get {
                if (m_Participant == null) {
                    return false;
                }

                var appMan = from c in m_Participant.ParticipantRole_CentreGroup
                                 where c.role_id == (int)UserRole.ApprovalManager
                                 select c;

                return ((appMan != null) && (appMan.ToList().Count > 0));
            }
        }

        public bool IsParticipantCompanyStatisticsManager {
            get {
                if (m_Participant == null) {
                    return false;
                }

                var appMan = from c in m_Participant.Participant_Office_Role
                             where c.role_id == (int)UserRole.StatisticsCompanyManager
                             select c;

                return ((appMan != null) && (appMan.ToList().Count > 0));
            }
        }

        public bool IsParticipantSubstituteManager {
            get {
                if (m_Participant == null) {
                    return false;
                }

                var substMan = from c in m_Participant.Participant_Office_Role
                             where c.role_id == (int)UserRole.SubstituteCompanyManager
                             select c;

                return ((substMan != null) && (substMan.ToList().Count > 0));
            }
        }

        public bool IsHasParticipantPhoto {
            get {
                if (m_Participant == null) {
                    return false;
                }

                if (m_Participant.ParticipantPhoto != null) {
                    return true;
                }

                return false;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get User Roles
        /// </summary>
        /// <returns></returns>
        private List<string> GetCurrentUserRoles() {
            if (m_Participant == null) {
                return null;
            }

            var sortUSerRole = m_Participant.ParticipantRole_CentreGroup.OrderBy(cr => cr.role_id);
            var sortCompUserRole = m_Participant.Participant_Office_Role.OrderBy(cr => cr.role_id);

            string strRequestor = "";
            string strApproveManager = "";
            string strOrderer = "";
            string strCentreGroupAdmin = "";
            string strReadOnly = "";
            string strCompAdmin = "";

            Hashtable htCgNameAdmin = new Hashtable();
            foreach (ParticipantRole_CentreGroup prCg in sortUSerRole) {
                if (prCg.Centre_Group.active == false) {
                    continue;
                }

                if (prCg.role_id == (int)UserRole.Requestor) {
                    if (strRequestor.Length > 0) {
                        strRequestor += ", ";
                    }
                    strRequestor += prCg.Centre_Group.name;
                }

                if (prCg.role_id == (int)UserRole.ApprovalManager) {
                    if (strApproveManager.Length > 0) {
                        strApproveManager += ", ";
                    }
                    strApproveManager += prCg.Centre_Group.name;
                }

                if (prCg.role_id == (int)UserRole.Orderer) {
                    if (strOrderer.Length > 0) {
                        strOrderer += ", ";
                    }
                    strOrderer += prCg.Centre_Group.name;
                }

                if (prCg.role_id == (int)UserRole.CentreGroupPropAdmin ||
                    prCg.role_id == (int)UserRole.ApproveMatrixAdmin ||
                    prCg.role_id == (int)UserRole.OrdererAdmin ||
                    prCg.role_id == (int)UserRole.RequestorAdmin) {
                    if (!htCgNameAdmin.ContainsKey(prCg.Centre_Group.name)) {
                        if (strCentreGroupAdmin.Length > 0) {
                            strCentreGroupAdmin += ", ";
                        }
                        strCentreGroupAdmin += prCg.Centre_Group.name;
                        htCgNameAdmin.Add(prCg.Centre_Group.name, null);
                    }
                }


            }

            foreach (Participant_Office_Role prCg in sortCompUserRole) {
                if (prCg.role_id == (int)UserRole.ReadOnly) {
                    if (strReadOnly.Length > 0) {
                        strReadOnly += ", ";
                    }
                    strReadOnly += prCg.Company.country_code;
                } else if (prCg.role_id == (int)UserRole.OfficeAdministrator) {
                    if (strCompAdmin.Length > 0) {
                        strCompAdmin += ", ";
                    }
                    strCompAdmin += prCg.Company.country_code;
                }
            }

            if (!String.IsNullOrEmpty(strRequestor)) {
                strRequestor = RequestResource.Requestor + ": " + strRequestor;
            }

            if (!String.IsNullOrEmpty(strApproveManager)) {
                strApproveManager = RequestResource.ApproveMan + ": " + strApproveManager;
            }

            if (!String.IsNullOrEmpty(strOrderer)) {
                strOrderer = RequestResource.Orderer + ": " + strOrderer;
            }

            if (!String.IsNullOrEmpty(strCentreGroupAdmin)) {
                strCentreGroupAdmin = RequestResource.CentreGroupAdmin + ": " + strCentreGroupAdmin;
            }

            if (!String.IsNullOrEmpty(strReadOnly)) {
                strReadOnly = RequestResource.ReadOnly + ": " + strReadOnly;
            }

            if (!String.IsNullOrEmpty(strCompAdmin)) {
                strCompAdmin = RequestResource.CompanyAdmin + ": " + strCompAdmin;
            }


            List<string> strRoles = new List<string>();
            if (!String.IsNullOrEmpty(strRequestor)) {
                strRoles.Add(strRequestor);
            }

            if (!String.IsNullOrEmpty(strApproveManager)) {
                strRoles.Add(strApproveManager);
            }

            if (!String.IsNullOrEmpty(strOrderer)) {
                strRoles.Add(strOrderer);
            }

            if (!String.IsNullOrEmpty(strCentreGroupAdmin)) {
                strRoles.Add(strCentreGroupAdmin);
            }

            if (!String.IsNullOrEmpty(strReadOnly)) {
                strRoles.Add(strReadOnly);
            }

            if (!String.IsNullOrEmpty(strCompAdmin)) {
                strRoles.Add(strCompAdmin);
            }

            return strRoles;

        }

        public bool IsParticipantSubstituteManagerOfCompany(int companyId) {
            
                if (m_Participant == null) {
                    return false;
                }

                var substMan = from c in m_Participant.Participant_Office_Role
                               where c.role_id == (int)UserRole.SubstituteCompanyManager
                               && c.office_id == companyId
                               select c;

                return ((substMan != null) && (substMan.ToList().Count > 0));
            
        }

        //private List<Company> GetSupplierAdminCompanies() {
        //    var compAdminList = (from compAdminListDb in m_Participant.Participant_Office_Role
        //                         where (compAdminListDb.role_id == (int)UserRole.OfficeAdministrator || 
        //                         compAdminListDb.role_id == (int)UserRole.SupplierAdmin)
        //                         select compAdminListDb).ToList();

        //    Hashtable htCompanies = new Hashtable();
        //    List<Company> companies = new List<Company>();
        //    if (compAdminList != null) {
        //        foreach (var adminRole in compAdminList) {
        //            if (!htCompanies.ContainsKey(adminRole.Company.id)) {
        //                Company company = new Company();
        //                company.id = adminRole.Company.id;
        //                company.country_code = adminRole.Company.country_code;
        //                companies.Add(company);
        //            }
        //        }
        //    }

        //    List<Company> retCompanies = companies.OrderBy(x => x.country_code).ToList();

        //    return retCompanies;
        //}

        private List<CompanyExtended> GetUserCompanies() {
            List<CompanyExtended> companies = new List<CompanyExtended>();

            List<int> lstPassedCg = new List<int>();
            List<int> lstPassedCompanies = new List<int>();

            foreach (var prCg in m_Participant.ParticipantRole_CentreGroup) {
                if (prCg.Centre_Group.active != true) {
                    continue;
                }

                bool isCompanyAdmin = (prCg.role_id == (int)UserRole.OfficeAdministrator);
                //bool isSupplierAdmin = (prCg.role_id == (int)UserRole.SupplierAdmin);

                if (lstPassedCg.Contains(prCg.centre_group_id)) {
                    if (prCg.role_id == (int)UserRole.SupplierAdmin) {
                        var comp = (from compRet in companies
                                    where compRet.id == prCg.Centre_Group.Company.id
                                    select compRet).FirstOrDefault();
                        if (comp != null) {
                            if (isCompanyAdmin) {
                                comp.is_company_admin = true;
                                comp.is_supplier_admin = true;
                            }
                            //if (isSupplierAdmin) {
                            //    comp.is_supplier_admin = true;
                            //}
                        }
                    }
                                        
                    continue;
                }

                lstPassedCg.Add(prCg.centre_group_id);
                

                if (prCg.Centre_Group.active == false) {
                    continue;
                }

                if (lstPassedCompanies.Contains(prCg.Centre_Group.Company.id)) {
                    continue;
                } else {
                    lstPassedCompanies.Add(prCg.Centre_Group.Company.id);
                }

                if (prCg.Centre_Group.Company.active == false) {
                    continue;
                }

                CompanyExtended company = new CompanyExtended();
                company.id = prCg.Centre_Group.Company.id;
                company.country_code = prCg.Centre_Group.Company.country_code;
                company.supplier_source = prCg.Centre_Group.Company.supplier_source;
                company.is_supplier_maintenance_allowed = (prCg.Centre_Group.Company.is_supplier_maintenance_allowed == true);
                if (isCompanyAdmin) {
                    company.is_company_admin = true;
                    company.is_supplier_admin = true;
                }
                
                companies.Add(company);
            }


            foreach (var por in m_Participant.Participant_Office_Role) {
                if (por.Company.active != true) {
                    continue;
                }

                bool isCompanyAdmin = (por.role_id == (int)UserRole.OfficeAdministrator);
                bool isSupplierAdmin = (por.role_id == (int)UserRole.SupplierAdmin);
                //bool isCompanyStatisticsAdmin = (por.role_id == (int)UserRole.StatisticsCompanyManager);

                if (lstPassedCompanies.Contains(por.Company.id)) {
                    var comp = (from compRet in companies
                                where compRet.id == por.Company.id
                                select compRet).FirstOrDefault();
                    if (comp != null) {
                        if (isCompanyAdmin) {
                            comp.is_company_admin = true;
                            comp.is_company_admin = true;
                        }
                        if (isSupplierAdmin) {
                            comp.is_supplier_admin = true;
                        }
                    }
                    continue;
                }
                lstPassedCompanies.Add(por.Company.id);


                if (por.Company.active == false) {
                    continue;
                }

                CompanyExtended company = new CompanyExtended();
                company.id = por.Company.id;
                company.country_code = por.Company.country_code;
                company.is_supplier_admin = (por.role_id == (int)UserRole.OfficeAdministrator) || (por.role_id == (int)UserRole.SupplierAdmin);
                company.is_company_admin = (por.role_id == (int)UserRole.OfficeAdministrator);
                company.is_supplier_maintenance_allowed = (por.Company.is_supplier_maintenance_allowed == true);
                company.supplier_source = por.Company.supplier_source;
                if (isCompanyAdmin) {
                    company.is_company_admin = true;
                    company.is_company_admin = true;
                }
                if (isSupplierAdmin) {
                    company.is_supplier_admin = true;
                }
                //company.is_company_statistics_admin = isCompanyStatisticsAdmin;
                companies.Add(company);
            }


            List<CompanyExtended> retCompanies = companies.OrderBy(x => x.country_code).ToList();

            return retCompanies;
        }
        #endregion
    }
}