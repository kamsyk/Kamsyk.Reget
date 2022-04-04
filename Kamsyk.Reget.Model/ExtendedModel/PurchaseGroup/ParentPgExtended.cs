using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel.PurchaseGroup {
    public class ParentPgExtended {
        public int id { get; set; }
        public string name { get; set; }
        public string name_wo_diacritics { get; set; }
        public List<int> selected_companies { get; set; }
    }
}
