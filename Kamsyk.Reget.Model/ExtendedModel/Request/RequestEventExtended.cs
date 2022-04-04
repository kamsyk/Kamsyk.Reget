using Kamsyk.Reget.Model.ExtendedModel;
using Kamsyk.Reget.Model.ExtendedModel.Attachment;
using Kamsyk.Reget.Model.ExtendedModel.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.Request.ExtendedModel {
    public class RequestEventExtended  {
        //public string requestor_name_surname_first { get; set; }
        //public int row_index { get; set; }
        //public string hyperlink { get; set; }
        //public string pg_name { get; set; }
        //public string pg_name_en { get; set; }
        //public bool is_approval_needed { get; set; }
        //public string estimated_price_text { get; set; }
        //public string currency_code { get; set; }
        //public string centre_name { get; set; }

        //public ICollection<AttachmentExtend> attachments { get; set; }
        //public ICollection<RequestEventApprovalExtended> request_event_approval { get; set; }
        //public ICollection<OrdererExtended> orderers { get; set; }
        //public ICollection<CustomFieldExtend> custom_fields { get; set; }

        public int id { get; set; }
        public string request_nr { get; set; }
        public Nullable<int> requestor { get; set; }
        public Nullable<int> request_centre_id { get; set; }
        public string requestor_name_surname_first { get; set; }
        public string centre_name { get; set; }
        public Nullable<int> supplier_id { get; set; }
        public Nullable<int> request_status { get; set; }
        public string hyperlink { get; set; }
        public string imgurl { get; set; }
        public string imgstyle { get; set; }
        public Nullable<int> purchase_group_id { get; set; }
        public string pg_name { get; set; }
        public string pg_name_en { get; set; }
        public bool is_approval_needed { get; set; }
        public bool is_order_needed { get; set; }
        public Nullable<decimal> estimated_price { get; set; }
        public string estimated_price_text { get; set; }
        public string request_text { get; set; }
        public string remarks { get; set; }
        public Nullable<int> currency_id { get; set; }
        public string currency_code { get; set; }
        public Nullable<int> orderer_id { get; set; }
        public Nullable<int> privacy_id { get; set; }
        public string privacy_name { get; set; }
        public Nullable<System.DateTime> lead_time { get; set; }
        public Nullable<System.DateTime> issued { get; set; }
        public bool is_requestor_can_edit { get; set; }
        public int row_index { get; set; }
        public bool is_revertable { get; set; }
        public bool is_deletable { get; set; }
        public int ship_to_address_id { get; set; }
        public bool use_supplier_list { get; set; }
        public string supplier_remark { get; set; }
        public bool is_approvable { get; set; }
        public bool is_order_available { get; set; }
        public string request_status_text { get; set; }

        public string ship_to_address_company_name { get; set; }
        public string ship_to_address_street { get; set; }
        public string ship_to_address_city { get; set; }
        public string ship_to_address_zip { get; set; }

        public ICollection<CustomFieldExtend> custom_fields { get; set; }
        public ICollection<AttachmentExtend> attachments { get; set; }
        public ICollection<RequestEventApprovalExtended> request_event_approval { get; set; }
        public ICollection<OrdererExtended> orderers { get; set; }
        public ICollection<WorkflowItem> workflow_items { get; set; }
    }
}
