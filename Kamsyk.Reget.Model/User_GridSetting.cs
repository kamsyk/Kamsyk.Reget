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
    
    public partial class User_GridSetting
    {
        public int user_id { get; set; }
        public string grid_name { get; set; }
        public int grid_page_size { get; set; }
        public string filter { get; set; }
        public string sort { get; set; }
        public string columns { get; set; }
    
        public virtual Participants Participants { get; set; }
    }
}
