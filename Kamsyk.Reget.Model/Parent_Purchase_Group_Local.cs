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
    
    public partial class Parent_Purchase_Group_Local
    {
        public int parent_purchase_group_id { get; set; }
        public string culture { get; set; }
        public string local_text { get; set; }
    
        public virtual Parent_Purchase_Group Parent_Purchase_Group { get; set; }
    }
}
