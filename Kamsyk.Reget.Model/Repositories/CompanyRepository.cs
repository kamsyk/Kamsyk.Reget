using Kamsyk.Reget.Model.Common;
using Kamsyk.Reget.Model.DataDictionary;
using Kamsyk.Reget.Model.ExtendedModel;
using Kamsyk.Reget.Model.Repositories.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static Kamsyk.Reget.Model.Repositories.UserRepository;

namespace Kamsyk.Reget.Model.Repositories {
    public class CompanyRepository : BaseRepository<Company>, ICompanyRepository {
        #region Constants

        #endregion

        #region Enum

        #endregion

        #region Properties
        private ICompanyRepository m_TestICompanyRepository = null;
        #endregion

        #region Constructor
        public CompanyRepository() : base() { }

        public CompanyRepository(ICompanyRepository ICompanyRepository) : base() { }
        #endregion

        #region Methods
        public Company GetCompanyById(int countryId) {

            var company = (from countryDb in m_dbContext.Company
                               where countryDb.id == countryId
                               select countryDb).FirstOrDefault();

                        
            return company;

        }


        public List<Company> GetActiveCompanies() {

            var companies = (from compDb in m_dbContext.Company
                           where compDb.active == true
                           select compDb).ToList();


            return companies;

        }

        public List<CompanyDropDown> GetActiveCompaniesByUserId(int userId, List<int> companyAdminIds) {

            var companies = (from compDb in m_dbContext.Company
                             join cgDb in m_dbContext.Centre_Group on
                             compDb.id equals cgDb.company_id
                             join prCgDb in m_dbContext.ParticipantRole_CentreGroup
                             on cgDb.id equals prCgDb.centre_group_id
                             where compDb.active == true &&
                             //cgDb.active == true &&
                             prCgDb.participant_id == userId
                             orderby compDb.country_code
                             select compDb).Distinct();

            Hashtable htCompanies = new Hashtable();
            List<CompanyDropDown> companiesDd = new List<CompanyDropDown>();
            foreach (var company in companies) {
                CompanyDropDown companyDropDown = new CompanyDropDown();
                companyDropDown.company_id = company.id;
                companyDropDown.country_code = company.country_code;

                companiesDd.Add(companyDropDown);

                htCompanies.Add(company.id, null);
            }

            if (companyAdminIds != null) {
                foreach (var compId in companyAdminIds) {
                    if (!htCompanies.ContainsKey(compId)) {
                        var company = (from compDb in m_dbContext.Company
                                       where compDb.id == compId
                                       select compDb).FirstOrDefault();

                        CompanyDropDown companyDropDown = new CompanyDropDown();
                        companyDropDown.company_id = company.id;
                        companyDropDown.country_code = company.country_code;

                        companiesDd.Add(companyDropDown);
                    }
                }
            }

            var tmpCgs = companiesDd.Distinct().OrderBy(x => x.country_code).ToList();

            return tmpCgs;

        }

        public int GetCompanyIdByName(string officeName) {

            var company = (from com in m_dbContext.Company
                           where com.country_code == officeName
                           select com).FirstOrDefault();

            if (company != null) {
                return company.id;
            }


            return DataNulls.INT_NULL;

        }

        public bool IsSupplierMaintenanceAllowed(int companyId) {

            var company = (from com in m_dbContext.Company
                           where com.id == companyId
                           select com).FirstOrDefault();

            if (company == null) {
                return false;
            }


            return (company.is_supplier_maintenance_allowed == true);

        }

        public DateTime GetLastSupplierUploadDate(int id) {
            var comp = (from dbComp in m_dbContext.Company
                        where dbComp.id == id
                        select new { last_supplier_upload = dbComp.last_supplier_upload }).FirstOrDefault();

            if (comp != null) {
                if (comp.last_supplier_upload == null) {
                    return DataNulls.DATETIME_NULL;
                }

                return (DateTime)comp.last_supplier_upload;
            }

            return DataNulls.DATETIME_NULL;
        }

        public void SaveSuppMaintenance(
            int companyId, 
            bool isManualSuppMaintenance,
            int currentUserId,
            out string eventErrorMsg) {

            var comp = (from dbComp in m_dbContext.Company
                        where dbComp.id == companyId
                        select dbComp).FirstOrDefault();

            comp.is_supplier_maintenance_allowed = isManualSuppMaintenance;

            string msg = "Allow Manual Supplier Maintenance=" + (isManualSuppMaintenance ? "Yes" : "No");
            eventErrorMsg = null;
            AddMaintenanceEvent(
                EventRepository.EventCode.SupplierManualMaintModif,
                msg,
                comp.id,
                comp.country_code,
                DataNulls.INT_NULL,
                null,
                DataNulls.INT_NULL,
                null,
                DataNulls.INT_NULL,
                null,
                currentUserId,
                ref eventErrorMsg);

            m_dbContext.SaveChanges();
        }

        public void AddParentPg(int companyId, int parentPgId) {
            var comp = (from dbComp in m_dbContext.Company
                        where dbComp.id == companyId
                        select dbComp).FirstOrDefault();

            var ppg = (from dbPpg in m_dbContext.Parent_Purchase_Group
                        where dbPpg.id == parentPgId
                       select dbPpg).FirstOrDefault();

            comp.Parent_Purchase_Group.Add(ppg);

            m_dbContext.SaveChanges();
        }

        public int GetMaxCompanyId() {

            var company = (from compDb in m_dbContext.Company
                           orderby compDb.id descending
                           select compDb).Take(1).FirstOrDefault();

            if (company == null) {
                return -1;
            }

            return company.id;

        }

        public IEnumerable<int> GetActiveCompaniesWoSubstApproval() {

           
            var companies = (from compDb in m_dbContext.Company
                             join userRoleDb in m_dbContext.Participant_Office_Role
                             on compDb.id equals userRoleDb.office_id
                             where compDb.active == true
                             && userRoleDb.role_id != (int)UserRole.SubstitutionApproveManager
                             select compDb.id).ToList().Distinct();


            return companies;

        }

        public IEnumerable<int> GetActiveCompaniesWithSubstApproval() {
            
            var companies = (from compDb in m_dbContext.Company
                             join userRoleDb in m_dbContext.Participant_Office_Role
                             on compDb.id equals userRoleDb.office_id
                             where compDb.active == true
                             && userRoleDb.role_id == (int)UserRole.SubstitutionApproveManager
                             select compDb.id).ToList().Distinct();


            return companies;

        }

        public IEnumerable<int> GetActiveCompaniesWithoutSubstApproval() {
            string strSelect = "SELECT cd.* from " + CompanyData.TABLE_NAME + " cd "
                + "LEFT OUTER JOIN " + ParticipantOfficeRoleData.TABLE_NAME + " pord "
                + " ON cd." + CompanyData.ID_FIELD + "=pord." + ParticipantOfficeRoleData.OFFICE_ID_FIELD
                + " AND pord." + ParticipantOfficeRoleData.ROLE_ID_FIELD + "=" + (int)UserRole.SubstitutionApproveManager
                + " WHERE pord." + ParticipantOfficeRoleData.OFFICE_ID_FIELD + " IS NULL";
            List<Company> companies = m_dbContext.Company.SqlQuery(strSelect).ToList<Company>();

            var companiesIds = (from compDb in companies
                             select compDb.id).ToList().Distinct();

            return companiesIds;

        }
        #endregion
    }
}
