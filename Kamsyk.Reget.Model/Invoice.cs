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
    
    public partial class Invoice
    {
        public int id { get; set; }
        public int request_id { get; set; }
        public int request_version { get; set; }
        public string invoice_nr { get; set; }
        public Nullable<decimal> amount { get; set; }
        public Nullable<System.DateTime> invoice_date { get; set; }
    
        public virtual Request_Event Request_Event { get; set; }
    }
}
