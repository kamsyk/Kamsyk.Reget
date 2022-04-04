using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel.HttpResult {
    public class HttpResult {
        public const int NOT_AUTHORIZED_ERROR = 110;

        public int int_value { get; set; }
        public string string_value { get; set; }
        public int error_id { get; set; }
        //public List<string> string_list_value { get; set; }
    }
}
