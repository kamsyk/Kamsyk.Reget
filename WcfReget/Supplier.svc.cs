using OTISCZ.ConcordeDataDictionary;
using Kamsyk.Reget.Model.Common;
using Kamsyk.Reget.Model.DataDictionary;
using Kamsyk.Reget.Model.Repositories;
using OTISCZ.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfReget.Authorization;
using Kamsyk.ExcelOpenXml.ExcelActiveX;
using Kamsyk.Reget.Interface;
using System.Security.Principal;
using OTISCZ.OtisUser;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using WcfReget.Common;
using OTISCZ.Baan.DbLayer.DbData;

namespace WcfReget {
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Supplier : BaseWcf, ISupplier {
        #region Constants
        private const string CXAL_USER_NAME = "ConcordeWebProxyClient";
        private const string CXAL_PASSWORD = "d4ghj6,p}87'";
        private const string CXAL_ENCRYPT_PASSWORD = "rg.;r5er8ee.-";
        #endregion

        #region Properties
        public enum ConcordeVersion {
            CZ,
            SK,
            ESC,
            Unknown
        }
        #endregion

        #region Service Methods
        public void ImportSupplier(int companyId) {
            //try {
            //    Authenticate(Authentication);
            //} catch (Exception ex) {
            //    throw new Exception("Unauthorized Access!!!\r\n" + ex.Message);
            //}

            var comp = new CompanyRepository().GetCompanyById(companyId);
            if (comp.is_supplier_maintenance_allowed == true) {
                return;
            }

            int sgId = -1;

            switch (companyId) {
                case 0:
                case 1:
                case 2:
                    LoadConcordeSuppliers(companyId);
                    break;
                case 4:
                case 8:
                    WsSupplier.WsSupplier wsSupplierBaan = new WsSupplier.WsSupplier();
                    wsSupplierBaan.Timeout = 900000;
                    sgId = new SupplierRepository().GetCentreGroupId(companyId);
                    wsSupplierBaan.ImportSuppliersFromBaanFile(sgId);
                    break;
                case 3:
                    throw new Exception("Suppliers import has not been implemented yet for this office");
                case 6:
                    //ImportSlovenianSuppliers();
                    ImportSuppliersFromBaan(6, 740);
                    break;
                case 7:
                    ImportSerbianSuppliers();
                    break;
                default:
                    throw new Exception("Suppliers import has not been implemented yet for this office");
                    
            }

            
        }

        #region Slovenian Suppliers
        private void ImportSlovenianSuppliers() {
            int supplierGroupId = 6;
                        
            //*** work without impersonate *******************************************************************
            WindowsImpersonationContext wic = new UserClass().ImpersonateUser(
                ConfigUser.SupplierExcelUserName,
                ConfigUser.SupplierExcelUserDomain,
                ConfigUser.SupplierExcelUserPwd);
            //*************************************************************************************************

            string tmpSuppFileName = RegetFolder.GetSuppTmpFileName(supplierGroupId);

            File.Copy(RegetFolders.SuppSlovenianMasterFile, tmpSuppFileName);
            Kamsyk.ExcelOpenXml.ExcelActiveX.Application excelApp = new Kamsyk.ExcelOpenXml.ExcelActiveX.Application();
            var wb = excelApp.Workbooks.Open(tmpSuppFileName);
            var ws = wb.Worksheets[0];

            string startText = "Supplier Name";

            try {
                
                int iRow = 1;
                bool isReady = false;
                while (!isReady && iRow < 100) {
                    object oValue = GetSupplierCellValue(ws.Cells(iRow, 1));
                    if (oValue != null) {
                        string strValue = oValue.ToString().ToUpper().Trim();
                        if (startText.ToUpper().Trim() == strValue) {
                            isReady = true;
                        }
                    }

                    iRow++;
                }

                if (!isReady) {
                    return;
                }


                int emptyRowsCount = 0;
                List<Kamsyk.Reget.Interface.DbEntity.Supplier> suppliers = new List<Kamsyk.Reget.Interface.DbEntity.Supplier>();
                while (emptyRowsCount < 10) {
                    var oValue = GetSupplierCellValue(ws.Cells(iRow, 1));

                    if (oValue != null) {
                        emptyRowsCount = 0;
                        Kamsyk.Reget.Interface.DbEntity.Supplier supplier = new Kamsyk.Reget.Interface.DbEntity.Supplier();

                        string suplierId = GetSupplierCellValue(ws.Cells(iRow, 3));
                        if (suplierId != null) {
                            suplierId = suplierId.Replace("SI", "");
                        }
                        supplier.supplier_id = suplierId;

                        supplier.dic = GetSupplierCellValue(ws.Cells(iRow, 4));

                        supplier.supp_name = GetSupplierCellValue(ws.Cells(iRow, 1));

                        supplier.street_part1 = GetSupplierCellValue(ws.Cells(iRow, 5));
                        supplier.city = GetSupplierCellValue(ws.Cells(iRow, 6));
                        supplier.zip = GetSupplierCellValue(ws.Cells(iRow, 7));
                        string country = GetSupplierCellValue(ws.Cells(iRow, 8));
                        if (country == null || country.Trim().Length == 0) {
                            country = "Slovenia";
                        }
                        supplier.country = country;

                        supplier.supplier_local_app_id = GetSupplierCellValue(ws.Cells(iRow, 2));
                        supplier.supplier_group_id = supplierGroupId;
                        supplier.active = true;

                        if (supplier.supp_name == null || supplier.supp_name.Trim().Length == 0) {
                            iRow++;
                            continue;
                        }


                        suppliers.Add(supplier);


                    } else {
                        emptyRowsCount++;
                    }

                    iRow++;
                }

                new SupplierImport().ImportSuppliers(supplierGroupId, suppliers);
            } catch (Exception ex) {
                throw ex;
            } finally {
                excelApp.Quit();
                RegetFolder.DeleteTmpSuppFiles();
                if (wic != null) {
                    wic.Undo();
                }
            }
        }
        #endregion

        #region Serbian Suppliers
        private void ImportSerbianSuppliers() {
            int supplierGroupId = 7;
                        
            //*** work without impersonate *******************************************************************
            WindowsImpersonationContext wic = new UserClass().ImpersonateUser(
                ConfigUser.SupplierExcelUserName,
                ConfigUser.SupplierExcelUserDomain,
                ConfigUser.SupplierExcelUserPwd);
            //*************************************************************************************************

            string tmpSuppFileName = RegetFolder.GetSuppTmpFileName(supplierGroupId);

            File.Copy(RegetFolders.SuppSerbianMasterFile, tmpSuppFileName);
            Kamsyk.ExcelOpenXml.ExcelActiveX.Application excelApp = new Kamsyk.ExcelOpenXml.ExcelActiveX.Application();
            var wb = excelApp.Workbooks.Open(tmpSuppFileName);
            var ws = wb.Worksheets[0];

            string startText = "ŠIFRA";

            int iRow = 1;
            try {
                                
                bool isReady = false;
                while (!isReady && iRow < 100) {
                    object oValue = GetSupplierCellValue(ws.Cells(iRow, 1));
                    if (oValue != null) {
                        string strValue = oValue.ToString().ToUpper().Trim();
                        if (startText.ToUpper().Trim() == strValue) {
                            isReady = true;
                        }
                    }

                    iRow++;
                }

                if (!isReady) {
                    return;
                }


                int emptyRowsCount = 0;
                List<Kamsyk.Reget.Interface.DbEntity.Supplier> suppliers = new List<Kamsyk.Reget.Interface.DbEntity.Supplier>();
                while (emptyRowsCount < 10) {
                    var oValue = GetSupplierCellValue(ws.Cells(iRow, 2));

                    if (oValue != null) {
                        emptyRowsCount = 0;
                        Kamsyk.Reget.Interface.DbEntity.Supplier supplier = new Kamsyk.Reget.Interface.DbEntity.Supplier();

                        string suplierId = GetSupplierCellValue(ws.Cells(iRow, 7));
                        supplier.supplier_id = suplierId;

                        supplier.supp_name = GetSupplierCellValue(ws.Cells(iRow, 2));

                        supplier.street_part1 = GetSupplierCellValue(ws.Cells(iRow, 3));
                        supplier.city = GetSupplierCellValue(ws.Cells(iRow, 4));
                        supplier.zip = GetSupplierCellValue(ws.Cells(iRow, 5));
                        string country = GetSupplierCellValue(ws.Cells(iRow, 6));
                        if (country == null || country.Trim().Length == 0) {
                            country = "Serbia";
                        }
                        supplier.country = country;

                        supplier.phone = GetSupplierCellValue(ws.Cells(iRow, 8));
                        supplier.fax = GetSupplierCellValue(ws.Cells(iRow, 9));

                        supplier.supplier_local_app_id = GetSupplierCellValue(ws.Cells(iRow, 1));
                        supplier.supplier_group_id = supplierGroupId;
                        supplier.active = true;

                        if (supplier.supp_name == null || supplier.supp_name.Trim().Length == 0) {
                            iRow++;
                            continue;
                        }


                        suppliers.Add(supplier);


                    } else {
                        emptyRowsCount++;
                    }

                    iRow++;
                }

                new SupplierImport().ImportSuppliers(supplierGroupId, suppliers);
            } catch (Exception ex) {
                throw ex;
            } finally {
                excelApp.Quit();
                RegetFolder.DeleteTmpSuppFiles();
                if (wic != null) {
                    wic.Undo();
                }
            }
        }
        #endregion

        private string GetSupplierCellValue(Kamsyk.ExcelOpenXml.ExcelActiveX.Cell oCell) {
            if (oCell == null || oCell.Value == null || oCell.Value.ToString().Trim().Length == 0) {
                return null;
            }

            return oCell.Value.ToString().Trim();
        }

        private void LoadConcordeSuppliers(int companyId) {
            WsConcordeSuplier.InternalRequest wsConcordeSupplier = new WsConcordeSuplier.InternalRequest();
            WsConcordeSuplier.AuthHeader authHeader = new WsConcordeSuplier.AuthHeader();
            WsConcordeSuplier.CXALVersion cxalVer = WsConcordeSuplier.CXALVersion.Undefined;

            authHeader.Username = CXAL_USER_NAME;
            authHeader.Password = Des.Encrypt(CXAL_PASSWORD, CXAL_ENCRYPT_PASSWORD);
            wsConcordeSupplier.AuthHeaderValue = authHeader;
            wsConcordeSupplier.Timeout = 1800000;

            ConcordeVersion concordeVersion = ConcordeVersion.Unknown;
            switch (companyId) {
                case 0:
                    cxalVer = WsConcordeSuplier.CXALVersion.CZ;
                    concordeVersion = ConcordeVersion.CZ;
                    break;
                case 1:
                    cxalVer = WsConcordeSuplier.CXALVersion.SK;
                    concordeVersion = ConcordeVersion.SK;
                    break;
                case 2:
                    cxalVer = WsConcordeSuplier.CXALVersion.ESC;
                    concordeVersion = ConcordeVersion.ESC;
                    break;

            }

            DataSet concordeSuppliersData = wsConcordeSupplier.GetActiveCreditors(cxalVer, true);

            //if (concordeVersion == ConcordeVersion.CZ || concordeVersion == ConcordeVersion.ESC) {
            //    CompaileCzEscSuppliers(concordeVersion, concordeSuppliersData, wsConcordeSupplier);
            //}

            SupplierRepository supplierRepository = new SupplierRepository();
            List<int> activeDbSupplierIds = supplierRepository.GetActiveSuppliersIds(companyId);


            Hashtable htActiveSuppliers = new Hashtable();
            foreach (DataRow concordeSuppRow in concordeSuppliersData.Tables[0].Rows) {
                //id
                string accNr = null;
                if (concordeSuppRow[CredTableTable.ACCOUNTNUMBER] != null) {
                    accNr = concordeSuppRow[CredTableTable.ACCOUNTNUMBER].ToString();
                }
                if (accNr == null) {
                    //k++;
                    continue;
                }

                accNr = accNr.Trim();

#if DEBUG
                if (accNr == "28085400") {
                    //k++;
                    int i = 5;
                }
#endif

                //name
                string name = null;
                if (concordeSuppRow[CredTableTable.NAME] != null) {
                    name = TransformConcordeString(concordeSuppRow[CredTableTable.NAME].ToString(), concordeVersion);
                }
                if (name == null || name.Trim().Length == 0) continue;

                int supplierGroupId;
                var supplier = supplierRepository.GetSupplierByCompanyIdSuppIdName(companyId, accNr, name, out supplierGroupId);
                if (supplier == null) {
                    supplier = new Kamsyk.Reget.Model.Supplier();
                    supplier.id = DataNulls.INT_NULL;
                } else {
                    if (!htActiveSuppliers.ContainsKey(supplier.id)) {
                        htActiveSuppliers.Add(supplier.id, null);
                    }
                }

                supplier.supplier_id = RemoveNotAllowedExcelChars(accNr);
                supplier.supplier_group_id = supplierGroupId;
                supplier.active = true;
                supplier.supp_name = RemoveNotAllowedExcelChars(name);

                //dic
                string dic = null;
                if (concordeSuppRow[CredTableTable.VATNUMBER] != null) {
                    dic = concordeSuppRow[CredTableTable.VATNUMBER].ToString();
                }
                supplier.dic = RemoveNotAllowedExcelChars(dic);

                //country
                string country = null;
                if (concordeSuppRow[CredTableTable.COUNTRY] != null) {
                    country = TransformConcordeString(concordeSuppRow[CredTableTable.COUNTRY].ToString(), concordeVersion);
                }
                supplier.country = RemoveNotAllowedExcelChars(country);

                //contact person
                string contactPerson = null;
                if (concordeSuppRow[CredTableTable.ATTENTION] != null) {
                    contactPerson = TransformConcordeString(concordeSuppRow[CredTableTable.ATTENTION].ToString(), concordeVersion);
                }
                supplier.contact_person = RemoveNotAllowedExcelChars(contactPerson);
                
                //phone
                string phone = null;
                if (concordeSuppRow[CredTableTable.PHONE] != null) {
                    phone = concordeSuppRow[CredTableTable.PHONE].ToString();
                }
                supplier.phone = RemoveNotAllowedExcelChars(phone);

                if (IsCzSkConcorde(supplierGroupId)) {
                    //only for CZ SK
                    //mobile phone
                    string mobilePhone = null;
                    if (concordeSuppRow[CredTableTable.OTISMOBIL] != null) {
                        mobilePhone = concordeSuppRow[CredTableTable.OTISMOBIL].ToString();
                    }
                    supplier.mobile_phone = RemoveNotAllowedExcelChars(mobilePhone);
                }

                //fax
                string fax = null;
                if (concordeSuppRow[CredTableTable.FAX] != null) {
                    fax = concordeSuppRow[CredTableTable.FAX].ToString();
                }
                supplier.fax = RemoveNotAllowedExcelChars(fax);

                //mail
                if (IsCzSkConcorde(supplierGroupId)) {
                    string mail = null;
                    if (concordeSuppRow[CredTableTable.OTISEMAIL] != null) {
                        mail = concordeSuppRow[CredTableTable.OTISEMAIL].ToString();
                    }
                    supplier.email = RemoveNotAllowedExcelChars(mail);
                }

                //street_part1
                string streetPart1 = null;
                if (concordeSuppRow[CredTableTable.ADDRESS1] != null) {
                    streetPart1 = TransformConcordeString(concordeSuppRow[CredTableTable.ADDRESS1].ToString(), concordeVersion);
                }
                supplier.street_part1 = RemoveNotAllowedExcelChars(streetPart1);

                //street_part2
                string streetPart2 = null;
                if (concordeSuppRow[CredTableTable.ADDRESS2] != null) {
                    streetPart2 = TransformConcordeString(concordeSuppRow[CredTableTable.ADDRESS2].ToString(), concordeVersion);
                }
                supplier.street_part2 = RemoveNotAllowedExcelChars(streetPart2);

                //city
                string city = null;
                if (concordeSuppRow[CredTableTable.ADDRESS3] != null) {
                    city = TransformConcordeString(concordeSuppRow[CredTableTable.ADDRESS3].ToString(), concordeVersion);
                }
                supplier.city = RemoveNotAllowedExcelChars(city);

                //zip code
                string zip = null;
                if (concordeSuppRow[CredTableTable.ZIP] != null) {
                    zip = concordeSuppRow[CredTableTable.ZIP].ToString();
                }
                supplier.zip = RemoveNotAllowedExcelChars(zip);

                //creditor group
                int creditorGroup = DataNulls.INT_NULL;
                if (concordeSuppRow[CredTableTable.CREDITORGROUP] != null) {
                    creditorGroup = ConvertData.ToInt32(concordeSuppRow[CredTableTable.CREDITORGROUP].ToString().Trim());
                }
                supplier.creditor_group = creditorGroup;

                //bank account
                string bankAccount = null;
                if (concordeSuppRow[CredTableTable.BANKACCOUNT] != null) {
                    bankAccount = concordeSuppRow[CredTableTable.BANKACCOUNT].ToString().Trim();
                }
                supplier.bank_account = RemoveNotAllowedExcelChars(bankAccount);

                List<string> msgItems = null;
                supplierRepository.SaveSupplier(supplier, UserRepository.REGET_SYSTEM_USER, companyId, true, out msgItems);
            }

            

            //Deactive non active suppliers
            foreach (var suppId in activeDbSupplierIds) {
                if (!htActiveSuppliers.ContainsKey(suppId)) {
                    supplierRepository.DeactivateSupplier(suppId);
                }
            }

            supplierRepository.UpdateLastSuppUpdateDateByCompany(companyId);
        }

        private string RemoveNotAllowedExcelChars(string strRawString) {
            if (strRawString == null) {
                return null;
            }

            string fixedValue = strRawString.Replace('\u0006'.ToString(), "");

            return fixedValue;
        }

        private void CompaileCzEscSuppliers(ConcordeVersion concordeVersion, DataSet concordeSuppliersData, WsConcordeSuplier.InternalRequest wsConcordeSupplier) {
            WsConcordeSuplier.CXALVersion cxalVer = WsConcordeSuplier.CXALVersion.Undefined;
            if (concordeVersion == ConcordeVersion.CZ) {
                cxalVer = WsConcordeSuplier.CXALVersion.ESC;
            } else {
                cxalVer = WsConcordeSuplier.CXALVersion.CZ;
            }
            DataSet concordeSupplierDataExt = wsConcordeSupplier.GetActiveCreditors(cxalVer, true);
            foreach (DataRow row in concordeSupplierDataExt.Tables[0].Rows) {
                string accNr = null;
                if (row[CredTableTable.ACCOUNTNUMBER] != null) {
                    accNr = row[CredTableTable.ACCOUNTNUMBER].ToString();
                }

                if (accNr == null) {
                    continue;
                }

                string select = CredTableTable.ACCOUNTNUMBER + "='" + accNr + "'";
                if (concordeSuppliersData.Tables[0].Select(select).Length > 0) {
                    continue;
                }

                DataRow newRow = concordeSuppliersData.Tables[0].NewRow();
                foreach (DataColumn col in concordeSupplierDataExt.Tables[0].Columns) {
                    newRow[col.ColumnName] = row[col.ColumnName];
                }
                concordeSuppliersData.Tables[0].Rows.Add(newRow);
            }

            concordeSuppliersData.AcceptChanges();
        }

        private string TransformConcordeString(string rawString, ConcordeVersion country) {
           

            if (country == ConcordeVersion.CZ || country == ConcordeVersion.ESC || country == ConcordeVersion.SK) {
                return ConvertCzSKString(rawString).Trim();
            }

            return rawString.Trim();
        }

        private string ConvertCzSKString(string czSkString) {
            string unicodeString = czSkString;

            unicodeString = unicodeString.Replace("Ŕ", "Ü");

            return unicodeString;
        }

        private bool IsCzSkConcorde(int supplierGroupId) {
            return (supplierGroupId == 0 || supplierGroupId == 1 || supplierGroupId == 2);
        }

        private string GetDbString(object oValue) {
            if (oValue == null) {
                return null;
            }

            if (oValue == DBNull.Value) {
                return null;
            }

            return oValue.ToString().Trim();
        }

        private void ImportSuppliersFromBaan(int regetCompanyId, int baanCompanyId) {
            WsBaan.Baan wsBaan = new WsBaan.Baan();

            var dsBaanSuppliers = wsBaan.GetSuppliers(baanCompanyId);

            SupplierRepository supplierRepository = new SupplierRepository();
            List<int> activeDbSupplierIds = supplierRepository.GetActiveSuppliersIds(regetCompanyId);

            Hashtable htActiveSuppliers = new Hashtable();

            foreach (DataRow baanRow in dsBaanSuppliers.Tables[0].Rows) {
                //id
                string accNr = null;
                if (baanRow[Ttcom100Data.T_LGID_FIELD] != null) {
                    accNr = GetDbString(baanRow[Ttcom100Data.T_LGID_FIELD]);
                }
                if (accNr == null) {
                    continue;
                }
                accNr = accNr.Trim();

                //name
                string name = null;
                if (baanRow[Ttcom100Data.T_NAMA_FIELD] != null) {
                    name = baanRow[Ttcom100Data.T_NAMA_FIELD].ToString();
                }
                if (name == null || name.Trim().Length == 0) continue;

#if DEBUG
                if (name.Trim().ToUpper() == "BIG BANG D.O.O.") {
                    int h = 5;
                }
#endif

                int supplierGroupId;
                var supplier = supplierRepository.GetSupplierByCompanyIdSuppIdName(regetCompanyId, accNr, name, out supplierGroupId);
                if (supplier == null) {
                    supplier = new Kamsyk.Reget.Model.Supplier();
                    supplier.id = DataNulls.INT_NULL;
                } else {
                    if (!htActiveSuppliers.ContainsKey(supplier.id)) {
                        htActiveSuppliers.Add(supplier.id, null);
                    }
                }

                supplier.supplier_id = accNr;
                supplier.supplier_group_id = supplierGroupId;
                supplier.active = true;
                supplier.supp_name = name;

                //dic
                string dic = null;
                if (baanRow[Ttcom100Data.T_LGID_FIELD] != null) {
                    dic = GetDbString(baanRow[Ttcom100Data.T_LGID_FIELD]);
                }
                supplier.dic = dic;

                //country
                string country = null;
                if (baanRow[Ttccom130Data.T_CCTY_FIELD] != null) {
                    country = GetDbString(baanRow[Ttccom130Data.T_CCTY_FIELD]);
                }
                supplier.country = country;

                //street_part1
                string streetPart1 = null;
                if (baanRow[Ttccom130Data.T_NAMC_FIELD] != null) {
                    streetPart1 = GetDbString(baanRow[Ttccom130Data.T_NAMC_FIELD]);
                }
                supplier.street_part1 = streetPart1;

                //street_part1
                string streetNr = null;
                if (baanRow[Ttccom130Data.T_HONO_FIELD] != null) {
                    streetNr = GetDbString(baanRow[Ttccom130Data.T_HONO_FIELD]);
                    if (String.IsNullOrEmpty(supplier.street_part1)) {
                        supplier.street_part1 = streetNr;
                    } else {
                        supplier.street_part1 += " " + streetNr;
                    }
                }
                
                //city
                string city = null;
                if (baanRow[Ttccom130Data.T_NAME_FIELD] != null) {
                    city = GetDbString(baanRow[Ttccom130Data.T_NAME_FIELD]);
                }
                supplier.city = city;

                //zip code
                string zip = null;
                if (baanRow[Ttccom130Data.T_PSTC_FIELD] != null) {
                    zip = GetDbString(baanRow[Ttccom130Data.T_PSTC_FIELD]);
                }
                supplier.zip = zip;


                List<string> msgItems = null;
                supplierRepository.SaveSupplier(supplier, UserRepository.REGET_SYSTEM_USER, regetCompanyId, true, out msgItems);

                
                
            }

            //Deactive non active suppliers
            foreach (var suppId in activeDbSupplierIds) {
                if (!htActiveSuppliers.ContainsKey(suppId)) {
                    supplierRepository.DeactivateSupplier(suppId);
                }
            }

            supplierRepository.UpdateLastSuppUpdateDateByCompany(regetCompanyId);
        }
        #endregion
    }
}
