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
    
    public partial class Unit_Measurement_Local
    {
        public int unit_id { get; set; }
        public string culture { get; set; }
        public string local_name { get; set; }
    
        public virtual Unit_Measurement Unit_Measurement { get; set; }
    }
}