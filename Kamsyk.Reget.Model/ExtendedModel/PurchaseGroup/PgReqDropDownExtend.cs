using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kamsyk.Reget.Model.Repositories.PgRepository;

namespace Kamsyk.Reget.Model.ExtendedModel.PurchaseGroup {
    public class PgReqDropDownExtend {
        public int id { get; set; }
        public string name { get; set; }
        public int pg_type = (int)PurchaseGroupType.Standard;
    }
}
