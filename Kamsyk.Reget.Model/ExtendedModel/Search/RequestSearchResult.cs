using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel.Search {
    public class RequestSearchResult {
        public int request_id { get; set; }
        public string request_nr { get; set; }
        public string found_text { get; set; }
        public string found_text_short { get; set; }
    }
}
