var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var Kamsyk;
(function (Kamsyk) {
    var RegetApp;
    (function (RegetApp) {
        var Supplier = /** @class */ (function () {
            function Supplier() {
                this.id = null;
                this.company_id = null;
                this.row_index = null;
                this.supp_name = null;
                this.supplier_id = null;
                this.street_part1 = null;
                this.city = null;
                this.zip = null;
                this.country = null;
                this.contact_person = null;
                this.phone = null;
                this.email = null;
                this.active = false;
                //public address: string = null;
            }
            return Supplier;
        }());
        RegetApp.Supplier = Supplier;
        var SupplierContact = /** @class */ (function () {
            function SupplierContact() {
                this.id = null;
                this.supplier_id = null;
                this.row_index = null;
                this.surname = null;
                this.first_name = null;
                this.email = null;
                this.phone = null;
            }
            return SupplierContact;
        }());
        RegetApp.SupplierContact = SupplierContact;
        var Participant = /** @class */ (function () {
            function Participant() {
                this.id = null;
                this.first_name = null;
                this.surname = null;
                this.user_name = null;
                this.substituted_by = null;
                this.substituted_until = null;
                this.name_surname_first = null;
                this.email = null;
                this.phone = null;
                this.office_name = null;
                this.photo240_url = null;
                this.row_index = null;
                this.active = false;
                this.is_external = false;
                this.country_flag = null;
            }
            return Participant;
        }());
        RegetApp.Participant = Participant;
        //export class DoubleParticipant {
        //    public participant1_surname: string;
        //    public participant2_surname: string;
        //}
        var Company = /** @class */ (function () {
            function Company() {
                this.id = null;
                this.default_currency_id = null;
                this.is_company_admin = null;
                this.is_supplier_admin = null;
                this.is_supplier_maintenance_allowed = null;
                this.country_code = null;
                this.supplier_source = null;
            }
            return Company;
        }());
        RegetApp.Company = Company;
        var PurchaseGroup = /** @class */ (function () {
            function PurchaseGroup() {
                this.id = null;
                this.group_name = null;
                this.group_loc_name = null;
                this.is_visible = false;
                this.is_html_show = false;
                this.default_supplier = null;
                this.parent_pg_id = null;
                this.parent_pg_loc_name = null;
                this.self_ordered = false;
                this.centre_group_id = null;
                this.purchase_type = null;
                this.requestor = null;
                this.is_coppied = false;
                this.is_coppied_default = false;
                this.is_disabled = false;
                this.orderer = null;
                this.is_active = false;
                //public is_approval_only: boolean = false;
                this.is_order_needed = false;
                this.is_approval_needed = false;
                this.is_multi_orderer = false;
                this.centre = null;
                this.local_text = null;
                this.purchase_group_limit = null;
                this.delete_requestors_all_categories = null;
                this.delete_orderers_all_categories = null;
                this.custom_field = null;
            }
            return PurchaseGroup;
        }());
        RegetApp.PurchaseGroup = PurchaseGroup;
        var CentreGroup = /** @class */ (function () {
            function CentreGroup() {
                this.id = null;
                this.name = null;
                this.flag_url = null;
                this.status_url = null;
                this.company_id = null;
                this.company_name = null;
                this.currency_id = null;
                this.supplier_group_id = null;
                this.allow_free_supplier = false;
                this.centre = null;
                this.currencies = null;
                this.deputy_orderer = null;
                this.cg_admin = null;
                this.orderer_supplier_appmatrix = null;
                this.currency = null;
                this.is_active = false;
            }
            return CentreGroup;
        }());
        RegetApp.CentreGroup = CentreGroup;
        var Centre = /** @class */ (function () {
            function Centre(id, name) {
                this.id = null;
                this.name = null;
                this.centre_group_id = null;
                this.id = id;
                this.name = name;
            }
            return Centre;
        }());
        RegetApp.Centre = Centre;
        var Currency = /** @class */ (function () {
            function Currency() {
                this.id = null;
                this.currency_name = null;
                this.currency_code = null;
                this.currency_code_name = null;
                this.is_set = false;
            }
            return Currency;
        }());
        RegetApp.Currency = Currency;
        var UserRoles = /** @class */ (function () {
            function UserRoles() {
                this.id = null;
                this.is_cg_property_admin = false;
                this.is_cg_appmatrix_admin = false;
                this.is_cg_orderer_admin = false;
                this.is_readonly = false;
                this.is_cg_requestor_admin = false;
            }
            return UserRoles;
        }());
        RegetApp.UserRoles = UserRoles;
        var CgAdmin = /** @class */ (function () {
            function CgAdmin() {
                this.id = null;
                this.surname = null;
                this.first_name = null;
                this.is_cg_prop_admin = false;
                this.is_appmatrix_admin = false;
                this.is_orderer_admin = false;
                this.is_requestor_admin = false;
            }
            return CgAdmin;
        }());
        RegetApp.CgAdmin = CgAdmin;
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
        var OrdererSupplier = /** @class */ (function () {
            function OrdererSupplier() {
                this.userid = null;
                this.surname = null;
                this.first_name = null;
                this.supplier_id = null;
                this.supplier_name = null;
                this.orderer_id = null;
                this.removetext = null;
                this.rooturl = null;
            }
            return OrdererSupplier;
        }());
        RegetApp.OrdererSupplier = OrdererSupplier;
        //export class OrdererSupplierAppmatrix {
        //}
        var PurchaseGroupLimit = /** @class */ (function () {
            function PurchaseGroupLimit() {
                this.pg_id = null;
                this.limit_id = null;
                this.limit_bottom = null;
                this.limit_bottom_text = null;
                this.limit_bottom_text_ro = null;
                this.limit_top = null;
                this.limit_top_text = null;
                this.limit_top_text_ro = null;
                this.is_top_unlimited = false;
                this.is_bottom_unlimited = false;
                this.is_limit_bottom_multipl = false;
                this.is_limit_top_multipl = false;
                this.is_app_man_selected = false;
                this.is_visible = false;
                this.is_first = false;
                this.is_last = false;
                this.app_level_id = null;
                this.limit_bottom_loc_curr_text_ro = null;
                this.limit_top_loc_curr_text_ro = null;
                this.manager_role = null;
            }
            return PurchaseGroupLimit;
        }());
        RegetApp.PurchaseGroupLimit = PurchaseGroupLimit;
        var ParentPurchaseGroup = /** @class */ (function () {
            function ParentPurchaseGroup() {
                this.row_index = null;
                this.id = null;
                this.name = null;
            }
            return ParentPurchaseGroup;
        }());
        RegetApp.ParentPurchaseGroup = ParentPurchaseGroup;
        var ParentPgExtended = /** @class */ (function () {
            function ParentPgExtended() {
                this.row_index = null;
                this.id = null;
                this.name = null;
                this.name_wo_diacritics = null;
                this.selected_companies = null;
                this.local_text = null;
            }
            return ParentPgExtended;
        }());
        RegetApp.ParentPgExtended = ParentPgExtended;
        var LocalText = /** @class */ (function () {
            function LocalText() {
                this.text = null;
                this.label = null;
                this.flag_url = null;
            }
            return LocalText;
        }());
        RegetApp.LocalText = LocalText;
        //export class ApproveLimit {
        //    public limit_id: number = null;
        //    public limit_bottom: number = null;
        //    public limit_top: number = null;
        //    public is_bottom_unlimited: boolean = false;
        //    public is_top_unlimited: boolean = false;
        //    public is_app_man_selected: boolean = false;
        //    public manager_role: ManagerRole[] = null;
        //}
        var ManagerRole = /** @class */ (function () {
            function ManagerRole() {
                this.participant_id = null;
                this.participant = null;
                this.substituted = null;
                this.approve_level_id = null;
                this.approve_status = null;
                //public modif_date: Date = null;
                this.modif_date_text = null;
            }
            return ManagerRole;
        }());
        RegetApp.ManagerRole = ManagerRole;
        var RequestorOrderer = /** @class */ (function () {
            function RequestorOrderer() {
                this.participant_id = null;
                this.first_name = null;
                this.surname = null;
                this.substituted_by = null;
                this.substituted_until = null;
                this.is_all = false;
                this.centre_id = null;
            }
            return RequestorOrderer;
        }());
        RegetApp.RequestorOrderer = RequestorOrderer;
        var Orderer = /** @class */ (function (_super) {
            __extends(Orderer, _super);
            function Orderer() {
                var _this = _super !== null && _super.apply(this, arguments) || this;
                _this.substituted_by = null;
                _this.substituted_until = null;
                return _this;
            }
            return Orderer;
        }(RequestorOrderer));
        RegetApp.Orderer = Orderer;
        var Requestor = /** @class */ (function (_super) {
            __extends(Requestor, _super);
            function Requestor() {
                var _this = _super !== null && _super.apply(this, arguments) || this;
                _this.centre_id = null;
                return _this;
            }
            return Requestor;
        }(RequestorOrderer));
        RegetApp.Requestor = Requestor;
        var AllRequestorOrdererExtended = /** @class */ (function () {
            function AllRequestorOrdererExtended() {
                this.participant_id = null;
                this.centre_id = null;
            }
            return AllRequestorOrdererExtended;
        }());
        RegetApp.AllRequestorOrdererExtended = AllRequestorOrdererExtended;
        var Address = /** @class */ (function () {
            function Address() {
                this.row_index = null;
                this.id = null;
                this.company_name = null;
                this.company_name_text = null;
                this.street = null;
                this.city = null;
                this.zip = null;
                this.country = null;
                this.address_text = null;
                this.is_selected = null;
            }
            return Address;
        }());
        RegetApp.Address = Address;
        var UsedPg = /** @class */ (function () {
            function UsedPg() {
                this.row_index = null;
                this.id = null;
                this.parent_pg_id = null;
                this.parent_pg_name = null;
                this.parent_pg_loc_name = null;
                this.pg_name = null;
                this.pg_loc_name = null;
                this.centre_group_id = null;
                this.centre_group_name = null;
                this.company_id = null;
                this.company_name = null;
                this.active = null;
                this.local_text = null;
            }
            return UsedPg;
        }());
        RegetApp.UsedPg = UsedPg;
        var AgDropDown = /** @class */ (function () {
            function AgDropDown() {
                this.id = null;
                this.value = null;
                this.label = null;
            }
            return AgDropDown;
        }());
        RegetApp.AgDropDown = AgDropDown;
        var PgReqDropDownExtend = /** @class */ (function () {
            function PgReqDropDownExtend() {
                this.id = null;
                this.name = null;
                this.pg_type = 0;
            }
            return PgReqDropDownExtend;
        }());
        RegetApp.PgReqDropDownExtend = PgReqDropDownExtend;
        var DropDownExtend = /** @class */ (function () {
            function DropDownExtend() {
                this.id = null;
                this.name = null;
                this.is_disabled = null;
            }
            return DropDownExtend;
        }());
        RegetApp.DropDownExtend = DropDownExtend;
        var CentreReqDropDownItem = /** @class */ (function () {
            function CentreReqDropDownItem() {
                this.id = null;
                this.name = null;
                this.is_free_supplier_allowed = null;
            }
            return CentreReqDropDownItem;
        }());
        RegetApp.CentreReqDropDownItem = CentreReqDropDownItem;
        var PurchaseGroupType;
        (function (PurchaseGroupType) {
            PurchaseGroupType[PurchaseGroupType["Standard"] = 0] = "Standard";
            //ApprovalOnly = 1,
            PurchaseGroupType[PurchaseGroupType["Items"] = 1] = "Items";
            PurchaseGroupType[PurchaseGroupType["Unknow"] = -1] = "Unknow";
        })(PurchaseGroupType = RegetApp.PurchaseGroupType || (RegetApp.PurchaseGroupType = {}));
        var ApprovalStatus;
        (function (ApprovalStatus) {
            ApprovalStatus[ApprovalStatus["Empty"] = -1] = "Empty";
            ApprovalStatus[ApprovalStatus["Approved"] = 1] = "Approved";
            ApprovalStatus[ApprovalStatus["Rejected"] = 0] = "Rejected";
            ApprovalStatus[ApprovalStatus["NotNeeded"] = 2] = "NotNeeded";
            ApprovalStatus[ApprovalStatus["WaitForApproval"] = 3] = "WaitForApproval";
        })(ApprovalStatus = RegetApp.ApprovalStatus || (RegetApp.ApprovalStatus = {}));
        var DataType;
        (function (DataType) {
            DataType[DataType["String"] = 0] = "String";
            DataType[DataType["Unknow"] = -1] = "Unknow";
        })(DataType = RegetApp.DataType || (RegetApp.DataType = {}));
        var RequestStatus;
        (function (RequestStatus) {
            RequestStatus[RequestStatus["Unknown"] = -1] = "Unknown";
            RequestStatus[RequestStatus["CanceledRequestor"] = 10] = "CanceledRequestor";
            RequestStatus[RequestStatus["CanceledOrderer"] = 12] = "CanceledOrderer";
            RequestStatus[RequestStatus["CanceledSystem"] = 14] = "CanceledSystem";
            RequestStatus[RequestStatus["New"] = 90] = "New";
            RequestStatus[RequestStatus["Draft"] = 100] = "Draft";
            RequestStatus[RequestStatus["WaitForApproval"] = 190] = "WaitForApproval";
            RequestStatus[RequestStatus["Rejected"] = 1200] = "Rejected";
            RequestStatus[RequestStatus["Approved"] = 1700] = "Approved";
        })(RequestStatus = RegetApp.RequestStatus || (RegetApp.RequestStatus = {}));
        var Privacy;
        (function (Privacy) {
            Privacy[Privacy["Private"] = 0] = "Private";
            Privacy[Privacy["Centre"] = 10] = "Centre";
            Privacy[Privacy["Public"] = 20] = "Public";
        })(Privacy = RegetApp.Privacy || (RegetApp.Privacy = {}));
        var ExchangeRate = /** @class */ (function () {
            function ExchangeRate() {
                this.source_currency_id = null;
                this.destin_currency_id = null;
                this.exchange_rate1 = null;
                this.currency = null;
                this.currency1 = null;
            }
            return ExchangeRate;
        }());
        RegetApp.ExchangeRate = ExchangeRate;
        var UserSubstitution = /** @class */ (function () {
            function UserSubstitution(id, substituted_user_id, substitute_user_id, substitute_start_date, substitute_end_date, substituted_name_surname_first, substitutee_name_surname_first, is_allow_take_over, remark, approval_status, approval_status_text, 
            //active_status_text: string,
            approval_men, modified_user_name, row_index, is_editable_author, is_editable_app_man, is_approve_hidden, is_reject_hidden, is_can_be_deleted, modified_date, active) {
                this.id = -1;
                this.substituted_user_id = -1;
                this.substitute_user_id = -1;
                this.discussion_bkg_color = null;
                this.discussion_border_color = null;
                this.discussion_user_color = null;
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
            return UserSubstitution;
        }());
        RegetApp.UserSubstitution = UserSubstitution;
        var Request = /** @class */ (function () {
            function Request() {
                this.id = null;
                this.version = null;
                this.request_nr = null;
                this.requestor = null;
                this.requestor_name_surname_first = null;
                this.currency_code = null;
                this.currency_id = null;
                this.request_centre_id = null;
                this.estimated_price = null;
                this.estimated_price_text = null;
                this.hyperlink = null;
                this.purchase_group_id = null;
                this.request_status = null;
                this.privacy_id = null;
                this.privacy_name = null;
                this.supplier_id = null;
                this.request_text = null;
                this.remarks = null;
                this.lead_time = null;
                this.centre_name = null;
                this.pg_name = null;
                this.orderer_id = null;
                this.is_approval_needed = false;
                this.is_order_needed = false;
                this.issued = null;
                this.is_revertable = false;
                this.is_deletable = false;
                this.ship_to_address_id = null;
                this.use_supplier_list = false;
                this.supplier_remark = null;
                this.is_approvable = false;
                this.is_order_available = false;
                this.ship_to_address_company_name = null;
                this.ship_to_address_street = null;
                this.ship_to_address_city = null;
                this.ship_to_address_zip = null;
                this.request_items = null;
                this.attachments = null;
                this.custom_fields = null;
                this.request_event_approval = null;
                this.orderers = null;
                this.workflow_items = null;
                this.request_sm = null;
            }
            return Request;
        }());
        RegetApp.Request = Request;
        var RequestSm = /** @class */ (function () {
            function RequestSm() {
                this.id = null;
                this.version = null;
                this.request_nr = null;
                this.requestor_name_surname_first = null;
            }
            return RequestSm;
        }());
        RegetApp.RequestSm = RequestSm;
        var Attachment = /** @class */ (function () {
            function Attachment() {
                this.id = -1;
                this.file_name = null;
                this.size_kb = null;
                this.is_can_be_deleted = false;
            }
            return Attachment;
        }());
        RegetApp.Attachment = Attachment;
        var RequestItem = /** @class */ (function () {
            function RequestItem() {
                this.row_index = null;
                //public request_id: number = null;
                this.name = null;
                this.amount = null;
                this.amount_text = null;
                this.unit_price = null;
                this.unit_price_text = null;
                this.unit_measurement_id = null;
                this.currency_id = null;
                this.currency_code = null;
                this.unit_code = null;
            }
            return RequestItem;
        }());
        RegetApp.RequestItem = RequestItem;
        var DiscussionItem = /** @class */ (function () {
            function DiscussionItem() {
                this.id = null;
                this.author_id = null;
                this.disc_text = null;
                this.author_name = null;
                this.author_initials = null;
                this.author_photo_url = null;
                this.bkg_color = null;
                this.border_color = null;
                this.user_color = null;
                this.modif_date_text = null;
            }
            return DiscussionItem;
        }());
        RegetApp.DiscussionItem = DiscussionItem;
        var Discussion = /** @class */ (function () {
            function Discussion() {
                this.discussion_bkg_color = null;
                this.discussion_border_color = null;
                this.discussion_user_color = null;
                this.discussion_items = null;
            }
            return Discussion;
        }());
        RegetApp.Discussion = Discussion;
        var CustomField = /** @class */ (function () {
            function CustomField() {
                this.id = null;
                this.label = null;
                this.data_type_id = null;
                this.is_mandatory = null;
                this.string_value = null;
                this.strvalue = null;
            }
            return CustomField;
        }());
        RegetApp.CustomField = CustomField;
        var RequestEventApproval = /** @class */ (function () {
            function RequestEventApproval() {
                this.request_event_id = null;
                this.request_event_version = null;
                this.app_man_id = null;
                this.app_level_id = null;
                this.approve_status = null;
                this.app_man_surname = null;
                this.app_man_first_name = null;
                this.modif_date_text = null;
            }
            return RequestEventApproval;
        }());
        RegetApp.RequestEventApproval = RequestEventApproval;
        var RequestSearchResult = /** @class */ (function () {
            function RequestSearchResult() {
                this.request_id = null;
                this.request_nr = null;
                this.found_text = null;
                this.found_text_short = null;
            }
            return RequestSearchResult;
        }());
        RegetApp.RequestSearchResult = RequestSearchResult;
        var WorkflowItem = /** @class */ (function () {
            function WorkflowItem() {
                this.resp_man = null;
                this.status_id = null;
            }
            return WorkflowItem;
        }());
        RegetApp.WorkflowItem = WorkflowItem;
        var Request_AccessRequest = /** @class */ (function () {
            function Request_AccessRequest() {
                this.request_id = null;
                this.status_id = null;
                this.request_date = null;
            }
            return Request_AccessRequest;
        }());
        RegetApp.Request_AccessRequest = Request_AccessRequest;
        var Order = /** @class */ (function () {
            function Order() {
                this.id = null;
                this.request_id = null;
                this.request_version = null;
                this.request_nr = null;
                this.otis_company_address = null;
                this.requestor_address = null;
                this.orderer_name = null;
                this.orderer_mail = null;
                this.orderer_phone = null;
                this.orderer_fax = null;
                this.requestor_name = null;
                this.requestor_mail = null;
                this.requestor_phone = null;
                this.supplier_address = null;
                this.request_text = null;
                this.currency_id = null;
                this.deliv_date = null;
                this.is_price_exported = false;
                this.mail_addresses = null;
                this.attachment_ids = null;
                this.culture_name = null;
                this.supplier_contact_name = null;
                this.supplier_mail = null;
                this.supplier_phone = null;
                this.supplier_id = null;
                this.attachments = null;
            }
            return Order;
        }());
        RegetApp.Order = Order;
        var Checkbox = /** @class */ (function () {
            function Checkbox() {
                this.id = null;
                this.text = null;
                this.is_selected = false;
            }
            return Checkbox;
        }());
        RegetApp.Checkbox = Checkbox;
        var DropDownMultiSelectItem = /** @class */ (function () {
            function DropDownMultiSelectItem() {
                this.id = null;
                this.label = null;
            }
            return DropDownMultiSelectItem;
        }());
        RegetApp.DropDownMultiSelectItem = DropDownMultiSelectItem;
    })(RegetApp = Kamsyk.RegetApp || (Kamsyk.RegetApp = {}));
})(Kamsyk || (Kamsyk = {}));
//# sourceMappingURL=reget-entity.js.map