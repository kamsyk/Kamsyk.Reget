using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel.PurchaseGroup {
    public class UsedPg {
        public int id { get; set; }
        public int parent_pg_id { get; set; }
        public string parent_pg_name { get; set; }
        public string parent_pg_loc_name { get; set; }
        public string pg_name { get; set; }
        public string pg_loc_name { get; set; }
        public int centre_group_id { get; set; }
        public string centre_group_name { get; set; }
        public int company_id { get; set; }
        public string company_name { get; set; }
        //public string centre_group_loc_name { get; set; }
        public bool active { get; set; }
        public int row_index { get; set; }

        public List<LocalText> local_text { get; set; }
        
    }
}
