using Kamsyk.Reget.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Misc {
    public class AppMatrix {
        public void PopulatePgLimitAppLevelId() {
            new PgLimitRepository().CheckPgLimitsAppLevel();
        }

        public void ReplacePgsByPgName(string strName) {
            PgRepository pgRepository = new PgRepository();

            //pgRepository.ReplacePg();
        }
    }
}
