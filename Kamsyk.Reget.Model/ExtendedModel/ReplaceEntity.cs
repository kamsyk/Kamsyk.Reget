using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel {
    public class ReplaceEntity {
        public ICollection<CentreGroupReplace> cg_app_men { get; set; }
        public ICollection<CentreGroupReplace> cg_requestors { get; set; }
        public ICollection<CentreGroupReplace> cg_orderers { get; set; }
        public ICollection<CentreGroupReplace> centre_man { get; set; }
    }
}
