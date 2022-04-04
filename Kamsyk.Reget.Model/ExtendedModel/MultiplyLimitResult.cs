using Kamsyk.Reget.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel {
    public class MultiplyLimitResult {
        public int affected_limits_count = 0;
        public List<MultiplyLimitMsg> err_msg = null;
    }
}
