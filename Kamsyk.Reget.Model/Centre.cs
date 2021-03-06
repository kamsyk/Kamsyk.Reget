//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kamsyk.Reget.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Centre
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Centre()
        {
            this.Centre_BusinessTripCentreGroup = new HashSet<Centre_BusinessTripCentreGroup>();
            this.PurchaseGroup_Requestor = new HashSet<PurchaseGroup_Requestor>();
            this.Request_Event = new HashSet<Request_Event>();
            this.Request_Identificator = new HashSet<Request_Identificator>();
            this.Ship_To_Address = new HashSet<Ship_To_Address>();
            this.Asset_Requestor_Centre = new HashSet<Participants>();
            this.Asset_Centre_Group = new HashSet<Asset_Centre_Group>();
            this.Centre_Group = new HashSet<Centre_Group>();
            this.Requestor_Centre = new HashSet<Participants>();
            this.Participants_Centre = new HashSet<Participants>();
            this.Maintenance_Event = new HashSet<Maintenance_Event>();
            this.Asset_Request_Event = new HashSet<Asset_Request_Event>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
        public string remark { get; set; }
        public Nullable<int> export_price_to_order { get; set; }
        public Nullable<bool> multi_orderer { get; set; }
        public Nullable<bool> other_orderer_can_takeover { get; set; }
        public Nullable<bool> is_approved_by_requestor { get; set; }
        public Nullable<int> manager_id { get; set; }
        public bool active { get; set; }
        public int company_id { get; set; }
        public Nullable<int> address_id { get; set; }
        public int modify_user { get; set; }
        public System.DateTime modify_date { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Centre_BusinessTripCentreGroup> Centre_BusinessTripCentreGroup { get; set; }
        public virtual Participants Manager { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseGroup_Requestor> PurchaseGroup_Requestor { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request_Event> Request_Event { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request_Identificator> Request_Identificator { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ship_To_Address> Ship_To_Address { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Participants> Asset_Requestor_Centre { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Asset_Centre_Group> Asset_Centre_Group { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Centre_Group> Centre_Group { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Participants> Requestor_Centre { get; set; }
        public virtual Address Address { get; set; }
        public virtual Company Company { get; set; }
        public virtual Participants Participants_Modify { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Participants> Participants_Centre { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Maintenance_Event> Maintenance_Event { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Asset_Request_Event> Asset_Request_Event { get; set; }
    }
}
