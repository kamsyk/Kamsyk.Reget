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
    
    public partial class RequestEvent_CustomFieldValue
    {
        public int request_event_id { get; set; }
        public int request_event_version { get; set; }
        public int custom_field_id { get; set; }
        public string string_value { get; set; }
        public bool is_active { get; set; }
    
        public virtual Custom_Field Custom_Field { get; set; }
        public virtual Request_Event Request_Event { get; set; }
    }
}
