using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel {
    public class CentreAdminExtended : Centre {

        public string company_name { get; set; }
        public string manager_name { get; set; }
        public string export_price_text { get; set; }
        public string address_text { get; set; }
        public int row_index { get; set; }

        #region Constructor
        public CentreAdminExtended() : base() {
            
        }
        #endregion
    }
}
