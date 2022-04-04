using System.Collections.Generic;

namespace Kamsyk.Reget.Model.Repositories {
    internal class LightCg {
        public bool? is_active { get; set; }
        public bool? allow_free_supplier { get; set; }
        public int company_id { get; set; }
        public int currency_id { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public ICollection<Orderer_Supplier> orderer_supplier { get; set; }
        public SupplierGroup supplier_group { get; set; }
    }
}