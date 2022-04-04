using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel {
    public class CentreGroupReplace {
        public int cg_id { get; set; }
        public string name { get; set; }
        public bool is_selected { get; set; }
        public int orig_user_id { get; set; }
        public int replace_user_id { get; set; }
        public string centres { get; set; }
    }
}
