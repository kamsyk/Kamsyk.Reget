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
    
    public partial class Asset_Manager_Role
    {
        public int asset_centre_group_id { get; set; }
        public int asset_request_type_id { get; set; }
        public int app_level_id { get; set; }
        public int manager_id { get; set; }
        public bool active { get; set; }
    
        public virtual Asset_Centre_Group Asset_Centre_Group { get; set; }
        public virtual Participants Participants { get; set; }
        public virtual Asset_Request_Type Asset_Request_Type { get; set; }
    }
}