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
    
    public partial class Attachement
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Attachement()
        {
            this.Event_Attachement = new HashSet<Event_Attachement>();
        }
    
        public int id { get; set; }
        public string file_name { get; set; }
        public byte[] file_content { get; set; }
        public Nullable<bool> orderer_attachment { get; set; }
        public System.DateTime modify_date { get; set; }
        public int modify_user { get; set; }
        public Nullable<int> att_type { get; set; }
        public Nullable<decimal> size_kb { get; set; }
    
        public virtual Participants Participants { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Event_Attachement> Event_Attachement { get; set; }
    }
}