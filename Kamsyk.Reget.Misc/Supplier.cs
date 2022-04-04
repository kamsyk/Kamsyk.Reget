using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kamsyk.Reget.Model.Repositories;
using System.Collections;

namespace Kamsyk.Reget.Misc {
    public class Supplier {
        public void RemoveDuplicities() {
            var suppliers = new SupplierRepository().GetSupplierByCompany(1);
            string currKey = null;
            Hashtable htDuplicities = new Hashtable();

            foreach (var supplier in suppliers) {
                string supKey = GetSuppKey(supplier.supp_name, supplier.supplier_id);
                if (currKey == null) {
                    currKey = supKey;
                    continue;
                }

                if (currKey == supKey) {
                    
                    if (htDuplicities.ContainsKey(supKey)) {
                    } else {
                        htDuplicities.Add(supKey, null);
                    }

                    Console.WriteLine(currKey);
                }

                currKey = supKey;
            }
        }

        private string GetSuppKey(string suppName, string suppId) {
            if (suppId == null) {
                suppId = "";
            }
            return (suppName.Trim().ToLower() + "|" + suppId.Trim().ToLower());
        }

        public void SetSearchKey() {
            SupplierRepository supplierRepository = new SupplierRepository();
            List<Model.Supplier> suppliers = supplierRepository.GetAllSuppliers();

            foreach (var supplier in suppliers) {
                if (!String.IsNullOrEmpty(supplier.supplier_search_key)) {
                    continue;
                }
                int companyId = supplier.SupplierGroup.Company.ElementAt(0).id;
                
                supplierRepository.SaveSupplierSearchKey(supplier);
                Console.WriteLine(supplier.supp_name);
            }
        }
    }
}
