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
    
    public partial class PurchaseGroup_CustomField
    {
        public int purchase_group_id { get; set; }
        public int custom_field_id { get; set; }
        public int position_index { get; set; }
        public int role_id { get; set; }
    
        public virtual Purchase_Group Purchase_Group { get; set; }
        public virtual Custom_Field Custom_Field { get; set; }
        public virtual Participant_Role Participant_Role { get; set; }
    }
}
