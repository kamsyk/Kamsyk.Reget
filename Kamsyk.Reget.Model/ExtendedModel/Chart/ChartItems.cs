using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel.Chart {
    public class ChartItems {
        public ChartData ChartData { get; set; }
        public ChartOptions ChartOptions { get; set; }
        public int CurrencyId { get; set; }
        public string ErrMsg { get; set; }
        public List<Exchange_Rate> ExchangeRates = null;
    }
}
