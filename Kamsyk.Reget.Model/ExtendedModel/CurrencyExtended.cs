using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel {
    public class CurrencyExtended : Kamsyk.Reget.Model.Currency {
        public bool is_set { get; set; }
        public string currency_code_name { get; set; }

    }
}
