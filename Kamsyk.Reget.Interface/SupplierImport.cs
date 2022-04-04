using Kamsyk.Reget.Model;
using Kamsyk.Reget.Model.Common;
using Kamsyk.Reget.Model.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Interface {
    public class SupplierImport {
        #region Methods
        public void ImportSuppliers(int supplierGroupId, List<Kamsyk.Reget.Interface.DbEntity.Supplier> suppliers) {
            if (suppliers == null) {
                return;
            }

//#if DEBUG
//            supplierGroupId = 0;
//#endif

            Hashtable htSuppIds = new Hashtable();
            SupplierRepository supplierRepository = new SupplierRepository();
            foreach (var supplier in suppliers) {
                Kamsyk.Reget.Model.Supplier dbSupp = null;

                if (!String.IsNullOrEmpty(supplier.supplier_id)) {
                    dbSupp = supplierRepository.GetSupplierDataBySuppId(supplierGroupId, supplier.supplier_id);
                } else if (!String.IsNullOrEmpty(supplier.supplier_local_app_id)) {
                    dbSupp = supplierRepository.GetSupplierDataByLocalAppId(supplierGroupId, supplier.supplier_local_app_id);
                } else {
                    dbSupp = supplierRepository.GetSupplierDataByName(supplierGroupId, supplier.supp_name);
                }

                if (dbSupp == null) {
                    dbSupp = new Supplier();
                    dbSupp.id = DataNulls.INT_NULL;
                }

                dbSupp.supplier_id = supplier.supplier_id;
                dbSupp.dic = supplier.dic;
                dbSupp.supplier_local_app_id = supplier.supplier_local_app_id;
                dbSupp.supp_name = supplier.supp_name;
                dbSupp.street_part1 = supplier.street_part1;
                dbSupp.city = supplier.city;
                dbSupp.zip = supplier.zip;
                dbSupp.country = supplier.country;
                dbSupp.supplier_group_id = supplier.supplier_group_id;
                dbSupp.active = supplier.active;

                int id = supplierRepository.UpdateSupplier(dbSupp);
                if (!htSuppIds.ContainsKey(id)) {
                    htSuppIds.Add(id, null);
                }
            }

            supplierRepository.DeactiveSuppliers(supplierGroupId, htSuppIds);

            supplierRepository.UpdateLastSuppUpdateDateBySuppGroup(supplierGroupId);
        }
        #endregion
    }
}
