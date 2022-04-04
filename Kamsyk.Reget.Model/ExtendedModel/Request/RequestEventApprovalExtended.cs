using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel.Request {
    public class RequestEventApprovalExtended : Request_Event_Approval {
        public string app_man_surname { get; set; }
        public string app_man_first_name { get; set; }
        public string modif_date_text { get; set; }
    }
}
