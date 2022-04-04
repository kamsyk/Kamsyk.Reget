using Kamsyk.Reget.Model.ExtendedModel.Discussion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel {
    public class UserSubstitutionExtended {
        public int id { get; set; }
        public int substituted_user_id { get; set; }
        public int substitute_user_id { get; set; }
        public string substituted_name_surname_first { get; set; }
        public string substitutee_name_surname_first { get; set; }
        public DateTime substitute_start_date { get; set; }
        public DateTime substitute_end_date { get; set; }
        public string substitute_start_date_text { get; set; }
        public string substitute_end_date_text { get; set; }
        public Nullable<bool> is_allow_take_over { get; set; }
        public string remark { get; set; }
        public int approval_status { get; set; }
        public string approval_status_text { get; set; }
        public string active_status_text { get; set; }
        public string approval_men { get; set; }
        public string companies_ids { get; set; }
        public int row_index { get; set; }
        public bool is_editable_author { get; set; }
        public bool is_editable_app_man { get; set; }
        public bool is_can_be_deleted { get; set; }
        public bool is_date_time_ro1 { get; set; }
        public bool is_date_time_ro2 { get; set; }
        public bool is_cmb_ro { get; set; }
        public bool is_edit_hidden { get; set; }
        public bool is_delete_hidden { get; set; }
        public bool is_approve_hidden { get; set; }
        public bool is_reject_hidden { get; set; }
        public int author_id { get; set; }
        public int? app_man_id { get; set; }
        public int modified_user { get; set; }
        public string modified_user_name { get; set; }
        public DateTime modified_date { get; set; }
        public string modified_date_text { get; set; }
        public bool active { get; set; }
        //public List<DiscussionExtended> discussion { get; set; }
        
        #region Constructor

        #endregion
    }
}
