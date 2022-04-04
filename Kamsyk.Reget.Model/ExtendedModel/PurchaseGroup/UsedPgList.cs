using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel.PurchaseGroup {
    public class UsedPgList {
        public int company_id { get; set; }
        public string company_name { get; set; }
        public ICollection<UsedPg> purchase_groups { get; set; }
        
    }
}
