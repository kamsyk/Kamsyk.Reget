using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel {
    public class CompanyExtended  {
        public int id { get; set; }
        public string country_code { get; set; }
        public bool is_supplier_admin { get; set; }
        public bool is_company_admin { get; set; }
        //public bool is_company_statistics_admin { get; set; }
        public string supplier_source { get; set; }
        public bool is_supplier_maintenance_allowed { get; set; }
        public bool is_selected { get; set; }

        #region Constructor

        #endregion
    }
}
