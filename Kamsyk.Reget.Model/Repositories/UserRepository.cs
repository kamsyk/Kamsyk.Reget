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
using static Kamsyk.Reget.Model.Repositories.RequestRepository;
using Kamsyk.Reget.Model.ExtendedModel.Supplier;
using Kamsyk.Reget.Model.ExtendedModel.HttpResult;

namespace Kamsyk.Reget.Model.Repositories {
    public class UserRepository : BaseRepository<Participants>, IUserRepository {
        #region Constant
        public const int REGET_SYSTEM_USER = -1;
        public const int USER_CANNOT_BE_DEACTIVATED_APPMAN = -10;
        public const int USER_CANNOT_BE_DELETED_APPMAN = -15;
        public const int USER_CANNOT_BE_DEACTIVATED_ORDERER = -20;
        public const int USER_CANNOT_BE_DELETED_ORDERER = -25;
        #endregion

        #region Enums
        public enum UserRole {
            Requestor = 0,
            ReadOnly = 5,
            Orderer = 100,
            DeputyOrderer = 110,
            SupplierAdmin = 200,
            CentreGroupPropAdmin = 300,
            ApproveMatrixAdmin = 310,
            RequestorAdmin = 320,
            OrdererAdmin = 330,
            ApprovalManager = 400,
            SubstitutionApproveManager = 480,
            StatisticsCompanyManager = 600,
            SubstituteCompanyManager = 650,
            OfficeAdministrator = 2000,
            ApplicationAdmin = 10000
        }

        public enum UserDisplayType {
            Users,
            NonActiveUsers
        }
        #endregion

        #region Properties
        private readonly IUserRepository m_IUserRepository;
        #endregion

        #region Constructor
        public UserRepository() : base() { }

        public UserRepository(IUserRepository iUserRepo) : base() {
            m_IUserRepository = iUserRepo;
        }
        #endregion

        #region Static Methods
        //private static List<StructRemovalMap> GetRemovalMap() {
        //    List<StructRemovalMap> defaultDiacriticsRemovalMap = new List<StructRemovalMap>();

        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("A", "\u0041\u24B6\uFF21\u00C0\u00C1\u00C2\u1EA6\u1EA4\u1EAA\u1EA8\u00C3\u0100\u0102\u1EB0\u1EAE\u1EB4\u1EB2\u0226\u01E0\u00C4\u01DE\u1EA2\u00C5\u01FA\u01CD\u0200\u0202\u1EA0\u1EAC\u1EB6\u1E00\u0104\u023A\u2C6F"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("AA", "\uA732"));

        //    return defaultDiacriticsRemovalMap;
        //}

        //private static Hashtable GetDiacriticsHashtable() {
        //    Hashtable ht = new Hashtable();

        //    for (int i = 0; i < m_DefaultDiacriticsRemovalMap.Count; i++) {
        //        var letters = m_DefaultDiacriticsRemovalMap.ElementAt(i).Letters;
        //    }

        //    return ht;
        //}
        public static string GetUserNameSurnameFirst(Participants p) {
            string userName = "";
            if (!String.IsNullOrEmpty(p.surname)) {
                userName = p.surname;
            }

            if (!String.IsNullOrEmpty(p.first_name)) {
                if (userName.Length > 0) {
                    userName += " ";
                }
                userName += p.first_name;
            }

            return userName;
        }

        public static string GetUserNameFirstnameFirst(Participants p) {
            string userName = "";
            if (!String.IsNullOrEmpty(p.first_name)) {
                userName = p.first_name;
            }

            if (!String.IsNullOrEmpty(p.surname)) {
                if (userName.Length > 0) {
                    userName += " ";
                }
                userName += p.surname;
            }

            return userName;
        }

        public static string GetUserNameSurnameFirst(string surname, string firstname) {
            string userName = "";
            if (!String.IsNullOrEmpty(surname)) {
                userName = surname;
            }

            if (!String.IsNullOrEmpty(firstname)) {
                if (userName.Length > 0) {
                    userName += " ";
                }
                userName += firstname;
            }

            return userName;
        }

        
        public static string GetSqlUserNameSurnameFirstFilter(string parTablePrefix, string filterValue) {
            string srtSql = "((" + parTablePrefix + "." + ParticipantsData.USER_SEARCH_KEY_FIELD + " LIKE '%" + filterValue.Trim().Replace(" ", "") + "%')"
                                + " OR (" + parTablePrefix + "." + ParticipantsData.SURNAME_FIELD + "+" + parTablePrefix + "." + ParticipantsData.FIRST_NAME_FIELD + " LIKE '%" + filterValue.Trim().Replace(" ", "") + "%'))";

            return srtSql;
        }
        #endregion

        #region Interface Methods
        public Participants GetActiveParticipantByUserName(string userName) {
            if (userName == null) {
                return null;
            }

            var participant = GetFirstOrDefault(p => p.user_name.ToLower() == userName.ToLower() && p.active == true);
            //var participant = SqlQuery("select user_name from Participants where user_name = '" + userName.ToLower() + "'");

            if (participant == null) {
                return null;
            }

            return (Participants)participant;
        }

        //public Participants GetParticipantById(int id) {
        //    var part = (from partDb in m_dbContext.Participants
        //                 where partDb.id == id
        //                 select partDb).FirstOrDefault();

        //    return part;
        //}

        public List<Participants> GetActiveParticipantInRoleByCompanyId(int companyId, int roleId) {
            var parts = (from partDb in m_dbContext.Participants
                         where partDb.company_id == companyId
                         join roleDb in m_dbContext.ParticipantRole_CentreGroup
                         on partDb.id equals roleDb.participant_id
                         where partDb.active == true && roleDb.role_id == roleId
                         select partDb).ToList();

            var partsOffice = (from partDb in m_dbContext.Participants
                         where partDb.company_id == companyId
                         join roleDb in m_dbContext.Participant_Office_Role
                         on partDb.id equals roleDb.participant_id
                         where partDb.active == true && roleDb.role_id == roleId
                         select partDb).ToList();

            foreach (var partOffice in partsOffice) {
                var part = (from patsDb in parts
                            where patsDb.id == partOffice.id
                            select patsDb).FirstOrDefault();
                if (part == null) {
                    parts.Add(partOffice);
                }
            }

            return parts;
        }

        public List<Participants> GetActiveParticipantInRoleByCompanyId(int companyId, int roleId, int notUserId) {
            var parts = (from partDb in m_dbContext.Participants
                         where partDb.company_id == companyId
                         join roleDb in m_dbContext.ParticipantRole_CentreGroup
                         on partDb.id equals roleDb.participant_id
                         where partDb.active == true && roleDb.role_id == roleId
                         && partDb.id != notUserId
                         select partDb).ToList();

            var partsOffice = (from partDb in m_dbContext.Participants
                               where partDb.company_id == companyId
                               join roleDb in m_dbContext.Participant_Office_Role
                               on partDb.id equals roleDb.participant_id
                               where partDb.active == true && roleDb.role_id == roleId
                               && partDb.id != notUserId
                               select partDb).ToList();

            foreach (var partOffice in partsOffice) {
                var part = (from patsDb in parts
                            where patsDb.id == partOffice.id
                            select patsDb).FirstOrDefault();
                if (part == null) {
                    parts.Add(partOffice);
                }
            }

            return parts;
        }

        public List<Participants> GetActiveParticipantNotInRoleByCompanyId(int companyId, int roleId) {
            var parts = (from partDb in m_dbContext.Participants
                         where partDb.company_id == companyId
                         join roleDb in m_dbContext.ParticipantRole_CentreGroup
                         on partDb.id equals roleDb.participant_id
                         //join comRoleDb in m_dbContext.Participant_Office_Role
                         //on partDb.id equals comRoleDb.participant_id
                         where partDb.active == true && roleDb.role_id != roleId
                         select partDb).ToList();

            List<Participants> retParts = new List<Participants>();
            foreach (var part in parts) {
                var compRoles = (from roleDb in part.Participant_Office_Role
                                 where roleDb.role_id == roleId
                                 select roleDb).FirstOrDefault();
                if(compRoles == null) {
                    retParts.Add(part);
                }
            }

            return retParts;
        }

        public List<Participants> GetActiveParticipantNotInRole(int roleId) {
            //var parts = (from partDb in m_dbContext.Participants
            //             join roleDb in m_dbContext.ParticipantRole_CentreGroup
            //             on partDb.id equals roleDb.participant_id
            //             where partDb.active == true && roleDb.role_id != roleId
            //             select partDb).ToList();


            //var parts = (from partDb in m_dbContext.Participants
            //             join roleDb in m_dbContext.ParticipantRole_CentreGroup
            //             on partDb.id equals roleDb.participant_id into jointable
            //             from z in jointable.DefaultIfEmpty()
            //             where partDb.active == true && z.role_id == roleId
            //             select partDb).ToList();

            string sql = "SELECT pd.* FROM " + ParticipantsData.TABLE_NAME + " pd"
                + " LEFT OUTER JOIN " + ParticipantroleCentregroupData.TABLE_NAME + " prcg"
                + " ON pd." + ParticipantsData.ID_FIELD + "=prcg." + ParticipantroleCentregroupData.PARTICIPANT_ID_FIELD
                + " AND prcg." + ParticipantroleCentregroupData.ROLE_ID_FIELD + "=" + roleId
                + " WHERE pd." + ParticipantsData.ACTIVE_FIELD + "=1"
                + " AND prcg." + ParticipantroleCentregroupData.ROLE_ID_FIELD + " IS NULL"
                + " AND pd." + ParticipantsData.USER_NAME_FIELD + " NOT LIKE 'NO_DOMAIN%'";
            var parts = m_dbContext.Database.SqlQuery<Participants>(sql).ToList();

            List<Participants> retParts = new List<Participants>();
            foreach (var part in parts) {
                var compRoles = (from roleDb in part.Participant_Office_Role
                                 where roleDb.role_id == roleId
                                 select roleDb).FirstOrDefault();

                var partRoles = (from roleDb in part.ParticipantRole_CentreGroup
                                 where roleDb.role_id == roleId
                                 select roleDb).FirstOrDefault();

                if (compRoles == null) {
                    retParts.Add(part);
                }
            }

            return retParts;
        }

        public List<Participants> GetActiveParticipantByCompanyId(int companyId) {
            var parts = (from partDb in m_dbContext.Participants
                         where partDb.company_id == companyId
                         && partDb.active == true
                         && !partDb.user_name.StartsWith("NO_DOMAIN")
                         select partDb).ToList();

            return parts;
        }

        public Participants GetParticipantById(int id) {
            if (id == DataNulls.INT_NULL) {
                return null;
            }

            var participant = GetFirstOrDefault(p => p.id == id);
            //var participant = SqlQuery("select user_name from Participants where user_name = '" + userName.ToLower() + "'");

            if (participant == null) {
                return null;
            }

            return (Participants)participant;
        }

        public ParticipantsExtended GetParticipantByIdJs(int id, string rootUrl) {
            if (id == DataNulls.INT_NULL) {
                return null;
            }

            var participant = GetFirstOrDefault(p => p.id == id);

            if (participant == null) {
                return null;
            }

            var retPart = new ParticipantsExtended();
            SetValues(participant, retPart);
            retPart.name_surname_first = GetUserNameSurnameFirst(participant);
            retPart.office_name = participant.Company.country_code;

            if (participant.ParticipantPhoto != null) {
                retPart.photo240_url = participant.id.ToString();
            }
            retPart.country_flag = GetCountryFlag(participant.company_id, rootUrl);

            return retPart;
        }


        public void SetNonActiveUser(int userId) {
            var participant = GetFirstOrDefault(p => p.id == userId);

            if (participant == null) {
                return;
            }

            participant.is_non_active = true;

            m_dbContext.SaveChanges();
        }

        public void SetRevertNonActiveUser(int userId) {
            var participant = GetFirstOrDefault(p => p.id == userId);

            if (participant == null) {
                return;
            }

            participant.is_non_active = false;

            m_dbContext.SaveChanges();
        }

        public Participants GetParticipantByUserName(string userName) {
#if TEST
            if (m_IUserRepository != null) {
                //Test
                return m_IUserRepository.GetParticipantByUserName(userName);
            }
#endif

            if (userName == null) {
                return null;
            }

            var participant = GetFirstOrDefault(p => p.user_name.ToLower() == userName.ToLower());
            //var participant = SqlQuery("select user_name from Participants where user_name = '" + userName.ToLower() + "'");

            if (participant == null) {
                return null;
            }

            return (Participants)participant;
        }

        public void SetParticipantLang(int participantId, string cultureName) {
            using (TransactionScope transaction = new TransactionScope()) {
                var participant = GetFirstOrDefault(p => p.id == participantId);

                if (participant.User_Setting == null) {
                    User_Setting user_Setting = new User_Setting();
                    user_Setting.user_id = participantId;
                    user_Setting.default_lang = cultureName;
                    m_dbContext.User_Setting.Add(user_Setting);
                    m_dbContext.SaveChanges();
                } else {
                    participant.User_Setting.default_lang = cultureName;
                    m_dbContext.Entry(participant).State = EntityState.Modified;
                    m_dbContext.SaveChanges();
                }

                transaction.Complete();
            }
        }

        public bool DeleteUser(int participantId) {
            bool isDeleted = false;

            using (TransactionScope transaction = new TransactionScope()) {
                try {
                    DeleteUserFromApprovalMatrix(participantId);
                    DeleteUserRoles(participantId);

                    var part = (from partDb in m_dbContext.Participants
                                where partDb.id == participantId
                                select partDb).FirstOrDefault();

                    //User Settings
                    if (part.User_Setting != null) {
                        m_dbContext.User_Setting.Remove(part.User_Setting);
                    }
                    m_dbContext.User_GridSetting.RemoveRange(part.User_GridSetting);

                    //Ship To Address
                    m_dbContext.Ship_To_Address.RemoveRange(part.Ship_To_Address);

                    //requestor centre
                    part.Requestor_Centre.ToList().ForEach(x => part.Requestor_Centre.Remove(x));
                    part.Asset_Requestor_Centre.ToList().ForEach(x => part.Asset_Requestor_Centre.Remove(x));

                    //substitutions
                    //part.Participant_Substitute.ToList().ForEach(x => part.Participant_Substitute.Remove(x));
                    //part.Participant_Substituted.ToList().ForEach(x => part.Participant_Substituted.Remove(x));
                    var partSubst = (from substDb in m_dbContext.Participant_Substitute
                                     where substDb.substituted_user_id == participantId ||
                                     substDb.substitute_user_id == participantId
                                     select substDb).ToList();

                    if (partSubst != null && partSubst.Count > 0) {
                        for (int i = partSubst.Count - 1; i >= 0; i--) {
                            m_dbContext.Participant_Substitute.Remove(partSubst.ElementAt(i));
                        }
                    }

                    //read only access
                    part.Purchase_Group_ROAccess.ToList().ForEach(x => part.Purchase_Group_ROAccess.Remove(x));

                    if (IsHasParticipantRequest(participantId)) {
                        part.active = false;
                        part.is_non_active = false;
                    } else {
                        m_dbContext.Participants.Remove(part);
                        isDeleted = true;
                    }

                    SaveChanges();
                    transaction.Complete();

                    return isDeleted;
                } catch (Exception ex) {
                    throw ex;
                }
            }
        }

        public void DeleteUsersByName(string userName) {
            List<Participants> parts = (from partDb in m_dbContext.Participants
                                        where partDb.user_name == userName
                                        select partDb).ToList();

            if (parts != null) {
                for (int i = parts.Count - 1; i >= 0; i--) {
                    DeleteUser(parts.ElementAt(i).id);
                }
            }
        }

        private bool IsHasParticipantRequest(int participantId) {
            var request = (from requestDb in m_dbContext.Request_Event
                           where requestDb.requestor == participantId
                           select new {
                               id = requestDb.id
                           }).FirstOrDefault();
            if (request != null) {
                return true;
            }

            request = (from requestDb in m_dbContext.Request_Event
                       where requestDb.orderer_id == participantId
                       select new {
                           id = requestDb.id
                       }).FirstOrDefault();
            if (request != null) {
                return true;
            }

            request = (from requestDb in m_dbContext.Request_Event
                       where requestDb.manager1 == participantId ||
                       requestDb.manager2 == participantId ||
                       requestDb.manager3 == participantId ||
                       requestDb.manager4 == participantId ||
                       requestDb.manager5 == participantId ||
                       requestDb.manager6 == participantId
                       select new {
                           id = requestDb.id
                       }).FirstOrDefault();
            if (request != null) {
                return true;
            }

            request = (from requestDb in m_dbContext.Asset_Request_Event
                       where requestDb.requestor_id == participantId ||
                       requestDb.manager1_id == participantId ||
                       requestDb.manager2_id == participantId ||
                       requestDb.manager3_id == participantId ||
                       requestDb.manager4_id == participantId ||
                       requestDb.manager5_id == participantId ||
                       requestDb.financ_man_id == participantId ||
                       requestDb.centre_manager_id == participantId ||
                       requestDb.fm_manager_id == participantId
                       select new {
                           id = requestDb.id
                       }).FirstOrDefault();
            if (request != null) {
                return true;
            }

            //pg limit
            var purchaseGroupLimit = (from limitDb in m_dbContext.Purchase_Group_Limit
                                      where limitDb.modify_user == participantId
                                      select new {
                                          limit_id = limitDb.limit_id
                                      }).FirstOrDefault();
            if (request != null) {
                return true;
            }

            //substitution modif_user
            var substitution = (from substDb in m_dbContext.Participant_Substitute
                                where substDb.modified_user == participantId
                                select new {
                                    id = substDb.id
                                }).FirstOrDefault();
            if (request != null) {
                return true;
            }

            //pg commentator
            var comment = (from commentDb in m_dbContext.Purchase_Group
                           where commentDb.commentator_id == participantId
                           select new {
                               id = commentDb.id
                           }).FirstOrDefault();
            if (request != null) {
                return true;
            }

            //orderer supplier
            var ordSupp = (from ordSuppDb in m_dbContext.Orderer_Supplier
                           where ordSuppDb.orderer_id == participantId
                           select new {
                               orderer_id = ordSuppDb.orderer_id
                           }).FirstOrDefault();
            if (request != null) {
                return true;
            }

            return false;
        }

        private void DeleteUserRoles(int participantId) {
            var participant = (from partDb in m_dbContext.Participants
                               where partDb.id == participantId
                               select partDb).FirstOrDefault();

            //user role
            for (int i = participant.ParticipantRole_CentreGroup.Count - 1; i >= 0; i--) {
                participant.ParticipantRole_CentreGroup.Remove(participant.ParticipantRole_CentreGroup.ElementAt(i));
            }

            //office admin
            for (int i = participant.Participant_Office_Role.Count - 1; i >= 0; i--) {
                participant.Participant_Office_Role.Remove(participant.Participant_Office_Role.ElementAt(i));
            }
        }

        public void DeleteUserFromApprovalMatrix(int participantId) {
            var participant = (from partDb in m_dbContext.Participants
                               where partDb.id == participantId
                               select partDb).FirstOrDefault();
            if (participant == null) {
                return;
            }

            //pr cg
            for (int i = participant.ParticipantRole_CentreGroup.Count - 1; i >= 0; i--) {
                participant.ParticipantRole_CentreGroup.Remove(participant.ParticipantRole_CentreGroup.ElementAt(i));
            }

            //requestor centre
            for (int i = participant.Requestor_Centre.Count - 1; i >= 0; i--) {
                participant.Requestor_Centre.Remove(participant.Requestor_Centre.ElementAt(i));
            }

            //manager role
            for (int i = participant.Manager_Role.Count - 1; i >= 0; i--) {
                participant.Manager_Role.Remove(participant.Manager_Role.ElementAt(i));
            }

            //implicite requestor
            for (int i = participant.PurchaseGroup_ImplicitRequestor.Count - 1; i >= 0; i--) {
                participant.PurchaseGroup_ImplicitRequestor.Remove(participant.PurchaseGroup_ImplicitRequestor.ElementAt(i));
            }

            //exclude requestor
            for (int i = participant.PurchaseGroup_ExcludeRequestor.Count - 1; i >= 0; i--) {
                participant.PurchaseGroup_ExcludeRequestor.Remove(participant.PurchaseGroup_ExcludeRequestor.ElementAt(i));
            }

            //implicite orderer
            for (int i = participant.PurchaseGroup_ImplicitRequestor.Count - 1; i >= 0; i--) {
                participant.PurchaseGroup_ImplicitRequestor.Remove(participant.PurchaseGroup_ImplicitRequestor.ElementAt(i));
            }

            //exclude orderer
            for (int i = participant.PurchaseGroup_ExcludeRequestor.Count - 1; i >= 0; i--) {
                participant.PurchaseGroup_ExcludeRequestor.Remove(participant.PurchaseGroup_ExcludeRequestor.ElementAt(i));
            }

            //pg requestor
            for (int i = participant.PurchaseGroup_Requestor.Count - 1; i >= 0; i--) {
                participant.PurchaseGroup_Requestor.Remove(participant.PurchaseGroup_Requestor.ElementAt(i));
            }

            //pg orderer
            for (int i = participant.PurchaseGroup_Orderer.Count - 1; i >= 0; i--) {
                participant.PurchaseGroup_Orderer.Remove(participant.PurchaseGroup_Orderer.ElementAt(i));
            }

            //orderer supplier
            for (int i = participant.Orderer_Supplier.Count - 1; i >= 0; i--) {
                participant.Orderer_Supplier.Remove(participant.Orderer_Supplier.ElementAt(i));
            }

            //asset app man
            for (int i = participant.Asset_Manager_Role.Count - 1; i >= 0; i--) {
                participant.Asset_Manager_Role.Remove(participant.Asset_Manager_Role.ElementAt(i));
            }
        }

        public List<ParticipantsExtended> GetParticipants(List<int> companyIds) {
            var tmpParticipant = GetParticipantsCommon(companyIds);

            List<ParticipantsExtended> participants = new List<ParticipantsExtended>();
            foreach (var part in tmpParticipant) {
                ParticipantsExtended p = new ParticipantsExtended();
                SetValues(part, p, new List<string> { "id", "surname", "first_name", "user_name", "email", "phone", "user_search_key", "active" });
                if (!String.IsNullOrEmpty(p.phone)) {
                    //p.phone = p.phone.Trim();
                    if (p.phone.IndexOf("(+)") == -1) {
                        p.phone = p.phone.Trim().Replace("+", "(+) ");
                    }
                }
                p.office_name = part.Company.country_code;

                participants.Add(p);


            }

            return participants;
        }

        public List<Participants> GetAllParticipants() {
            var participants = (from partDb in m_dbContext.Participants
                                select partDb).ToList();

            return participants;
        }

        private void GetUserSearchKey(
            Participants participant,
            out string firstNameKey,
            out string surnameKey,
            out string nameKey) {

            firstNameKey = "";
            surnameKey = "";
            nameKey = "";

            if (!String.IsNullOrEmpty(participant.surname)) {
                surnameKey = RemoveDiacritics(participant.surname.Replace(" ", "")).Trim().ToLower();
            }

            if (!String.IsNullOrEmpty(participant.first_name)) {
                firstNameKey = RemoveDiacritics(participant.first_name.Replace(" ", "")).Trim().ToLower();
            }

            nameKey = surnameKey + " " + firstNameKey;
        }

        public List<Participants> GetSubsitutedApproversJs(int substitutedId, int currentUserId) {
            var substParticipant = (from partDb in m_dbContext.Participants
                                    where partDb.id == substitutedId
                                    select partDb).FirstOrDefault();

            int companyId = substParticipant.company_id;

            var participants = (from partDb in m_dbContext.Participants
                                join partOfficeRole in m_dbContext.Participant_Office_Role
                                on partDb.id equals partOfficeRole.participant_id
                                where partOfficeRole.office_id == companyId &&
                                partOfficeRole.role_id == (int)UserRole.SubstitutionApproveManager &&
                                partDb.active == true
                                orderby partDb.surname, partDb.first_name
                                select partDb).ToList();

            List<Participants> retParticipants = new List<Participants>();
            foreach (var participant in participants) {
                if (participant.id == currentUserId) {
                    //substituted user is approval
                    return null;
                }
                Participants part = new Participants();
                SetValues(participant, part);
                retParticipants.Add(part);
            }

            return retParticipants;
        }


        public List<ParticipantsExtended> GetParticipantsByFilterPaged(
            List<int> companyIds,
            string filter,
            string sort,
            int pageSize,
            int pageNr,
            UserDisplayType userDisplayType,
            out int rowsCount) {

            string strFilterWhere = GetFilter(filter, userDisplayType);

            string strOrder = GetOrder(sort);

            string sqlPure = "SELECT pd.*, cd.country_code, ROW_NUMBER() OVER(" + strOrder + ") AS RowNum";

            string sqlPureBody = GetPureBody(companyIds);
            sqlPureBody += strFilterWhere;

            //Get Row count
            string selectCount = "SELECT COUNT(*) " + sqlPureBody;
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

            var tmpParticipant = m_dbContext.Database.SqlQuery<Participants>(sql).ToList();

            Hashtable htCompany = new Hashtable();
            List<ParticipantsExtended> participants = new List<ParticipantsExtended>();
            int rowIndex = (pageNr - 1) * pageSize + 1;
            foreach (var part in tmpParticipant) {
                ParticipantsExtended p = new ParticipantsExtended();
                SetValues(part, p, new List<string> { "id", "surname", "first_name", "user_name", "email", "phone", "is_external", "active" });

                p.row_index = rowIndex++;

                if (!String.IsNullOrEmpty(p.phone)) {
                    //p.phone = p.phone.Trim();
                    if (p.phone.IndexOf("(+)") == -1) {
                        p.phone = p.phone.Trim().Replace("+", "(+) ");
                    }
                }

                if (htCompany.ContainsKey(part.company_id)) {
                    p.office_name = htCompany[part.company_id].ToString();
                } else {
                    var company = (from compDb in m_dbContext.Company
                                   where compDb.id == part.company_id
                                   select compDb).FirstOrDefault();
                    p.office_name = company.country_code;
                    htCompany.Add(company.id, company.country_code);
                }
                //p.office_name = part.Company.country_code;

                participants.Add(p);


            }

            return participants;
        }

        public List<ParticipantsExtended> GetParticipantsReport(
            List<int> companyIds,
            string filter,
            string sort,
            UserDisplayType userDisplayType) {

            string strFilterWhere = GetFilter(filter, userDisplayType);

            string strOrder = GetOrder(sort);

            string sqlPure = "SELECT pd.*, cd.country_code, ROW_NUMBER() OVER(" + strOrder + ") AS RowNum";


            string sqlPureBody = GetPureBody(companyIds);
            sqlPureBody += strFilterWhere;
            string sql = sqlPure + sqlPureBody;

            List<Participants> tmpParticipants = m_dbContext.Database.SqlQuery<Participants>(sql).ToList();

            Hashtable htCompany = new Hashtable();
            List<ParticipantsExtended> participants = new List<ParticipantsExtended>();
            foreach (var part in tmpParticipants) {
                ParticipantsExtended p = new ParticipantsExtended();
                SetValues(part, p, new List<string> { "id", "surname", "first_name", "user_name", "email", "phone", "is_external", "active" });

                if (!String.IsNullOrEmpty(p.phone)) {
                    if (p.phone.IndexOf("(+)") == -1) {
                        p.phone = p.phone.Trim().Replace("+", "(+) ");
                    }
                }

                if (htCompany.ContainsKey(part.company_id)) {
                    p.office_name = htCompany[part.company_id].ToString();
                } else {
                    var company = (from compDb in m_dbContext.Company
                                   where compDb.id == part.company_id
                                   select compDb).FirstOrDefault();
                    p.office_name = company.country_code;
                    htCompany.Add(company.id, company.country_code);
                }

                participants.Add(p);
            }

            return participants;
        }

        private string GetFilter(string filter, UserDisplayType userDisplayType) {
            string strFilterWhere = "";
            if (userDisplayType == UserDisplayType.NonActiveUsers) {
                strFilterWhere += " AND " + ParticipantsData.IS_NON_ACTIVE_FIELD + "=1";
            }

            if (!String.IsNullOrEmpty(filter)) {
                string[] filterItems = filter.Split(UrlParamDelimiter.ToCharArray());
                foreach (string filterItem in filterItems) {
                    string[] strItemProp = filterItem.Split(UrlParamValueDelimiter.ToCharArray());
                    strFilterWhere += " AND ";

                    string columnName = strItemProp[0].Trim().ToUpper();
                    if (columnName == ParticipantsData.SURNAME_FIELD.Trim().ToUpper()) {
                        strFilterWhere += "(pd." + ParticipantsData.SURNAME_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'" +
                          " OR pd." + ParticipantsData.SURNAME_SEARCH_KEY_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%')";
                    }
                    if (columnName == ParticipantsData.FIRST_NAME_FIELD.Trim().ToUpper()) {
                        strFilterWhere += "(pd." + ParticipantsData.FIRST_NAME_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'" +
                         " OR pd." + ParticipantsData.FIRST_NAME_SEARCH_KEY_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%')";
                    }
                    if (columnName == ParticipantsData.USER_NAME_FIELD.Trim().ToUpper()) {
                        strFilterWhere += "pd." + ParticipantsData.USER_NAME_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                    }
                    if (columnName == ParticipantsData.EMAIL_FIELD.Trim().ToUpper()) {
                        strFilterWhere += "pd." + ParticipantsData.EMAIL_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                    }
                    if (columnName == ParticipantsData.PHONE_FIELD.Trim().ToUpper()) {
                        strFilterWhere += "pd." + ParticipantsData.PHONE_FIELD + " LIKE '%" + strItemProp[1].Trim() + "%'";
                    }
                    if (columnName == ParticipantsData.IS_EXTERNAL_FIELD.Trim().ToUpper()) {
                        if (strItemProp[1].Trim().ToLower() == "true") {
                            strFilterWhere += "pd." + ParticipantsData.IS_EXTERNAL_FIELD + "=1";
                        } else {
                            strFilterWhere += "(pd." + ParticipantsData.IS_EXTERNAL_FIELD + "=0 " + " OR pd." + ParticipantsData.IS_EXTERNAL_FIELD + " IS NULL)";
                        }
                    }
                    if (columnName == ParticipantsData.ACTIVE_FIELD.Trim().ToUpper()) {
                        if (strItemProp[1].Trim().ToLower() == "true") {
                            strFilterWhere += "pd." + ParticipantsData.ACTIVE_FIELD + "=1";
                        } else {
                            strFilterWhere += "pd." + ParticipantsData.ACTIVE_FIELD + "=0";
                        }
                    }
                    if (columnName == "office_name".Trim().ToUpper()) {
                        int officeId = new CompanyRepository().GetCompanyIdByName(strItemProp[1]);
                        strFilterWhere += "pd." + ParticipantsData.COMPANY_ID_FIELD + "=" + officeId;
                    }
                }
            }

            return strFilterWhere;
        }

        private string GetOrder(string sort) {
            string strOrder = "ORDER BY pd." + ParticipantsData.ID_FIELD;

            if (!String.IsNullOrEmpty(sort)) {
                strOrder = "";
                string[] sortItems = sort.Split(UrlParamDelimiter.ToCharArray());
                foreach (string sortItem in sortItems) {
                    string[] strItemProp = sortItem.Split(UrlParamValueDelimiter.ToCharArray());
                    if (strOrder.Length > 0) {
                        strOrder += ", ";
                    }

                    if (strItemProp[0] == "office_name") {
                        strOrder = "cd." + CompanyData.COUNTRY_CODE_FIELD + " " + strItemProp[1]; ;
                    } else {
                        strOrder += "pd." + strItemProp[0] + " " + strItemProp[1];
                    }
                }

                strOrder = " ORDER BY " + strOrder;
            }

            return strOrder;
        }

        private string GetPureBody(List<int> companyIds) {
            string sqlPureBody = " FROM " + ParticipantsData.TABLE_NAME + " pd" +
                " INNER JOIN " + CompanyData.TABLE_NAME + " cd" +
                " ON pd." + ParticipantsData.COMPANY_ID_FIELD + "=cd." + CompanyData.ID_FIELD +
                " WHERE pd." + ParticipantsData.COMPANY_ID_FIELD + " IN" + GetSqlIn(companyIds) +
                " AND pd." + ParticipantsData.ID_FIELD + ">=0" +
                " AND pd." + ParticipantsData.USER_NAME_FIELD + " NOT LIKE 'NO_DOMAIN%'";

            return sqlPureBody;
        }

        public Company GetParticipantCompanyByUserName(string userName) {
            var part = (from dbPart in m_dbContext.Participants
                        where dbPart.user_name == userName
                        select dbPart).FirstOrDefault();
            if (part == null) {
                return null;
            }

            Company companyRet = new Company();
            SetValues(part.Company, companyRet);

            return companyRet;
        }

        public List<Participants> GetParticipantsReport(
            string filter,
            string sort, List<int> companyIds) {

            var participants = GetParticipantsCommon(companyIds);

            return participants;
        }

        public List<Participants> GetNotActiveParticipantsReport(List<int> companyIds) {
            var participants = GetNonActiveParticipants(companyIds);


            return participants;
        }

        public Participants GetNotCompanyAdmin() {
            string sql = "SELECT pd.* FROM " + ParticipantsData.TABLE_NAME + " pd" +
                " LEFT OUTER JOIN " + ParticipantOfficeRoleData.TABLE_NAME + " pord" +
                " ON pd." + ParticipantsData.ID_FIELD + "=pord." + ParticipantOfficeRoleData.PARTICIPANT_ID_FIELD +
                " AND pord." + ParticipantOfficeRoleData.ROLE_ID_FIELD + "=" + (int)UserRole.OfficeAdministrator +
                " WHERE pord." + ParticipantOfficeRoleData.ROLE_ID_FIELD + " IS NULL" +
                " AND pd." + ParticipantsData.ACTIVE_FIELD + "=1" +
                " AND pd." + ParticipantsData.ID_FIELD + ">= 0";
            var participant = (from pd in m_dbContext.Participants.SqlQuery(sql)
                               select pd).FirstOrDefault();
            return participant;
        }

        private List<Participants> GetParticipantsCommon(List<int> companyIds) {
            var participants = (from pd in m_dbContext.Participants
                                where companyIds.Contains(pd.company_id) &&
                                !pd.user_name.StartsWith("NO_DOMAIN") &&
                                pd.id > -1
                                select pd).ToList();

            return participants;
        }

        private List<Participants> GetNonActiveParticipants(List<int> companyIds) {
            var participants = (from pd in m_dbContext.Participants
                                where companyIds.Contains(pd.company_id) &&
                                pd.is_non_active == true &&
                                !pd.user_name.StartsWith("NO_DOMAIN") &&
                                pd.id > -1
                                select pd).ToList();

            return participants;
        }

        public List<Participants> GetActiveParticipants() {
            var participants = (from pd in m_dbContext.Participants
                                where pd.active == true &&
                                !pd.user_name.StartsWith("NO_DOMAIN") &&
                                pd.id > -1
                                select pd).ToList();

            return participants;
        }

        public List<Participants> GetPhotoActiveParticipants() {
            var participants = (from pd in m_dbContext.Participants
                                where pd.active == true &&
                                !pd.user_name.StartsWith("NO_DOMAIN") &&
                                pd.id > -1
                                orderby pd.id
                                select pd).ToList();

            //var participants = (from pd in m_dbContext.Participants
            //                    join ppd in m_dbContext.ParticipantPhoto
            //                    on pd.id equals ppd.participant_id into jointable
            //                    from pppd in jointable.DefaultIfEmpty()
            //                    where pd.active == true &&
            //                    !pd.user_name.StartsWith("NO_DOMAIN") &&
            //                    pd.id > -1
            //                    orderby pppd.modif_date
            //                    select pd).Take(100).ToList();

            return participants;
        }

        public List<Participants> GetDisabledParticipantsNonActive() {
            var participants = (from pd in m_dbContext.Participants
                                where pd.active == false &&
                                pd.is_non_active == true &&
                                !pd.user_name.StartsWith("NO_DOMAIN") &&
                                pd.id > -1
                                select pd).ToList();

            return participants;
        }


        public List<Company> GetCompanies() {

            var companies = (from cDb in m_dbContext.Company
                             where cDb.active == true
                             orderby cDb.country_code
                             select cDb).ToList();


            return companies;
        }

        //public List<Participants> GetOfficeAdmins() {
        //    List<Participants> oficeAdmins = new List<Participants>();

        //   var pors = (from porDb in m_dbContext.Participant_Office_Role
        //                        where porDb.role_id == (int)UserRole.OfficeAdministrator
        //                        select porDb).ToList();

        //    if (pors != null) {
        //        foreach (var por in pors) {
        //            if (por.Participants.active == true) {
        //                oficeAdmins.Add(por.Participants);
        //            }
        //        }
        //    }

        //    return oficeAdmins;
        //}

        public List<AgDropDown> GetCompaniesByParticipantId(List<int> companyIds) {
            var tmpCompanies = (from cmp in m_dbContext.Company
                                where companyIds.Contains(cmp.id)
                                orderby cmp.country_code
                                select cmp).ToList();

            List<AgDropDown> companies = new List<AgDropDown>();
            foreach (var tmpCompany in tmpCompanies) {
                AgDropDown agDropDown = new AgDropDown();
                agDropDown.label = tmpCompany.country_code;
                agDropDown.value = tmpCompany.country_code;//Convert.ToString(tmpCompany.id);
                //SetValues(tmpCompany, company, new List<string> { "id", "country_code" });

                companies.Add(agDropDown);


            }

            return companies;
        }

        public bool IsAuthorized(int userId, int roleId, int officeId) {

            var roleOffice = (from por in m_dbContext.Participant_Office_Role
                              where por.participant_id == userId &&
                              por.role_id == roleId &&
                              por.office_id == officeId
                              select por).FirstOrDefault();

            if (roleOffice != null) {
                return true;
            }


            return false;

        }


        public ReplaceEntity GetReplaceEntity(int userToBeReplaced) {
            ReplaceEntity replaceEntity = new ReplaceEntity();

            #region AppMan
            //App Men
            var cgs = (from cgDb in m_dbContext.Centre_Group
                       join manRoleDb in m_dbContext.Manager_Role on cgDb.id equals manRoleDb.centre_group_id
                       where manRoleDb.participant_id == userToBeReplaced &&
                       cgDb.active == true
                       select new {
                           id = cgDb.id,
                           name = cgDb.name
                       }).Distinct().ToList();

            if (cgs != null && cgs.Count > 0) {
                replaceEntity.cg_app_men = new List<CentreGroupReplace>();
            }

            foreach (var cg in cgs) {
                CentreGroupReplace tmpCentreGroup = new CentreGroupReplace();
                SetValues(cg, tmpCentreGroup, new List<string> { "name" });
                //            tmpCentreGroup.name = "<a onclick=\"window.open('legal.tos')\" ng-click=\"$event.stopPropagation()\">" +
                //    "Terms of Service" + 
                //"</a>";
                tmpCentreGroup.cg_id = cg.id;
                tmpCentreGroup.replace_user_id = DataNulls.INT_NULL;
                replaceEntity.cg_app_men.Add(tmpCentreGroup);
            }

            if (replaceEntity.cg_app_men != null) {
                replaceEntity.cg_app_men = replaceEntity.cg_app_men.OrderBy(x => x.name).ToList();
            }
            #endregion

            #region Requestor
            //Requestor
            var cgsReq = (from cgDb in m_dbContext.Centre_Group
                          join prcgDb in m_dbContext.ParticipantRole_CentreGroup on cgDb.id equals prcgDb.centre_group_id
                          where prcgDb.participant_id == userToBeReplaced &&
                          prcgDb.role_id == (int)UserRole.Requestor &&
                          cgDb.active == true
                          select new {
                              id = cgDb.id,
                              name = cgDb.name
                          }).Distinct().ToList();

            //if (cgsReq != null && cgsReq.Count > 0) {
            replaceEntity.cg_requestors = new List<CentreGroupReplace>();
            //}

            foreach (var cg in cgsReq) {
                CentreGroupReplace tmpCentreGroup = new CentreGroupReplace();
                SetValues(cg, tmpCentreGroup, new List<string> { "name" });
                tmpCentreGroup.cg_id = cg.id;
                tmpCentreGroup.replace_user_id = DataNulls.INT_NULL;

                //tmpCentreGroup.centres = "xxx";
                //var cgC = (from cgDb in m_dbContext.Centre_Group
                //           where cgDb.id == cg.id
                //           select cgDb).FirstOrDefault();
                //var sortCg = cgC.Centre.OrderBy(x => x.name).ToList();
                //foreach (var centre in sortCg) {
                //    tmpCentreGroup.centres += centre.name;
                //}

                replaceEntity.cg_requestors.Add(tmpCentreGroup);
            }

            Hashtable htPartCentres = new Hashtable();
            var participant = (from partDb in m_dbContext.Participants
                               where partDb.id == userToBeReplaced
                               select partDb).FirstOrDefault();

            if (participant.centre_id != null) {
                htPartCentres.Add(participant.centre_id, null);
            }


            foreach (var reqCentre in participant.Requestor_Centre) {
                var centre = (from centreDb in m_dbContext.Centre
                              where centreDb.id == reqCentre.id
                              select centreDb).FirstOrDefault();

                if (centre.Centre_Group == null || centre.Centre_Group.Count == 0) {
                    continue;
                }

                if (centre.Centre_Group.FirstOrDefault().active != true) {
                    continue;
                }

                int cgId = centre.Centre_Group.FirstOrDefault().id;

                var selCg = (from selCgDb in cgsReq
                             where selCgDb.id == cgId
                             select selCgDb).FirstOrDefault();
                if (selCg == null) {
                    CentreGroupReplace tmpCentreGroup = new CentreGroupReplace();
                    SetValues(centre.Centre_Group.FirstOrDefault(), tmpCentreGroup, new List<string> { "name" });
                    tmpCentreGroup.cg_id = cgId;
                    tmpCentreGroup.replace_user_id = DataNulls.INT_NULL;
                    replaceEntity.cg_requestors.Add(tmpCentreGroup);
                }

                if (!htPartCentres.ContainsKey(centre.id)) {
                    htPartCentres.Add(centre.id, null);
                }
            }

            if (replaceEntity.cg_requestors != null) {
                foreach (var cgr in replaceEntity.cg_requestors) {
                    cgr.centres = "";
                    var cgC = (from cgDb in m_dbContext.Centre_Group
                               where cgDb.id == cgr.cg_id
                               select cgDb).FirstOrDefault();
                    var sortCg = cgC.Centre.OrderBy(x => x.name).ToList();
                    foreach (var centre in sortCg) {
                        if (htPartCentres.ContainsKey(centre.id)) {
                            if (cgr.centres.Length > 0) {
                                cgr.centres += ", ";
                            }
                            cgr.centres += centre.name;
                        }
                    }
                    if (cgr.centres.Length > 0) {
                        cgr.centres = "(" + cgr.centres + ")";
                    }
                }
            }

            if (replaceEntity.cg_requestors != null) {
                replaceEntity.cg_requestors = replaceEntity.cg_requestors.OrderBy(x => x.name).ToList();
            }

            #endregion

            #region Orderer
            //Orderer
            var cgsOrd = (from cgDb in m_dbContext.Centre_Group
                          join prcgDb in m_dbContext.ParticipantRole_CentreGroup on cgDb.id equals prcgDb.centre_group_id
                          where prcgDb.participant_id == userToBeReplaced &&
                          prcgDb.role_id == (int)UserRole.Orderer &&
                          cgDb.active == true
                          select new {
                              id = cgDb.id,
                              name = cgDb.name
                          }).Distinct().ToList();

            var partOrig = (from partDb in m_dbContext.Participants
                            where partDb.id == userToBeReplaced
                            select partDb).FirstOrDefault();
            foreach (var implOrd in partOrig.PurchaseGroup_ImplicitOrderer) {
                if (implOrd.Purchase_Group.Centre_Group != null && implOrd.Purchase_Group.Centre_Group.Count > 0) {
                    Centre_Group cg = implOrd.Purchase_Group.Centre_Group.ElementAt(0);
                    if (cg.active == false) {
                        continue;
                    }

                    var selCg = (from cgDb in cgsOrd
                                 where cgDb.id == cg.id
                                 select cgDb).FirstOrDefault();
                    if (selCg == null) {
                        cgsOrd.Add(new {
                            id = cg.id,
                            name = cg.name
                        });
                    }
                }
            }



            replaceEntity.cg_orderers = new List<CentreGroupReplace>();

            foreach (var cg in cgsOrd) {
                CentreGroupReplace tmpCentreGroup = new CentreGroupReplace();
                SetValues(cg, tmpCentreGroup, new List<string> { "name" });
                tmpCentreGroup.cg_id = cg.id;
                tmpCentreGroup.replace_user_id = DataNulls.INT_NULL;
                replaceEntity.cg_orderers.Add(tmpCentreGroup);
            }

            if (replaceEntity.cg_orderers != null) {
                replaceEntity.cg_orderers = replaceEntity.cg_orderers.OrderBy(x => x.name).ToList();
            }
            #endregion

            #region Centre
            var centres = (from cDb in m_dbContext.Centre
                           where cDb.manager_id == userToBeReplaced
                           select new {
                               id = cDb.id,
                               name = cDb.name
                           }).ToList();

            if (centres != null) {
                replaceEntity.centre_man = new List<CentreGroupReplace>();
                foreach (var cg in centres) {
                    CentreGroupReplace tmpCentreGroup = new CentreGroupReplace();
                    SetValues(cg, tmpCentreGroup, new List<string> { "name" });
                    tmpCentreGroup.cg_id = cg.id;
                    tmpCentreGroup.replace_user_id = DataNulls.INT_NULL;
                    replaceEntity.centre_man.Add(tmpCentreGroup);
                }
            }

            if (replaceEntity.centre_man != null) {
                replaceEntity.centre_man = replaceEntity.centre_man.OrderBy(x => x.name).ToList();
            }
            #endregion

            return replaceEntity;
        }

        public int SaveParticipantData(ParticipantsExtended modifParticipant, int companyGroupId, out List<string> msg, out int errorId) {
            msg = new List<string>();
            errorId = 0;

            HttpResult httpResult = new HttpResult();

            if (modifParticipant.id >= 0) {

                Participants dbParticipant = (from part in m_dbContext.Participants
                                              where part.id == modifParticipant.id
                                              select part).FirstOrDefault();

                if (modifParticipant.active == false && dbParticipant.active == true) {
                    bool isActiveAppMan = false;
                    bool isActiveOrderer = false;

                    GetActiveManagerCg(
                        dbParticipant.id, 
                        out isActiveAppMan,
                        out isActiveOrderer);

                    if (isActiveAppMan) {
                        msg = new List<string> { modifParticipant.first_name + " " + modifParticipant.surname };
                        errorId = USER_CANNOT_BE_DEACTIVATED_APPMAN;
                        return USER_CANNOT_BE_DEACTIVATED_APPMAN;
                    }

                    if (isActiveOrderer) {
                        msg = new List<string> { modifParticipant.first_name + " " + modifParticipant.surname };
                        errorId = USER_CANNOT_BE_DEACTIVATED_ORDERER;
                        return USER_CANNOT_BE_DEACTIVATED_ORDERER;
                    }
                }
            }

            using (TransactionScope transaction = new TransactionScope()) {
                try {
                    var dbParticipant = new Participants();

                    if (modifParticipant.id >= 0) {

                        dbParticipant = (from part in m_dbContext.Participants
                                         where part.id == modifParticipant.id
                                         select part).FirstOrDefault();

                    }



                    dbParticipant.first_name = modifParticipant.first_name;
                    dbParticipant.surname = modifParticipant.surname;

                    string userSearchKey = "";
                    string firstNameSearchKey = "";
                    string surnameSearchKey = "";

                    GetUserSearchKey(
                        dbParticipant,
                        out firstNameSearchKey,
                        out surnameSearchKey,
                        out userSearchKey);

                    dbParticipant.first_name_search_key = firstNameSearchKey;
                    dbParticipant.surname_search_key = surnameSearchKey;
                    dbParticipant.user_search_key = userSearchKey;
                    dbParticipant.email = modifParticipant.email;
                    dbParticipant.phone = modifParticipant.phone;
                    dbParticipant.is_external = modifParticipant.is_external;
                    dbParticipant.active = modifParticipant.active;

                    var companies = (from compDb in m_dbContext.Company
                                     where compDb.country_code == modifParticipant.office_name
                                     select compDb).FirstOrDefault();
                    int compId = companies.id;
                    dbParticipant.company_id = compId;

                    int iLang = companies.id;


                    if (modifParticipant.id < 0) {
                        //New Participant

                        //Participants lastPart = m_dbContext.Participants.LastOrDefault<Participants>();
                        var lastPart = (from part in m_dbContext.Participants
                                        orderby part.id descending
                                        select part).Take(1).FirstOrDefault();

                        int lastId = -1;
                        if (lastPart != null) {
                            lastId = lastPart.id;
                        }

                        //int lastId = lastPart.id;
                        lastId++;
                        dbParticipant.id = lastId;
                        dbParticipant.user_name = modifParticipant.user_name;
                        dbParticipant.company_group_id = companyGroupId;

                        ////set default lang
                        //dbParticipant.country_lang_id = iLang;

                        m_dbContext.Participants.Add(dbParticipant);
                    }

                    SaveChanges();
                    transaction.Complete();

                    return dbParticipant.id;
                } catch (Exception ex) {
                    throw ex;
                }
            }
        }



        public void SaveParticipantData(Participants modifParticipant) {

            using (TransactionScope transaction = new TransactionScope()) {
                try {

                    var dbParticipant = (from part in m_dbContext.Participants
                                         where part.id == modifParticipant.id
                                         select part).FirstOrDefault();

                    string userSearchKey = "";
                    string firstNameSearchKey = "";
                    string surnameSearchKey = "";

                    GetUserSearchKey(
                        dbParticipant,
                        out firstNameSearchKey,
                        out surnameSearchKey,
                        out userSearchKey);

                    dbParticipant.first_name_search_key = firstNameSearchKey;
                    dbParticipant.surname_search_key = surnameSearchKey;
                    dbParticipant.user_search_key = userSearchKey;

                    SaveChanges();
                    transaction.Complete();

                } catch (Exception ex) {
                    throw ex;
                }
            }
        }

        public Participant_Substitute GetParticipantActiveSubstitution(int userId) {
            var partSub = (from partSubDb in m_dbContext.Participant_Substitute
                           where partSubDb.active == true &&
                            partSubDb.substituted_user_id == userId &&
                            partSubDb.substitute_start_date <= DateTime.Now &&
                            partSubDb.substitute_end_date >= DateTime.Now &&
                            (partSubDb.approval_status == (int)ApproveStatus.Approved
                            || partSubDb.approval_status == (int)ApproveStatus.NotNeeded)
                           select partSubDb).FirstOrDefault();

            return partSub;
        }

        public void AddSubstitute(ParticipantsExtended participant) {
            //substitution
            Participant_Substitute partSubst = GetParticipantActiveSubstitution(participant.id);
            if (partSubst != null) {
                participant.substituted_by = partSubst.SubstituteUser.surname + " " + partSubst.SubstituteUser.first_name;
                participant.substituted_until = partSubst.substitute_end_date.ToString("dd.MM.yyyy");

            }
        }

        public void AddSubstitute(OrdererExtended orderer) {
            //substitution
            Participant_Substitute partSubst = GetParticipantActiveSubstitution(orderer.participant_id);
            if (partSubst != null) {
                orderer.substituted_by = partSubst.SubstituteUser.surname + " " + partSubst.SubstituteUser.first_name;
                orderer.substituted_until = partSubst.substitute_end_date.ToString("dd.MM.yyyy");

            }
        }

        public void DeleteEmptyAppManRole(int userId, int centreGroupId) {
            var manRoles = (from manRolesDb in m_dbContext.Manager_Role
                            where manRolesDb.participant_id == userId &&
                            manRolesDb.centre_group_id == centreGroupId
                            select manRolesDb).ToList();
            if (manRoles == null) {
                var prCg = (from prCgDb in m_dbContext.ParticipantRole_CentreGroup
                            where prCgDb.participant_id == userId &&
                            prCgDb.centre_group_id == centreGroupId &&
                            prCgDb.role_id == (int)UserRole.ApprovalManager
                            select prCgDb).FirstOrDefault();
                if (prCg != null) {
                    Console.WriteLine("Deleting empty AppMan role UserId:" + userId + " CgId:" + centreGroupId);
                    //m_dbContext.ParticipantRole_CentreGroup.Remove(prCg);
                    //m_dbContext.SaveChanges();
                }
            }
        }

        public List<Participants> GetAppAdmins() {
            var admins = (from partDb in m_dbContext.Participants
                          where partDb.is_app_admin == true && partDb.active == true
                          orderby partDb.surname, partDb.first_name
                          select partDb).ToList();

            return admins;
        }

        public List<SuppAdminsExtended> GetSupplierAdminJs(int companyId) {
            //check company admins
            var compAdmins = (from partOffDb in m_dbContext.Participant_Office_Role
                              where partOffDb.office_id == companyId &&
                              partOffDb.role_id == (int)UserRole.OfficeAdministrator
                              select partOffDb).ToList();

            var suppAdmins = (from partOffDb in m_dbContext.Participant_Office_Role
                              where partOffDb.office_id == companyId &&
                              partOffDb.role_id == (int)UserRole.SupplierAdmin
                              select partOffDb).ToList();

            Hashtable htAdmin = new Hashtable();
            List<SuppAdminsExtended> parts = new List<SuppAdminsExtended>();

            foreach (var admin in suppAdmins) {
                if (admin.Participants.active != true) {
                    continue;
                }

                SuppAdminsExtended suppAdminsExtended = new SuppAdminsExtended();
                suppAdminsExtended.id = admin.Participants.id;
                suppAdminsExtended.first_name = admin.Participants.first_name;
                suppAdminsExtended.surname = admin.Participants.surname;
                //suppAdminsExtended.is_company_admin = (admin.role_id == (int)UserRole.OfficeAdministrator);
                parts.Add(suppAdminsExtended);

                htAdmin.Add(admin.Participants.id, null);
            }

            foreach (var admin in compAdmins) {
                if (admin.Participants.active != true) {
                    continue;
                }

                if (htAdmin.ContainsKey(admin.Participants.id)) {
                    var compAdmin = (from partsDb in parts
                                     where partsDb.id == admin.Participants.id
                                     select partsDb).FirstOrDefault();
                    compAdmin.is_company_admin = true;
                    continue;
                }

                Participant_Office_Role participant_Office_Role = new Participant_Office_Role();
                participant_Office_Role.participant_id = admin.Participants.id;
                participant_Office_Role.office_id = companyId;
                participant_Office_Role.role_id = (int)UserRole.SupplierAdmin;
                m_dbContext.Participant_Office_Role.Add(participant_Office_Role);
                m_dbContext.SaveChanges();

                SuppAdminsExtended suppAdminsExtended = new SuppAdminsExtended();
                suppAdminsExtended.id = admin.Participants.id;
                suppAdminsExtended.first_name = admin.Participants.first_name;
                suppAdminsExtended.surname = admin.Participants.surname;
                suppAdminsExtended.is_company_admin = true;
                parts.Add(suppAdminsExtended);

                htAdmin.Add(admin.Participants.id, null);
            }

            List<SuppAdminsExtended> partsSort = parts.OrderBy(cr => cr.surname).ThenBy(cr => cr.first_name).ToList();

            return partsSort;
        }

        public List<ParticipantsExtended> GetActiveParticipantsData(string rootUrl, bool isUserNameDisplayed, int companyGroupId) {
            return GetActiveParticipantsData(rootUrl, null, isUserNameDisplayed, companyGroupId);
        }

        public List<ParticipantsExtended> GetActiveParticipantsData(
            string rootUrl,
            List<int> companyAdminIds,
            bool isUserNameDisplayed,
            int companyGroupId) {
            IEnumerable tmpParticipants = null;
            if (companyAdminIds != null && companyAdminIds.Count > 0) {
                tmpParticipants = (from pd in m_dbContext.Participants
                                       //join officeRole in m_dbContext.Participant_Office_Role
                                       //on pd.id equals officeRole.participant_id
                                   where pd.active == true && !(pd.user_name.StartsWith("NO_DOMAIN")) &&
                                   (pd.is_external == null || pd.is_external == false) &&
                                   //officeRole.role_id == (int)UserRole.OfficeAdministrator &&
                                   companyAdminIds.Contains(pd.company_id)
                                   orderby pd.surname, pd.first_name
                                   select new {
                                       id = pd.id,
                                       user_name = pd.user_name,
                                       surname = pd.surname,
                                       first_name = pd.first_name,
                                       company_id = pd.company_id,
                                       user_search_key = pd.user_search_key
                                   }).ToList();
            } else {
                tmpParticipants = (from pd in m_dbContext.Participants
                                   where pd.active == true && !(pd.user_name.StartsWith("NO_DOMAIN")) &&
                                   (pd.is_external == null || pd.is_external == false) &&
                                   pd.Company_Group.id == companyGroupId
                                   orderby pd.surname, pd.first_name
                                   select new {
                                       id = pd.id,
                                       user_name = pd.user_name,
                                       surname = pd.surname,
                                       first_name = pd.first_name,
                                       company_id = pd.company_id,
                                       user_search_key = pd.user_search_key
                                   }).ToList();
            }

            //var tmpParticipants = participants.ToList();


            List<ParticipantsExtended> retParticipants = new List<ParticipantsExtended>();
            foreach (var tmpParticipant in tmpParticipants) {
                ParticipantsExtended participant = new ParticipantsExtended();
                SetValues(tmpParticipant, participant);

                if (isUserNameDisplayed) {
                    participant.first_name += " (" + participant.user_name + ")";
                }

                //set name
                participant.name_surname_first = participant.surname + " " + participant.first_name;
                //country flag
                participant.country_flag = GetCountryFlag(participant.company_id, rootUrl);

                //substitution
                Participant_Substitute partSubst = new UserRepository().GetParticipantActiveSubstitution(participant.id);
                if (partSubst != null) {
                    participant.substituted_by = partSubst.SubstituteUser.surname + " " + partSubst.SubstituteUser.first_name;
                    participant.substituted_until = partSubst.substitute_end_date.ToString("dd.MM.yyyy");

                }

                retParticipants.Add(participant);
            }

            return retParticipants;

        }

        public List<ParticipantsExtended> GetParticipantByName(
            string name,
            string rootUrl,
            List<int> companyAdminIds,
            int companyGroupId) {

            string nameWoDia = null;
            if (name != null && name != "undefined" && name != "null") {
                nameWoDia = RemoveDiacritics(name).ToLower();
            }

            var participants = (from partDb in m_dbContext.Participants
                                where partDb.user_search_key.Contains(name)
                                select partDb).ToList();

            IEnumerable tmpParticipants = null;
            if (companyAdminIds != null && companyAdminIds.Count > 0) {
                tmpParticipants = (from pd in m_dbContext.Participants
                                   where pd.active == true && !(pd.user_name.StartsWith("NO_DOMAIN")) &&
                                   (pd.is_external == null || pd.is_external == false) &&
                                   companyAdminIds.Contains(pd.company_id) &&
                                   (pd.user_search_key.Contains(nameWoDia) || String.IsNullOrEmpty(nameWoDia))
                                   orderby pd.surname, pd.first_name
                                   select new {
                                       id = pd.id,
                                       user_name = pd.user_name,
                                       surname = pd.surname,
                                       first_name = pd.first_name,
                                       company_id = pd.company_id,
                                       user_search_key = pd.user_search_key
                                   }).ToList();
            } else {
                tmpParticipants = (from pd in m_dbContext.Participants
                                   where pd.active == true && !(pd.user_name.StartsWith("NO_DOMAIN")) &&
                                   (pd.is_external == null || pd.is_external == false) &&
                                   pd.Company_Group.id == companyGroupId &&
                                   (pd.user_search_key.Contains(nameWoDia) || String.IsNullOrEmpty(nameWoDia))
                                   orderby pd.surname, pd.first_name
                                   select new {
                                       id = pd.id,
                                       user_name = pd.user_name,
                                       surname = pd.surname,
                                       first_name = pd.first_name,
                                       company_id = pd.company_id,
                                       user_search_key = pd.user_search_key
                                   }).ToList();
            }

            //var tmpParticipants = participants.ToList();


            List<ParticipantsExtended> retParticipants = new List<ParticipantsExtended>();
            foreach (var tmpParticipant in tmpParticipants) {
                ParticipantsExtended participant = new ParticipantsExtended();
                SetValues(tmpParticipant, participant);

                //if (isUserNameDisplayed) {
                //    participant.first_name += " (" + participant.user_name + ")";
                //}

                //set name
                participant.name_surname_first = participant.surname + " " + participant.first_name;
                //country flag
                participant.country_flag = GetCountryFlag(participant.company_id, rootUrl);

                //substitution
                Participant_Substitute partSubst = new UserRepository().GetParticipantActiveSubstitution(participant.id);
                if (partSubst != null) {
                    participant.substituted_by = partSubst.SubstituteUser.surname + " " + partSubst.SubstituteUser.first_name;
                    participant.substituted_until = partSubst.substitute_end_date.ToString("dd.MM.yyyy");

                }

                retParticipants.Add(participant);
            }

            return retParticipants;


        }

        public List<Participants> GetUserByDefaultLangIdNoUserSettings(int companyId) {
            string sql = "SELECT pd.* FROM " + ParticipantsData.TABLE_NAME + " pd" +
                " LEFT OUTER JOIN " + UserSettingData.TABLE_NAME + " usd" +
                " ON pd." + ParticipantsData.ID_FIELD + "=usd." + UserSettingData.USER_ID_FIELD +
                " WHERE usd." + UserSettingData.USER_ID_FIELD + " IS NULL" +
                " AND pd." + ParticipantsData.ACTIVE_FIELD + "=1" +
                " AND pd." + ParticipantsData.ID_FIELD + ">= 0" +
                " AND pd." + ParticipantsData.COMPANY_ID_FIELD + "=" + companyId;

            var tmpParticipant = m_dbContext.Participants.SqlQuery(sql).ToList();

            return tmpParticipant;
        }
        #endregion

        #region Integration Test
        /// <summary>
        /// This Methods works for Integration Test
        /// </summary>
        public void SwitchUserToDefaultTestUser(int id, string userName) {
            ClearDefaultTestUser(userName);

            using (TransactionScope transaction = new TransactionScope()) {

                if (id != 0) {
                    var participant = GetFirstOrDefault(p => p.id == id);
                    var participantSyka = GetFirstOrDefault(p => p.id == 0);


                    participantSyka.user_name = "_" + userName;
                    participant.user_name = userName;

                    m_dbContext.Entry(participantSyka).State = EntityState.Modified;
                    SaveChanges();
                    m_dbContext.Entry(participant).State = EntityState.Modified;

                    SaveChanges();
                }
                transaction.Complete();

            }
        }

        public void RevertDefaultTestUser(string defaultUserName, string origUserName) {
            using (TransactionScope transaction = new TransactionScope()) {

                var participant = GetFirstOrDefault(p => p.user_name.ToLower() == defaultUserName);

                if (participant != null && participant.id != 0) {
                    participant.user_name = origUserName;
                    m_dbContext.Entry(participant).State = EntityState.Modified;
                    SaveChanges();
                }

                var participantSyka = GetFirstOrDefault(p => p.id == 0);
                participantSyka.user_name = defaultUserName;
                m_dbContext.Entry(participantSyka).State = EntityState.Modified;
                SaveChanges();
                transaction.Complete();
            }
        }

        private void ClearDefaultTestUser(string userName) {
            using (TransactionScope transaction = new TransactionScope()) {

                var participant = GetFirstOrDefault(p => p.user_name.ToLower() == userName);

                if (participant != null && participant.id != 0) {
                    participant.user_name = GetUserName(participant.surname);
                    m_dbContext.Entry(participant).State = EntityState.Modified;
                    SaveChanges();
                }

                var participantSyka = GetFirstOrDefault(p => p.id == 0);
                participantSyka.user_name = userName;
                m_dbContext.Entry(participantSyka).State = EntityState.Modified;
                SaveChanges();
                transaction.Complete();
            }
        }

        public void ChangeUserName(int userId, string userName) {
            using (TransactionScope transaction = new TransactionScope()) {

                var participant = GetFirstOrDefault(p => p.id == userId);

                if (participant != null) {
                    participant.user_name = userName;
                    m_dbContext.Entry(participant).State = EntityState.Modified;
                    SaveChanges();
                }

                transaction.Complete();
            }

        }

        private string GetUserName(string surname) {
            return ("_" + surname + DateTime.Now);
        }

        public void DeleteUserSettings(int userId) {
            using (TransactionScope transaction = new TransactionScope()) {
                var participant = GetFirstOrDefault(p => p.id == userId);
                if (participant.User_Setting != null) {
                    m_dbContext.User_Setting.Remove(participant.User_Setting);
                    m_dbContext.Entry(participant).State = EntityState.Modified;
                    SaveChanges();
                }

                transaction.Complete();
            }
        }

        private void AddMissingRoles(int participantId) {
            using (TransactionScope transaction = new TransactionScope()) {
                var manRoles = (from manRoleDb in m_dbContext.Manager_Role
                                where manRoleDb.participant_id == participantId &&
                                manRoleDb.active == true
                                select manRoleDb).ToList();

                if (manRoles != null && manRoles.Count > 0) {
                    Hashtable htCheckedCg = new Hashtable();
                    foreach (var manRole in manRoles) {
                        int cgId = manRole.centre_group_id;
                        if (htCheckedCg.ContainsKey(cgId)) {
                            continue;
                        }

                        var prCg = (from prCgDb in m_dbContext.ParticipantRole_CentreGroup
                                    where prCgDb.participant_id == participantId &&
                                    prCgDb.centre_group_id == cgId &&
                                    prCgDb.role_id == (int)UserRole.ApprovalManager
                                    select prCgDb).FirstOrDefault();

                        if (prCg == null) {
                            ParticipantRole_CentreGroup newPrCg = new ParticipantRole_CentreGroup();
                            newPrCg.centre_group_id = cgId;
                            newPrCg.participant_id = participantId;
                            newPrCg.role_id = (int)UserRole.ApprovalManager;

                            m_dbContext.ParticipantRole_CentreGroup.Add(newPrCg);
                            SaveChanges();
                        }

                        htCheckedCg.Add(cgId, null);
                    }
                }

                transaction.Complete();
            }
        }

        public void GetActiveManagerCg(
            int participantId,
            out bool isActiveAppMan,
            out bool isActiveOrderer) {

            isActiveAppMan = false;
            isActiveOrderer = false;

            AddMissingRoles(participantId);

            using (TransactionScope transaction = new TransactionScope()) {
                //remove from deactivated cg, commodities
                RemoveUserFormDeactivatedCgPg(participantId);

                //List<int> usedCgId = new List<int>();
                //List<string> usedCgNames = new List<string>();

                var prcg = (from prcgDb in m_dbContext.ParticipantRole_CentreGroup
                            join cgDb in m_dbContext.Centre_Group on prcgDb.centre_group_id equals cgDb.id
                            where prcgDb.participant_id == participantId
                            && (prcgDb.role_id == (int)UserRole.ApprovalManager ||
                            prcgDb.role_id == (int)UserRole.Orderer ||
                            prcgDb.role_id == (int)UserRole.CentreGroupPropAdmin ||
                            prcgDb.role_id == (int)UserRole.ApproveMatrixAdmin ||
                            prcgDb.role_id == (int)UserRole.OrdererAdmin ||
                            prcgDb.role_id == (int)UserRole.RequestorAdmin) &&
                            cgDb.active == true
                            select prcgDb).ToList();



                foreach (var role in prcg) {
                    int cgId = role.centre_group_id;
                    //string cgName = null;

                    Centre_Group cg = role.Centre_Group;

                    //if (role.Centre_Group != null) {
                    //    cgName = role.Centre_Group.name;
                    //} else {
                    //    cg = (from cgDb in m_dbContext.Centre_Group
                    //          where cgDb.id == cgId
                    //          select cgDb).FirstOrDefault();
                    //    cgName = cg.name;
                    //}

                    #region App Manager
                    if (role.role_id == (int)UserRole.ApprovalManager) {

                        //if (usedCgId.Contains(cgId)) {
                        //    continue;
                        //}
                        var manRoles = (from manRoleDb in m_dbContext.Manager_Role
                                        where manRoleDb.participant_id == participantId
                                        && manRoleDb.centre_group_id == cgId
                                        && manRoleDb.active == true
                                        select manRoleDb).ToList();

                        //bool isOtherAppMan = false;
                        foreach (var manRole in manRoles) {
                            var pgLimitOtherMan = (from manRoleDb in m_dbContext.Manager_Role
                                              where manRoleDb.purchase_group_limit_id == manRole.purchase_group_limit_id
                                              && manRoleDb.participant_id != participantId
                                              && manRoleDb.centre_group_id == cgId
                                              && manRoleDb.active == true
                                              select manRoleDb).FirstOrDefault();

                            if (pgLimitOtherMan != null) {
                                m_dbContext.Manager_Role.Remove(manRole);
                            } else { 
                                isActiveAppMan = true;
                                return;
                                
                            }
                        }

                       
                    }
                    #endregion

                    #region Orderer

                    if (role.role_id == (int)UserRole.Orderer) {
                        var part = (from partDb in m_dbContext.Participants
                                    where partDb.id == participantId
                                    select partDb).FirstOrDefault();
                        bool isOtherOrderer = false;
                        foreach (var pgORderer in part.PurchaseGroup_Orderer) {
                            if (pgORderer.PurchaseGroup_Orderer.Count > 1) {
                                isOtherOrderer = true;
                                break;
                            }
                        }

                        if (!isOtherOrderer) {
                            isActiveOrderer = true;
                            return;
                            //if (!usedCgId.Contains(cgId)) {
                            //    usedCgId.Add(cgId);
                            //}

                            //if (!usedCgNames.Contains(cgName)) {
                            //    usedCgNames.Add(cgName);
                            //}
                        }
                    }
                    #endregion

                    #region Cg Admin
                    //if (role.role_id == (int)UserRole.CentreGroupPropAdmin ||
                    //    role.role_id == (int)UserRole.ApproveMatrixAdmin ||
                    //    role.role_id == (int)UserRole.OrdererAdmin ||
                    //    role.role_id == (int)UserRole.RequestorAdmin) {
                    //    var prCg = (from prCgDb in m_dbContext.ParticipantRole_CentreGroup
                    //                join partDb in m_dbContext.Participants
                    //                on prCgDb.participant_id equals partDb.id
                    //                where prCgDb.centre_group_id == cgId
                    //                && prCgDb.participant_id != participantId
                    //                && (prCgDb.role_id == (int)UserRole.CentreGroupPropAdmin &&
                    //                prCgDb.role_id == (int)UserRole.ApproveMatrixAdmin &&
                    //                prCgDb.role_id == (int)UserRole.OrdererAdmin &&
                    //                prCgDb.role_id == (int)UserRole.RequestorAdmin)
                    //                && partDb.active == true
                    //                select prCgDb).FirstOrDefault();

                    //    if (prCg == null) {
                    //        if (!usedCgId.Contains(cgId)) {
                    //            usedCgId.Add(cgId);
                    //        }

                    //        if (!usedCgNames.Contains(cgName)) {
                    //            usedCgNames.Add(cgName);
                    //        }
                    //    }
                    //}
                    #endregion
                }

                #region Country Manager
                ////Country Manager
                //var countryMen = (from cmDb in m_dbContext.Participant_Office_Role
                //                  where cmDb.participant_id == participantId
                //                  select cmDb).ToList();
                //foreach (var countryMan in countryMen) {
                //    var countryOtherMen = (from cmDb in m_dbContext.Participant_Office_Role
                //                           join partDb in m_dbContext.Participants
                //                           on cmDb.participant_id equals partDb.id
                //                           where cmDb.participant_id != participantId &&
                //                           partDb.active == true
                //                           select cmDb).FirstOrDefault();

                //    if (countryOtherMen == null) {
                //        if (!usedCgNames.Contains(countryMan.Company.country_code)) {
                //            usedCgNames.Add(countryMan.Company.country_code);
                //        }
                //    }
                //}
                #endregion

                #region Centre Manager
                //var centres = (from centreDb in m_dbContext.Centre
                //               join partDb in m_dbContext.Participants
                //               on centreDb.manager_id equals partDb.id
                //               where centreDb.manager_id == participantId &&
                //               partDb.active == true
                //               select centreDb).ToList();
                //if (centres != null && centres.Count > 0) {
                //    foreach (var centre in centres) {
                //        if (!usedCgNames.Contains(centre.name)) {
                //            usedCgNames.Add(centre.name);
                //        }
                //    }

                //}
                #endregion

                if (!isActiveAppMan && !isActiveOrderer) {
                    SaveChanges();
                    transaction.Complete();
                } else {
                    transaction.Dispose();
                }


                //return usedCgNames;
            }
        }

        private void RemoveUserFormDeactivatedCgPg(int userId) {
            #region App Man
            //Approval Manager
            var manRoles = (from manRolesDb in m_dbContext.Manager_Role
                            where manRolesDb.participant_id == userId
                            select manRolesDb).ToList();

            foreach (var manRole in manRoles) {
                if (manRole.Purchase_Group_Limit.Purchase_Group.active == true) {
                    if (manRole.Centre_Group.active == false) {
                        new PgRepository().DeactivatePg(m_dbContext, manRole.Purchase_Group_Limit.Purchase_Group.id);
                        //manRole.Purchase_Group_Limit.Purchase_Group.active = false;
                        m_dbContext.Manager_Role.Remove(manRole);
                    } else {
                        return;
                    }
                } else {
                    m_dbContext.Manager_Role.Remove(manRole);
                }
            }

            var appManPrCgs = new PgCgRepository().GetPgCgs(m_dbContext, userId, UserRole.ApprovalManager);
            if (appManPrCgs != null && appManPrCgs.Count > 0) {
                for (int i=appManPrCgs.Count - 1; i>=0; i--) {
                    m_dbContext.ParticipantRole_CentreGroup.Remove(appManPrCgs.ElementAt(i));
                }
            }
            #endregion

            #region Orderer
            var part = new UserRepository().GetParticipantById(userId);
            foreach (var pg in part.PurchaseGroup_Orderer) {
                if (pg.active == true) {
                    if (pg.Centre_Group.ElementAt(0).active == false) {
                        new PgRepository().DeactivatePg(m_dbContext, pg.id);
                        part.PurchaseGroup_Orderer.Remove(pg);
                    } else {
                        return;
                    }
                } else {
                    part.PurchaseGroup_Orderer.Remove(pg);
                }
            }

            var ordererPrCgs = new PgCgRepository().GetPgCgs(m_dbContext, userId, UserRole.Orderer);
            if (ordererPrCgs != null && ordererPrCgs.Count > 0) {
                for (int i = ordererPrCgs.Count - 1; i >= 0; i--) {
                    m_dbContext.ParticipantRole_CentreGroup.Remove(appManPrCgs.ElementAt(i));
                }
            }
            #endregion

            SaveChanges();
        }

        public void ReplaceAppMan(List<CentreGroupReplace> cge, int modifUserId) {

            if (cge == null) {
                return;
            }


            //App Man Role
            //bool isReplaced = false;
            foreach (var appManRepl in cge) {
                //var replPart = (from partDB in m_dbContext.Participants
                //            where partDB.id == appManRepl.replace_user_id
                //            select partDB).FirstOrDefault();

                if (appManRepl.is_selected && appManRepl.replace_user_id >= 0) {
                    using (TransactionScope transaction = new TransactionScope()) {
                        //App Man roles
                        var appManRoles = (from manRoleDb in m_dbContext.Manager_Role
                                           where manRoleDb.participant_id == appManRepl.orig_user_id &&
                                           manRoleDb.centre_group_id == appManRepl.cg_id
                                           select manRoleDb).ToList();

                        if (appManRoles != null && appManRoles.Count > 0) {
                            int origUserId = DataNulls.INT_NULL;
                            int replaceUserId = DataNulls.INT_NULL;
                            for (int i = appManRoles.Count - 1; i >= 0; i--) {
                                int limitId = appManRoles.ElementAt(i).purchase_group_limit_id;
                                origUserId = appManRepl.orig_user_id;
                                replaceUserId = appManRepl.replace_user_id;
                                var appManOrigRoles = (from manRoleDb in m_dbContext.Manager_Role
                                                       where manRoleDb.participant_id == appManRepl.replace_user_id &&
                                                       manRoleDb.centre_group_id == appManRepl.cg_id &&
                                                       manRoleDb.purchase_group_limit_id == limitId
                                                       select manRoleDb).FirstOrDefault();
                                if (appManOrigRoles == null) {
                                    Manager_Role mrd = new Manager_Role();
                                    SetValues(appManRoles.ElementAt(i), mrd);
                                    mrd.participant_id = replaceUserId;
                                    mrd.modify_date = DateTime.Now;
                                    mrd.modify_user = modifUserId;
                                    m_dbContext.Manager_Role.Add(mrd);
                                } else {
                                    appManOrigRoles.active = true;

                                }

                                //appManRoles.RemoveAt(i);
                                // m_dbContext.Entry(appManRoles[i]).State = EntityState.Deleted;
                                m_dbContext.Manager_Role.Remove(appManRoles[i]);
                                //m_dbContext.Entry(appManRoles[i]).State = EntityState.Deleted;
                            }

                            var partRoleOrig = (from partRoleDb in m_dbContext.ParticipantRole_CentreGroup
                                                where partRoleDb.participant_id == origUserId &&
                                                partRoleDb.role_id == (int)UserRole.ApprovalManager
                                                select partRoleDb).FirstOrDefault();

                            var partRoleReplace = (from partRoleDb in m_dbContext.ParticipantRole_CentreGroup
                                                   where partRoleDb.participant_id == replaceUserId &&
                                                   partRoleDb.role_id == (int)UserRole.ApprovalManager
                                                   select partRoleDb).FirstOrDefault();
                            if (partRoleReplace == null) {
                                ParticipantRole_CentreGroup prcg = new ParticipantRole_CentreGroup();
                                SetValues(partRoleOrig, prcg);
                                prcg.participant_id = replaceUserId;

                                m_dbContext.ParticipantRole_CentreGroup.Add(prcg);
                            }
                            if (partRoleOrig != null) {
                                m_dbContext.ParticipantRole_CentreGroup.Remove(partRoleOrig);
                                //m_dbContext.Entry(partRoleOrig).State = EntityState.Deleted;
                            }

                        }
                        //isReplaced = true;

                        SaveChanges();

                        transaction.Complete();
                    }
                }
            }


        }

        public void ReplaceRequestor(List<CentreGroupReplace> cge) {

            if (cge == null) {
                return;
            }


            foreach (var requestorRepl in cge) {
                if (requestorRepl.is_selected && requestorRepl.replace_user_id >= 0) {
                    using (TransactionScope transaction = new TransactionScope()) {
                        int origUserId = requestorRepl.orig_user_id;
                        int replaceUserId = requestorRepl.replace_user_id;

                        #region Participant Role Centre Group
                        //Participant Role Centre Group
                        var partRolesReplace = (from partRoleDb in m_dbContext.ParticipantRole_CentreGroup
                                                where partRoleDb.participant_id == requestorRepl.orig_user_id &&
                                                partRoleDb.centre_group_id == requestorRepl.cg_id &&
                                                partRoleDb.role_id == (int)UserRole.Requestor
                                                select partRoleDb).ToList();

                        for (int i = partRolesReplace.Count - 1; i >= 0; i--) {
                            // int cgId = partRolesReplace[i].centre_group_id;
                            var partRolesReplacer = (from partRoleDb in m_dbContext.ParticipantRole_CentreGroup
                                                     where partRoleDb.participant_id == replaceUserId &&
                                                     partRoleDb.centre_group_id == requestorRepl.cg_id &&
                                                     partRoleDb.role_id == (int)UserRole.Requestor
                                                     select partRoleDb).FirstOrDefault();

                            if (partRolesReplacer == null) {
                                ParticipantRole_CentreGroup prcg = new ParticipantRole_CentreGroup();
                                SetValues(partRolesReplace[i], prcg);
                                prcg.participant_id = requestorRepl.replace_user_id;
                                m_dbContext.ParticipantRole_CentreGroup.Add(prcg);
                            }

                            m_dbContext.ParticipantRole_CentreGroup.Remove(partRolesReplace[i]);
                        }
                        #endregion

                        #region Requestor Centre
                        //Requestor Centre
                        var partOrig = (from partDb in m_dbContext.Participants
                                        where partDb.id == requestorRepl.orig_user_id
                                        select partDb).FirstOrDefault();

                        var partReplacer = (from partDb in m_dbContext.Participants
                                            where partDb.id == requestorRepl.replace_user_id
                                            select partDb).FirstOrDefault();

                        int partCentreId = DataNulls.INT_NULL;
                        if (partReplacer.centre_id == null && partOrig.centre_id != null) {
                            partReplacer.centre_id = partOrig.centre_id;
                            partCentreId = (int)partOrig.centre_id;
                            partOrig.centre_id = null;

                        }

                        for (int i = partOrig.Requestor_Centre.Count - 1; i >= 0; i--) {
                            int centreId = partOrig.Requestor_Centre.ElementAt(i).id;
                            Centre centr = (from centreDB in m_dbContext.Centre
                                            where centreDB.id == centreId
                                            select centreDB).FirstOrDefault();
                            if (centr.Centre_Group == null ||
                                centr.Centre_Group.Count == 0 ||
                                centr.Centre_Group.ElementAt(0).id != requestorRepl.cg_id) {
                                continue;
                            }

                            if (centreId != partCentreId) {
                                var replReqCentre = (from replReqCentreDb in partReplacer.Requestor_Centre
                                                     where replReqCentreDb.id == centreId
                                                     select replReqCentreDb).FirstOrDefault();
                                if (replReqCentre == null) {
                                    partReplacer.Requestor_Centre.Add(centr);
                                }
                            }

                            partOrig.Requestor_Centre.Remove(centr);
                        }

                        //set orig requestor centre
                        if (partOrig.centre_id == null && partOrig.Requestor_Centre != null) {
                            var centre = partOrig.Requestor_Centre.FirstOrDefault();
                            if (centre != null) {
                                partOrig.centre_id = centre.id;
                                partOrig.Requestor_Centre.Remove(centre);
                            }
                        }
                        #endregion

                        #region Purchase Group Requestor
                        //Purchase Group Requestor
                        var pgrs = (from pgrDb in m_dbContext.PurchaseGroup_Requestor
                                    where pgrDb.requestor_id == requestorRepl.orig_user_id
                                    select pgrDb).ToList();


                        for (int i = pgrs.Count - 1; i >= 0; i--) {
                            int centreId = pgrs[i].centre_id;

                            if (pgrs[i].Centre.Centre_Group == null || pgrs[i].Centre.Centre_Group.Count == 0) {
                                continue;
                            }

                            if (requestorRepl.cg_id != pgrs[i].Centre.Centre_Group.ElementAt(0).id) {
                                continue;
                            }

                            int pgId = pgrs[i].purchase_group_id;
                            var pgrReplacer = (from pgrDb in m_dbContext.PurchaseGroup_Requestor
                                               where pgrDb.requestor_id == requestorRepl.replace_user_id &&
                                               pgrDb.centre_id == centreId &&
                                               pgrDb.purchase_group_id == pgId
                                               select pgrDb).FirstOrDefault();
                            if (pgrReplacer == null) {
                                PurchaseGroup_Requestor pgr = new PurchaseGroup_Requestor();
                                SetValues(pgrs[i], pgr);
                                pgr.requestor_id = requestorRepl.replace_user_id;
                                m_dbContext.PurchaseGroup_Requestor.Add(pgr);
                            }

                            if (partReplacer.centre_id != centreId) {
                                var partCentre = (from pcDb in partReplacer.Requestor_Centre
                                                  where pcDb.id == centreId
                                                  select pcDb).FirstOrDefault();
                                if (partCentre == null) {
                                    Centre centr = (from centreDB in m_dbContext.Centre
                                                    where centreDB.id == centreId
                                                    select centreDB).FirstOrDefault();
                                    partReplacer.Requestor_Centre.Add(centr);
                                }
                            }

                            partOrig.PurchaseGroup_Requestor.Remove(pgrs[i]);
                        }
                        #endregion

                        #region Implicite Requestor
                        var implReq = (from impReqDb in m_dbContext.PurchaseGroup_ImplicitRequestor
                                       where impReqDb.requestor_id == requestorRepl.orig_user_id
                                       select impReqDb).ToList();

                        for (int i = implReq.Count - 1; i >= 0; i--) {
                            if (implReq[i].Purchase_Group.Centre_Group == null ||
                                implReq[i].Purchase_Group.Centre_Group.Count == 0 ||
                                implReq[i].Purchase_Group.Centre_Group.ElementAt(0).id != requestorRepl.cg_id) {
                                continue;
                            }

                            int pgId = implReq[i].purchase_category_id;

                            var implReqReplacer = (from impReqDb in m_dbContext.PurchaseGroup_ImplicitRequestor
                                                   where impReqDb.requestor_id == requestorRepl.replace_user_id &&
                                               impReqDb.purchase_category_id == pgId
                                                   select impReqDb).FirstOrDefault();

                            if (implReqReplacer == null) {
                                PurchaseGroup_ImplicitRequestor pgir = new PurchaseGroup_ImplicitRequestor();
                                SetValues(implReq[i], pgir);
                                pgir.requestor_id = requestorRepl.replace_user_id;
                                m_dbContext.PurchaseGroup_ImplicitRequestor.Add(pgir);

                                var exclReqReplacer = (from excReqDb in implReq[i].Purchase_Group.ParticipantsExcludeRequestor
                                                       where excReqDb.id == requestorRepl.replace_user_id
                                                       select excReqDb).FirstOrDefault();
                                if (exclReqReplacer != null) {
                                    implReq[i].Purchase_Group.ParticipantsExcludeRequestor.Remove(exclReqReplacer);
                                }


                            }


                            partOrig.PurchaseGroup_ImplicitRequestor.Remove(implReq[i]);
                        }
                        #endregion

                        SaveChanges();

                        transaction.Complete();
                    }
                }
            }


        }

        public void ReplaceOrderer(List<CentreGroupReplace> cge) {

            if (cge == null) {
                return;
            }

            foreach (var requestorRepl in cge) {
                if (requestorRepl.is_selected && requestorRepl.replace_user_id >= 0) {
                    using (TransactionScope transaction = new TransactionScope()) {
                        int origUserId = requestorRepl.orig_user_id;
                        int replaceUserId = requestorRepl.replace_user_id;

                        #region Participant Role Centre Group
                        //Participant Role Centre Group
                        var partRolesReplace = (from partRoleDb in m_dbContext.ParticipantRole_CentreGroup
                                                where partRoleDb.participant_id == requestorRepl.orig_user_id &&
                                                partRoleDb.centre_group_id == requestorRepl.cg_id &&
                                                partRoleDb.role_id == (int)UserRole.Orderer
                                                select partRoleDb).ToList();

                        for (int i = partRolesReplace.Count - 1; i >= 0; i--) {
                            // int cgId = partRolesReplace[i].centre_group_id;
                            var partRolesReplacer = (from partRoleDb in m_dbContext.ParticipantRole_CentreGroup
                                                     where partRoleDb.participant_id == replaceUserId &&
                                                     partRoleDb.centre_group_id == requestorRepl.cg_id &&
                                                     partRoleDb.role_id == (int)UserRole.Orderer
                                                     select partRoleDb).FirstOrDefault();

                            if (partRolesReplacer == null) {
                                ParticipantRole_CentreGroup prcg = new ParticipantRole_CentreGroup();
                                SetValues(partRolesReplace[i], prcg);
                                prcg.participant_id = requestorRepl.replace_user_id;
                                m_dbContext.ParticipantRole_CentreGroup.Add(prcg);
                            }

                            m_dbContext.ParticipantRole_CentreGroup.Remove(partRolesReplace[i]);
                        }
                        #endregion

                        #region Purchase Group Orderer
                        //Purchase Group Orderer
                        var partOrig = (from partDb in m_dbContext.Participants
                                        where partDb.id == requestorRepl.orig_user_id
                                        select partDb).FirstOrDefault();

                        var partReplacer = (from partDb in m_dbContext.Participants
                                            where partDb.id == requestorRepl.replace_user_id
                                            select partDb).FirstOrDefault();

                        for (int i = partOrig.PurchaseGroup_Orderer.Count - 1; i >= 0; i--) {
                            int pgId = partOrig.PurchaseGroup_Orderer.ElementAt(i).id;
                            Purchase_Group purchaseGroup = (from purchaseGroupDb in m_dbContext.Purchase_Group
                                                            where purchaseGroupDb.id == pgId
                                                            select purchaseGroupDb).FirstOrDefault();

                            if (purchaseGroup.Centre_Group == null ||
                                purchaseGroup.Centre_Group.Count == 0 ||
                                purchaseGroup.Centre_Group.ElementAt(0).id != requestorRepl.cg_id) {
                                continue;
                            }
                            var replPurchaseGroup = (from replPurchaseGroupDb in partReplacer.PurchaseGroup_Orderer
                                                     where replPurchaseGroupDb.id == pgId
                                                     select replPurchaseGroupDb).FirstOrDefault();
                            if (replPurchaseGroup == null) {
                                partReplacer.PurchaseGroup_Orderer.Add(purchaseGroup);
                            }

                            partOrig.PurchaseGroup_Orderer.Remove(purchaseGroup);
                        }

                        #endregion

                        #region Implicite Orderer
                        var implOrd = (from impReqDb in m_dbContext.PurchaseGroup_ImplicitOrderer
                                       where impReqDb.orderer_id == requestorRepl.orig_user_id
                                       select impReqDb).ToList();

                        for (int i = implOrd.Count - 1; i >= 0; i--) {
                            if (implOrd[i].Purchase_Group.Centre_Group == null ||
                                implOrd[i].Purchase_Group.Centre_Group.Count == 0 ||
                                implOrd[i].Purchase_Group.Centre_Group.ElementAt(0).id != requestorRepl.cg_id) {
                                continue;
                            }

                            int pgId = implOrd[i].purchase_category_id;

                            var implOrdReplacer = (from impReqDb in m_dbContext.PurchaseGroup_ImplicitRequestor
                                                   where impReqDb.requestor_id == requestorRepl.replace_user_id &&
                                                    impReqDb.purchase_category_id == pgId
                                                   select impReqDb).FirstOrDefault();

                            if (implOrdReplacer == null) {
                                PurchaseGroup_ImplicitOrderer pgio = new PurchaseGroup_ImplicitOrderer();
                                SetValues(implOrd[i], pgio);
                                pgio.orderer_id = requestorRepl.replace_user_id;
                                m_dbContext.PurchaseGroup_ImplicitOrderer.Add(pgio);

                                var exclReqReplacer = (from excReqDb in implOrd[i].Purchase_Group.ParticipantsExcludeRequestor
                                                       where excReqDb.id == requestorRepl.replace_user_id
                                                       select excReqDb).FirstOrDefault();
                                if (exclReqReplacer != null) {
                                    implOrd[i].Purchase_Group.ParticipantsExcludeRequestor.Remove(exclReqReplacer);
                                }


                            }


                            partOrig.PurchaseGroup_ImplicitOrderer.Remove(implOrd[i]);
                        }
                        #endregion

                        #region Orderer Supplier
                        var ordSupps = (from ordSuppDb in m_dbContext.Orderer_Supplier
                                        where ordSuppDb.orderer_id == requestorRepl.orig_user_id
                                        select ordSuppDb).ToList();

                        for (int i = ordSupps.Count - 1; i >= 0; i--) {
                            Orderer_Supplier newOrdSupp = new Orderer_Supplier();
                            newOrdSupp.centre_group_id = ordSupps.ElementAt(i).centre_group_id;
                            newOrdSupp.orderer_allowed_for_other_group = ordSupps.ElementAt(i).orderer_allowed_for_other_group;
                            newOrdSupp.supplier_id = ordSupps.ElementAt(i).supplier_id;
                            newOrdSupp.orderer_id = replaceUserId;
                            m_dbContext.Orderer_Supplier.Add(newOrdSupp);

                            m_dbContext.Orderer_Supplier.Remove(ordSupps.ElementAt(i));
                        }
                        #endregion

                        SaveChanges();

                        transaction.Complete();
                    }
                }
            }
        }

        public void ReplaceCentreMan(List<CentreGroupReplace> cge) {

            if (cge == null) {
                return;
            }

            foreach (var centreManRepl in cge) {
                if (centreManRepl.is_selected && centreManRepl.replace_user_id >= 0) {
                    using (TransactionScope transaction = new TransactionScope()) {
                        int origUserId = centreManRepl.orig_user_id;
                        int replaceUserId = centreManRepl.replace_user_id;

                        #region Centre Manager
                        var centreReplace = (from centreDb in m_dbContext.Centre
                                             where centreDb.manager_id == centreManRepl.orig_user_id &&
                                             centreDb.id == centreManRepl.cg_id
                                             select centreDb).FirstOrDefault();

                        if (centreReplace != null) {
                            centreReplace.manager_id = centreManRepl.replace_user_id;
                        }

                        #endregion

                        SaveChanges();

                        transaction.Complete();
                    }
                }
            }
        }

        public List<Participants> GetActiveParticipantsByCgRole(UserRole role) {
           
            var participants = (from partDb in m_dbContext.Participants
                                join cgRoleDb in m_dbContext.ParticipantRole_CentreGroup 
                                on partDb.id equals cgRoleDb.participant_id
                                where cgRoleDb.role_id == (int) role 
                                && partDb.active == true
                                select partDb).ToList();

            return participants;
        }

        public List<Participants> GetActiveParticipantsByOfficeRole(UserRole role) {

            var participants = (from partDb in m_dbContext.Participants
                                join compRoleDb in m_dbContext.Participant_Office_Role
                                on partDb.id equals compRoleDb.participant_id
                                where compRoleDb.role_id == (int)role
                                && partDb.active == true
                                select partDb).ToList();

            return participants;
        }

        public void ActivateUser(int participantId) {
            var participant = (from partDb in m_dbContext.Participants
                                where partDb.id == participantId
                                select partDb).FirstOrDefault();

            participant.active = true;

            SaveChanges();

        }
        #endregion

        #region Methods
        public bool IsCompanyAdmin(int companyId, int userId) {
            var parCompRole = (from por in m_dbContext.Participant_Office_Role
                               where por.office_id == companyId &&
                               por.participant_id == userId &&
                               por.role_id == (int)UserRole.OfficeAdministrator
                               select por).FirstOrDefault();

            return (parCompRole != null);
        }

        public bool IsCompanyStatAdmin(int companyId, int userId) {
            var parCompRole = (from por in m_dbContext.Participant_Office_Role
                               where por.office_id == companyId &&
                               por.participant_id == userId &&
                               por.role_id == (int)UserRole.StatisticsCompanyManager
                               select por).FirstOrDefault();

            return (parCompRole != null);
        }

        public void AddUserOfficeRole(int participantId, int companyId, UserRole role) {
            var user = (from userDB in m_dbContext.Participants
                        where userDB.id == participantId
                        select userDB).FirstOrDefault();

            Participant_Office_Role por = new Participant_Office_Role();
            por.office_id = companyId;
            por.participant_id = participantId;
            por.role_id = (int)role;
            user.Participant_Office_Role.Add(por);
            m_dbContext.SaveChanges(); 
        }

        public void RemoveUserOfficeRole(int participantId, int companyId, UserRole role) {
            var user = (from userDB in m_dbContext.Participants
                        where userDB.id == participantId
                        select userDB).FirstOrDefault();

            var userRole = (from roleDB in user.Participant_Office_Role
                        where roleDB.role_id == (int)role 
                        && roleDB.office_id == companyId
                        select roleDB).FirstOrDefault();

            if (userRole != null) {
                user.Participant_Office_Role.Remove(userRole);
            }
            
            m_dbContext.SaveChanges();
        }

        
        #endregion
    }
}
