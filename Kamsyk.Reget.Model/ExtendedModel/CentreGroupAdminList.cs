using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel {
    public class CentreGroupAdminList {
        public int id { get; set; }
        public string name { get; set; }
        public string flag_url { get; set; }
        public string status_url { get; set; }
        public bool is_active { get; set; }
    }
}
