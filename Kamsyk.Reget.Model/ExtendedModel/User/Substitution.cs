using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel.User {
    public class Substitution {
        public PartData<UserSubstitutionExtended> user_substitution { get; set; }
        public bool is_approve_visible { get; set; }
        public bool is_edit_visible { get; set; }
    }
}
