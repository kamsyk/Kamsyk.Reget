using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel.Chart {
    public class ChartData {
        public ICollection<string> labels { get; set; }
        public ICollection<ChartDataSet> datasets { get; set; }
        
        
    }
}
