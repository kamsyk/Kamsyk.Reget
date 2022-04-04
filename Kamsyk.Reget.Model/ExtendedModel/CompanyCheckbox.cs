using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel {
    public class CompanyCheckbox {
        public int company_id { get; set; }
        public string country_code { get; set; }
        public bool is_selected { get; set; }
    }
}
