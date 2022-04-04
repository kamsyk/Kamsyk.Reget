using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel.Statistics {
    public class StatisticsCompany {
        public int id { get; set; }
        public string country_code { get; set; }
        public bool is_user_company_stat_admin { get; set; }
        public bool is_user_company_stat_requestor { get; set; }
        public bool is_user_company_stat_orderer { get; set; }
        public bool is_user_company_stat_appman { get; set; }
        public bool is_user_company_stat_cgadmin { get; set; }
    }
}
