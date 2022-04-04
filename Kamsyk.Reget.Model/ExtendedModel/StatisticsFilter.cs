using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel {
    public class StatisticsFilter {
        public int id { get; set; }
        public string name { get; set; }
        public string name_search_key { get; set; }
        public int company_id { get; set; }
        public bool is_selected { get; set; }
        public string flag_url { get; set; }

       
    }
}
