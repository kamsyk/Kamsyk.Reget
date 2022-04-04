using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel {
    public class OrdererExtended : RequestorOrdererExtended {
        public string substituted_by { get; set; }
        public string substituted_until { get; set; }
    }
}
