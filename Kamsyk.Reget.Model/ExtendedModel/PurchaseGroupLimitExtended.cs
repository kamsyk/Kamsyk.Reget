using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel {
    public class PurchaseGroupLimitExtended {
        public int limit_id { get; set; }
        public Nullable<decimal> limit_bottom { get; set; }
        public Nullable<decimal> limit_top { get; set; }
        public string limit_bottom_text { get; set; }
        public string limit_top_text { get; set; }
        public string limit_bottom_text_ro { get; set; }
        public string limit_top_text_ro { get; set; }
        public string limit_bottom_loc_curr_text_ro { get; set; }
        public string limit_top_loc_curr_text_ro { get; set; }

        public bool is_bottom_unlimited { get; set; }
        public bool is_top_unlimited { get; set; }
        public bool is_limit_bottom_multipl { get; set; }
        public bool is_limit_top_multipl { get; set; }
        public bool is_first { get; set; }
        public bool is_last { get; set; }
        public int app_level_id { get; set; }
        public bool is_visible { get; set; }
        public bool is_app_man_selected { get; set; }
        
        public ICollection<ManagerRoleExtended> manager_role { get; set; }

        #region Constructor
        public PurchaseGroupLimitExtended() {
            this.manager_role = new HashSet<ManagerRoleExtended>();
        }
        #endregion
    }
}
