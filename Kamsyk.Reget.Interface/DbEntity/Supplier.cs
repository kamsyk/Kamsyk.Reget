using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Interface.DbEntity {
    public class Supplier {
        public int id { get; set; }
        public string supp_name { get; set; }
        public int supplier_group_id { get; set; }
        public string supplier_local_app_id { get; set; }
        public string supplier_id { get; set; }
        public string dic { get; set; }
        public string country { get; set; }
        public Nullable<bool> vat { get; set; }
        public string contact_person { get; set; }
        public string phone { get; set; }
        public string mobile_phone { get; set; }
        public string fax { get; set; }
        public string street_part1 { get; set; }
        public string street_part2 { get; set; }
        public string city { get; set; }
        public string zip { get; set; }
        public string email { get; set; }
        public Nullable<int> creditor_group { get; set; }
        public string bank_account { get; set; }
        public string lang { get; set; }
        public string email_used { get; set; }
        public Nullable<bool> active { get; set; }
    }
}
