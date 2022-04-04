using Kamsyk.Reget.Model.ExtendedModel.Supplier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel {
    public class PurchaseGroupExtended {
        public int id { get; set; }
        public int centre_group_id { get; set; }
        public string group_name { get; set; }
        public string group_loc_name { get; set; }
        public int parent_pg_id { get; set; }
        public string parent_pg_loc_name { get; set; }
        public bool self_ordered { get; set; }
        public bool is_visible { get; set; }
        public bool is_active { get; set; }
        public bool is_coppied { get; set; }
        public bool is_coppied_default { get; set; }
        public bool is_disabled { get; set; }
        public bool is_html_show { get; set; }
        public bool is_approval_needed { get; set; }
        public bool is_order_needed { get; set; }
        public bool is_multi_orderer { get; set; }
        //public bool is_approval_only { get; set; }
        public string decimal_separator { get; set; }
        public SupplierSimpleExtended default_supplier { get; set; }
        public ICollection<OrdererExtended> orderer { get; set; }
        public ICollection<RequestorExtended> requestor { get; set; }
        public ICollection<Centre> centre { get; set; }
        public ICollection<AllRequestorOrdererExtended> delete_requestors_all_categories { get; set; }
        public List<int> delete_orderers_all_categories { get; set; }
        public List<LocalText> local_text { get; set; }
        public ICollection<PurchaseGroupLimitExtended> purchase_group_limit { get; set; }
        public ICollection<CustomFieldExtend> custom_field { get; set; }
        

        #region Constructor
        public PurchaseGroupExtended() : base() {
            this.parent_pg_id = -1;
            this.purchase_group_limit = new HashSet<PurchaseGroupLimitExtended>();
            this.orderer = new HashSet<OrdererExtended>();
            this.requestor = new HashSet<RequestorExtended>();
            this.centre = new HashSet<Centre>();
            this.delete_requestors_all_categories = new HashSet<AllRequestorOrdererExtended>();
            this.delete_orderers_all_categories = new List<int>();
            this.local_text = new List<LocalText>();
        }
        #endregion
    }
}
