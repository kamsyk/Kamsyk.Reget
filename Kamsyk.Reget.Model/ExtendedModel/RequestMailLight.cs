using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel {
    public class RequestMailLight {
        public int request_id { get; set; }
        public string recipients { get; set; }
        public string sender { get; set; }
    }
}
