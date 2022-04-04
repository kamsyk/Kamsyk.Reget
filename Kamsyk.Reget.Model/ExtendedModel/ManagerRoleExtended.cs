using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel {
    public class ManagerRoleExtended {
        public int approve_level_id { get; set; }
        public int participant_id { get; set; }
        //public int approve_status { get; set; }
        public virtual ParticipantsExtended participant { get; set; }
        public virtual ParticipantsExtended substituted { get; set; }

        #region Constructor
        public ManagerRoleExtended() : base() {
            
        }
        #endregion
    }
}
