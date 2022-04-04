using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel {
    public class AgDropDown {
        public string value { get; set; }
        //public string id { get; set; }
        public string label { get; set; }

        public AgDropDown() { }
        public AgDropDown(string value, string label) {
            this.value = value;
            this.label = label;
        }
    }
}
