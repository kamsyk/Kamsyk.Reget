using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel {
    public class AddressAdminExtended : Kamsyk.Reget.Model.Address {
        public int row_index { get; set; }
        public string company_name_text { get; set; }

        #region Constructor
        public AddressAdminExtended() : base() {
            
        }
        #endregion
    }
}
