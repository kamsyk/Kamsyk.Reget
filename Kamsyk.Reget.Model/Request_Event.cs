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
    
    public partial class Request_Event
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Request_Event()
        {
            this.Invoice = new HashSet<Invoice>();
            this.Request_Mail = new HashSet<Request_Mail>();
            this.Request_Order = new HashSet<Request_Order>();
            this.Participants10 = new HashSet<Participants>();
            this.Request_Item = new HashSet<Request_Item>();
            this.RequestEvent_CustomFieldValue = new HashSet<RequestEvent_CustomFieldValue>();
            this.Event_Attachement = new HashSet<Event_Attachement>();
            this.Request_Event_Orderer = new HashSet<Request_Event_Orderer>();
            this.Request_AuthorizedUsers = new HashSet<Request_AuthorizedUsers>();
            this.Request_Text1 = new HashSet<Request_Text>();
            this.Request_Discussion = new HashSet<Request_Discussion>();
            this.Request_AccessRequest = new HashSet<Request_AccessRequest>();
            this.Request_Event_Approval = new HashSet<Request_Event_Approval>();
        }
    
        public int id { get; set; }
        public int version { get; set; }
        public string request_nr { get; set; }
        public Nullable<int> requestor { get; set; }
        public Nullable<int> manager1 { get; set; }
        public Nullable<int> manager2 { get; set; }
        public Nullable<int> manager3 { get; set; }
        public Nullable<int> manager4 { get; set; }
        public Nullable<int> manager5 { get; set; }
        public Nullable<System.DateTime> issued { get; set; }
        public string request_text { get; set; }
        public Nullable<System.DateTime> lead_time { get; set; }
        public Nullable<decimal> estimated_price { get; set; }
        public string supplier { get; set; }
        public string remarks { get; set; }
        public Nullable<bool> purch_mat_included { get; set; }
        public Nullable<int> approved_1 { get; set; }
        public Nullable<int> approved_2 { get; set; }
        public Nullable<int> approved_3 { get; set; }
        public Nullable<int> approved_4 { get; set; }
        public Nullable<int> approved_5 { get; set; }
        public Nullable<bool> ordered { get; set; }
        public Nullable<bool> delivered { get; set; }
        public Nullable<int> request_status { get; set; }
        public Nullable<int> currency_id { get; set; }
        public int centre_group_id { get; set; }
        public Nullable<int> country_id { get; set; }
        public Nullable<int> supplier_id { get; set; }
        public Nullable<bool> use_supplier_list { get; set; }
        public string manager1Remark { get; set; }
        public string manager2Remark { get; set; }
        public string manager3Remark { get; set; }
        public string manager4Remark { get; set; }
        public string manager5Remark { get; set; }
        public string orderer_remark { get; set; }
        public Nullable<int> orderer_id { get; set; }
        public string send_back_reason { get; set; }
        public Nullable<int> purchase_group_id { get; set; }
        public Nullable<decimal> actual_price { get; set; }
        public Nullable<System.DateTime> actula_delivery_date { get; set; }
        public string invoice_number { get; set; }
        public Nullable<int> act_currency_id { get; set; }
        public Nullable<decimal> exchange_rate { get; set; }
        public string ship_to_address_company_name { get; set; }
        public string ship_to_address_street { get; set; }
        public string ship_to_address_city { get; set; }
        public string ship_to_address_zip { get; set; }
        public string order_file_name { get; set; }
        public Nullable<int> request_centre_id { get; set; }
        public Nullable<bool> export_price_to_order { get; set; }
        public string profylax_nr { get; set; }
        public Nullable<bool> year_order { get; set; }
        public string contract_nr { get; set; }
        public string unit_nr { get; set; }
        public string job_nr { get; set; }
        public Nullable<int> commentator_id { get; set; }
        public string comment { get; set; }
        public Nullable<bool> last_event { get; set; }
        public Nullable<bool> temporary_request { get; set; }
        public int modify_user { get; set; }
        public System.DateTime modify_date { get; set; }
        public string integration_code { get; set; }
        public Nullable<System.DateTime> not_ordered_notify_date { get; set; }
        public Nullable<bool> is_manager1_inspector { get; set; }
        public Nullable<bool> is_manager2_inspector { get; set; }
        public Nullable<bool> is_manager3_inspector { get; set; }
        public Nullable<bool> is_manager4_inspector { get; set; }
        public Nullable<bool> is_manager5_inspector { get; set; }
        public Nullable<int> manager6 { get; set; }
        public Nullable<int> approved_6 { get; set; }
        public string manager6Remark { get; set; }
        public Nullable<bool> is_manager6_inspector { get; set; }
        public string car_nr { get; set; }
        public Nullable<bool> is_out_of_eu { get; set; }
        public Nullable<int> privacy_id { get; set; }
        public Nullable<bool> is_order_needed { get; set; }
        public Nullable<int> ship_to_address_id { get; set; }
    
        public virtual Centre Centre { get; set; }
        public virtual Centre_Group Centre_Group { get; set; }
        public virtual Company Company { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Invoice> Invoice { get; set; }
        public virtual Participants Requestor { get; set; }
        public virtual Participants Participants1 { get; set; }
        public virtual Participants Participants2 { get; set; }
        public virtual Participants Participants3 { get; set; }
        public virtual Participants Orderer { get; set; }
        public virtual Participants Participants5 { get; set; }
        public virtual Participants Participants6 { get; set; }
        public virtual Participants Participants7 { get; set; }
        public virtual Participants Participants8 { get; set; }
        public virtual Participants Participants9 { get; set; }
        public virtual Purchase_Group Purchase_Group { get; set; }
        public virtual Request_Notification Request_Notification { get; set; }
        public virtual Request_Event_Expand Request_Event_Expand { get; set; }
        public virtual Supplier Supplier1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request_Mail> Request_Mail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request_Order> Request_Order { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Participants> Participants10 { get; set; }
        public virtual Currency Currency { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request_Item> Request_Item { get; set; }
        public virtual Privacy Privacy { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RequestEvent_CustomFieldValue> RequestEvent_CustomFieldValue { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Event_Attachement> Event_Attachement { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request_Event_Orderer> Request_Event_Orderer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request_AuthorizedUsers> Request_AuthorizedUsers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request_Text> Request_Text1 { get; set; }
        public virtual Address Address { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request_Discussion> Request_Discussion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request_AccessRequest> Request_AccessRequest { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request_Event_Approval> Request_Event_Approval { get; set; }
    }
}
