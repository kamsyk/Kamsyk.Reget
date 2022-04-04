module Kamsyk.RegetApp {
    export class Supplier {
        public id: number = null;
        public company_id: number = null;
        public row_index: number = null;
        public supp_name: string = null;
        public supplier_id: string = null;
        public street_part1: string = null;
        public city: string = null;
        public zip: string = null;
        public country: string = null;
        public contact_person: string = null;
        public phone: string = null;
        public email: string = null;
        public active: boolean = false;
        //public address: string = null;
    }

    export class SupplierContact {
        public id: number = null;
        public supplier_id: number = null;
        public row_index: number = null;
        public surname: string = null;
        public first_name: string = null;
        public email: string = null;
        public phone: string = null;
    }

    export class Participant {
        public id: number = null;
        public first_name: string = null;
        public surname: string = null;
        public user_name: string = null;
        public substituted_by: string = null;
        public substituted_until: string = null;
        public name_surname_first: string = null;
        public email: string = null;
        public phone: string = null;
        public office_name: string = null;
        public photo240_url: string = null;
        public row_index: number = null;
        public active: boolean = false;
        public is_external: boolean = false;
        public country_flag: string = null;
    }

    //export class DoubleParticipant {
    //    public participant1_surname: string;
    //    public participant2_surname: string;
    //}

    export class Company {
        public id: number = null;
        public default_currency_id: number = null;
        public is_company_admin: boolean = null;
        public is_supplier_admin: boolean = null;
        public is_supplier_maintenance_allowed: boolean = null;
        public country_code: string = null;
        public supplier_source: string = null;
    }

    export class PurchaseGroup {
        public id: number = null;
        public group_name: string = null;
        public group_loc_name: string = null;
        public is_visible: boolean = false;
        public is_html_show: boolean = false;
        public default_supplier: Supplier = null;
        public parent_pg_id: number = null;
        public parent_pg_loc_name: string = null;
        public self_ordered: boolean = false;
        public centre_group_id: number = null;
        public purchase_type: number = null;
        public requestor: Requestor[] = null;
        public is_coppied: boolean = false;
        public is_coppied_default: boolean = false;
        public is_disabled: boolean = false;
        public orderer: Orderer[] = null;
        public is_active: boolean = false;
        //public is_approval_only: boolean = false;
        public is_order_needed: boolean = false;
        public is_approval_needed: boolean = false;
        public is_multi_orderer: boolean = false;
        public centre: Centre[] = null;
        public local_text: LocalText[] = null;
        public purchase_group_limit: PurchaseGroupLimit[] = null;
        public delete_requestors_all_categories: AllRequestorOrdererExtended[] = null;
        public delete_orderers_all_categories: number[] = null;
        public custom_field: CustomField[] = null;
        
    }

    export class CentreGroup {
        public id: number = null;
        public name: string = null;
        public flag_url: string = null;
        public status_url: string = null;
        public company_id: number = null;
        public company_name: string = null;
        public currency_id: number = null;
        public supplier_group_id: number = null;
        public allow_free_supplier: boolean = false;
        public centre: Centre[] = null;
        public currencies: Currency[] = null;
        public deputy_orderer: Orderer[] = null;
        public cg_admin: CgAdmin[] = null;
        public orderer_supplier_appmatrix: OrdererSupplier[] = null;
        public currency: Currency[] = null;
        public is_active: boolean = false;
    }

    export class Centre {
        public id: number = null;
        public name: string = null;
        public centre_group_id: number = null;

        public constructor(id: number, name: string) {
            this.id = id;
            this.name = name;
        }
    }

    export class Currency {
        public id: number = null;
        public currency_name: string = null;
        public currency_code: string = null;
        public currency_code_name: string = null;
        public is_set: boolean = false;
    }

    export class UserRoles {
        public id: number = null;
        public is_cg_property_admin: boolean = false;
        public is_cg_appmatrix_admin: boolean = false;
        public is_cg_orderer_admin: boolean = false;
        public is_readonly: boolean = false;
        public is_cg_requestor_admin: boolean = false;
    }

    export class CgAdmin {
        public id: number = null;
        public surname: string = null;
        public first_name: string = null;
        public is_cg_prop_admin: boolean = false;
        public is_appmatrix_admin: boolean = false;
        public is_orderer_admin: boolean = false;
        public is_requestor_admin: boolean = false;
    }

    //export class Supplier {
    //    public id: number = null;
    //    public company_id: number = null;
    //    public row_index: number = null;
    //    public supp_name: string = null;
    //    public supplier_id: string = null;
    //    public street_part1: string = null;
    //    public city: string = null;
    //    public zip: string = null;
    //    public country: string = null;
    //    public contact_person: string = null;
    //    public phone: string = null;
    //    public email: string = null;
    //    public active: boolean = false;
    //}

    export class OrdererSupplier {
        public userid: number = null;
        public surname: string = null;
        public first_name: string = null;
        public supplier_id: number = null;
        public supplier_name: string = null;
        public orderer_id: number = null;
        public removetext: string = null;
        public rooturl: string = null;
    }

    //export class OrdererSupplierAppmatrix {
    //}

    export class PurchaseGroupLimit {
        public pg_id: number = null;
        public limit_id: number = null;
        public limit_bottom: number = null;
        public limit_bottom_text: string = null;
        public limit_bottom_text_ro: string = null;
        public limit_top: number = null;
        public limit_top_text: string = null;
        public limit_top_text_ro: string = null;
        public is_top_unlimited: boolean = false;
        public is_bottom_unlimited: boolean = false;
        public is_limit_bottom_multipl: boolean = false;
        public is_limit_top_multipl: boolean = false;
        public is_app_man_selected: boolean = false;
        public is_visible: boolean = false;
        public is_first: boolean = false;
        public is_last: boolean = false;
        public app_level_id: number = null;
        public limit_bottom_loc_curr_text_ro: string = null;
        public limit_top_loc_curr_text_ro: string = null;
        public manager_role: ManagerRole[] = null;
    }

    export class ParentPurchaseGroup {
        public row_index: number = null;
        public id: number = null;
        public name: string = null;
    }

    export class ParentPgExtended {
        public row_index: number = null;
        public id: number = null;
        public name: string = null;
        public name_wo_diacritics: string = null;
        public selected_companies: number[] = null;
        public local_text: LocalText[] = null;
    }

    export class LocalText {
        public text: string = null;
        public label: string = null;
        public flag_url: string = null;
    }

    //export class ApproveLimit {
    //    public limit_id: number = null;
    //    public limit_bottom: number = null;
    //    public limit_top: number = null;
    //    public is_bottom_unlimited: boolean = false;
    //    public is_top_unlimited: boolean = false;
    //    public is_app_man_selected: boolean = false;
    //    public manager_role: ManagerRole[] = null;
    //}

    export class ManagerRole {
        public participant_id: number = null;
        public participant: Participant = null;
        public substituted: Participant = null;
        public approve_level_id: number = null;
        public approve_status: number = null;
        //public modif_date: Date = null;
        public modif_date_text: string = null;
    }

    export class RequestorOrderer {
        public participant_id: number = null;
        public first_name: string = null;
        public surname: string = null;
        public substituted_by: string = null;
        public substituted_until: string = null;
        public is_all: boolean = false;
        public centre_id: number = null;
    }

    export class Orderer extends RequestorOrderer {
        public substituted_by: string = null;
        public substituted_until: string = null;
    }

    export class Requestor extends RequestorOrderer {
        public centre_id: number = null;
    }

    export class AllRequestorOrdererExtended {
        public participant_id: number = null;
        public centre_id: number = null;
    }

    export class Address {
        public row_index: number = null;
        public id: number = null;
        public company_name: string = null;
        public company_name_text: string = null;
        public street: string = null;
        public city: string = null;
        public zip: string = null;
        public country: string = null;
        public address_text: string = null;
        public is_selected: boolean = null;
    }

    export class UsedPg {
        public row_index: number = null;
        public id: number = null;
        public parent_pg_id: number = null;
        public parent_pg_name: string = null;
        public parent_pg_loc_name: string = null;
        public pg_name: string = null;
        public pg_loc_name: string = null;
        public centre_group_id: number = null;
        public centre_group_name: string = null;
        public company_id: number = null;
        public company_name: string = null;
        public active: boolean = null;
        public local_text: LocalText[] = null;
    }

    export class AgDropDown {
        public id: string = null;
        public value: string = null;
        public label: string = null;
    }

    export class PgReqDropDownExtend {
        public id: number = null;
        public name: string = null;
        public pg_type: number = 0;
    }

    export class DropDownExtend {
        public id: number = null;
        public name: string = null;
        public is_disabled: boolean = null;
    }

    export class CentreReqDropDownItem {
        public id: number = null;
        public name: string = null;
        public is_free_supplier_allowed: boolean = null;
    }
        
    export enum PurchaseGroupType {
        Standard = 0,
        //ApprovalOnly = 1,
        Items,
        Unknow = -1
    }

    export enum ApprovalStatus {
        Empty = -1,
        Approved = 1,
        Rejected = 0,
        NotNeeded = 2,
        WaitForApproval = 3
    }

    export enum DataType {
        String = 0,
        Unknow = -1
    }

    export enum RequestStatus {
        Unknown = -1,
        CanceledRequestor = 10,
        CanceledOrderer = 12,
        CanceledSystem = 14,
        New = 90,
        Draft = 100,
        WaitForApproval = 190,
        Rejected = 1200,
        Approved = 1700,
        
    }

    export enum Privacy {
        Private = 0,
        Centre = 10,
        Public = 20
    }

    export class ExchangeRate {
        public source_currency_id: number = null;
        public destin_currency_id: number = null;
        public exchange_rate1: number = null;

        public currency: Currency = null;
        public currency1: Currency = null;
    }

    export class UserSubstitution {
        public id: number = -1;
        public substituted_user_id: number = -1;
        public substitute_user_id: number = -1;
        public substitute_start_date: Date;
        public substitute_start_date_text: string;
        public substitute_end_date: Date;
        public substitute_end_date_text: string;
        public modified_date: Date;
        public modified_date_text: string;
        public substituted_name_surname_first: string;
        public substitutee_name_surname_first: string;
        public remark: string;
        public approval_status: number;
        public approval_status_text: string;
        public active_status_text: string;
        public approval_men: string;
        public modified_user_name: string;
        public row_index: number;
        public is_editable_author: boolean;
        public is_can_be_deleted: boolean;
        public is_editable_app_man: boolean;
        public is_approve_hidden: boolean;
        public is_reject_hidden: boolean;
        public is_allow_take_over: boolean;
        public is_edit_hidden: boolean;
        //public is_can_be_deleted: boolean;
        public active: boolean;
        public discussion_bkg_color: string = null;
        public discussion_border_color: string = null;
        public discussion_user_color: string = null;

        constructor(
            id: number,
            substituted_user_id: number,
            substitute_user_id: number,
            substitute_start_date: Date,
            substitute_end_date: Date,
            substituted_name_surname_first: string,
            substitutee_name_surname_first: string,
            is_allow_take_over: boolean,
            remark: string,
            approval_status: number,
            approval_status_text: string,
            //active_status_text: string,
            approval_men: string,
            modified_user_name: string,
            row_index: number,
            is_editable_author: boolean,
            is_editable_app_man: boolean,
            is_approve_hidden: boolean,
            is_reject_hidden: boolean,
            is_can_be_deleted: boolean,
            modified_date: Date,
            active: boolean
        ) {
            this.id = id;
            this.substituted_user_id = substituted_user_id;
            this.substitute_user_id = substitute_user_id;
            this.substitute_start_date = substitute_start_date;
            this.substitute_end_date = substitute_end_date;
            this.substituted_name_surname_first = substituted_name_surname_first;
            this.substitutee_name_surname_first = substitutee_name_surname_first;
            this.is_allow_take_over = is_allow_take_over;
            this.remark = remark;
            this.approval_status = approval_status;
            this.approval_status_text = approval_status_text;
            this.approval_men = approval_men;
            this.modified_user_name = modified_user_name;
            this.row_index = row_index;
            this.is_editable_author = is_editable_author;
            this.is_editable_app_man = is_editable_app_man;
            this.is_approve_hidden = is_approve_hidden;
            this.is_reject_hidden = is_reject_hidden;
            this.is_can_be_deleted = is_can_be_deleted;
            this.modified_date = modified_date;
            this.active = active;
        }
    }

    export class Request {
        public id: number = null;
        public version: number = null;
        public request_nr: string = null;
        public requestor: number = null;
        public requestor_name_surname_first: string = null;
        public currency_code: string = null;
        public currency_id: number = null;
        public request_centre_id: number = null;
        public estimated_price: number = null;
        public estimated_price_text: string = null;
        public hyperlink: string = null;
        public purchase_group_id: number = null;
        public request_status: number = null;
        public privacy_id: number = null;
        public privacy_name: string = null;
        public supplier_id: number = null;
        public request_text: string = null;
        public remarks: string = null;
        public lead_time: Date = null;
        public centre_name: string = null;
        public pg_name: string = null;
        public orderer_id: number = null;
        public is_approval_needed: boolean = false;
        public is_order_needed: boolean = false;
        public issued: Date = null;
        public is_revertable: boolean = false;
        public is_deletable: boolean = false;
        public ship_to_address_id: number = null;
        public use_supplier_list: boolean = false;
        public supplier_remark: string = null;
        public is_approvable: boolean = false;
        public is_order_available: boolean = false;

        public ship_to_address_company_name: string = null;
        public ship_to_address_street: string = null;
        public ship_to_address_city: string = null;
        public ship_to_address_zip: string = null;

        public request_items: RequestItem[] = null;
        public attachments: Attachment[] = null;
        public custom_fields: CustomField[] = null;
        public request_event_approval: RequestEventApproval[] = null;
        public orderers: Orderer[] = null;
        public workflow_items: WorkflowItem[] = null;
        public request_sm: RequestSm = null;
    }

    export class RequestSm {
        public id: number = null;
        public version: number = null;
        public request_nr: string = null;
        public requestor_name_surname_first: string = null;
    }

    export class Attachment {
        public id: number = -1;
        public file_name: string = null;
        public size_kb: number = null;
        public is_can_be_deleted: boolean = false;
    }

    
    export class RequestItem {
        public row_index: number = null;
        //public request_id: number = null;
        public name: string = null;
        public amount: number = null;
        public amount_text: string = null;
        public unit_price: number = null;
        public unit_price_text: string = null;
        public unit_measurement_id: number = null;
        public currency_id: number = null;
        public currency_code: string = null;
        public unit_code: string = null;
       
    }

    export class DiscussionItem {
        public id: number = null;
        public author_id: number = null;
        public disc_text: string = null;
        public author_name: string = null;
        public author_initials: string = null;
        public author_photo_url: string = null;
        public bkg_color: string = null;
        public border_color: string = null;
        public user_color: string = null;
        public modif_date_text: string = null;
    }

    export class Discussion {
        public discussion_bkg_color: string = null;
        public discussion_border_color: string = null;
        public discussion_user_color: string = null;
        public discussion_items: DiscussionItem[] = null;
    }

    export class CustomField {
        public id: number = null;
        public label: string = null;
        public data_type_id: number = null;
        public is_mandatory: boolean = null;
        public string_value: string = null;
        public strvalue: string = null;
    }

    export class RequestEventApproval {
        public request_event_id: number = null;
        public request_event_version: number = null;
        public app_man_id: number = null;
        public app_level_id: number = null;
        public approve_status: number = null;
        public app_man_surname: string = null;
        public app_man_first_name: string = null;
        public modif_date_text: string = null;
    }

    export class RequestSearchResult {
        public request_id: number = null;
        public request_nr: string = null;
        public found_text: string = null;
        public found_text_short: string = null;
    }

    export class WorkflowItem {
        public resp_man: string = null;
        public status_id: number = null;
    }

    export class Request_AccessRequest {
        public request_id: number = null;
        public status_id: number = null;
        public request_date: Date = null;
    }

    export class Order {
        public id: number = null;
        public request_id: number = null;
        public request_version: number = null;
        public request_nr: string = null;
        public otis_company_address: string = null;
        public requestor_address: string = null;
        public orderer_name: string = null;
        public orderer_mail: string = null;
        public orderer_phone: string = null;
        public orderer_fax: string = null;
        public requestor_name: string = null;
        public requestor_mail: string = null;
        public requestor_phone: string = null;
        public supplier_address: string = null;
        public request_text: string = null;
        public currency_id: number = null;
        public deliv_date: Date = null;
        public is_price_exported: boolean = false;
        public mail_addresses: string = null;
        public attachment_ids: string = null;
        public culture_name: string = null;
        public supplier_contact_name: string = null;
        public supplier_mail: string = null;
        public supplier_phone: string = null;
        public supplier_id: number = null;
        public attachments: Attachment[] = null;
    }

    export class Checkbox {
        public id: number = null;
        public text: string = null;
        public is_selected: boolean = false;
    }

    export class DropDownMultiSelectItem {
        public id: string = null;
        public label: string = null;
        
    }
}