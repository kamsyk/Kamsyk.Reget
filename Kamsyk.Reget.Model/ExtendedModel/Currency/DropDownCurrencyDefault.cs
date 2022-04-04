using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel.Currency {
    public class DropDownCurrencyDefault {
        public int default_currency_id { get; set; }
        public List<DropDownItem> currency_drop_down { get; set; }
    }
}
