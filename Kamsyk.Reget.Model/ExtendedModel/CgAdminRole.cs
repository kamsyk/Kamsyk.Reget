using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel {
    public class CgAdminRoles {
        public bool is_readonly { get; set; }
        public bool is_cg_property_admin { get; set; }
        public bool is_cg_appmatrix_admin { get; set; }
        public bool is_cg_requestor_admin { get; set; }
        public bool is_cg_orderer_admin { get; set; }
    }
}
