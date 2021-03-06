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
    
    public partial class Asset_Centre_Group
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Asset_Centre_Group()
        {
            this.Asset_Manager_Role = new HashSet<Asset_Manager_Role>();
            this.ParticipantRole_CentreAssetGroup = new HashSet<ParticipantRole_CentreAssetGroup>();
            this.Centre = new HashSet<Centre>();
            this.Asset_Request_Event = new HashSet<Asset_Request_Event>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
        public int currency_id { get; set; }
        public bool active { get; set; }
    
        public virtual Currency Currency { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Asset_Manager_Role> Asset_Manager_Role { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ParticipantRole_CentreAssetGroup> ParticipantRole_CentreAssetGroup { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Centre> Centre { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Asset_Request_Event> Asset_Request_Event { get; set; }
    }
}
