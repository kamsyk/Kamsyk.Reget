using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel {
    public class StatisticsRequest {
        public int id { get; set; }
        public int company_id { get; set; }
        public int? request_status { get; set; }
        public DateTime? issued { get; set; }
        public decimal? price { get; set; }
        public decimal? estimated_price { get; set; }

        public int? requestor { get; set; }
        public int? orderer_id { get; set; }
        public int? request_centre_id { get; set; }
        public int? purchase_group_id { get; set; }
        public int? supplier_id { get; set; }
        public int? centre_group_id { get; set; }
        public int? currency_id { get; set; }
        //public int? app_man_id { get; set; }
    }
}
