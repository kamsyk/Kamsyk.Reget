using Kamsyk.Reget.Model.Repositories.Interfaces;
using System.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Kamsyk.Reget.Model.ExtendedModel;
using Kamsyk.Reget.Model.Common;
using Kamsyk.Reget.Model.DataDictionary;
using System.Collections;
using Kamsyk.Reget.Model.ExtendedModel.HttpResult;

namespace Kamsyk.Reget.Model.Repositories {
    public class AddressRepository : BaseRepository<Address>, IAddressRepository {
        #region Constant
        
        #endregion

        #region Enums
       
        #endregion

        #region Methods
        public List<AddressAdminExtended> GetAddressAdminData(
            List<int> companyIds,
            string filter,
            string sort,
            int pageSize,
            int pageNr,
            out int rowsCount) {

            string strFilterWhere = GetFilter(filter);
            string strOrder = GetOrder(sort);
            
            string sqlPure = "SELECT ad.*, cd.country_code, ROW_NUMBER() OVER(" + strOrder + ") AS RowNum";
            string sqlPureBody = GetPureBody(companyIds, strFilterWhere);

            //Get Row count
            string selectCount = "SELECT COUNT(*) " + sqlPureBody;
            rowsCount = m_dbContext.Database.SqlQuery<int>(selectCount).Single();

            //Get Part Data
            string sqlPart = sqlPure + sqlPureBody;
            int partStart = pageSize * (pageNr - 1) + 1;
            int partStop = partStart + pageSize - 1;

            while (partStart > rowsCount) {
                partStart -= pageSize;
                partStart = partStart + pageSize - 1;
            }

            string sql = "SELECT * FROM(" + sqlPart + ") AS RegetPartData" +
                " WHERE RegetPartData.RowNum BETWEEN " + partStart + " AND " + partStop;

            var tmpAddresses = m_dbContext.Database.SqlQuery<Address>(sql).ToList();

            Hashtable htCompany = new Hashtable();
            List<AddressAdminExtended> addresses = new List<AddressAdminExtended>();
            int rowIndex = (pageNr - 1) * pageSize + 1;
            foreach (var tmpAddress in tmpAddresses) {
                AddressAdminExtended address = new AddressAdminExtended();
                SetValues(tmpAddress, address);

                address.row_index = rowIndex++;
                
                //company                 
                if (htCompany.ContainsKey(tmpAddress.company_id)) {
                    address.company_name_text = htCompany[tmpAddress.company_id].ToString();
                } else {
                    var company = (from compDb in m_dbContext.Company
                                   where compDb.id == tmpAddress.company_id
                                   select compDb).FirstOrDefault();
                    address.company_name_text = company.country_code;
                    htCompany.Add(company.id, company.country_code);
                }

                addresses.Add(address);

            }

            return addresses;
            
        }

        public List<AddressAdminExtended> GetAddressAdminData(List<int> companyIds, string filter, string sort) {
            string strFilterWhere = GetFilter(filter);
            string strOrder = GetOrder(sort);

            string sqlPure = "SELECT ad.*, cd.country_code";
            string sqlPureBody = GetPureBody(companyIds, strFilterWhere);

            string sql = sqlPure + sqlPureBody;

            var tmpAddresses = m_dbContext.Database.SqlQuery<Address>(sql).ToList();
            Hashtable htCompany = new Hashtable();
            List<AddressAdminExtended> addresses = new List<AddressAdminExtended>();
            
            foreach (var tmpAddress in tmpAddresses) {
                AddressAdminExtended address = new AddressAdminExtended();
                SetValues(tmpAddress, address);
                               
                //company                 
                if (htCompany.ContainsKey(tmpAddress.company_id)) {
                    address.company_name_text = htCompany[tmpAddress.company_id].ToString();
                } else {
                    var company = (from compDb in m_dbContext.Company
                                   where compDb.id == tmpAddress.company_id
                                   select compDb).FirstOrDefault();
                    address.company_name_text = company.country_code;
                    htCompany.Add(company.id, company.country_code);
                }

                addresses.Add(address);

            }

            return addresses;
        }

        public List<AddressDbGrid> GetAddressDataByAddressText(string addressText, string companyName) {
            if (String.IsNullOrEmpty(addressText)) {
                List<AddressDbGrid> addresses = (from adDb in m_dbContext.Address
                                                 join compDb in m_dbContext.Company
                                                 on adDb.company_id equals compDb.id
                                                 where compDb.country_code == companyName 
                                                 select new AddressDbGrid {
                                                     id = adDb.id,
                                                     address_text = adDb.address_text }).ToList();

                return addresses;
            } else {
                List<AddressDbGrid> addresses = (from adDb in m_dbContext.Address
                                                 join compDb in m_dbContext.Company
                                                 on adDb.company_id equals compDb.id
                                                 where compDb.country_code == companyName &&
                                           adDb.address_text.Contains(addressText)
                                                 select new AddressDbGrid {
                                                     id = adDb.id,
                                                     address_text = adDb.address_text }).ToList();

                return addresses;
            }
        }

        private string GetFilter(string filter) {
            string strFilterWhere = "";
            if (!String.IsNullOrEmpty(filter)) {
                string[] filterItems = filter.Split(UrlParamDelimiter.ToCharArray());
                foreach (string filterItem in filterItems) {
                    string[] strItemProp = filterItem.Split(UrlParamValueDelimiter.ToCharArray());
                    strFilterWhere += " AND ";

                    string columnName = strItemProp[0].Trim().ToUpper();
                    if (columnName == AddressData.COMPANY_NAME_FIELD.Trim().ToUpper()) {
                        strFilterWhere += "ad." + AddressData.COMPANY_NAME_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                    }
                    if (columnName == AddressData.STREET_FIELD.Trim().ToUpper()) {
                        strFilterWhere += "ad." + AddressData.STREET_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                    }
                    if (columnName == AddressData.CITY_FIELD.Trim().ToUpper()) {
                        strFilterWhere += "ad." + AddressData.CITY_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                    }
                    if (columnName == AddressData.ZIP_FIELD.Trim().ToUpper()) {
                        strFilterWhere += "ad." + AddressData.ZIP_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                    }
                    if (columnName == "company_name_text".Trim().ToUpper()) {
                        int officeId = new CompanyRepository().GetCompanyIdByName(strItemProp[1]);
                        strFilterWhere += "ad." + AddressData.COMPANY_ID_FIELD + "=" + officeId;
                    }
                }
            }

            return strFilterWhere;
        }

        private string GetOrder(string sort) {
            string strOrder = "ORDER BY ad." + AddressData.ID_FIELD;

            if (!String.IsNullOrEmpty(sort)) {
                strOrder = "";
                string[] sortItems = sort.Split(UrlParamDelimiter.ToCharArray());
                foreach (string sortItem in sortItems) {
                    string[] strItemProp = sortItem.Split(UrlParamValueDelimiter.ToCharArray());
                    if (strOrder.Length > 0) {
                        strOrder += ", ";
                    } else if (strItemProp[0] == "company_name_text") {
                        strOrder = "cd." + CompanyData.COUNTRY_CODE_FIELD + " " + strItemProp[1]; ;
                    } else {
                        strOrder += "ad." + strItemProp[0] + " " + strItemProp[1];
                    }
                }

                strOrder = " ORDER BY " + strOrder;
            }

            return strOrder;
        }

        private string GetPureBody(List<int> companyIds, string strFilterWhere) {
            string sqlPureBody = " FROM " + AddressData.TABLE_NAME + " ad" +
                 " INNER JOIN " + CompanyData.TABLE_NAME + " cd" +
                " ON ad." + AddressData.COMPANY_ID_FIELD + "=cd." + CompanyData.ID_FIELD +
                " WHERE ad." + AddressData.COMPANY_ID_FIELD + " IN" + GetSqlIn(companyIds);
            sqlPureBody += strFilterWhere;

            return sqlPureBody;
        }

        public List<Address> GetAddressReport(List<int> companyIds) {
            var addresses = (from ad in m_dbContext.Address
                                where companyIds.Contains(ad.company_id) 
                                select ad).ToList();

            return addresses;
        }

        public Address GetAddressDataById(int addressId) {
            var addresses = (from addDb in m_dbContext.Address
                             where addDb.id == addressId
                             select addDb).FirstOrDefault();

            return addresses;
        }

       
        public int SaveAddressData(AddressAdminExtended modifAddress, int userId, out List<string> msg) {
            msg = new List<string>();

            HttpResult httpResult = new HttpResult();

            
            using (TransactionScope transaction = new TransactionScope()) {
                try {
                    var dbAddress = new Address();

                    string addressText = GetFullAddress(modifAddress);
                    var companies = (from compDb in m_dbContext.Company
                                     where compDb.country_code == modifAddress.company_name_text
                                     select compDb).FirstOrDefault();
                    int compId = companies.id;

                    if (modifAddress.id >= 0) {

                        dbAddress = (from addr in m_dbContext.Address
                                     where addr.id == modifAddress.id
                                     select addr).FirstOrDefault();

                    } else {
                        var dbUAddress = (from addr in m_dbContext.Address
                                         where addr.address_text == addressText &&
                                         addr.company_id == compId
                                         select addr).FirstOrDefault();

                        if (dbUAddress != null) {
                            //duplicity
                            msg.Add(DUPLICITY);
                            return -1;
                        }
                    }

                    dbAddress.company_name = modifAddress.company_name;
                    dbAddress.street = modifAddress.street;
                    dbAddress.city = modifAddress.city;
                    dbAddress.zip = modifAddress.zip;
                    dbAddress.address_text = GetFullAddress(modifAddress);
                    dbAddress.modify_user = userId;
                    dbAddress.modify_date = DateTime.Now;
                                        
                    dbAddress.company_id = compId;
                                        
                    if (modifAddress.id < 0) {
                        
                        var lastAddr = (from addr in m_dbContext.Address
                                        orderby addr.id descending
                                        select addr).Take(1).FirstOrDefault();

                        int lastId = -1;
                        if (lastAddr != null) {
                            lastId = lastAddr.id;
                        } 

                        //int lastId = lastAddr.id;
                        lastId++;
                        dbAddress.id = lastId;
                        //dbParticipant.user_name = modifParticipant.user_name;

                       
                        m_dbContext.Address.Add(dbAddress);
                    }

                    SaveChanges();
                    transaction.Complete();

                    return dbAddress.id;
                } catch (Exception ex) {
                    throw ex;
                }
            }
        }

        public void DeleteAddress(int addressId) {
            
            using (TransactionScope transaction = new TransactionScope()) {
                try {
                    
                    var address = (from partDb in m_dbContext.Address
                                where partDb.id == addressId
                                select partDb).FirstOrDefault();

                    var centres = (from centreDb in m_dbContext.Centre
                                   where centreDb.address_id == addressId
                                   select centreDb).ToList();
                    if (centres != null) {
                        foreach (var centre in centres) {
                            centre.address_id = null; 
                        }
                    }

                    m_dbContext.Address.Remove(address);
                                        
                    SaveChanges();
                    transaction.Complete();
                                        
                } catch (Exception ex) {
                    throw ex;
                }
            }
        }

        public List<AgDropDown> GetAddressesDataByCompanyId(List<int> companyIds) {
            var tmpAddresses = (from add in m_dbContext.Address
                                where companyIds.Contains(add.company_id)
                                select add).ToList();

            List<AgDropDown> addresses = new List<AgDropDown>();
            foreach (var tmpAddress in tmpAddresses) {
                string strFullAddress = GetFullAddress(tmpAddress);
                
                AgDropDown agDropDown = new AgDropDown();
                agDropDown.label = strFullAddress;
                agDropDown.value = strFullAddress;

                addresses.Add(agDropDown);

            }

            return addresses;
        }

        private static string GetFullAddress(Address address) {
            string strFullAddress = address.company_name;
            if (!String.IsNullOrEmpty(address.street)) {
                strFullAddress += ", " + address.street;
            }
            if (!String.IsNullOrEmpty(address.city)) {
                strFullAddress += ", " + address.city;
            }
            if (!String.IsNullOrEmpty(address.zip)) {
                strFullAddress += ", " + address.zip;
            }

            return strFullAddress;
        }

        public void SetAddresses() {
            var addresses = (from addressDb in m_dbContext.Address
                           select addressDb).ToList();

            foreach (Address address in addresses) {

                var strAddressText = GetFullAddress(address);
                address.address_text = strAddressText;
            }

            m_dbContext.SaveChanges();
        }

        public void DeleteAddressByName(string strName) {
            List<Address> addresses = (from addDb in m_dbContext.Address
                                    where addDb.company_name == strName
                                    select addDb).ToList();
            foreach (var address in addresses) {
                DeleteAddress(address.id);
            }
        }
        #endregion
    }
}
