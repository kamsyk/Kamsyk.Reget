using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel.Chart {
    public class ChartOptions {
        public ChartScales scales { get; set; }
        public ChartTitle title { get; set; }
        public bool maintainAspectRatio { get; set; }
    }
}
