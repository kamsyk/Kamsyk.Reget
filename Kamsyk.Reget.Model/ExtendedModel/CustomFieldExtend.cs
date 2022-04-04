using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel {
    public class CustomFieldExtend {
        public int id { get; set; }
        public string label { get; set; }
        public int data_type_id { get; set; }
        public bool is_mandatory { get; set; }
        public string string_value { get; set; }
    }
}
