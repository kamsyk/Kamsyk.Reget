using Kamsyk.Reget.Model.Common;
using Kamsyk.Reget.Model.DataDictionary;
using Kamsyk.Reget.Model.ExtendedModel;
using Kamsyk.Reget.Model.ExtendedModel.Attachment;
using Kamsyk.Reget.Model.ExtendedModel.Request;
using Kamsyk.Reget.Model.Repositories;
using Kamsyk.Reget.Model.Repositories.Interfaces;
using Kamsyk.Reget.Model.Request.ExtendedModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static Kamsyk.Reget.Model.Repositories.AppTextStoreRepository;

namespace Kamsyk.Reget.Model.Repositories {
    public class RequestRepository : BaseRepository<Request_Event>, IRequestRepository {
        #region Constants
        //public const int STATUS_TMP_WAIT_FOR_APPROVAL = -100;
        //public const int STATUS_TMP_REJECTED = -200;
        public const int STATUS_UNKNOWN = -1;
        public const int STATUS_NEW = 90;
        public const int STATUS_DRAFT = 100;
        public const int STATUS_WAIT_FOR_APPROVAL = 190;
        //public const int STATUS_WAIT_FOR_L1 = 200;
        //public const int STATUS_WAIT_FOR_L2 = 300;
        //public const int STATUS_WAIT_FOR_L3 = 400;
        //public const int STATUS_WAIT_FOR_L4 = 500;
        //public const int STATUS_WAIT_FOR_L5 = 600;
        //public const int STATUS_WAIT_FOR_L6 = 610;
        //public const int STATUS_APPROVED_L1 = 700;
        //public const int STATUS_APPROVED_L2 = 800;
        //public const int STATUS_APPROVED_L3 = 900;
        //public const int STATUS_APPROVED_L4 = 1000;
        //public const int STATUS_APPROVED_L5 = 1100;
        //public const int STATUS_APPROVED_L6 = 1110;
        //public const int STATUS_REFUSED_L1 = 1200;
        //public const int STATUS_REFUSED_L2 = 1300;
        //public const int STATUS_REFUSED_L3 = 1400;
        //public const int STATUS_REFUSED_L4 = 1500;
        //public const int STATUS_REFUSED_L5 = 1600;
        //public const int STATUS_REFUSED_L6 = 1610;
        public const int STATUS_REJECTED = 1200;
        public const int STATUS_APPROVED = 1700;
        public const int STATUS_WAIT_FOR_COMMENT = 1750;
        public const int STATUS_ORDERED = 1800;
        public const int STATUS_SUPPLIED = 1900;
        public const int STATUS_CLOSED = 2000;
        public const int STATUS_CANCELED_REQUESTOR = 10;
        public const int STATUS_CANCELED_ORDERER = 12;
        public const int STATUS_CANCELED_SYSTEM = 14;


        #endregion

        #region Enums
        public enum RequestStatus {
            Unknown = STATUS_UNKNOWN,
            New = STATUS_NEW,
            Draft = STATUS_DRAFT,
            //WaitForAppL1 = STATUS_WAIT_FOR_L1,
            //WaitForAppL2 = STATUS_WAIT_FOR_L2,
            //WaitForAppL3 = STATUS_WAIT_FOR_L3,
            //WaitForAppL4 = STATUS_WAIT_FOR_L4,
            //WaitForAppL5 = STATUS_WAIT_FOR_L5,
            //WaitForAppL6 = STATUS_WAIT_FOR_L6,
            //RefusedL1 = STATUS_REFUSED_L1,
            //RefusedL2 = STATUS_REFUSED_L2,
            //RefusedL3 = STATUS_REFUSED_L3,
            //RefusedL4 = STATUS_REFUSED_L4,
            //RefusedL5 = STATUS_REFUSED_L5,
            //RefusedL6 = STATUS_REFUSED_L6,
            Approved = STATUS_APPROVED,
            WaitForComment = STATUS_WAIT_FOR_COMMENT,
            Ordered = STATUS_ORDERED,
            Supplied = STATUS_SUPPLIED,
            Closed = STATUS_CLOSED,
            CanceledRequestor = STATUS_CANCELED_REQUESTOR,
            CanceledOrderer = STATUS_CANCELED_ORDERER,
            CanceledSystem = STATUS_CANCELED_SYSTEM,
            WaitForApproval = STATUS_WAIT_FOR_APPROVAL,
            Rejected = STATUS_REJECTED
        }

        public enum ApproveStatus {
            Empty = -1,
            Approved = 1,
            Rejected = 0,
            NotNeeded = 2,
            WaitForApproval = 3
        }

        public enum Privacy {
            Private = 0,
            Centre = 10,
            Public = 20
        }

        private enum WorkflowItemStatus {
            Wait = 0,
            Processing = 1,
            Finished = 2
        }
        #endregion

        #region Methods
        public RequestEventExtended GetNewRequest(int currentUserId) {
            RequestEventExtended requestExtended = new RequestEventExtended();

            requestExtended.requestor = DataNulls.INT_NULL;
            requestExtended.request_nr = null;
            requestExtended.request_status = (int)RequestStatus.New;
            requestExtended.privacy_id = (int)Privacy.Private;

            //requestor centres
            //LoadRequestorCentres(requestExtended, currentUserId);

            return requestExtended;
        }

        public bool IsSupplierUsed(int supplierId) {
            var request = (from requestDb in m_dbContext.Request_Event
                           where requestDb.supplier_id == supplierId
                           select new { id = requestDb.id }).FirstOrDefault();

            return (request != null);
        }

        public Request_Event GetRequestEventByNr(string requestNr) {
            Request_Event requestEvent = (from requestDb in m_dbContext.Request_Event
                                          where requestDb.request_nr == requestNr &&
                                          ((bool)requestDb.last_event) == true
                                          select requestDb).FirstOrDefault();

            return requestEvent;
        }

        public Request_Event GetRequestEventById(int id) {
            Request_Event requestEvent = (from requestDb in m_dbContext.Request_Event
                                          where requestDb.id == id &&
                                          ((bool)requestDb.last_event) == true
                                          select requestDb).FirstOrDefault();


            return requestEvent;
        }

        public RequestEventExtended GetRequestEventByIdJs(int id, string cultureName, int userId) {
            Request_Event requestEvent = (from requestDb in m_dbContext.Request_Event
                                          where requestDb.id == id &&
                                          ((bool)requestDb.last_event) == true
                                          select requestDb).FirstOrDefault();

            RequestEventExtended requestExtended = new RequestEventExtended();
            SetValues(requestEvent, requestExtended);
            requestExtended.estimated_price_text = ConvertData.GetStringValueRemoveUselessZeros(requestEvent.estimated_price, cultureName, false);
            if (requestEvent.Centre != null) {
                requestExtended.centre_name = requestEvent.Centre.name;
            }
            if (requestEvent.purchase_group_id != null) {
                int pgId = (int)requestEvent.purchase_group_id;
                string pgName = new PgRepository().GetLocalName(pgId, cultureName);
                requestExtended.pg_name = pgName;
                requestExtended.is_approval_needed = requestEvent.Purchase_Group.is_approval_needed;
                requestExtended.is_order_needed = requestEvent.Purchase_Group.is_order_needed;
            }

            if (requestEvent.Currency != null) {
                requestExtended.currency_code = requestEvent.Currency.currency_code;
            }

            requestExtended.requestor_name_surname_first = UserRepository.GetUserNameSurnameFirst(requestEvent.Requestor);
            if (requestEvent.privacy_id == null) {
                requestExtended.privacy_id = (int)Privacy.Public;
            }

            //Set Custom Fields
            requestExtended.custom_fields = new List<CustomFieldExtend>();
            if (requestEvent.RequestEvent_CustomFieldValue != null) {
                foreach (var custField in requestEvent.RequestEvent_CustomFieldValue) {
                    if (requestEvent.request_status == (int)RequestStatus.Draft) {
                        //Remove Non Active Custom Fields
                        if (custField.is_active == false) {
                            custField.is_active = false;
                            SaveChanges();
                            continue;
                        }

                        if (requestEvent.Purchase_Group != null) {
                            var pgCf = (from pgCfDb in requestEvent.Purchase_Group.PurchaseGroup_CustomField
                                        where pgCfDb.custom_field_id == custField.custom_field_id
                                        select pgCfDb).FirstOrDefault();
                            if (pgCf == null) {
                                custField.is_active = false;
                                SaveChanges();
                                continue;
                            }
                        }
                    }

                    CustomFieldExtend customFieldExtend = new CustomFieldExtend();
                    SetValues(custField, customFieldExtend);
                    customFieldExtend.id = custField.Custom_Field.id;

                    requestExtended.custom_fields.Add(customFieldExtend);
                }
            }

            //set attachment
            requestExtended.attachments = new List<AttachmentExtend>();
            if (requestEvent.Event_Attachement != null) {
                foreach (var attEvent in requestEvent.Event_Attachement) {
                    AttachmentExtend attachmentExtend = new AttachmentExtend();
                    attachmentExtend.id = attEvent.Attachement.id;
                    attachmentExtend.file_name = attEvent.Attachement.file_name;
                    if (attEvent.Attachement.size_kb != null) {
                        attachmentExtend.size_kb = (double)attEvent.Attachement.size_kb;
                    }

                    attachmentExtend.is_can_be_deleted = requestExtended.is_requestor_can_edit;
                    //attachmentExtend.icon_url = GetFileIconUrl(attachmentExtend.file_name);
                    //attachmentExtend.icon_url = attEvent.Attachement.ur

                    requestExtended.attachments.Add(attachmentExtend);
                }
            }

            //set app men
            var sortApp = (from reqEvAppDb in m_dbContext.Request_Event_Approval
                           where reqEvAppDb.request_event_id == requestEvent.id
                           && reqEvAppDb.is_last_version == true
                           orderby reqEvAppDb.app_level_id
                           select reqEvAppDb).ToList();

            #region Temporary Workaround
            //this code will be cnaceled as soon as old version of the app will be decomissioned
            if (requestEvent.manager1 == null) {
                TmpRemoveAppLevel(requestEvent, 0);
            } else {
                TmpUpdateAppLevel(requestEvent, 0, (int)requestEvent.manager1, (int)requestEvent.approved_1);
            }

            if (requestEvent.manager2 == null) {
                TmpRemoveAppLevel(requestEvent, 1);
            } else {
                TmpUpdateAppLevel(requestEvent, 1, (int)requestEvent.manager2, (int)requestEvent.approved_2);
            }

            if (requestEvent.manager3 == null) {
                TmpRemoveAppLevel(requestEvent, 2);
            } else {
                TmpUpdateAppLevel(requestEvent, 2, (int)requestEvent.manager3, (int)requestEvent.approved_3);
            }

            if (requestEvent.manager4 == null) {
                TmpRemoveAppLevel(requestEvent, 3);
            } else {
                TmpUpdateAppLevel(requestEvent, 3, (int)requestEvent.manager4, (int)requestEvent.approved_4);
            }

            if (requestEvent.manager5 == null) {
                TmpRemoveAppLevel(requestEvent, 4);
            } else {
                TmpUpdateAppLevel(requestEvent, 4, (int)requestEvent.manager5, (int)requestEvent.approved_5);
            }

            if (requestEvent.manager6 == null) {
                TmpRemoveAppLevel(requestEvent, 5);
            } else {
                TmpUpdateAppLevel(requestEvent, 5, (int)requestEvent.manager6, (int)requestEvent.approved_6);
            }

            sortApp = (from reqEvAppDb in m_dbContext.Request_Event_Approval
                       where reqEvAppDb.request_event_id == requestEvent.id
                       && reqEvAppDb.is_last_version == true
                       orderby reqEvAppDb.app_level_id
                       select reqEvAppDb).ToList();

            //if (sortApp == null || sortApp.Count == 0) {
            //    if (requestEvent.manager1 != null) {
            //        Request_Event_Approval rea = new Request_Event_Approval();
            //        rea.request_event_id = requestEvent.id;
            //        rea.request_event_version = requestEvent.version;
            //        rea.app_level_id = 0;
            //        rea.app_man_id = (int)requestEvent.manager1;
            //        rea.approve_status = (int)requestEvent.approved_1;
            //        rea.is_last_version = true;
            //        m_dbContext.Request_Event_Approval.Add(rea);
            //        SaveChanges();
            //    }

            //    if (requestEvent.manager2 != null) {
            //        Request_Event_Approval rea = new Request_Event_Approval();
            //        rea.request_event_id = requestEvent.id;
            //        rea.request_event_version = requestEvent.version;
            //        rea.app_level_id = 1;
            //        rea.app_man_id = (int)requestEvent.manager2;
            //        rea.approve_status = (int)requestEvent.approved_2;
            //        rea.is_last_version = true;
            //        m_dbContext.Request_Event_Approval.Add(rea);
            //        SaveChanges();
            //    }

            //    if (requestEvent.manager3 != null) {
            //        Request_Event_Approval rea = new Request_Event_Approval();
            //        rea.request_event_id = requestEvent.id;
            //        rea.request_event_version = requestEvent.version;
            //        rea.app_level_id = 2;
            //        rea.app_man_id = (int)requestEvent.manager3;
            //        rea.approve_status = (int)requestEvent.approved_3;
            //        rea.is_last_version = true;
            //        m_dbContext.Request_Event_Approval.Add(rea);
            //        SaveChanges();
            //    }

            //    if (requestEvent.manager4 != null) {
            //        Request_Event_Approval rea = new Request_Event_Approval();
            //        rea.request_event_id = requestEvent.id;
            //        rea.request_event_version = requestEvent.version;
            //        rea.app_level_id = 3;
            //        rea.app_man_id = (int)requestEvent.manager4;
            //        rea.approve_status = (int)requestEvent.approved_4;
            //        rea.is_last_version = true;
            //        m_dbContext.Request_Event_Approval.Add(rea);
            //        SaveChanges();
            //    }

            //    if (requestEvent.manager5 != null) {
            //        Request_Event_Approval rea = new Request_Event_Approval();
            //        rea.request_event_id = requestEvent.id;
            //        rea.request_event_version = requestEvent.version;
            //        rea.app_level_id = 4;
            //        rea.app_man_id = (int)requestEvent.manager5;
            //        rea.approve_status = (int)requestEvent.approved_5;
            //        rea.is_last_version = true;
            //        m_dbContext.Request_Event_Approval.Add(rea);
            //        SaveChanges();
            //    }

            //    if (requestEvent.manager6 != null) {
            //        Request_Event_Approval rea = new Request_Event_Approval();
            //        rea.request_event_id = requestEvent.id;
            //        rea.request_event_version = requestEvent.version;
            //        rea.app_level_id = 5;
            //        rea.app_man_id = (int)requestEvent.manager6;
            //        rea.approve_status = (int)requestEvent.approved_6;
            //        rea.is_last_version = true;
            //        m_dbContext.Request_Event_Approval.Add(rea);
            //        SaveChanges();
            //    }

            //    sortApp = (from reqEvAppDb in m_dbContext.Request_Event_Approval
            //               where reqEvAppDb.request_event_id == requestEvent.id
            //               && reqEvAppDb.is_last_version == true
            //               orderby reqEvAppDb.app_level_id
            //               select reqEvAppDb).ToList();
            //} else {

            //}

           
            #endregion

            requestExtended.request_event_approval = new List<RequestEventApprovalExtended>();
            if (sortApp != null) {
                foreach (var appMan in sortApp) {
                    if (appMan.AppMan == null) {
                        appMan.AppMan = new UserRepository().GetParticipantById(appMan.app_man_id);
                    }

                    RequestEventApprovalExtended rea = new RequestEventApprovalExtended();
                    SetValues(appMan, rea);
                    //rea.app_man_id = appMan.app_man_id;
                    rea.app_man_surname = appMan.AppMan.surname;
                    rea.app_man_first_name = appMan.AppMan.first_name;
                    //rea.approve_status = appMan.approve_status;
                    //rea.app_level_id = appMan.app_level_id;
                    //rea.modif_date = appMan.modif_date;
                    requestExtended.request_event_approval.Add(rea);
                }
            }            

            //set orderers
            if (requestEvent.Purchase_Group != null) {
                if (requestEvent.Purchase_Group.is_order_needed) {
                    requestExtended.orderers = new List<OrdererExtended>();
                    var evOrderers = (from evOrderersDb in m_dbContext.Request_Event_Orderer
                                      where evOrderersDb.request_event_id == requestEvent.id
                                      && evOrderersDb.is_last_version == true
                                      select evOrderersDb).ToList();
                    foreach (var reqOrderer in evOrderers) {
                        OrdererExtended ordererExtended = new OrdererExtended();
                        ordererExtended.participant_id = reqOrderer.orderer_id;
                        ordererExtended.surname = reqOrderer.Orderer.surname;
                        ordererExtended.first_name = reqOrderer.Orderer.first_name;
                        requestExtended.orderers.Add(ordererExtended);
                    }

                }
            }

            //set isrevertable
            requestExtended.is_revertable = GetIsRevertable(requestEvent, userId);

            //set is deletable
            requestExtended.is_deletable = GetIsDeletable(requestEvent, userId);

            //set is approvable
            requestExtended.is_approvable = GetIsApprovable(requestEvent, userId);

            //set is order available
            requestExtended.is_order_available = GetIsOrderAvailable(requestEvent, userId);

            //set remark
            var appTextRemark = (from reqTextDb in m_dbContext.Request_Text
                                 where reqTextDb.request_id == requestEvent.id
                                 join appTextDb in m_dbContext.App_Text_Store
                                 on reqTextDb.app_text_store_id equals appTextDb.id
                                 where appTextDb.text_type == (int)TextType.RequestRemark
                                 && reqTextDb.is_last_version == true
                                 select appTextDb).FirstOrDefault();
            if (appTextRemark != null) {
                requestExtended.remarks = appTextRemark.text_content;
            }

            //set supplier text
            var suppRemark = (from reqTextDb in m_dbContext.Request_Text
                                 where reqTextDb.request_id == requestEvent.id
                                 join appTextDb in m_dbContext.App_Text_Store
                                 on reqTextDb.app_text_store_id equals appTextDb.id
                                 where appTextDb.text_type == (int)TextType.RequestSupplier
                                 && reqTextDb.is_last_version == true
                                 select appTextDb).FirstOrDefault();
            if (suppRemark != null) {
                requestExtended.supplier_remark = suppRemark.text_content;
            }

            //workflow info
            SetWorkflowItems(requestExtended);

            return requestExtended;
        }

        private void TmpRemoveAppLevel(Request_Event requestEvent, int appLevelId) {
            var appLevel = (from appLevelDb in requestEvent.Request_Event_Approval
                            where appLevelDb.app_level_id == appLevelId
                            && appLevelDb.is_last_version == true
                            select appLevelDb).FirstOrDefault();
            if (appLevel != null) {
                //foreach (var appLevel in appLevels) {
                    m_dbContext.Request_Event_Approval.Remove(appLevel);
                //}
                SaveChanges();
            }
        }

        private void TmpUpdateAppLevel(
            Request_Event requestEvent, 
            int appLevelId,
            int managerId,
            int appStatus) {
            var appLevel = (from appLevelDb in requestEvent.Request_Event_Approval
                             where appLevelDb.app_level_id == appLevelId
                             && appLevelDb.is_last_version == true
                            select appLevelDb).FirstOrDefault();
            if (appLevel == null) {
                new RequestEventApprovalRepository().AddRequestEventApproval(
                    m_dbContext,
                    requestEvent.id,
                    requestEvent.version,
                    appLevelId,
                    managerId,
                    null,
                    appStatus);
                //Request_Event_Approval rea = new Request_Event_Approval();
                //rea.request_event_id = requestEvent.id;
                //rea.request_event_version = requestEvent.version;
                //rea.app_level_id = appLevelId;
                //rea.app_man_id = managerId;
                //rea.approve_status = appStatus;
                //rea.is_last_version = true;
                //m_dbContext.Request_Event_Approval.Add(rea);
                SaveChanges();
            } else {
                if (appLevel.approve_status != appStatus) {
                    appLevel.approve_status = appStatus;
                    SaveChanges();
                }
            }
        }

        private void SetWorkflowItems(RequestEventExtended requestExtended) {
            if (requestExtended.request_status != (int)RequestStatus.New) {
                requestExtended.workflow_items = new List<WorkflowItem>();

                //Requestor
                WorkflowItem workflowItem = new WorkflowItem();
                workflowItem.resp_man = requestExtended.requestor_name_surname_first;
                if (requestExtended.request_status == (int)RequestStatus.Draft) {
                    workflowItem.status_id = (int)WorkflowItemStatus.Processing;
                } else {
                    workflowItem.status_id = (int)WorkflowItemStatus.Finished;
                }
                requestExtended.workflow_items.Add(workflowItem);

                //App Men
                if (requestExtended.request_event_approval != null) {
                    var sortApprs = requestExtended.request_event_approval.OrderBy(x => x.app_level_id).ToList();
                    foreach (var eventApp in sortApprs) {
                        workflowItem = new WorkflowItem();
                        workflowItem.resp_man = UserRepository.GetUserNameSurnameFirst(eventApp.app_man_surname, eventApp.app_man_first_name);
                        if (eventApp.approve_status == (int)ApproveStatus.WaitForApproval) {
                            workflowItem.status_id = (int)WorkflowItemStatus.Processing;
                            
                        } else if (eventApp.approve_status == (int)ApproveStatus.Approved
                            || eventApp.approve_status == (int)ApproveStatus.Rejected
                            || eventApp.approve_status == (int)ApproveStatus.NotNeeded) {
                            workflowItem.status_id = (int)WorkflowItemStatus.Finished;
                        } else {
                            workflowItem.status_id = (int)WorkflowItemStatus.Wait;
                        }
                        requestExtended.workflow_items.Add(workflowItem);
                    }
                }

                //Orderers
                if (requestExtended.orderers != null) {
                    workflowItem = new WorkflowItem();
                    if (requestExtended.request_status == (int)RequestStatus.Closed) {
                        workflowItem.status_id = (int)WorkflowItemStatus.Finished;
                    } else {
                        workflowItem.status_id = (int)WorkflowItemStatus.Wait;
                    }
                    string strOrderers = "";
                    foreach (var orderer in requestExtended.orderers) {
                        if (strOrderers.Length > 0) {
                            strOrderers += ", ";
                        }
                        strOrderers += UserRepository.GetUserNameSurnameFirst(orderer.surname, orderer.first_name);
                    }
                    workflowItem.resp_man = strOrderers;
                    requestExtended.workflow_items.Add(workflowItem);
                }
            }
        }

        public bool GetIsRevertable(Request_Event requestEvent, int userId) {
            if (requestEvent.request_status == (int)RequestStatus.Draft) {
                return false;
            }

            if (requestEvent.requestor == userId) {
                if (requestEvent.request_status == (int)RequestStatus.WaitForApproval) {
                    var firstAppLevel = (from appDb in requestEvent.Request_Event_Approval
                                         where appDb.request_event_id == requestEvent.id
                                         && appDb.is_last_version == true
                                         && appDb.app_level_id == 0
                                         select appDb).FirstOrDefault();
                    if (firstAppLevel != null && firstAppLevel.approve_status == (int)ApproveStatus.WaitForApproval) {
                        return true;
                    }
                } else if (requestEvent.request_status == (int)RequestStatus.CanceledOrderer
                    || requestEvent.request_status == (int)RequestStatus.CanceledRequestor
                    || requestEvent.request_status == (int)RequestStatus.CanceledSystem) {
                    return true;
                }
            }

            if (requestEvent.Request_Event_Approval != null) {
                var lastProcessAppLevel = (from appDb in requestEvent.Request_Event_Approval
                                           where appDb.request_event_id == requestEvent.id
                                     && (appDb.approve_status == (int)ApproveStatus.Approved
                                     || appDb.approve_status == (int)ApproveStatus.Rejected)
                                     && appDb.is_last_version == true
                                     orderby appDb.app_level_id descending
                                     select appDb).FirstOrDefault();

                if (lastProcessAppLevel != null) {
                    int iLastProcessedLevel = lastProcessAppLevel.app_level_id;

                    var tmpAppLevel = (from appDb in requestEvent.Request_Event_Approval
                                       where appDb.request_event_id == requestEvent.id
                                         && appDb.is_last_version == true
                                         && appDb.app_level_id == iLastProcessedLevel
                                         && appDb.app_man_id == userId
                                         select appDb).FirstOrDefault();
                    if (tmpAppLevel != null) {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool GetIsApprovable(Request_Event requestEvent, int userId) {
            if (requestEvent.request_status != (int)RequestStatus.WaitForApproval) {
                return false;
            }

            int iLastAppLevel = -1;
            if (requestEvent.Request_Event_Approval != null) {
                var sortApprovals = (from appLevelDb in requestEvent.Request_Event_Approval
                                                    where appLevelDb.is_last_version == true
                                                    orderby appLevelDb.app_level_id
                                     select appLevelDb).ToList();

                for (int i = 0; i < sortApprovals.Count; i++) {
                    if (sortApprovals[i].approve_status == (int)ApproveStatus.WaitForApproval
                        || sortApprovals[i].approve_status == (int)ApproveStatus.Empty) {
                        if (sortApprovals[i].app_man_id == userId 
                            && (iLastAppLevel == -1 || sortApprovals[i].app_level_id == iLastAppLevel)) {
                            return true;
                        }
                        iLastAppLevel = sortApprovals[i].app_level_id;
                    }
                }
            }

            return false;
        }

        public bool GetIsOrderAvailable(Request_Event requestEvent, int userId) {
            if (requestEvent.request_status != (int)RequestStatus.Approved) {
                return false;
            }

            if (requestEvent.is_order_needed != true) {
                return false;
            }

            if (requestEvent.orderer_id != userId) {
                return false;
            }

            return true;
        }

        public bool GetIsCancelable(Request_Event requestEvent, int userId) {
            if (requestEvent.request_status == (int)RequestStatus.Closed 
                || requestEvent.request_status == (int)RequestStatus.CanceledOrderer
                || requestEvent.request_status == (int)RequestStatus.CanceledRequestor
                || requestEvent.request_status == (int)RequestStatus.CanceledSystem) {
                return false;
            }

            if (requestEvent.requestor == userId) {
                return true;
            }

            return false;
        }

        public bool GetIsDeletable(Request_Event requestEvent, int userId) {
           
            if (requestEvent.requestor == userId) {
                if (requestEvent.request_status == (int)RequestStatus.New
                    || requestEvent.request_status == (int)RequestStatus.Draft
                    || requestEvent.request_status == (int)RequestStatus.WaitForApproval
                    || requestEvent.request_status == (int)RequestStatus.WaitForComment
                    || requestEvent.request_status == (int)RequestStatus.Approved) {
                    return true;
                }
            }

            return false;
        }

        public bool IsRequestorAuthorized(int userId, int centreId) {
            var centre = (from centreDb in m_dbContext.Centre
                          where centreDb.id == centreId
                          select centreDb).FirstOrDefault();

            var part = (from partDb in centre.Participants_Centre
                        where partDb.id == userId
                        select new { id = partDb.id }).FirstOrDefault();

            if (part == null) {
                var cpart = (from partDb in centre.Requestor_Centre
                             where partDb.id == userId
                             select new { id = partDb.id }).FirstOrDefault();

                if (cpart == null) {
                    return false;
                }
            }

            return true;
        }

        public List<RequestEventExtended> GetRequestData(
            List<int> companyIds,
            string filter,
            string sort,
            int pageSize,
            int pageNr,
            string rootUrl,
            string strCultureCode,
            int userId,
            out int rowsCount) {

            string strFilterWhere = GetFilter(filter);
            string strOrder = GetOrder(sort);

            string sqlPureBody = GetPureBody(companyIds, strFilterWhere, strCultureCode, userId);

            string sqlPure = "SELECT "
                + "red." + RequestEventData.ID_FIELD
                + ",red." + RequestEventData.REQUEST_NR_FIELD
                + ",red." + RequestEventData.REQUESTOR_FIELD
                + ",red." + RequestEventData.ISSUED_FIELD
                + ",red." + RequestEventData.REQUEST_STATUS_FIELD
                + ",pdRequestor." + ParticipantsData.SURNAME_FIELD + "+' '+pdRequestor." + ParticipantsData.FIRST_NAME_FIELD + " AS requestor_name_surname_first"
                + ",centre." + CentreData.NAME_FIELD + " AS centre_name"
                + ",pgd." + PurchaseGroupData.GROUP_NAME_FIELD + " AS pg_name_en"
                + ",pgld." + PurchaseGroupLocalData.LOCAL_TEXT_FIELD + " AS pg_name"
                + ",ROW_NUMBER() OVER(" + strOrder + ") AS RowNum";

            //Get Row count
            string selectCount = "SELECT COUNT(red.id) " + sqlPureBody;
            rowsCount = m_dbContext.Database.SqlQuery<int>(selectCount).Single();

            //Get Part Data
            string sqlPart = sqlPure + sqlPureBody;
            int partStart = pageSize * (pageNr - 1) + 1;
            int partStop = partStart + pageSize - 1;

            while (partStart > rowsCount) {
                partStart -= pageSize;
                partStart = partStart + pageSize - 1;
            }

            string sql = "SELECT *"
                + " FROM(" + sqlPart + ") AS RegetPartData"
                + " WHERE RegetPartData.RowNum BETWEEN " + partStart + " AND " + partStop;

            var requestList = m_dbContext.Database.SqlQuery<RequestEventExtended>(sql).ToList();

            List<RequestEventExtended> retRequestList = new List<RequestEventExtended>();
            int rowIndex = (pageNr - 1) * pageSize + 1;
            foreach (var req in requestList) {

                RequestEventExtended requestExtended = new RequestEventExtended();
                SetValues(req, requestExtended);
                requestExtended.row_index = rowIndex++;
                requestExtended.hyperlink = rootUrl + "Request?id=" + req.id;
                 
                if (req.pg_name == null) {
                    requestExtended.pg_name = req.pg_name_en;
                }
                retRequestList.Add(requestExtended);
            }

            return retRequestList;
        }

        

        private string GetFilter(string filter) {
            string strFilterWhere = "";

            //strFilterWhere += " AND " + RequestEventData.LAST_EVENT_FIELD + "=1";


            if (!String.IsNullOrEmpty(filter)) {
                if (filter.Trim().ToLower() == "null") {
                    return strFilterWhere;
                }

                string[] filterItems = filter.Split(UrlParamDelimiter.ToCharArray());
                foreach (string filterItem in filterItems) {

                }
            }

            return strFilterWhere;
        }

        private string GetOrder(string sort) {

            string strOrder = "ORDER BY red." + RequestEventData.ID_FIELD + " DESC";

            if (!String.IsNullOrEmpty(sort)) {
                if (sort.Trim().ToLower() == "null") {
                    return strOrder;
                }

                strOrder = "";
                string[] sortItems = sort.Split(UrlParamDelimiter.ToCharArray());
                foreach (string sortItem in sortItems) {

                }

                strOrder = " ORDER BY " + strOrder;
            }

            return strOrder;
        }

        private string GetPureBody(List<int> companyIds, string strFilterWhere, string strCulture, int currentUserId) {
            string sqlPureBody = " FROM " + RequestEventData.TABLE_NAME + " red" +
                " LEFT OUTER JOIN " + RequestAuthorizedusersData.TABLE_NAME + " rad" +
                " ON red." + RequestEventData.ID_FIELD + "=rad." + RequestAuthorizedusersData.REQUEST_ID_FIELD +
                //" AND red." + RequestEventData.VERSION_FIELD + "=rad." + RequestAuthorizedusersData.REQUEST_VERSION_FIELD +
                " INNER JOIN " + CentreGroupData.TABLE_NAME + " cgd" +
                " ON red." + RequestEventData.CENTRE_GROUP_ID_FIELD + "=cgd." + CentreGroupData.ID_FIELD +
                " INNER JOIN " + ParticipantsData.TABLE_NAME + " pdRequestor" +
                " ON red." + RequestEventData.REQUESTOR_FIELD + "=pdRequestor." + ParticipantsData.ID_FIELD +
                " LEFT OUTER JOIN " + CentreData.TABLE_NAME + " centre" +
                " ON red." + RequestEventData.REQUEST_CENTRE_ID_FIELD + "=centre." + CentreData.ID_FIELD +
                " LEFT OUTER JOIN " + PurchaseGroupData.TABLE_NAME + " pgd" +
                " ON red." + RequestEventData.PURCHASE_GROUP_ID_FIELD + "=pgd." + PurchaseGroupData.ID_FIELD +
                " LEFT OUTER JOIN " + PurchaseGroupLocalData.TABLE_NAME + " pgld" +
                " ON red." + RequestEventData.PURCHASE_GROUP_ID_FIELD + "=pgld." + PurchaseGroupLocalData.PURCHASE_GROUP_ID_FIELD +
                " AND pgld." + PurchaseGroupLocalData.CULTURE_FIELD + "='" + strCulture + "'" +
                " WHERE red." + RequestEventData.LAST_EVENT_FIELD + "=1" +
                " AND cgd." + CentreGroupData.COMPANY_ID_FIELD + " IN" + GetSqlIn(companyIds) +
                " AND ((rad." + RequestAuthorizedusersData.USER_ID_FIELD + "=" + currentUserId +
                " AND rad." + RequestAuthorizedusersData.IS_LAST_VERSION_FIELD + "=1)" +
                " OR red." + RequestEventData.PRIVACY_ID_FIELD + "=" + ((int)Privacy.Public) +
                " OR red." + RequestEventData.PRIVACY_ID_FIELD + " IS NULL)";
            sqlPureBody += strFilterWhere;

            return sqlPureBody;
        }

        public void SaveDraft(RequestEventExtended requestUpdated, int currentUserId, out int requestId, out string requestNr) {
            SaveNewVersion(requestUpdated, currentUserId, true, out requestId, out requestNr);
        }

        private int GetRequestStatus(int? requestStatus) {
            if (requestStatus != null) {
                return (int)requestStatus;
            } 

            return (int)RequestStatus.Unknown;
        }

        public int Save(RequestEventExtended requestUpdated, int currentUserId) {
            //int requestId;
            //string requestNr;
            SaveNewVersion(requestUpdated, currentUserId, false, out int requestId, out string requestNr);

            return requestId;
        }

        private void SaveNewVersion(
            RequestEventExtended requestUpdated,
            int currentUserId, 
            bool isDraft,
            //int iOrigRequestStatus,
            out int requestId,
            out string requestNr) {

            bool isNewOrDraft = isDraft;
            bool isNew = false;
            bool isAuthUSersNeedToBeUpdated = false;
            using (TransactionScope transaction = new TransactionScope()) {
                try {
                    Request_Event requestEventLastVersion = null;

                    int lastVersion = -1;
                    if (requestUpdated.id > -1) {
                        requestEventLastVersion = (from requestDb in m_dbContext.Request_Event
                                                   where requestDb.id == requestUpdated.id
                                                   && requestDb.last_event == true
                                                   select requestDb).FirstOrDefault();
                        lastVersion = requestEventLastVersion.version;

                    }

                    Request_Event newRequestEvent = new Request_Event();

                    if (requestUpdated.use_supplier_list) {
                        requestUpdated.supplier_remark = null;
                    } else {
                        requestUpdated.supplier_id = null;
                    }

                    if (requestUpdated.id >= 0) {
                        SetValues(requestEventLastVersion, newRequestEvent);
                        if (requestEventLastVersion.privacy_id != requestUpdated.privacy_id) {
                            isAuthUSersNeedToBeUpdated = true;
                        }
                        SetValues(requestUpdated, newRequestEvent);
                        //if (iOrigRequestStatus == (int)RequestStatus.Draft) {
                        //    isNewOrDraft = true;
                        //}
                    } else {
                        //New 
                        isNew = true;

                        SetValues(requestUpdated, newRequestEvent);
                        int lastId = GetLastId();
                        int newId = lastId + 1;
                        newRequestEvent.id = newId;

                        if (isDraft) {
                            int newIdentificator = new RequestIdentificatorRepository().GetRequestIdentificator(-1);
                            newRequestEvent.request_nr = "Draft" + DateTime.Now.Year + newIdentificator;
                        } else {
                            int iCentreId = (int)newRequestEvent.request_centre_id;
                            int newIdentificator = new RequestIdentificatorRepository().GetRequestIdentificator(iCentreId);
                            string centreName = new CentreRepository().GetCentreNameById(iCentreId);
                            string strNr = newIdentificator.ToString();
                            while (strNr.Length < 4) {
                                strNr = "0" + strNr;
                            }
                            newRequestEvent.request_nr = centreName + DateTime.Now.Year.ToString().Substring(2, 2) + strNr;
                        }
                        isNewOrDraft = true;
                        newRequestEvent.issued = DateTime.Now;
                    }

                    if (newRequestEvent.request_status == null) {
                        newRequestEvent.request_status = (int)RequestStatus.Draft;
                    }

                    if (requestEventLastVersion != null) {
                        requestEventLastVersion.last_event = false;
                    }


                    if (requestUpdated.request_centre_id != null && requestUpdated.request_centre_id > -1) {
                        var centre = (from cDb in m_dbContext.Centre
                                      where cDb.id == requestUpdated.request_centre_id
                                      select cDb).FirstOrDefault();
                        int cgId = centre.Centre_Group.ElementAt(0).id;
                        newRequestEvent.centre_group_id = cgId;
                        newRequestEvent.country_id = centre.Centre_Group.ElementAt(0).company_id;
                    }


                    int newVersion = lastVersion + 1;
                    newRequestEvent.version = newVersion;

                    int? appTextId = null;
                    
                    //if (isNewOrDraft) {
                    if(currentUserId == requestUpdated.requestor) { 

                        appTextId = UpdateRequestText(
                            newRequestEvent.id,
                            newRequestEvent.version,
                            (int)newRequestEvent.country_id,
                            requestUpdated.request_text);

                        int? remarkAppTextId = UpdateSupplierText(
                            newRequestEvent,
                            //newRequestEvent.id,
                            //newRequestEvent.version,
                            //(int)newRequestEvent.country_id,
                            requestUpdated.supplier_remark,
                            appTextId);
                        if (remarkAppTextId != null) {
                            appTextId = remarkAppTextId;
                        }

                        UpdateCountry(
                           requestUpdated,
                           requestEventLastVersion,
                           newRequestEvent);

                        if (isDraft) {
                            remarkAppTextId = UpdateRequestRemark(
                                newRequestEvent.id,
                                (int)newRequestEvent.country_id,
                                newRequestEvent.version,
                                requestUpdated.remarks,
                                appTextId);
                            if (remarkAppTextId != null) {
                                appTextId = remarkAppTextId;
                            }
                        }
                        
                        isAuthUSersNeedToBeUpdated = true;
                    }

                    UpdateAppMen(
                            requestUpdated,
                            requestEventLastVersion,
                            newRequestEvent);

                    UpdateOrderer(
                       requestUpdated,
                       requestEventLastVersion,
                       newRequestEvent);

                    if (isAuthUSersNeedToBeUpdated) {
                        UpdateAuthorizedUsers(requestEventLastVersion, newRequestEvent, isNewOrDraft);
                    }

                    newRequestEvent.last_event = true;
                    newRequestEvent.modify_user = currentUserId;
                    newRequestEvent.modify_date = DateTime.Now;

                    if (isNewOrDraft) {
                        UpdateCustomFields(newRequestEvent, requestUpdated.custom_fields);
                    }
                    UpdateAttachments(newRequestEvent, requestUpdated.attachments, isNew);
                     
                    m_dbContext.Request_Event.Add(newRequestEvent);

                    SaveChanges();

                    if (!isDraft) {
                        //move remark to discussion
                        if (!String.IsNullOrEmpty(requestUpdated.remarks)) {
                            
                            App_Text_Store appText = new App_Text_Store();
                            int newTextId = DataNulls.INT_NULL;
                            if (appTextId >= 0) {
                                newTextId = (int)appTextId + 1;
                            } else {
                                newTextId = new AppTextStoreRepository().GetLastId();
                                newTextId++;
                            }
                            appText.id = newTextId;
                            appText.company_id = (int)newRequestEvent.country_id;
                            appText.text_content = requestUpdated.remarks;
                            appText.text_type = (int)TextType.RequestDisc;
                            m_dbContext.App_Text_Store.Add(appText);


                            int lastId = new DiscussionRepository().GetLastId();
                            int newId = ++lastId;

                            Discussion disc = new Discussion();
                            disc.id = newId;
                            disc.app_text_store_id = newTextId;
                            disc.author_id = currentUserId;
                            disc.modif_date = DateTime.Now;
                           
                            m_dbContext.Discussion.Add(disc);
                           
                            Request_Discussion requestDiscussion = new Request_Discussion();
                            requestDiscussion.request_id = newRequestEvent.id;
                            requestDiscussion.request_version = newRequestEvent.version;
                            requestDiscussion.discussion_id = disc.id;
                            requestDiscussion.is_active = true;
                            newRequestEvent.Request_Discussion.Add(requestDiscussion);
                            
                        }

                        new RequestTextRepository().DeactivateRequestRemark(newRequestEvent.id, m_dbContext);
                        SaveChanges();
                    }

                    transaction.Complete();

                    requestId = newRequestEvent.id;
                    requestNr = newRequestEvent.request_nr;

                } catch (Exception ex) {
                    throw ex;
                }
            }

        }

        private void UpdateCountry(
            RequestEventExtended requestUpdated,
            Request_Event requestEventLastVersion,
            Request_Event newRequestEvent) {

            if (requestUpdated.request_centre_id == null) {
                newRequestEvent.country_id = null;
                return;
            }

            bool isUpdated = false;
            if (requestUpdated.id < 0) {
                isUpdated = true;
            } else if (requestEventLastVersion == null) {
                isUpdated = true;
            } else if (requestEventLastVersion.request_centre_id != requestUpdated.request_centre_id) {
                isUpdated = true;
            }

            if (isUpdated) {
                var centre = new CentreRepository().GetCentreById((int)requestUpdated.request_centre_id);
                var cg = centre.Centre_Group.ElementAt(0);
                newRequestEvent.country_id = cg.company_id;
            }
        }

        private void UpdateAppMen(
            RequestEventExtended requestUpdated,
            Request_Event requestEventLastVersion,
            Request_Event newRequestEvent) {

            #region Temporary Code
            //****************************************************************************
            //kamsyk will be removed
            newRequestEvent.manager1 = null;
            newRequestEvent.manager2 = null;
            newRequestEvent.manager3 = null;
            newRequestEvent.manager4 = null;
            newRequestEvent.manager5 = null;
            newRequestEvent.manager6 = null;
            newRequestEvent.approved_1 = (int)ApproveStatus.Empty;
            newRequestEvent.approved_2 = (int)ApproveStatus.Empty;
            newRequestEvent.approved_3 = (int)ApproveStatus.Empty;
            newRequestEvent.approved_4 = (int)ApproveStatus.Empty;
            newRequestEvent.approved_5 = (int)ApproveStatus.Empty;
            newRequestEvent.approved_6 = (int)ApproveStatus.Empty;

            if (requestUpdated.request_event_approval != null) {
                foreach (var app in requestUpdated.request_event_approval) {
                    switch (app.app_level_id) {
                        case 0:
                            newRequestEvent.manager1 = app.app_man_id;
                            newRequestEvent.approved_1 = app.approve_status;
                            break;
                        case 1:
                            newRequestEvent.manager2 = app.app_man_id;
                            newRequestEvent.approved_2 = app.approve_status;
                            break;
                        case 2:
                            newRequestEvent.manager3 = app.app_man_id;
                            newRequestEvent.approved_3 = app.approve_status;
                            break;
                        case 3:
                            newRequestEvent.manager4 = app.app_man_id;
                            newRequestEvent.approved_4 = app.approve_status;
                            break;
                        case 4:
                            newRequestEvent.manager5 = app.app_man_id;
                            newRequestEvent.approved_5 = app.approve_status;
                            break;
                        case 5:
                            newRequestEvent.manager6 = app.app_man_id;
                            newRequestEvent.approved_6 = app.approve_status;
                            break;
                    }
                }
            }
            //**************************************************************************************
            #endregion

            //Compare
            bool isPrevVersionTheSame = true;
            if ((requestEventLastVersion == null || requestEventLastVersion.Request_Event_Approval == null)
                && requestUpdated.request_event_approval != null
                && requestUpdated.request_event_approval.Count > 0) {
                isPrevVersionTheSame = false;
            }

            if (isPrevVersionTheSame) {
                if (requestUpdated.request_event_approval == null
                && requestEventLastVersion.Request_Event_Approval != null
                && requestEventLastVersion.Request_Event_Approval.Count > 0) {
                    isPrevVersionTheSame = false;
                }
            }

            if (isPrevVersionTheSame) {
                if (requestEventLastVersion.Request_Event_Approval != null && requestUpdated.request_event_approval != null) {
                    if (requestEventLastVersion.Request_Event_Approval.Count != requestUpdated.request_event_approval.Count) {
                        isPrevVersionTheSame = false;
                    }
                }
            }

            if (isPrevVersionTheSame) {
                foreach (var reqAppDb in requestEventLastVersion.Request_Event_Approval) {
                    var reqAppUpd = (from requestUpdatedDb in requestUpdated.request_event_approval
                                     where requestUpdatedDb.app_man_id == reqAppDb.app_man_id
                                     && requestUpdatedDb.app_level_id == reqAppDb.app_level_id
                                     select requestUpdatedDb).FirstOrDefault();
                    if (reqAppUpd != null) {
                        isPrevVersionTheSame = false;
                        break;
                    }
                }
            }

            if (!isPrevVersionTheSame) {
                if (requestEventLastVersion != null && requestEventLastVersion.Request_Event_Approval != null) {
                    foreach (var reqAppDb in requestEventLastVersion.Request_Event_Approval) {
                        reqAppDb.is_last_version = false;
                    }
                }

                if (requestUpdated.request_event_approval != null) {
                    newRequestEvent.Request_Event_Approval = new List<Request_Event_Approval>();
                    int lastId = new RequestEventApprovalRepository().GetLastId();
                    int newId = ++lastId;
                    foreach (var reqAppUpd in requestUpdated.request_event_approval) {

                        Request_Event_Approval rep = new Request_Event_Approval();
                        rep.id = newId;
                        rep.app_man_id = reqAppUpd.app_man_id;
                        rep.request_event_id = newRequestEvent.id;
                        rep.request_event_version = newRequestEvent.version;
                        rep.app_level_id = reqAppUpd.app_level_id;
                        rep.approve_status = reqAppUpd.approve_status;
                        rep.modif_date = DateTime.Now;
                        rep.is_last_version = true;
                        newRequestEvent.Request_Event_Approval.Add(rep);
                        newId++;

                    }
                }
            }

           
        }

        private void UpdateOrderer(
            RequestEventExtended requestUpdated,
            Request_Event requestEventLastVersion,
            Request_Event newRequestEvent) {

            //kamsyk will be removed
            newRequestEvent.orderer_id = null;

            if (requestUpdated.orderers != null && requestUpdated.orderers.Count == 1) {
                newRequestEvent.orderer_id = requestUpdated.orderers.ElementAt(0).participant_id;
            }

            //Compare
            bool isPrevVersionTheSame = true;
            if ((requestEventLastVersion == null || requestEventLastVersion.Request_Event_Orderer == null)
                && requestUpdated.orderers != null
                && requestUpdated.orderers.Count > 0) {
                isPrevVersionTheSame = false;
            }

            if (isPrevVersionTheSame) {
                if (requestUpdated.orderers == null
                && (requestEventLastVersion != null && (requestEventLastVersion.Request_Event_Orderer != null
                && requestEventLastVersion.Request_Event_Orderer.Count > 0))) {
                    isPrevVersionTheSame = false;
                }
            }

            if (isPrevVersionTheSame) {
                if (requestEventLastVersion != null && requestEventLastVersion.Request_Event_Orderer != null
                    && requestUpdated.orderers != null) {
                    if (requestEventLastVersion.Request_Event_Orderer.Count != requestUpdated.orderers.Count) {
                        isPrevVersionTheSame = false;
                    }
                }
            }

            if (isPrevVersionTheSame) {
                if (requestEventLastVersion != null) {
                    foreach (var reqOrdDb in requestEventLastVersion.Request_Event_Orderer) {
                        var reqOrdUpd = (from requestUpdatedDb in requestUpdated.orderers
                                         where requestUpdatedDb.participant_id == reqOrdDb.orderer_id
                                         select requestUpdatedDb).FirstOrDefault();
                        if (reqOrdUpd != null) {
                            isPrevVersionTheSame = false;
                            break;
                        }
                    }
                }
            }

            if (!isPrevVersionTheSame) {
                if (requestEventLastVersion != null && requestEventLastVersion.Request_Event_Orderer != null) {
                    foreach (var reqAppDb in requestEventLastVersion.Request_Event_Orderer) {
                        reqAppDb.is_last_version = false;
                    }
                }

                if (requestUpdated.request_event_approval != null) {
                    newRequestEvent.Request_Event_Orderer = new List<Request_Event_Orderer>();
                    foreach (var reqOrd in requestUpdated.orderers) {
                        Request_Event_Orderer reo = new Request_Event_Orderer();
                        reo.orderer_id = reqOrd.participant_id;
                        reo.request_event_id = newRequestEvent.id;
                        reo.request_event_version = newRequestEvent.version;
                        reo.is_last_version = true;
                        newRequestEvent.Request_Event_Orderer.Add(reo);
                    }
                }
            }
        }

        private void UpdateAuthorizedUsers(
            Request_Event requestEventLastVersion, 
            Request_Event newRequestEvent,
            bool isNewOrDraft) {
            if (newRequestEvent.privacy_id == (int)Privacy.Public) {
                return;
            }

            RequestAuthUserRepository requestAuthUserRepository = new RequestAuthUserRepository();
            var authUsers = requestAuthUserRepository.GetRequestAuthUsersByReqId(newRequestEvent.id);
            Hashtable htAuthUser = new Hashtable();
            if (newRequestEvent.privacy_id == (int)Privacy.Centre) {
                List<Participants> centerParts = (from partDb in m_dbContext.Participants
                                                  where partDb.centre_id == newRequestEvent.request_centre_id
                                                  && partDb.active == true
                                                  select partDb).ToList();
                if (centerParts != null) {
                    foreach (var part in centerParts) {
                        if (!htAuthUser.ContainsKey(part.id)) {
                            htAuthUser.Add(part.id, null);
                        }
                    }
                }

                string sql = "SELECT pd.* FROM " + ParticipantsData.TABLE_NAME + " pd"
                    + " INNER JOIN " + RequestorCentreData.TABLE_NAME + " rcd"
                    + " ON pd." + ParticipantsData.ID_FIELD + "=rcd." + RequestorCentreData.PARTICIPANT_ID_FIELD
                    + " WHERE rcd." + RequestorCentreData.CENTRE_ID_FIELD + "=" + newRequestEvent.request_centre_id
                    + " AND pd." + ParticipantsData.ACTIVE_FIELD + "=1";
                var requestorCentres = m_dbContext.Database.SqlQuery<Participants>(sql).ToList();
                if (requestorCentres != null) {
                    foreach (var part in requestorCentres) {
                        if (!htAuthUser.ContainsKey(part.id)) {
                            htAuthUser.Add(part.id, null);
                        }
                    }
                }
            }

            if (newRequestEvent.privacy_id == (int)Privacy.Centre || newRequestEvent.privacy_id == (int)Privacy.Private) {
                if (!htAuthUser.ContainsKey(newRequestEvent.requestor)) {
                    htAuthUser.Add(newRequestEvent.requestor, null);
                }

                if (isNewOrDraft) {
                    if (newRequestEvent.Request_Event_Approval != null) {
                        foreach (var reqApp in newRequestEvent.Request_Event_Approval) {
                            if (!htAuthUser.ContainsKey(reqApp.app_man_id)) {
                                htAuthUser.Add(reqApp.app_man_id, null);
                            }
                        }
                    }
                } else {
                    var reqAppMen = (from reqAppDb in m_dbContext.Request_Event_Approval
                                     where reqAppDb.request_event_id == requestEventLastVersion.id
                                     && reqAppDb.is_last_version == true
                                     select reqAppDb).ToList();
                    foreach (var reqAppMan in reqAppMen) {
                        if (!htAuthUser.ContainsKey(reqAppMan.app_man_id)) {
                            htAuthUser.Add(reqAppMan.app_man_id, null);
                        }
                    }
                }

                if (isNewOrDraft) {
                    if (newRequestEvent.Request_Event_Orderer != null) {
                        foreach (var reqOrd in newRequestEvent.Request_Event_Orderer) {
                            if (!htAuthUser.ContainsKey(reqOrd.orderer_id)) {
                                htAuthUser.Add(reqOrd.orderer_id, null);
                            }
                        }
                    }
                } else {
                    var reqAppOrderers = (from reqOrdDb in m_dbContext.Request_Event_Orderer
                                     where reqOrdDb.request_event_id == requestEventLastVersion.id
                                     && reqOrdDb.is_last_version == true
                                     select reqOrdDb).ToList();
                    foreach (var reqAppOrderer in reqAppOrderers) {
                        if (!htAuthUser.ContainsKey(reqAppOrderer.orderer_id)) {
                            htAuthUser.Add(reqAppOrderer.orderer_id, null);
                        }
                    }
                }
            }

            //Delete
            List<int> lstAuthDb = new List<int>();
            if (requestEventLastVersion != null) {
                var lastAuths = (from lastAuthDb in m_dbContext.Request_AuthorizedUsers
                                 where lastAuthDb.request_id == requestEventLastVersion.id
                                 && lastAuthDb.is_last_version == true
                                 select lastAuthDb).ToList();
                foreach (var authUser in lastAuths) {
                    if (!htAuthUser.ContainsKey(authUser.user_id)) {
                        authUser.is_last_version = false;
                    }
                    if (!lstAuthDb.Contains(authUser.user_id)) {
                        lstAuthDb.Add(authUser.user_id);
                    }
                }
            }

            //Add New
            IDictionaryEnumerator iEnum = htAuthUser.GetEnumerator();
            while (iEnum.MoveNext()) {
                int iUserId = (int)iEnum.Key;
                if (!lstAuthDb.Contains(iUserId)) {
                    Request_AuthorizedUsers newRau = new Request_AuthorizedUsers();
                    newRau.request_id = newRequestEvent.id;
                    newRau.request_version = newRequestEvent.version;
                    newRau.user_id = iUserId;
                    newRau.is_last_version = true;

                    m_dbContext.Request_AuthorizedUsers.Add(newRau);
                }
            }


            //bool isTheSame = true;
            //if (requestEventLastVersion != null && requestEventLastVersion.Request_AuthorizedUsers != null) {
            //    if (htAuthUser.Count != requestEventLastVersion.Request_AuthorizedUsers.Count) {
            //        isTheSame = false;
            //    }

            //    if (isTheSame) {
            //        IDictionaryEnumerator iEnum = htAuthUser.GetEnumerator();
            //        while (iEnum.MoveNext()) {
            //            int iAuthUser = (int)iEnum.Key;
            //            var prevAuthUser = (from prevAuthDb in requestEventLastVersion.Request_AuthorizedUsers
            //                                where prevAuthDb.user_id == iAuthUser
            //                                select prevAuthDb).FirstOrDefault();
            //            if (prevAuthUser == null) {
            //                isTheSame = false;
            //                break;
            //            }
            //        }
            //    }

            //    if (isTheSame) {
            //        foreach (var authUser in requestEventLastVersion.Request_AuthorizedUsers) {
            //            m_dbContext.Request_AuthorizedUsers.Remove(authUser);
            //        }
            //    }
            //}

            ////Add Auth users
            //IDictionaryEnumerator iEnumN = htAuthUser.GetEnumerator();
            //while (iEnumN.MoveNext()) {
            //    int iAuthUser = (int)iEnumN.Key;

            //    Request_AuthorizedUsers newRau = new Request_AuthorizedUsers();
            //    newRau.request_id = newRequestEvent.id;
            //    newRau.request_version = newRequestEvent.version;
            //    newRau.user_id = iAuthUser;

            //    m_dbContext.Request_AuthorizedUsers.Add(newRau);
            //}
        }


        private int UpdateRequestText(
            int requestId,
            int requestVersion,
            int companyId,
            string requestText) {

            int iAppTextId = DataNulls.INT_NULL;

            //request text
            var requestTextDb = new RequestTextRepository().GetLastVersion(requestId, AppTextStoreRepository.TextType.RequestText, m_dbContext);

            if (String.IsNullOrEmpty(requestText)) {
                if (requestTextDb != null) {
                    requestTextDb.is_last_version = false;
                }
            } else {
                if (requestTextDb != null) {
                    if (requestTextDb.App_Text_Store.text_content != requestText) {
                        App_Text_Store appText = new App_Text_Store();
                        int newId = new AppTextStoreRepository().GetLastId();
                        newId++;
                        appText.id = newId;
                        appText.text_content = requestText;
                        appText.company_id = companyId;
                        appText.text_type = (int)AppTextStoreRepository.TextType.RequestText;
                        m_dbContext.App_Text_Store.Add(appText);

                        Request_Text rt = new Request_Text();
                        rt.request_id = requestId;
                        rt.request_version = requestVersion;
                        rt.app_text_store_id = appText.id;
                        rt.modify_date = DateTime.Now;
                        rt.is_last_version = true;

                        m_dbContext.Request_Text.Add(rt);

                        requestTextDb.is_last_version = false;
                        iAppTextId = newId;
                    }
                } else {
                    App_Text_Store appText = new App_Text_Store();
                    int newId = new AppTextStoreRepository().GetLastId();
                    newId++;
                    appText.id = newId;
                    appText.text_content = requestText;
                    appText.text_type = (int)AppTextStoreRepository.TextType.RequestText;
                    m_dbContext.App_Text_Store.Add(appText);

                    Request_Text rt = new Request_Text();
                    rt.request_id = requestId;
                    rt.request_version = requestVersion;
                    rt.app_text_store_id = appText.id;
                    rt.modify_date = DateTime.Now;
                    rt.is_last_version = true;

                    m_dbContext.Request_Text.Add(rt);

                    iAppTextId = newId;
                }
            }

            return iAppTextId;
        }

        private int? UpdateRequestRemark(
             int requestId,
             int requestVersion,
             int companyId,
             string remarkText,
             int? appTextId) {

            int? addAppTextId = null;

            //request text
            var requestRemarkDb = new RequestTextRepository().GetLastVersion(requestId, AppTextStoreRepository.TextType.RequestRemark, m_dbContext);

            if (String.IsNullOrEmpty(remarkText)) {
                if (requestRemarkDb != null) {
                    requestRemarkDb.is_last_version = false;
                }
            } else {
                if (requestRemarkDb != null) {
                    if (requestRemarkDb.App_Text_Store.text_content != remarkText) {
                        App_Text_Store appText = new App_Text_Store();
                        int newId = new AppTextStoreRepository().GetLastId();
                        newId++;
                        appText.id = newId;
                        appText.text_content = remarkText;
                        appText.text_type = (int)AppTextStoreRepository.TextType.RequestRemark;
                        m_dbContext.App_Text_Store.Add(appText);

                        Request_Text rt = new Request_Text();
                        rt.request_id = requestId;
                        rt.request_version = requestVersion;
                        rt.app_text_store_id = appText.id;
                        rt.modify_date = DateTime.Now;
                        rt.is_last_version = true;

                        m_dbContext.Request_Text.Add(rt);

                        requestRemarkDb.is_last_version = false;
                    }
                } else {
                    App_Text_Store appText = new App_Text_Store();
                    int newId = -1;
                    if (appTextId != null) {
                        newId = (int)appTextId + 1;
                    } else {
                        newId = new AppTextStoreRepository().GetLastId();
                        newId++;
                    }
                    addAppTextId = newId;
                    appText.id = newId;
                    appText.company_id = companyId;
                    appText.text_content = remarkText;
                    appText.text_type = (int)AppTextStoreRepository.TextType.RequestRemark;
                    m_dbContext.App_Text_Store.Add(appText);

                    Request_Text rt = new Request_Text();
                    rt.request_id = requestId;
                    rt.request_version = requestVersion;
                    rt.app_text_store_id = appText.id;
                    rt.modify_date = DateTime.Now;
                    rt.is_last_version = true;

                    m_dbContext.Request_Text.Add(rt);
                }
            }

            return addAppTextId;
        }

        private int? UpdateSupplierText(
            Request_Event newRequestVersion,
             //int requestId,
             //int requestVersion,
             //int companyId,
             string supplierText,
             int? appTextId) {

            int? addAppTextId = null;

            //request text
            var requestRemarkDb = new RequestTextRepository().GetLastVersion(
                newRequestVersion.id, 
                AppTextStoreRepository.TextType.RequestSupplier,
                m_dbContext);

            if (String.IsNullOrEmpty(supplierText)) {
                if (requestRemarkDb != null) {
                    requestRemarkDb.is_last_version = false;
                }
            } else {
                if (requestRemarkDb != null) {
                    if (requestRemarkDb.App_Text_Store.text_content != supplierText) {
                        App_Text_Store appText = new App_Text_Store();
                        int newId = new AppTextStoreRepository().GetLastId();
                        newId++;
                        appText.id = newId;
                        appText.text_content = supplierText;
                        appText.text_type = (int)AppTextStoreRepository.TextType.RequestSupplier;
                        m_dbContext.App_Text_Store.Add(appText);

                        Request_Text rt = new Request_Text();
                        rt.request_id = newRequestVersion.id;
                        rt.request_version = newRequestVersion.version;
                        rt.app_text_store_id = appText.id;
                        rt.modify_date = DateTime.Now;
                        rt.is_last_version = true;
                        m_dbContext.Request_Text.Add(rt);

                        requestRemarkDb.is_last_version = false;
                    }
                } else {
                    App_Text_Store appText = new App_Text_Store();
                    int newId = -1;
                    if (appTextId != null) {
                        newId = (int)appTextId + 1;
                    } else {
                        newId = new AppTextStoreRepository().GetLastId();
                        newId++;
                    }
                    addAppTextId = newId;
                    appText.id = newId;
                    appText.company_id = (int)newRequestVersion.country_id;
                    appText.text_content = supplierText;
                    appText.text_type = (int)AppTextStoreRepository.TextType.RequestSupplier;
                    m_dbContext.App_Text_Store.Add(appText);

                    Request_Text rt = new Request_Text();
                    rt.request_id = newRequestVersion.id;
                    rt.request_version = newRequestVersion.version;
                    rt.app_text_store_id = appText.id;
                    rt.modify_date = DateTime.Now;
                    rt.is_last_version = true;
                                        
                    m_dbContext.Request_Text.Add(rt);
                }
            }

                        
            return addAppTextId;
        }

        //private void UpdateShipToAddress(
        //    RequestExtended requestUpdated,
        //    Request_Event newRequestEvent) {

        //}

        private void UpdateCustomFields(Request_Event requestUpdated, ICollection<CustomFieldExtend> customFields) {
            

            //Add, Update
            if (customFields != null) {
                foreach (var custField in customFields) {
                    //var reqCustFiled = (from custFDb in requestUpdated.RequestEvent_CustomFieldValue
                    //                    where custFDb.custom_field_id == custField.id
                    //                    && custFDb.is_active == true
                    //                    select custFDb).FirstOrDefault();

                    var reqCustFiled = (from custFDb in m_dbContext.RequestEvent_CustomFieldValue
                                        where custFDb.request_event_id == requestUpdated.id
                                        && custFDb.custom_field_id == custField.id
                                        && custFDb.is_active == true
                                        select custFDb).FirstOrDefault();

                    if (reqCustFiled == null) {
                        RequestEvent_CustomFieldValue newReqCustField = new RequestEvent_CustomFieldValue();
                        newReqCustField.request_event_id = requestUpdated.id;
                        newReqCustField.request_event_version = requestUpdated.version;
                        newReqCustField.custom_field_id = custField.id;
                        newReqCustField.string_value = custField.string_value;
                        newReqCustField.is_active = true;
                        m_dbContext.RequestEvent_CustomFieldValue.Add(newReqCustField);
                    } else {
                        bool isChanged = false;
                        if (reqCustFiled.string_value != custField.string_value) {
                            isChanged = true;
                            
                        }
                        if (isChanged) {
                            reqCustFiled.is_active = false;

                            RequestEvent_CustomFieldValue newReqCustField = new RequestEvent_CustomFieldValue();
                            newReqCustField.request_event_id = requestUpdated.id;
                            newReqCustField.request_event_version = requestUpdated.version;
                            newReqCustField.custom_field_id = custField.id;
                            newReqCustField.string_value = custField.string_value;
                            newReqCustField.is_active = true;
                            m_dbContext.RequestEvent_CustomFieldValue.Add(newReqCustField);
                            
                            //SaveChanges();
                        }
                    }
                }
            }

            //Delete Custom Fields
            var reqCustFields = (from custFDb in m_dbContext.RequestEvent_CustomFieldValue
                                where custFDb.request_event_id == requestUpdated.id
                                && custFDb.is_active == true
                                select custFDb).ToList();
            if (reqCustFields != null) {
                foreach (var reqCustField in reqCustFields) {
                    int cfId = reqCustField.custom_field_id;
                    var updCustFields = (from updCustomFields in customFields
                                         where updCustomFields.id == cfId
                                         select updCustomFields).FirstOrDefault();
                    if (updCustFields == null) {
                        //Delete
                        m_dbContext.RequestEvent_CustomFieldValue.Remove(reqCustField);
                    }
                }
            }
        }

        private void UpdateAttachments(Request_Event requestNewVersion, ICollection<AttachmentExtend> attsUpdated, bool isNewRequest) {
            //check whether attachment were updated
            bool isTheSame = true;

            if (isNewRequest) {
                int prevVersion = requestNewVersion.version - 1;
                var currDbAtt = (from evAttDb in m_dbContext.Event_Attachement
                                 where evAttDb.request_event_id == requestNewVersion.id
                                 && evAttDb.request_version == prevVersion
                                 select evAttDb).ToList();


                if ((currDbAtt == null || currDbAtt.Count == 0) && (attsUpdated == null || attsUpdated.Count == 0)) {
                    isTheSame = true;
                } else if ((currDbAtt == null || currDbAtt.Count == 0) && (attsUpdated != null && attsUpdated.Count > 0)) {
                    isTheSame = false;
                } else if ((currDbAtt != null && currDbAtt.Count > 0) && (attsUpdated == null || attsUpdated.Count == 0)) {
                    isTheSame = false;
                } else if (currDbAtt.Count != attsUpdated.Count) {
                    isTheSame = false;
                } else {
                    foreach (var attDb in currDbAtt) {
                        int attId = attDb.att_id;
                        var updAtt = (from attUpdList in attsUpdated
                                      where attUpdList.id == attId
                                      select attUpdList).FirstOrDefault();
                        if (updAtt == null) {
                            isTheSame = false;
                            break;
                        }
                    }
                }

                if (isTheSame) {
                    foreach (var attDb in currDbAtt) {
                        m_dbContext.Event_Attachement.Remove(attDb);
                    }
                } else {
                    //Remove - not neccessary to remove record, "deleted" att are not added to new version
                    foreach (var attDb in currDbAtt) {
                        attDb.is_last_version = false;
                    }
                }
            }

            //Add, Update
            if (attsUpdated != null) {
                foreach (var att in attsUpdated) {
                    Event_Attachement newAttVers = new Event_Attachement();
                    newAttVers.att_id = att.id;
                    newAttVers.request_event_id = requestNewVersion.id;
                    newAttVers.request_version = requestNewVersion.version;
                    newAttVers.att_id = att.id;
                    newAttVers.is_last_version = true;
                    requestNewVersion.Event_Attachement.Add(newAttVers);
                }
            }
        }

        private int GetLastId() {
            var lastReqId = (from reqDb in m_dbContext.Request_Event
                            orderby reqDb.id descending
                            select reqDb).Take(1).FirstOrDefault();

            int lastId = -1;
            if (lastReqId != null) {
                lastId = lastReqId.id;
            }

            return lastId;
        }

        public List<Request_Event> GetAllLastEvents(DateTime dateFrom, DateTime dateTo) {
            var requestEvent = (from requestDb in m_dbContext.Request_Event
                                          where requestDb.last_event == true 
                                          && requestDb.issued >= dateFrom
                                          && requestDb.issued < dateTo
                                select requestDb).ToList();


            return requestEvent;
        }

        public RequestStatus GetPreviousStatus(Request_Event requestEvent, int userId) {
            if (requestEvent.request_status == (int)RequestStatus.WaitForApproval) {
                var firstAppLevels = (from appDb in requestEvent.Request_Event_Approval
                                      where appDb.request_event_id == requestEvent.id
                                     && appDb.is_last_version == true
                                     && appDb.app_level_id == 0
                                      select appDb).ToList();
                bool isWaitForFirstApproval = true;
                if (firstAppLevels != null) {
                    foreach (var firstAppLevel in firstAppLevels) {
                        if (firstAppLevel.approve_status != (int)ApproveStatus.WaitForApproval
                                && firstAppLevel.approve_status != (int)ApproveStatus.Empty) { 
                                //&& firstAppLevel.app_man_id != userId) {
                            isWaitForFirstApproval = false;
                        }
                    }
                    if (isWaitForFirstApproval) {
                        return RequestStatus.Draft;
                    }
                }
            }

            if (requestEvent.request_status == (int)RequestStatus.WaitForApproval 
                || requestEvent.request_status == (int)RequestStatus.Approved
                || requestEvent.request_status == (int)RequestStatus.Rejected) {    
                var appLevels = (from appDb in requestEvent.Request_Event_Approval
                                      where appDb.request_event_id == requestEvent.id
                                     && appDb.is_last_version == true
                                     select appDb).ToList();
                if (appLevels != null) {
                    foreach (var appLevel in appLevels) {
                        if (appLevel.approve_status == (int)ApproveStatus.Approved
                            || appLevel.approve_status == (int)ApproveStatus.Rejected) {
                            return RequestStatus.WaitForApproval;
                        } 
                    }
                }

            }

            if (requestEvent.request_status == (int)RequestStatus.Approved) {
                return RequestStatus.WaitForApproval;
            }

            if (requestEvent.request_status == (int)RequestStatus.Ordered) {
                return RequestStatus.Approved;
            }

            if (requestEvent.request_status == (int)RequestStatus.CanceledOrderer
                || requestEvent.request_status == (int)RequestStatus.CanceledRequestor
                || requestEvent.request_status == (int)RequestStatus.CanceledSystem) {
                var prevReqStat = (from reqDB in m_dbContext.Request_Event
                                   where reqDB.id == requestEvent.id
                                   && reqDB.version == (requestEvent.version - 1)
                                   select reqDB).FirstOrDefault();
                if (prevReqStat != null) {
                    return GetRequestStatusByStatusId(prevReqStat.request_status);
                }
            }

            if (requestEvent.request_status == (int)RequestStatus.Closed) {
                var prevReqStat = (from reqDB in m_dbContext.Request_Event
                                   where reqDB.id == requestEvent.id
                                   && reqDB.version == (requestEvent.version - 1)
                                   select reqDB).FirstOrDefault();
                if (prevReqStat != null) {
                    return GetRequestStatusByStatusId(prevReqStat.request_status);
                }
            }

            return RequestStatus.Unknown;
        }

        public RequestStatus GetRequestStatus(int requestId) {
            var requestStatus = (from requestDb in m_dbContext.Request_Event
                            where requestDb.id == requestId && requestDb.last_event == true
                            select new { request_status = requestDb.request_status }).FirstOrDefault();
            if (requestStatus.request_status == null) {
                return RequestStatus.Unknown;
            }

            int iStatus = (int)requestStatus.request_status;
            return GetRequestStatusByStatusId(iStatus);
        }

        public int GetRequestVersion(int requestId) {
            var requestVersion = (from requestDb in m_dbContext.Request_Event
                                 where requestDb.id == requestId && requestDb.last_event == true
                                 select new { version = requestDb.version }).FirstOrDefault();
            
            int iVersion = (int)requestVersion.version;
            return iVersion;


        }

        public static RequestStatus GetRequestStatusByStatusId(int? iStatus) {
            switch (iStatus) {
                case STATUS_DRAFT:
                    return RequestStatus.Draft;
                case STATUS_WAIT_FOR_APPROVAL:
                    return RequestStatus.WaitForApproval;
                case STATUS_APPROVED:
                    return RequestStatus.Approved;
                case STATUS_CANCELED_ORDERER:
                    return RequestStatus.CanceledOrderer;
                case STATUS_CANCELED_REQUESTOR:
                    return RequestStatus.CanceledRequestor;
                case STATUS_CANCELED_SYSTEM:
                    return RequestStatus.CanceledSystem;
                default:
                    return RequestStatus.Unknown;
            }
        }

        public int GetRequestRequestor(int requestId, out string requestorName) {
            requestorName = null;
            var requestRequestor = (from requestDb in m_dbContext.Request_Event
                                 where requestDb.id == requestId && requestDb.last_event == true
                                 select new { requestor = requestDb.requestor }).FirstOrDefault();
            if (requestRequestor.requestor == null) {
                return DataNulls.INT_NULL;
            }

            var requestor = new UserRepository().GetParticipantById((int)requestRequestor.requestor);
            requestorName = UserRepository.GetUserNameSurnameFirst(requestor);

            return (int)requestRequestor.requestor;
            
        }

        public void Approve(RequestEventExtended requestEventExtended, int userId) {
            var appLevels = (from appLev in requestEventExtended.request_event_approval
                             where appLev.app_man_id == userId
                             && (appLev.approve_status == (int)ApproveStatus.WaitForApproval || appLev.approve_status == (int)ApproveStatus.Empty)
                             && appLev.is_last_version == true
                             orderby appLev.app_level_id
                             select appLev).ToList();

            if (appLevels != null && appLevels.Count > 0) {
                int lastAppLevelId = appLevels.ElementAt(0).app_level_id;
                bool isExit = false;
                foreach (var appLevel in appLevels) {
                    bool isForApproval = (appLevel.app_level_id == lastAppLevelId);
                    if (appLevel.app_level_id == (lastAppLevelId + 1)) {
                        lastAppLevelId++;
                        isForApproval = true;
                    }

                    if (isForApproval) {

                        appLevel.approve_status = (int)ApproveStatus.Approved;
                        lastAppLevelId = appLevel.app_level_id;


                        var appLevelsOut = (from appLev in requestEventExtended.request_event_approval
                                            where appLev.app_man_id != userId
                                            && (appLev.approve_status == (int)ApproveStatus.WaitForApproval || appLev.approve_status == (int)ApproveStatus.Empty)
                                            && appLev.app_level_id == lastAppLevelId
                                            && appLev.is_last_version == true
                                            select appLev).FirstOrDefault();
                        if (appLevelsOut != null) {
                            //there is another app man in the same level
                            isExit = true;
                        }

                    } else {
                        break;
                    }
                    if (isExit) {
                        break;
                    }
                }
            }

            bool isApprovedCompletelly = true;
            foreach (var appLevel in requestEventExtended.request_event_approval) {
                if (appLevel.approve_status != (int)ApproveStatus.Approved) {
                    isApprovedCompletelly = false;
                    break;
                }
            }

            if (isApprovedCompletelly) {
                requestEventExtended.request_status = (int)RequestStatus.Approved;
                
            }
            SaveNewVersion(requestEventExtended, userId, false, out int requestId, out string requestNr);

           
        }

        public void Reject(RequestEventExtended requestEventExtended, int userId) {
            var appLevels = (from appLev in requestEventExtended.request_event_approval
                             where appLev.app_man_id == userId
                             && (appLev.approve_status == (int)ApproveStatus.WaitForApproval || appLev.approve_status == (int)ApproveStatus.Empty)
                             && appLev.is_last_version == true
                             orderby appLev.app_level_id
                             select appLev).ToList();

            if (appLevels != null && appLevels.Count > 0) {
                int appLevelId = appLevels.ElementAt(0).app_level_id;
                foreach (var appLevel in appLevels) {
                    if (appLevel.app_level_id == appLevelId) {
                        appLevel.approve_status = (int)ApproveStatus.Rejected;
                        
                        break;
                    }
                }
            }

            requestEventExtended.request_status = (int)RequestStatus.Rejected;
            SaveNewVersion(requestEventExtended, userId, false, out int requestId, out string requestNr);
          
        }

        //private void TempApproveStatusUpdate(Request_Event requestEvent, int appLevelId, ApproveStatus appStatus) {
        //    switch (appLevelId) {
        //        case 0:
        //            requestEvent.approved_1 = (int)appStatus;
        //            break;
        //        case 1:
        //            requestEvent.approved_2 = (int)appStatus;
        //            break;
        //        case 2:
        //            requestEvent.approved_3 = (int)appStatus;
        //            break;
        //        case 3:
        //            requestEvent.approved_4 = (int)appStatus;
        //            break;
        //        case 4:
        //            requestEvent.approved_5 = (int)appStatus;
        //            break;
        //        case 5:
        //            requestEvent.approved_6 = (int)appStatus;
        //            break;
        //    }
        //}

        //private void TempApproveStatusUpdate(RequestEventExtended requestEvent, int appLevelId, ApproveStatus appStatus) {
        //    switch (appLevelId) {
        //        case 0:
        //            requestEvent.approved_1 = (int)appStatus;
        //            break;
        //        case 1:
        //            requestEvent.approved_2 = (int)appStatus;
        //            break;
        //        case 2:
        //            requestEvent.approved_3 = (int)appStatus;
        //            break;
        //        case 3:
        //            requestEvent.approved_4 = (int)appStatus;
        //            break;
        //        case 4:
        //            requestEvent.approved_5 = (int)appStatus;
        //            break;
        //        case 5:
        //            requestEvent.approved_6 = (int)appStatus;
        //            break;
        //    }
        //}
        #endregion
    }
}
