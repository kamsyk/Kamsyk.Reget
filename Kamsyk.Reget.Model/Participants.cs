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
    
    public partial class Participants
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Participants()
        {
            this.Asset_Manager_Role = new HashSet<Asset_Manager_Role>();
            this.Centre = new HashSet<Centre>();
            this.Manager_Role = new HashSet<Manager_Role>();
            this.Manager_Role1 = new HashSet<Manager_Role>();
            this.Orderer_Supplier = new HashSet<Orderer_Supplier>();
            this.Participant_Office_Role = new HashSet<Participant_Office_Role>();
            this.Participant_Substitutee = new HashSet<Participant_Substitute>();
            this.Participant_Substituted = new HashSet<Participant_Substitute>();
            this.Participant_Substitute2 = new HashSet<Participant_Substitute>();
            this.ParticipantRole_BusinessTripCentreGroup = new HashSet<ParticipantRole_BusinessTripCentreGroup>();
            this.ParticipantRole_CentreAssetGroup = new HashSet<ParticipantRole_CentreAssetGroup>();
            this.ParticipantRole_CentreGroup = new HashSet<ParticipantRole_CentreGroup>();
            this.Purchase_Group_Limit = new HashSet<Purchase_Group_Limit>();
            this.Purchase_Group = new HashSet<Purchase_Group>();
            this.PurchaseGroup_ImplicitOrderer = new HashSet<PurchaseGroup_ImplicitOrderer>();
            this.PurchaseGroup_ImplicitRequestor = new HashSet<PurchaseGroup_ImplicitRequestor>();
            this.PurchaseGroup_Requestor = new HashSet<PurchaseGroup_Requestor>();
            this.Request_Event = new HashSet<Request_Event>();
            this.Request_Event1 = new HashSet<Request_Event>();
            this.Request_Event2 = new HashSet<Request_Event>();
            this.Request_Event3 = new HashSet<Request_Event>();
            this.Request_Event4 = new HashSet<Request_Event>();
            this.Request_Event5 = new HashSet<Request_Event>();
            this.Request_Event6 = new HashSet<Request_Event>();
            this.Request_Event7 = new HashSet<Request_Event>();
            this.Request_Event8 = new HashSet<Request_Event>();
            this.Request_Event9 = new HashSet<Request_Event>();
            this.Request_Order = new HashSet<Request_Order>();
            this.Ship_To_Address = new HashSet<Ship_To_Address>();
            this.Supplier_Contact = new HashSet<Supplier_Contact>();
            this.Supplier_Requestor_Export_Price = new HashSet<Supplier_Requestor_Export_Price>();
            this.User_GridSetting = new HashSet<User_GridSetting>();
            this.Asset_Requestor_Centre = new HashSet<Centre>();
            this.Participant_Role = new HashSet<Participant_Role>();
            this.PurchaseGroup_ExcludeOrderer = new HashSet<Purchase_Group>();
            this.PurchaseGroup_ExcludeRequestor = new HashSet<Purchase_Group>();
            this.PurchaseGroup_Orderer = new HashSet<Purchase_Group>();
            this.Purchase_Group_ROAccess = new HashSet<Purchase_Group>();
            this.Request_Event10 = new HashSet<Request_Event>();
            this.Requestor_Centre = new HashSet<Centre>();
            this.Address = new HashSet<Address>();
            this.Centre_Manager = new HashSet<Centre>();
            this.Maintenance_Event = new HashSet<Maintenance_Event>();
            this.Maintenance_Event1 = new HashSet<Maintenance_Event>();
            this.Participant_Substitute3 = new HashSet<Participant_Substitute>();
            this.Asset_Request_Event = new HashSet<Asset_Request_Event>();
            this.Asset_Request_Event1 = new HashSet<Asset_Request_Event>();
            this.Asset_Request_Event2 = new HashSet<Asset_Request_Event>();
            this.Asset_Request_Event3 = new HashSet<Asset_Request_Event>();
            this.Asset_Request_Event4 = new HashSet<Asset_Request_Event>();
            this.Asset_Request_Event5 = new HashSet<Asset_Request_Event>();
            this.Asset_Request_Event6 = new HashSet<Asset_Request_Event>();
            this.Asset_Request_Event7 = new HashSet<Asset_Request_Event>();
            this.Participant_Substitute4 = new HashSet<Participant_Substitute>();
            this.Attachement = new HashSet<Attachement>();
            this.Request_Event_Orderer = new HashSet<Request_Event_Orderer>();
            this.Request_Event_Orderer1 = new HashSet<Request_Event_Orderer>();
            this.Request_AuthorizedUsers = new HashSet<Request_AuthorizedUsers>();
            this.Discussion = new HashSet<Discussion>();
            this.Request_AccessRequest = new HashSet<Request_AccessRequest>();
            this.Request_Event_Approval = new HashSet<Request_Event_Approval>();
            this.Request_Event_Approval1 = new HashSet<Request_Event_Approval>();
        }
    
        public int id { get; set; }
        public int company_id { get; set; }
        public string first_name { get; set; }
        public string surname { get; set; }
        public string user_name { get; set; }
        public string personal_nr { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public Nullable<int> centre_id { get; set; }
        public Nullable<int> country_lang_id { get; set; }
        public Nullable<System.DateTime> date_of_birth { get; set; }
        public Nullable<bool> is_external { get; set; }
        public Nullable<bool> is_non_active { get; set; }
        public Nullable<bool> active { get; set; }
        public Nullable<bool> is_app_admin { get; set; }
        public string user_search_key { get; set; }
        public string first_name_search_key { get; set; }
        public string surname_search_key { get; set; }
        public int company_group_id { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Asset_Manager_Role> Asset_Manager_Role { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Centre> Centre { get; set; }
        public virtual Company Company { get; set; }
        public virtual Company Company1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Manager_Role> Manager_Role { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Manager_Role> Manager_Role1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Orderer_Supplier> Orderer_Supplier { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Participant_Office_Role> Participant_Office_Role { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Participant_Substitute> Participant_Substitutee { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Participant_Substitute> Participant_Substituted { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Participant_Substitute> Participant_Substitute2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ParticipantRole_BusinessTripCentreGroup> ParticipantRole_BusinessTripCentreGroup { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ParticipantRole_CentreAssetGroup> ParticipantRole_CentreAssetGroup { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ParticipantRole_CentreGroup> ParticipantRole_CentreGroup { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Purchase_Group_Limit> Purchase_Group_Limit { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Purchase_Group> Purchase_Group { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseGroup_ImplicitOrderer> PurchaseGroup_ImplicitOrderer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseGroup_ImplicitRequestor> PurchaseGroup_ImplicitRequestor { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseGroup_Requestor> PurchaseGroup_Requestor { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request_Event> Request_Event { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request_Event> Request_Event1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request_Event> Request_Event2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request_Event> Request_Event3 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request_Event> Request_Event4 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request_Event> Request_Event5 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request_Event> Request_Event6 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request_Event> Request_Event7 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request_Event> Request_Event8 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request_Event> Request_Event9 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request_Order> Request_Order { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ship_To_Address> Ship_To_Address { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Supplier_Contact> Supplier_Contact { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Supplier_Requestor_Export_Price> Supplier_Requestor_Export_Price { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User_GridSetting> User_GridSetting { get; set; }
        public virtual User_Setting User_Setting { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Centre> Asset_Requestor_Centre { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Participant_Role> Participant_Role { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Purchase_Group> PurchaseGroup_ExcludeOrderer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Purchase_Group> PurchaseGroup_ExcludeRequestor { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Purchase_Group> PurchaseGroup_Orderer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Purchase_Group> Purchase_Group_ROAccess { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request_Event> Request_Event10 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Centre> Requestor_Centre { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Address> Address { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Centre> Centre_Manager { get; set; }
        public virtual Centre Centre_Default { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Maintenance_Event> Maintenance_Event { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Maintenance_Event> Maintenance_Event1 { get; set; }
        public virtual Company_Group Company_Group { get; set; }
        public virtual ParticipantPhoto ParticipantPhoto { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Participant_Substitute> Participant_Substitute3 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Asset_Request_Event> Asset_Request_Event { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Asset_Request_Event> Asset_Request_Event1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Asset_Request_Event> Asset_Request_Event2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Asset_Request_Event> Asset_Request_Event3 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Asset_Request_Event> Asset_Request_Event4 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Asset_Request_Event> Asset_Request_Event5 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Asset_Request_Event> Asset_Request_Event6 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Asset_Request_Event> Asset_Request_Event7 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Participant_Substitute> Participant_Substitute4 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Attachement> Attachement { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request_Event_Orderer> Request_Event_Orderer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request_Event_Orderer> Request_Event_Orderer1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request_AuthorizedUsers> Request_AuthorizedUsers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Discussion> Discussion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request_AccessRequest> Request_AccessRequest { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request_Event_Approval> Request_Event_Approval { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request_Event_Approval> Request_Event_Approval1 { get; set; }
    }
}
