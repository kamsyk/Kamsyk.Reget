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
    
    public partial class Request_AccessRequest
    {
        public int request_id { get; set; }
        public int request_version { get; set; }
        public int requestor_id { get; set; }
        public System.DateTime request_date { get; set; }
        public int status_id { get; set; }
        public Nullable<System.DateTime> status_chnage_date { get; set; }
    
        public virtual Participants Participants { get; set; }
        public virtual Request_Event Request_Event { get; set; }
    }
}