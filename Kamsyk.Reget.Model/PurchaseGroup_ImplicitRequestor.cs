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
    
    public partial class PurchaseGroup_ImplicitRequestor
    {
        public int purchase_category_id { get; set; }
        public int requestor_id { get; set; }
        public Nullable<bool> requestor_allowed_for_other_group { get; set; }
    
        public virtual Participants Participants { get; set; }
        public virtual Purchase_Group Purchase_Group { get; set; }
    }
}