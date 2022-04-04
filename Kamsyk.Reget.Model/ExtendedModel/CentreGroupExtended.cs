using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel {
    public class CentreGroupExtended {
        public int id { get; set; }
        public int currency_id { get; set; }
        public string name { get; set; }
        public bool? is_active { get; set; }
        public bool? allow_free_supplier { get; set; }
        public ICollection<CurrencyExtended> currency { get; set; }
        public ICollection<OrdererExtended> deputy_orderer { get; set; }
        public ICollection<CgAdminsExtended> cg_admin { get; set; }
        public string CgAdminsText { get; set; }
        public string DeputyOrdererRO { get; set; }
        public int supplier_group_id { get; set; }
        public string company_name { get; set; }
        public int company_id { get; set; }
        
        public ICollection<Centre> centre { get; set; }
        public ICollection<Purchase_Group> purchase_group { get; set; }
        public ICollection<ParticipantRole_CentreGroup> participantrole_centregroup { get; set; }
        public ICollection<OrdererSupplierAppMatrix> orderer_supplier_appmatrix { get; set; }

        #region Constructor
        public CentreGroupExtended() : base() {
            this.currency = new HashSet<CurrencyExtended>();
            this.deputy_orderer = new HashSet<OrdererExtended>();
            this.CgAdminsText = null;
            this.cg_admin = new HashSet<CgAdminsExtended>();
            this.DeputyOrdererRO = null;
            this.orderer_supplier_appmatrix = new HashSet<OrdererSupplierAppMatrix>();
            this.centre = new HashSet<Centre>();
            this.purchase_group = new HashSet<Purchase_Group>();
        }
        #endregion
    }
}
