using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel {
    public class ParticipantsExtended {
        public int id { get; set; }
        public int company_id { get; set; }
        public string first_name { get; set; }
        public string user_name { get; set; }
        public string email { get; set; }
        public string surname { get; set; }
        public int office_id { get; set; }
        public string office_name { get; set; }
        public string phone { get; set; }
        public bool is_external { get; set; }
        public string user_search_key { get; set; }
        public string photo240_url { get; set; }
        public bool active { get; set; }

        public string name_surname_first { get; set; }
        public string country_flag { get; set; }
        public int row_index { get; set; }
        public string substituted_by { get; set; }
        public string substituted_until { get; set; }

        #region Constructor

        #endregion
    }
}
