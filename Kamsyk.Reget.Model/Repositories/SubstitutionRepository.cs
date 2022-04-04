using Kamsyk.Reget.Model.Repositories.Interfaces;
using System.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Kamsyk.Reget.Model.ExtendedModel;
using Kamsyk.Reget.Model.Common;
using Kamsyk.Reget.Model.DataDictionary;
using System.Collections;
using static Kamsyk.Reget.Model.Repositories.UserRepository;
using static Kamsyk.Reget.Model.Repositories.RequestRepository;

namespace Kamsyk.Reget.Model.Repositories {
    public class SubstitutionRepository : BaseRepository<Participant_Substitute>, ISubstitutionRepository {
        #region Constant

        #endregion

        #region Enums

        #endregion

        #region Methods
        public List<UserSubstitutionExtended> GetUserSubstitutionData(
            int userId,
            List<int> companyIds,
            string filter,
            string sort,
            int pageSize,
            int pageNr,
            int currentUserId,
            out int rowsCount,
            out bool isApprovable,
            out bool isEditable) {

            DeactivatePastSubst();
            SetDefaultFilter(userId);

            isApprovable = false;
            isEditable = false;

            string strFilterWhere = GetFilter(filter);
            string strOrder = GetOrder(sort);

            string sqlPureBody = GetPureBody(companyIds, strFilterWhere);
            
            string sqlPure = "SELECT psd.*," +
                " pdSubstituted." + ParticipantsData.SURNAME_FIELD + "+' '+pdSubstituted." + ParticipantsData.FIRST_NAME_FIELD + " AS substituted_name_surname_first," +
                " pdSubstitutee." + ParticipantsData.SURNAME_FIELD + "+' '+pdSubstitutee." + ParticipantsData.FIRST_NAME_FIELD + " AS substitutee_name_surname_first," +
                " apptext." + AppTextStoreData.TEXT_CONTENT_FIELD + " AS remark," +
                " ROW_NUMBER() OVER(" + strOrder + ") AS RowNum";

            
            //Get Row count
            string selectCount = "SELECT COUNT(psd.id) " + sqlPureBody;
            rowsCount = m_dbContext.Database.SqlQuery<int>(selectCount).Single();

            //Get Part Data
            string sqlPart = sqlPure + sqlPureBody;
            int partStart = pageSize * (pageNr - 1) + 1;
            int partStop = partStart + pageSize - 1;

            while (partStart > rowsCount) {
                partStart -= pageSize;
                partStart = partStart + pageSize - 1;
            }

            string sql = "SELECT * FROM(" + sqlPart + ") AS RegetPartData" +
                " WHERE RegetPartData.RowNum BETWEEN " + partStart + " AND " + partStop;

            var tmpSubstitutes = m_dbContext.Database.SqlQuery<UserSubstitutionExtended>(sql).ToList();

            Participants currUser = (from partDb in m_dbContext.Participants
                                     where partDb.id == currentUserId
                                     select partDb).FirstOrDefault();

            AppTextStoreRepository appTextStoreRepository = new AppTextStoreRepository();
            int rowIndex = (pageNr - 1) * pageSize + 1;
            foreach (var tmpSubstitute in tmpSubstitutes) {
                tmpSubstitute.row_index = rowIndex;
                rowIndex++;

                tmpSubstitute.is_editable_author = false;
                tmpSubstitute.is_editable_app_man = false;

                if (tmpSubstitute.substitute_end_date >= DateTime.Now) {
                    if (tmpSubstitute.modified_user == currentUserId 
                        && (tmpSubstitute.approval_status == (int)ApproveStatus.NotNeeded)
                        || tmpSubstitute.approval_status == (int)ApproveStatus.WaitForApproval) {
                        tmpSubstitute.is_editable_author = true;
                    } else {

                        var substComp = (from partDb in m_dbContext.Participants
                                         where partDb.id == tmpSubstitute.substituted_user_id
                                         select new { company_id = partDb.company_id }).FirstOrDefault();

                        var compSubstAdminRole = (from roleDb in currUser.Participant_Office_Role
                                                  where roleDb.office_id == substComp.company_id &&
                                                  roleDb.role_id == (int)UserRole.SubstitutionApproveManager
                                                  select roleDb).FirstOrDefault();
                        if (compSubstAdminRole != null) {
                            tmpSubstitute.is_editable_app_man = true;
                        }
                    }

                    int modifUserId = tmpSubstitute.modified_user;
                    var participant = (from partDb in m_dbContext.Participants
                                       where partDb.id == modifUserId
                                       select partDb).FirstOrDefault();

                    //Participants tmpAppMan = null;
                    //if (tmpSubstitute.app_man_id != null) {
                    //    tmpAppMan = (from partDb in m_dbContext.Participants
                    //                 where partDb.id == tmpSubstitute.app_man_id
                    //                 select partDb).FirstOrDefault();
                    //}

                    tmpSubstitute.modified_user_name = UserRepository.GetUserNameSurnameFirst(participant);
                    SetAppMen(tmpSubstitute);
                    //if (tmpSubstitute.approval_status == (int)RequestRepository.ApproveStatus.WaitForApproval) {
                    //    DbInteger substedManCompId = (from partDb in m_dbContext.Participants
                    //                                  where partDb.id == tmpSubstitute.substituted_user_id
                    //                                  select new DbInteger { value = partDb.company_id }).FirstOrDefault();
                    //    int compId = substedManCompId.value;
                    //    var compSustAppMen = (from substAppDb in m_dbContext.Participant_Office_Role
                    //                          join partDb in m_dbContext.Participants
                    //                          on substAppDb.participant_id equals partDb.id
                    //                          where substAppDb.role_id == (int)UserRole.SubstitutionApproveManager
                    //                          && partDb.active == true
                    //                          && substAppDb.office_id == compId
                    //                          select substAppDb).ToList();
                    //    if (compSustAppMen != null && compSustAppMen.Count > 0) {
                    //        string strApppMen = "";
                    //        foreach (var appMan in compSustAppMen) {
                    //            if (strApppMen.Length > 0) {
                    //                strApppMen += ", ";
                    //            }
                    //            strApppMen += appMan.Participants.surname + " " + appMan.Participants.first_name;
                    //        }

                    //        tmpSubstitute.approval_men = strApppMen;
                    //    }

                    //} else if (tmpSubstitute.approval_status == (int)RequestRepository.ApproveStatus.Approved 
                    //    || tmpSubstitute.approval_status == (int)RequestRepository.ApproveStatus.Rejected) {
                    //    if (tmpAppMan != null) {
                    //        tmpSubstitute.approval_men = tmpAppMan.surname + " " + tmpAppMan.first_name;
                    //    }
                    //} else {
                    //    tmpSubstitute.approval_men = null;
                    //}

                    if (tmpSubstitute.remark == null) {
                        tmpSubstitute.remark = "";
                    }

                    bool tmpIsApprovable = false;
                    bool tmpIsEditable = false;

                    SetGridCellEditMode(
                        tmpSubstitute, 
                        currUser, 
                        out tmpIsApprovable,
                        out tmpIsEditable);

                    if (tmpIsApprovable) {
                        isApprovable = true;
                    }

                    if (tmpIsEditable) {
                        isEditable = true;
                    }
                }

                
            }

            return tmpSubstitutes;
        }

        public List<Participant_Substitute> GetActiveSubstitutions() {
            var substs = (from substDb in m_dbContext.Participant_Substitute
                          where substDb.active == true
                          select substDb).ToList();

            return substs;
        }

        private void SetAppMen(UserSubstitutionExtended tmpSubstitute) {

            Participants tmpAppMan = null;
            if (tmpSubstitute.app_man_id != null) {
                tmpAppMan = (from partDb in m_dbContext.Participants
                             where partDb.id == tmpSubstitute.app_man_id
                             select partDb).FirstOrDefault();
            }

            if (tmpSubstitute.approval_status == (int)RequestRepository.ApproveStatus.WaitForApproval) {
                DbInteger substedManCompId = (from partDb in m_dbContext.Participants
                                              where partDb.id == tmpSubstitute.substituted_user_id
                                              select new DbInteger { value = partDb.company_id }).FirstOrDefault();
                int compId = substedManCompId.value;
                var compSustAppMen = (from substAppDb in m_dbContext.Participant_Office_Role
                                      join partDb in m_dbContext.Participants
                                      on substAppDb.participant_id equals partDb.id
                                      where substAppDb.role_id == (int)UserRole.SubstitutionApproveManager
                                      && partDb.active == true
                                      && substAppDb.office_id == compId
                                      select substAppDb).ToList();
                if (compSustAppMen != null && compSustAppMen.Count > 0) {
                    string strApppMen = "";
                    foreach (var appMan in compSustAppMen) {
                        if (strApppMen.Length > 0) {
                            strApppMen += ", ";
                        }
                        strApppMen += appMan.Participants.surname + " " + appMan.Participants.first_name;
                    }

                    tmpSubstitute.approval_men = strApppMen;
                }

            } else if (tmpSubstitute.approval_status == (int)RequestRepository.ApproveStatus.Approved
                || tmpSubstitute.approval_status == (int)RequestRepository.ApproveStatus.Rejected) {
                if (tmpAppMan != null) {
                    tmpSubstitute.approval_men = tmpAppMan.surname + " " + tmpAppMan.first_name;
                }
            } else {
                tmpSubstitute.approval_men = null;
            }
        }

        public void SetDefaultFilter(int userId) {
            string grdSubstDefaultFilterIndicator = "grdSubsDefaultFilterIndicator";
            string grdSubstitutionSettingKey = "grdUserSubstitution_rg";

            DataGridRepository dataGridRepository = new DataGridRepository();
            var defaultSubstFilterIndicator = dataGridRepository.GetUserGridSettings(userId, grdSubstDefaultFilterIndicator);

            if (defaultSubstFilterIndicator == null) {
                User_GridSetting user_GridSetting = new User_GridSetting();
                user_GridSetting.grid_name = grdSubstDefaultFilterIndicator;
                user_GridSetting.user_id = userId;
                user_GridSetting.grid_page_size = 10;
                m_dbContext.User_GridSetting.Add(user_GridSetting);

                user_GridSetting = new User_GridSetting();
                user_GridSetting.grid_name = grdSubstitutionSettingKey;
                user_GridSetting.user_id = userId;
                user_GridSetting.grid_page_size = 10;
                user_GridSetting.filter = "active~true";
                user_GridSetting.columns = "row_index~true|action_buttons_detail~true|action_buttons~true|action_buttons_approve~true|substituted_name_surname_first~true|substitutee_name_surname_first~true|substitute_start_date~true|substitute_end_date~true|is_allow_take_over~true|approval_status_text~true|approval_men~true|remark~true|modified_user_name~true|modified_date~true|active~true";
                m_dbContext.User_GridSetting.Add(user_GridSetting);

                SaveChanges(); 
            }
        }

        private void SetGridCellEditMode(
            UserSubstitutionExtended userSubstitutionExtended,
            Participants currUser,
            out bool isApprovable,
            out bool isEditable) {

            isApprovable = false;
            isEditable = false;

            userSubstitutionExtended.is_date_time_ro1 = true;

            userSubstitutionExtended.is_edit_hidden = true;
            userSubstitutionExtended.is_editable_author = false;
            userSubstitutionExtended.is_editable_app_man = false;
            userSubstitutionExtended.is_can_be_deleted = false;

            userSubstitutionExtended.is_approve_hidden = true;
            userSubstitutionExtended.is_reject_hidden = true;

            if (userSubstitutionExtended.active == false) {
                return;
            }


            if (currUser.id == userSubstitutionExtended.author_id
                && userSubstitutionExtended.substitute_start_date > DateTime.Now
                && userSubstitutionExtended.approval_status != (int)RequestRepository.ApproveStatus.Approved) {
                userSubstitutionExtended.is_date_time_ro1 = false;
            }

            //Approval Column
            if (userSubstitutionExtended.approval_status == (int)RequestRepository.ApproveStatus.WaitForApproval) {
                var substitutedUser = (from partDb in m_dbContext.Participants
                                       where partDb.id == userSubstitutionExtended.substituted_user_id
                                       select partDb).FirstOrDefault();
                int companyId = substitutedUser.company_id;
                var curUserSubstMan = (from currUserDb in currUser.Participant_Office_Role
                                       where currUserDb.office_id == companyId
                                       && currUserDb.role_id == (int)UserRole.SubstitutionApproveManager
                                       select currUserDb).FirstOrDefault();
                if (curUserSubstMan != null) {
                    isApprovable = true;
                    userSubstitutionExtended.is_approve_hidden = false;
                    userSubstitutionExtended.is_reject_hidden = false;
                } else {
                    userSubstitutionExtended.is_approve_hidden = true;
                    userSubstitutionExtended.is_reject_hidden = true;
                }
            } else {
                userSubstitutionExtended.is_approve_hidden = true;
                userSubstitutionExtended.is_reject_hidden = true;
            }

            //Edit Column
            if ((userSubstitutionExtended.approval_status == (int)RequestRepository.ApproveStatus.NotNeeded
                || userSubstitutionExtended.approval_status == (int)RequestRepository.ApproveStatus.WaitForApproval)
                && userSubstitutionExtended.author_id == currUser.id) {
                isEditable = true;
                userSubstitutionExtended.is_edit_hidden = false;
                userSubstitutionExtended.is_delete_hidden = false;
                userSubstitutionExtended.is_editable_author = true;
                userSubstitutionExtended.is_can_be_deleted = true;
            //} else if (userSubstitutionExtended.author_id == currUser.id && userSubstitutionExtended.active 
            //    && userSubstitutionExtended.approval_status != (int)RequestRepository.ApproveStatus.Rejected) {
            //    isEditable = true;
            //    userSubstitutionExtended.is_edit_hidden = true;
            //    userSubstitutionExtended.is_delete_hidden = false;
            //    userSubstitutionExtended.is_editable_author = true;
            //    userSubstitutionExtended.is_can_be_deleted = true;
            } else if((userSubstitutionExtended.author_id == currUser.id 
                || userSubstitutionExtended.substituted_user_id == currUser.id)
                && userSubstitutionExtended.active
                ) {

                isEditable = true;
                userSubstitutionExtended.is_edit_hidden = true;
                userSubstitutionExtended.is_delete_hidden = false;
                userSubstitutionExtended.is_editable_author = false;
                userSubstitutionExtended.is_can_be_deleted = true;
            }

            //Delete button
            userSubstitutionExtended.is_delete_hidden = true;
            if (userSubstitutionExtended.author_id == currUser.id) {
                userSubstitutionExtended.is_delete_hidden = false;
            } else {
                
                    var substitututedUser = (from partDb in m_dbContext.Participants
                                             where partDb.id == userSubstitutionExtended.substituted_user_id
                                             select partDb).FirstOrDefault();
                    int compId = substitututedUser.company_id;

                    var substManAdmin = (from officeRoleDb in currUser.Participant_Office_Role
                                         where officeRoleDb.office_id == compId
                                         && officeRoleDb.role_id == (int)UserRole.SubstituteCompanyManager
                                         select officeRoleDb).FirstOrDefault();
                    if (substManAdmin != null) {
                        isEditable = true;
                        userSubstitutionExtended.is_edit_hidden = false;
                    }
                
            }
        }

        private string GetFilter(string filter) {
            string strFilterWhere = "";
            
            //strFilterWhere += " AND ";

            //strFilterWhere += "psd." + ParticipantSubstituteData.ACTIVE_FIELD + " = 1" +
            //    " AND psd." + ParticipantSubstituteData.SUBSTITUTE_END_DATE_FIELD + ">=" + ConvertData.ToDbDate(DateTime.Now);

            if (!String.IsNullOrEmpty(filter)) {
                if (filter.Trim().ToLower() == "null") {
                    return strFilterWhere;
                }

                string[] filterItems = filter.Split(UrlParamDelimiter.ToCharArray());
                foreach (string filterItem in filterItems) {
                    string[] strItemProp = filterItem.Split(UrlParamValueDelimiter.ToCharArray());
                    strFilterWhere += " AND ";

                    string columnName = strItemProp[0].Trim().ToUpper();
                    if (columnName == "substituted_name_surname_first".Trim().ToUpper()) {
                        strFilterWhere += UserRepository.GetSqlUserNameSurnameFirstFilter("pdSubstituted", strItemProp[1]);
                        //strFilterWhere += "((pdSubstituted." + ParticipantsData.USER_SEARCH_KEY_FIELD + " LIKE '%" + strItemProp[1].Trim().Replace(" ","") + "%')" 
                        //    + " OR (pdSubstituted." + ParticipantsData.SURNAME_FIELD + "+" + "pdSubstituted." + ParticipantsData.FIRST_NAME_FIELD + " LIKE '%" + strItemProp[1].Trim().Replace(" ","") + "%'))";
                    } else if (columnName == "substitutee_name_surname_first".Trim().ToUpper()) {
                        strFilterWhere += UserRepository.GetSqlUserNameSurnameFirstFilter("pdSubstitutee", strItemProp[1]);
                    } else if (columnName == "modified_user_name".Trim().ToUpper()) {
                        strFilterWhere += UserRepository.GetSqlUserNameSurnameFirstFilter("pdModifUser", strItemProp[1]);
                    } else if (columnName == ParticipantSubstituteData.SUBSTITUTE_START_DATE_FIELD.Trim().ToUpper()) {
                        DateTime date = ConvertData.ToDate(strItemProp[1], FILTER_DATETIME_FORMAT);
                        if (date == DataNulls.DATETIME_NULL) {
                            strFilterWhere += "1=1";
                        } else {
                            strFilterWhere += "psd." + ParticipantSubstituteData.SUBSTITUTE_START_DATE_FIELD + ">=" + ConvertData.ToDbDate(date);
                        }
                    } else if (columnName == ParticipantSubstituteData.SUBSTITUTE_END_DATE_FIELD.Trim().ToUpper()) {
                        DateTime date = ConvertData.ToDate(strItemProp[1], FILTER_DATETIME_FORMAT);
                        if (date == DataNulls.DATETIME_NULL) {
                            strFilterWhere += "1=1";
                        } else {
                            strFilterWhere += "psd." + ParticipantSubstituteData.SUBSTITUTE_END_DATE_FIELD + "<" + ConvertData.ToDbDate(date.AddDays(1));
                        }
                    } else if (columnName == ParticipantSubstituteData.APPROVAL_STATUS_FIELD.Trim().ToUpper()) {
                        strFilterWhere += "psd." + ParticipantSubstituteData.APPROVAL_STATUS_FIELD + "=" + strItemProp[1];
                    } else if (columnName == ParticipantSubstituteData.MODIFIED_DATE_FIELD.Trim().ToUpper()) {
                        if (strItemProp[1].StartsWith(URL_FROM_FILTER_DELIMITER)) {
                            string pureValue = strItemProp[1].Substring(URL_FROM_FILTER_DELIMITER.Length);
                            DateTime date = ConvertData.ToDate(pureValue, FILTER_DATETIME_FORMAT);
                            strFilterWhere += "psd." + ParticipantSubstituteData.MODIFIED_DATE_FIELD + ">=" + ConvertData.ToDbDate(date);
                        }

                        if (strItemProp[1].StartsWith(URL_TO_FILTER_DELIMITER)) {
                            string pureValue = strItemProp[1].Substring(URL_TO_FILTER_DELIMITER.Length);
                            DateTime date = ConvertData.ToDate(pureValue, FILTER_DATETIME_FORMAT);
                            strFilterWhere += "psd." + ParticipantSubstituteData.MODIFIED_DATE_FIELD + "<" + ConvertData.ToDbDate(date);
                        }
                    } else if (columnName == ParticipantSubstituteData.IS_ALLOW_TAKE_OVER_FIELD.Trim().ToUpper()) {
                        if (strItemProp[1].Trim().ToLower() == "true") {
                            strFilterWhere += "psd." + ParticipantSubstituteData.IS_ALLOW_TAKE_OVER_FIELD + "=1";
                        } else {
                            strFilterWhere += "psd." + ParticipantSubstituteData.IS_ALLOW_TAKE_OVER_FIELD + "=0";
                        }
                        
                    } else if (columnName == ParticipantSubstituteData.ACTIVE_FIELD.Trim().ToUpper()) {
                        if (strItemProp[1].Trim().ToLower() == "true") {
                            strFilterWhere += "psd." + ParticipantSubstituteData.ACTIVE_FIELD + "=1"
                                 + " AND psd." + ParticipantSubstituteData.SUBSTITUTE_END_DATE_FIELD + ">=" + ConvertData.ToDbDate(DateTime.Now);
                        } else {
                            strFilterWhere += "psd." + ParticipantSubstituteData.ACTIVE_FIELD + "=0";
                        }

                    } else if (columnName == "remark".Trim().ToUpper()) {
                        strFilterWhere += "apptext." + AppTextStoreData.TEXT_CONTENT_FIELD + " LIKE '%" + strItemProp[1] + "%'";
                    }

                }
            }

            return strFilterWhere;
        }

        private string GetOrder(string sort) {
            
            string strOrder = "ORDER BY psd." + ParticipantSubstituteData.ID_FIELD;
                        
            if (!String.IsNullOrEmpty(sort)) {
                if (sort.Trim().ToLower() == "null") {
                    return strOrder;
                }

                strOrder = "";
                string[] sortItems = sort.Split(UrlParamDelimiter.ToCharArray());
                foreach (string sortItem in sortItems) {
                    string[] strItemProp = sortItem.Split(UrlParamValueDelimiter.ToCharArray());
                    if (strOrder.Length > 0) {
                        strOrder += ", ";
                    }

                    string descAsc = "ASC";
                    if (strItemProp.Length > 1) {
                        descAsc = strItemProp[1];
                    }

                    if (strItemProp[0].Trim().ToUpper() == "substituted_name_surname_first".Trim().ToUpper()) {
                        strOrder = "pdSubstituted." + ParticipantsData.SURNAME_FIELD + " " + descAsc + ", " +
                            "pdSubstituted." + ParticipantsData.FIRST_NAME_FIELD + " " + descAsc;
                    } else if (strItemProp[0].Trim().ToUpper() == "substitutee_name_surname_first".Trim().ToUpper()) {
                        strOrder = "pdSubstitutee." + ParticipantsData.SURNAME_FIELD + " " + descAsc + ", " +
                            "pdSubstitutee." + ParticipantsData.FIRST_NAME_FIELD + " " + descAsc;
                    } else if (strItemProp[0].Trim().ToUpper() == "modified_user_name".Trim().ToUpper()) {
                        strOrder = "pdModifUser." + ParticipantsData.SURNAME_FIELD + " " + descAsc + ", " +
                            "pdSubstituted." + ParticipantsData.FIRST_NAME_FIELD + " " + descAsc;
                    } else if (strItemProp[0].Trim().ToUpper() == "approval_status_text".Trim().ToUpper()) {
                        strOrder = "psd." + ParticipantSubstituteData.APPROVAL_STATUS_FIELD + " " + descAsc;
                    } else {
                        strOrder += "psd." + strItemProp[0] + " " + descAsc;
                    }
                }

                strOrder = " ORDER BY " + strOrder;
            }

            return strOrder;
        }

        private void DeactivatePastSubst() {
            var pastSubsts = (from substDb in m_dbContext.Participant_Substitute
                             where substDb.substitute_end_date < DateTime.Now
                             && substDb.active == true
                             select substDb).ToList();

            foreach (var pastSubst in pastSubsts) {
                pastSubst.active = false;
            }

            SaveChanges();
        }

        private string GetPureBody(List<int> companyIds, string strFilterWhere) {
            string compFilter = "";
            foreach(int compId in companyIds) {
                if (compFilter.Length > 0) {
                    compFilter += " OR ";
                }
                compFilter += "psd." + ParticipantSubstituteData.COMPANIES_IDS_FIELD + " LIKE '%," + compId + ",%'";
            }
            compFilter = "(" + compFilter + ")";

            string sqlPureBody = " FROM " + ParticipantSubstituteData.TABLE_NAME + " psd" +
                " INNER JOIN " + ParticipantsData.TABLE_NAME + " pdSubstituted" +
                " ON psd." + ParticipantSubstituteData.SUBSTITUTED_USER_ID_FIELD + "=pdSubstituted." + ParticipantsData.ID_FIELD +
                " INNER JOIN " + ParticipantsData.TABLE_NAME + " pdSubstitutee" +
                " ON psd." + ParticipantSubstituteData.SUBSTITUTE_USER_ID_FIELD + "=pdSubstitutee." + ParticipantsData.ID_FIELD +
                " INNER JOIN " + ParticipantsData.TABLE_NAME + " pdModifUser" +
                " ON psd." + ParticipantSubstituteData.MODIFIED_USER_FIELD + "=pdModifUser." + ParticipantsData.ID_FIELD +
                " LEFT OUTER JOIN " + SubstitutionRemarkData.TABLE_NAME + " substrem" +
                " ON substrem." + SubstitutionRemarkData.SUBSTITUTION_ID_FIELD + "=psd." + ParticipantSubstituteData.ID_FIELD +
                " LEFT OUTER JOIN " + AppTextStoreData.TABLE_NAME + " apptext" +
                " ON apptext." + AppTextStoreData.ID_FIELD + "=substrem." + SubstitutionRemarkData.REMARK_TEXT_ID_FIELD +
            //" LEFT OUTER JOIN " + DiscussionData.TABLE_NAME + " disc" +
            //" ON disc." + DiscussionData.ID_FIELD + "=subdtdisc." + SubstituteDiscussionData.DISCUSSION_ID_FIELD +
            //" LEFT OUTER JOIN " + AppTextStoreData.TABLE_NAME + " apptext" +
            //" ON apptext." + AppTextStoreData.ID_FIELD + "=disc." + DiscussionData.TEXT_ID_FIELD +
            //" AND apptext." + AppTextStoreData.TEXT_TYPE_FIELD + "=" + (int)AppTextStoreRepository.TextType.SubstRemark +
            " WHERE " + compFilter;

            sqlPureBody += strFilterWhere;

            return sqlPureBody;
        }

        public List<UserSubstitutionExtended> GetSubstitution(
            int userId,
            List<int> companyIds, 
            string filter, 
            string sort) {

            DeactivatePastSubst();
            SetDefaultFilter(userId);

            string strFilterWhere = GetFilter(filter);

            string strOrder = GetOrder(sort);

            string sqlPure = "SELECT psd.*," +
                " pdSubstituted." + ParticipantsData.SURNAME_FIELD + "+' '+pdSubstituted." + ParticipantsData.FIRST_NAME_FIELD + " AS substituted_name_surname_first," +
                " pdSubstitutee." + ParticipantsData.SURNAME_FIELD + "+' '+pdSubstitutee." + ParticipantsData.FIRST_NAME_FIELD + " AS substitutee_name_surname_first ";
            string sqlPureBody = GetPureBody(companyIds, strFilterWhere);
            sqlPureBody += strOrder;

            string sql = sqlPure + sqlPureBody;

            var substitutions = m_dbContext.Database.SqlQuery<UserSubstitutionExtended>(sql).ToList();

            List<UserSubstitutionExtended> retSubsts = new List<UserSubstitutionExtended>();

            List<int> substIds = new List<int>();
            foreach (var tmpSubstitute in substitutions) {
                if (substIds.Contains(tmpSubstitute.id)) {
                    continue;
                }

                substIds.Add(tmpSubstitute.id);

                UserSubstitutionExtended userSubstitutionExtended = new UserSubstitutionExtended();
                SetValues(tmpSubstitute, userSubstitutionExtended);

                retSubsts.Add(userSubstitutionExtended);
            }

            //var subst = (from psd in m_dbContext.Participant_Substitute
            //             join pd1 in m_dbContext.Participants
            //             on psd.substituted_user_id equals pd1.id
            //             join pd2 in m_dbContext.Participants
            //             on psd.substitute_user_id equals pd2.id
            //             where companyIds.Contains(pd1.company_id) || companyIds.Contains(pd2.company_id)
            //             select psd).ToList();

            //if (!String.IsNullOrEmpty(filter)) {
            //    string[] filterItems = filter.Split(UrlParamDelimiter.ToCharArray());
            //    foreach (string filterItem in filterItems) {
            //        string[] strItemProp = filterItem.Split(UrlParamValueDelimiter.ToCharArray());
            //        string columnName = strItemProp[0].Trim().ToUpper();
            //        if (columnName == "substituted_name_surname_first".Trim().ToUpper()) {
            //            subst = FilterSubstUser(subst, filterItem);
            //        }
            //    }
            //}

            //if (String.IsNullOrEmpty(sort)) {
            //    subst = subst.OrderBy(x => x.id).ToList();
            //} else { 
            //    string[] sortItems = sort.Split(UrlParamDelimiter.ToCharArray());
            //    foreach (string sortItem in sortItems) {
            //        string[] strItemProp = sortItem.Split(UrlParamValueDelimiter.ToCharArray());
            //        if (strItemProp[0] == "substituted_name_surname_first") {
            //            subst = subst.OrderBy(x => x.SubstitutedUser.surname).ToList();
            //        }
            //    }
            //}

            return retSubsts;
        }

        private List<Participant_Substitute> FilterSubstUser(List<Participant_Substitute> source, string filterItem) {
            string[] strItemProp = filterItem.Split(UrlParamValueDelimiter.ToCharArray());
            string columnName = strItemProp[0].Trim().ToUpper();
            if (columnName == "substituted_name_surname_first".Trim().ToUpper()) {
                source = (from psd in source
                          join pd in m_dbContext.Participants
                          on psd.substituted_user_id equals pd.id
                          where pd.user_search_key.ToLower().IndexOf(strItemProp[1].ToLower().Replace(" ", "")) > -1 ||
                          (pd.surname + pd.first_name).ToLower().IndexOf(strItemProp[1].ToLower()) > -1
                          select psd).ToList();
            }

            return source;
        }

        public int SaveSubstitutionData(
            UserSubstitutionExtended modifSubst,
            int userId,
            out List<string> msg) {

            msg = new List<string>();
                                    
            using (TransactionScope transaction = new TransactionScope()) {
                try {
                    var dbSubst = new Participant_Substitute();

                    if (modifSubst.id >= 0) {
                        dbSubst = (from cd in m_dbContext.Participant_Substitute
                                    where cd.id == modifSubst.id
                                    select cd).FirstOrDefault();
                        if (dbSubst.active == false) {
                            throw new Exception("Substitution was deactivated, cannot be modified");
                        }
                    }

                    dbSubst.substituted_user_id = modifSubst.substituted_user_id;
                    dbSubst.substitute_user_id = modifSubst.substitute_user_id;
                    dbSubst.substitute_start_date = modifSubst.substitute_start_date;
                    dbSubst.substitute_end_date = modifSubst.substitute_end_date;
                    dbSubst.is_allow_take_over = modifSubst.is_allow_take_over;
                    //dbSubst.remark = modifSubst.remark;
                    dbSubst.approval_status = modifSubst.approval_status;
                    dbSubst.active = modifSubst.active;

                    dbSubst.active = modifSubst.active;
                    dbSubst.modified_user = userId;
                    dbSubst.modified_date = DateTime.Now;

                    bool isNew = (modifSubst.id < 0);

                    if (isNew) {

                        var lastSubstId = (from usd in m_dbContext.Participant_Substitute
                                         orderby usd.id descending
                                         select usd).Take(1).FirstOrDefault();

                        int lastId = -1;
                        if (lastSubstId != null) {
                            lastId = lastSubstId.id;
                        }

                        int newId = lastId;
                        newId++;
                        dbSubst.id = newId;
                        dbSubst.active = true;

                        //Get companies List
                        var partSubstituted = (from partDb in m_dbContext.Participants
                                               where partDb.id == dbSubst.substituted_user_id
                                               select partDb).FirstOrDefault();

                        List<int> compIds = new List<int>();
                        string strIds = ",";
                        if (partSubstituted.ParticipantRole_CentreGroup != null) {
                            foreach (var cgRole in partSubstituted.ParticipantRole_CentreGroup) {
                                int compId = cgRole.Centre_Group.company_id;
                                if (!compIds.Contains(compId)) {
                                    compIds.Add(compId);
                                    strIds += compId + ",";
                                }
                            }

                            //foreach (int compId in compIds) {
                            //    strIds += compId + ",";
                            //}
                            
                        }

                        if (partSubstituted.Participant_Office_Role != null) {
                            foreach (var cgRole in partSubstituted.Participant_Office_Role) {
                                int compId = cgRole.office_id;
                                if (!compIds.Contains(compId)) {
                                    compIds.Add(compId);
                                    strIds += compId + ",";
                                }
                            }

                            //foreach (int compId in compIds) {
                            //    strIds += compId + ",";
                            //}
                                                        
                        }

                        if (strIds.Length == 0) {
                            strIds = null;
                        }

                        dbSubst.companies_ids = strIds;

                        dbSubst.author_id = userId;

                        if (!String.IsNullOrWhiteSpace(modifSubst.remark)) {
                            var appTextId = new DiscussionRepository().AddSubstitutionDiscussion(
                                dbSubst,
                                modifSubst.remark, 
                                AppTextStoreRepository.TextType.SubstRemark,
                                userId, 
                                m_dbContext);

                            Substitution_Remark sr = new Substitution_Remark();
                            sr.substitution_id = dbSubst.id;
                            sr.remark_text_id = appTextId;
                            dbSubst.Substitution_Remark.Add(sr);
                        }

                        m_dbContext.Participant_Substitute.Add(dbSubst);
                    }

                    SaveChanges();
                    transaction.Complete();

                    return dbSubst.id;
                } catch (Exception ex) {
                    throw ex;
                }
            }
        }

        public List<ParticipantsExtended> GetSubstitutedParticipantsData(
           string rootUrl,
           List<int> companyIds,
           int currentUserId) {

            if (companyIds == null || companyIds.Count == 0) {
                return null;
            }
            var tmpParticipants = (from pd in m_dbContext.Participants
                                   join prCgDb in m_dbContext.ParticipantRole_CentreGroup
                                   on pd.id equals prCgDb.participant_id
                                   where pd.active == true && !(pd.user_name.StartsWith("NO_DOMAIN")) &&
                                   (pd.is_external == null || pd.is_external == false) &&
                                   (prCgDb.role_id == (int)UserRole.Orderer || prCgDb.role_id == (int)UserRole.ApprovalManager) &&
                                   companyIds.Contains(pd.company_id)
                                   orderby pd.surname, pd.first_name
                                   select new {
                                       id = pd.id,
                                       user_name = pd.user_name,
                                       surname = pd.surname,
                                       first_name = pd.first_name,
                                       company_id = pd.company_id,
                                       user_search_key = pd.user_search_key
                                   }).ToList().Distinct();


            List<ParticipantsExtended> substData = new List<ParticipantsExtended>();
            foreach (var tmpParticipant in tmpParticipants) {
                ParticipantsExtended participant = new ParticipantsExtended();
                SetValues(tmpParticipant, participant);

                //participant.first_name += " (" + participant.user_name + ")";

                //set name
                participant.name_surname_first = participant.surname + " " + participant.first_name;
                //country flag
                participant.country_flag = GetCountryFlag(participant.company_id, rootUrl);

                substData.Add(participant);
            }

            return substData;
        }

        public List<ParticipantsExtended> GetSubstitutedMen(
            string searchText,
            string rootUrl,
           List<int> companyIds) {

            if (companyIds == null || companyIds.Count == 0) {
                return null;
            }

            List<ParticipantsExtended> substData = new List<ParticipantsExtended>();
            
            if (String.IsNullOrWhiteSpace(searchText)) {
                var tmpParticipants = (from pd in m_dbContext.Participants
                                       join prCgDb in m_dbContext.ParticipantRole_CentreGroup
                                       on pd.id equals prCgDb.participant_id
                                       where pd.active == true && !(pd.user_name.StartsWith("NO_DOMAIN"))
                                       && (pd.is_external == null || pd.is_external == false)
                                       && (prCgDb.role_id == (int)UserRole.Orderer || prCgDb.role_id == (int)UserRole.ApprovalManager)
                                       && companyIds.Contains(pd.company_id)
                                       //&& pd.user_search_key.ToLower().Contains(searchTextWoDia)
                                       orderby pd.surname, pd.first_name
                                       select new {
                                           id = pd.id,
                                           user_name = pd.user_name,
                                           surname = pd.surname,
                                           first_name = pd.first_name,
                                           company_id = pd.company_id,
                                           user_search_key = pd.user_search_key
                                       }).ToList().Distinct();

                foreach (var tmpParticipant in tmpParticipants) {
                    ParticipantsExtended participant = new ParticipantsExtended();
                    SetValues(tmpParticipant, participant);

                    //set name
                    participant.name_surname_first = participant.surname + " " + participant.first_name;
                    //country flag
                    participant.country_flag = GetCountryFlag(participant.company_id, rootUrl);

                    substData.Add(participant);
                }
            } else {
                string searchTextWoDia = RemoveDiacritics(searchText).ToLower();
                var tmpParticipants = (from pd in m_dbContext.Participants
                                       join prCgDb in m_dbContext.ParticipantRole_CentreGroup
                                       on pd.id equals prCgDb.participant_id
                                       where pd.active == true && !(pd.user_name.StartsWith("NO_DOMAIN"))
                                       && (pd.is_external == null || pd.is_external == false)
                                       && (prCgDb.role_id == (int)UserRole.Orderer || prCgDb.role_id == (int)UserRole.ApprovalManager)
                                       && companyIds.Contains(pd.company_id)
                                       && pd.user_search_key.ToLower().Contains(searchTextWoDia)
                                       orderby pd.surname, pd.first_name
                                       select new {
                                           id = pd.id,
                                           user_name = pd.user_name,
                                           surname = pd.surname,
                                           first_name = pd.first_name,
                                           company_id = pd.company_id,
                                           user_search_key = pd.user_search_key
                                       }).ToList().Distinct();

                foreach (var tmpParticipant in tmpParticipants) {
                    ParticipantsExtended participant = new ParticipantsExtended();
                    SetValues(tmpParticipant, participant);

                    //set name
                    participant.name_surname_first = participant.surname + " " + participant.first_name;
                    //country flag
                    participant.country_flag = GetCountryFlag(participant.company_id, rootUrl);

                    substData.Add(participant);
                }
            }

            
            

            return substData;

        }

        public void DeactiveUserSubstitutionById(int substId) {
            var userSubst = (from ps in m_dbContext.Participant_Substitute
                             where ps.id == substId
                             select ps).FirstOrDefault();

            userSubst.active = false;

            m_dbContext.SaveChanges();
        }

        public void DeleteUserSubstitutionById(int substId) {
            var userSubst = (from ps in m_dbContext.Participant_Substitute
                             where ps.id == substId
                             select ps).FirstOrDefault();
                        
            m_dbContext.Participant_Substitute.Remove(userSubst);

            m_dbContext.SaveChanges();
        }

        public bool IsApprovalAllowed(int substId, int userId) {
            Participant_Substitute subst = (from substDb in m_dbContext.Participant_Substitute
                                            where substDb.id == substId
                                            select substDb).FirstOrDefault();

            if (subst.approval_status != (int)RequestRepository.ApproveStatus.WaitForApproval) {
                return false;
            }

            int compId = subst.SubstitutedUser.company_id;
            var compRoles = (from comRoleDb in m_dbContext.Participant_Office_Role
                             where comRoleDb.role_id == (int)UserRole.SubstitutionApproveManager
                             && comRoleDb.participant_id == userId
                             select comRoleDb).FirstOrDefault();
            if (compRoles == null) {
                return false;
            }

            return true;
        }

        public void ApproveSubstitution(int substId, int userId) {
            Participant_Substitute subst = (from substDb in m_dbContext.Participant_Substitute
                                            where substDb.id == substId
                                            select substDb).FirstOrDefault();

            subst.approval_status = (int)RequestRepository.ApproveStatus.Approved;
            subst.app_man_id = userId;
            subst.modified_user = userId;
            subst.modified_date = DateTime.Now;

            SaveChanges();
        }

        public void RejectSubstitution(int substId, int userId) {
            Participant_Substitute subst = (from substDb in m_dbContext.Participant_Substitute
                                            where substDb.id == substId
                                            select substDb).FirstOrDefault();

            subst.approval_status = (int)RequestRepository.ApproveStatus.Rejected;
            //subst.active = false;
            subst.modified_user = userId;
            subst.app_man_id = userId;
            subst.modified_date = DateTime.Now;

            SaveChanges();
        }

        public Participant_Substitute GetLastSubstitute() {
            Participant_Substitute subst = (from substDb in m_dbContext.Participant_Substitute
                                            orderby substDb.id descending
                                            select substDb).FirstOrDefault();

            return subst;
        }

        public Participant_Substitute GetSubstitutionById(int id) {
            Participant_Substitute subst = (from substDb in m_dbContext.Participant_Substitute
                                            where substDb.id == id
                                            select substDb).FirstOrDefault();

            return subst;
        }

        public UserSubstitutionExtended GetSubstitutionByIdJs(int id, Participants currentUser) {
            Participant_Substitute subst = (from substDb in m_dbContext.Participant_Substitute
                                            where substDb.id == id
                                            select substDb).FirstOrDefault();

            UserSubstitutionExtended userSubstitutionExtended = new UserSubstitutionExtended();
            SetValues(subst, userSubstitutionExtended);
            userSubstitutionExtended.substituted_name_surname_first = UserRepository.GetUserNameSurnameFirst(subst.SubstitutedUser);
            userSubstitutionExtended.substitutee_name_surname_first = UserRepository.GetUserNameSurnameFirst(subst.SubstituteUser);
            bool isApprovable;
            bool isEditable;
            SetGridCellEditMode(userSubstitutionExtended, currentUser, out isApprovable, out isEditable);

            SetAppMen(userSubstitutionExtended);

            int modifUserId = subst.modified_user;
            var participant = (from partDb in m_dbContext.Participants
                               where partDb.id == modifUserId
                               select partDb).FirstOrDefault();
            userSubstitutionExtended.modified_user_name = participant.surname + " " + participant.first_name;

            return userSubstitutionExtended;
        }

        //public int SaveSubstitutionData(
        //    UserSubstitutionExtended subst,
        //    int userId,
        //    out List<string> msg) {

        //    msg = new List<string>();

        //    HttpResult httpResult = new HttpResult();

        //    using (TransactionScope transaction = new TransactionScope()) {
        //        try {
        //            var dbSubst = new Participant_Substitute();

        //            dbSubst.substituted_user_id = subst.substituted_user_id;
        //            dbSubst.substitute_user_id = subst.substitute_user_id;
        //            dbSubst.substitute_start_date = subst.substitute_start_date;
        //            dbSubst.substitute_end_date = subst.substitute_end_date;
        //            dbSubst.modified_date = DateTime.Now;
        //            dbSubst.modified_user = userId;



        //            SaveChanges();
        //            transaction.Complete();

        //            return dbSubst.id;
        //        } catch (Exception ex) {
        //            throw ex;
        //        }
        //    }
        //}
        #endregion
    }
}
