using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel {
    public class RequestCentreGroup {
        public int id { get; set; }

        public ICollection<RequestPurchaseGroup> PurchaseGroup { get; set; }
    }
}
