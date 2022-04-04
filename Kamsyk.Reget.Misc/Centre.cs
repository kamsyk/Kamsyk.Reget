using Kamsyk.Reget.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Misc {
    public class Centre {
        public void SetCentreCompany() {
            new CentreRepository().SetCentreCompany();
        }
    }
}
