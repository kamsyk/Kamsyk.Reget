using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WcfReget.Common {
    #region Common
    public class ConfigCommon {
        public static string GetKeyValue(string keyName) {
            try {
                return ConfigurationManager.AppSettings[keyName].ToString();
            } catch (Exception ex) {
                throw ex;
            }
        }
    }
    #endregion

    #region Users
    public class ConfigUser {
        
        public static string SupplierExcelUserName {
            get {
                string keyName = "SupplierExcelUserName";
                try {
                    string keyValue = ConfigCommon.GetKeyValue(keyName);
                    return keyValue;
                } catch (Exception ex) {
                    throw ex;
                }
            }
        }

        public static string SupplierExcelUserPwd {
            get {
                string keyName = "SupplierExcelUserPwd";
                try {
                    string keyValue = ConfigCommon.GetKeyValue(keyName);
                    return keyValue;
                } catch (Exception ex) {
                    throw ex;
                }
            }
        }

        public static string SupplierExcelUserDomain {
            get {
                string keyName = "SupplierExcelUserDomain";
                try {
                    string keyValue = ConfigCommon.GetKeyValue(keyName);
                    return keyValue;
                } catch (Exception ex) {
                    throw ex;
                }
            }
        }



    }

    #endregion

    #region Folders
    public class RegetFolders {
        
        public static string TmpSupplierImportPath {
            get {
                string keyName = "TmpSupplierImportPath";
                try {
                    string wordFolder = ConfigCommon.GetKeyValue(keyName);
                    return wordFolder;
                } catch (Exception ex) {
                    throw ex;
                }
            }
        }

        public static string SuppSlovenianMasterFile {
            get {
                string keyName = "SuppSlovenianMasterFile";
                try {
                    string wordFolder = ConfigCommon.GetKeyValue(keyName);
                    return wordFolder;
                } catch (Exception ex) {
                    throw ex;
                }
            }
        }

        public static string SuppSerbianMasterFile {
            get {
                string keyName = "SuppSerbianMasterFile";
                try {
                    string wordFolder = ConfigCommon.GetKeyValue(keyName);
                    return wordFolder;
                } catch (Exception ex) {
                    throw ex;
                }
            }
        }


    }


    #endregion
}