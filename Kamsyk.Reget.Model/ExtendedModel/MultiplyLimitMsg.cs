using Kamsyk.Reget.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel {
    public class MultiplyLimitMsg {
        public int cgId = DataNulls.INT_NULL;
        public string cgName = null;
        public int pgId = DataNulls.INT_NULL;
        public string pgName = null;
        public int reason = DataNulls.INT_NULL;
        public string err_msg = null;
    }
}
