using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel.Order {
    public class OrderExtend {
        public int id { get; set; }
        public int request_id { get; set; }
        public int request_version { get; set; }
        public string request_nr { get; set; }
        public string otis_company_address { get; set; }
        public string requestor_address { get; set; }
        public string orderer_name { get; set; }
        public string orderer_mail { get; set; }
        public string orderer_phone { get; set; }
        public string orderer_fax { get; set; }
        public string requestor_name { get; set; }
        public string requestor_mail { get; set; }
        public string requestor_phone { get; set; }
        public string supplier_address { get; set; }
        public string request_text { get; set; }
        public Nullable<decimal> est_price { get; set; }
        public Nullable<int> currency_id { get; set; }
        public Nullable<System.DateTime> deliv_date { get; set; }
        public Nullable<bool> is_price_exported { get; set; }
        public List<Checkbox> mail_addresses { get; set; }
        public string attachment_ids { get; set; }
        public string culture_name { get; set; }
        public System.DateTime modify_date { get; set; }
        public string supplier_contact_name { get; set; }
        public string supplier_mail { get; set; }
        public string supplier_phone { get; set; }
        public int? supplier_id { get; set; }
        public string contract_nr { get; set; }
        public int requestor_id { get; set; }
    }
}
