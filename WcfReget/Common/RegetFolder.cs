using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WcfReget.Common {
    public class RegetFolder {
        #region Suppliers
        public static string GetSuppTmpFileName(int supplierGroupId) {
            int iIndex = 1;
            string strureFileName = "RegetSuppTmp_" + supplierGroupId + ".xlsx";
            string workVendorFile = Path.Combine(RegetFolders.TmpSupplierImportPath, strureFileName);
            while (File.Exists(workVendorFile)) {
                strureFileName = "RegetSuppTmp_" + supplierGroupId + "_" + iIndex + ".xlsx";
                workVendorFile = Path.Combine(RegetFolders.TmpSupplierImportPath, strureFileName);
                iIndex++;
            }

            return workVendorFile;
        }

        public static void DeleteTmpSuppFiles() {
            DeleteTmpFiles(RegetFolders.TmpSupplierImportPath);
        }

        public static void DeleteTmpFiles(string strFolder) {
            string[] files = Directory.GetFiles(strFolder);
            foreach (string file in files) {
                try {
                    FileInfo fi = new FileInfo(file);
                    if (fi.CreationTimeUtc.AddDays(7) < DateTime.Now) {
                        File.Delete(file);
                    }
                } catch { }
            }
        }
        #endregion
    }
}