using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel.HttpResult {
    public class HttpRequestResult : HttpResult {
        public int request_id { get; set; }
        public string request_nr { get; set; }
    }
}
