using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel {
    public class CentreReqDropDownItem {
        public int id { get; set; }
        public string name { get; set; }
        public bool is_free_supplier_allowed { get; set; }

       
        public CentreReqDropDownItem(int itemId, string itemName, bool isFreeSupplierAllowed) {
            id = itemId;
            name = itemName;
            is_free_supplier_allowed = isFreeSupplierAllowed;
        }
    }
}
