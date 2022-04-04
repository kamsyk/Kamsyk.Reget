using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel {
    public class CgAdminsExtended {
        public int id { get; set; }
        public string surname { get; set; }
        public string first_name { get; set; }
        public bool is_company_admin { get; set; }
        public bool is_cg_prop_admin { get; set; }
        public bool is_requestor_admin { get; set; }
        public bool is_orderer_admin { get; set; }
        public bool is_appmatrix_admin { get; set; }

        #region Constructor

        #endregion
    }
}
