using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel {
    public class RequestorOrdererExtended {
        public int participant_id { get; set; }
        public string first_name { get; set; }
        public string surname { get; set; }
        public bool is_implicit { get; set; }
        public bool is_implicit_other_allowed { get; set; }
        public bool is_excluded { get; set; }
        //public bool is_default { get; set; }
        public bool is_all { get; set; }
    }
}
