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
using static Kamsyk.Reget.Model.Repositories.FilterValueRepository;

namespace Kamsyk.Reget.Model.Repositories {
    public class StatisticsRepository : BaseRepository<Supplier>, IStatisticsRepository {
        #region Constant

        #endregion

        #region Enums

        #endregion
                
        #region Methods
        public List<StatisticsFilter> GetRequestorsFilter(
            int companyId, 
            int userId, 
            string rootUrl,
            bool isActiveOnly) {
            List<StatisticsFilter> retItems = new List<StatisticsFilter>();

            Hashtable htUsers = new Hashtable();

            //var participant = (from pd in m_dbContext.Participants
            //               where pd.id == userId
            //               select pd).FirstOrDefault();

            bool isCompanyAdmin = new UserRepository().IsCompanyAdmin(companyId, userId);
            bool isCompanyStatAdmin = new UserRepository().IsCompanyStatAdmin(companyId, userId);
            if (isCompanyAdmin || isCompanyStatAdmin) {
                var compParticipants = (from par in m_dbContext.ParticipantRole_CentreGroup
                                        join cg in m_dbContext.Centre_Group
                                        on par.centre_group_id equals cg.id
                                        where cg.company_id == companyId &&
                                        par.role_id == (int)UserRole.Requestor
                                        select par).ToList();

                if (compParticipants != null && compParticipants.Count > 0) {
                    foreach (var compP in compParticipants) {

                        AddFilterUser(
                            compP.Participants,
                            companyId,
                            htUsers,
                            rootUrl,
                            retItems,
                            isActiveOnly);
                    }
                }

                //Get Filter Data
                AddFilterUserFromFilter(companyId, htUsers, isActiveOnly, rootUrl, retItems, FilterType.Requestor);

            } else {
                List<ParticipantRole_CentreGroup> cgAdmins = GetCgAdmin(userId, companyId);
                if (cgAdmins != null && cgAdmins.Count > 0) {
                    foreach (var cgAdmin in cgAdmins) {
                        var cgParticipants = (from par in m_dbContext.ParticipantRole_CentreGroup
                                              where par.centre_group_id == cgAdmin.centre_group_id &&
                                              par.role_id == (int)UserRole.Requestor
                                              select par).ToList();
                        if (cgParticipants != null) {
                            foreach (var cgPart in cgParticipants) {
                                AddFilterUser(
                                    cgPart.Participants,
                                    companyId,
                                    htUsers,
                                    rootUrl,
                                    retItems,
                                    isActiveOnly);
                            }
                        }
                    }

                } else {

                    var compParticipant = (from par in m_dbContext.Participants
                                           where par.id == userId
                                           select par).FirstOrDefault();
                    AddFilterUser(
                        compParticipant,
                        companyId,
                        htUsers,
                        rootUrl,
                        retItems,
                        isActiveOnly);
                }
            }

            

            //var parAdminRole = (from par in m_dbContext.ParticipantRole_CentreGroup
            //                    join cg in m_dbContext.Centre_Group
            //                    on par.centre_group_id equals cg.id
            //                    where cg.company_id == companyId && 
            //                    par.participant_id == userId &&
            //                    (
            //                    par.role_id == (int)UserRole.CentreGroupPropAdmin ||
            //                    par.role_id == (int)UserRole.RequestorAdmin ||
            //                    par.role_id == (int)UserRole.ApproveMan ||
            //                    par.role_id == (int)UserRole.Orderer ||
            //                    )
            //                    select par).FirstOrDefault();

            return retItems;

            
        }

        private void AddFilterUserFromFilter(
            int companyId, 
            Hashtable htUsers, 
            bool isActiveOnly, 
            string rootUrl,
            List<StatisticsFilter> retItems,
            FilterType filterType) {

            var filterRequestors = (from filterValDb in m_dbContext.Filter_Value
                                    where filterValDb.country_id == companyId &&
                                    filterValDb.filed_type == (int)filterType
                                    select filterValDb).ToList();
            foreach (var filterRequestor in filterRequestors) {
                int participantId = (int)filterRequestor.item_id;
                if (!htUsers.ContainsKey(participantId)) {
                    var fReq = (from fpd in m_dbContext.Participants
                                where fpd.id == participantId
                                select fpd).FirstOrDefault();
                    if (isActiveOnly && fReq.active != true) {
                        continue;
                    }
                    AddFilterUser(
                            fReq,
                            companyId,
                            htUsers,
                            rootUrl,
                            retItems,
                            isActiveOnly);
                }
            }
        }

        public List<StatisticsFilter> GetOrderersFilter(
            int companyId,
            int userId,
            string rootUrl,
            bool isActiveOnly) {
            List<StatisticsFilter> retItems = new List<StatisticsFilter>();

            Hashtable htUsers = new Hashtable();

            bool isCompanyAdmin = new UserRepository().IsCompanyAdmin(companyId, userId);
            bool isCompanyStatAdmin = new UserRepository().IsCompanyStatAdmin(companyId, userId);
            if (isCompanyAdmin || isCompanyStatAdmin) {
                var compOrderers = (from par in m_dbContext.ParticipantRole_CentreGroup
                                        join cg in m_dbContext.Centre_Group
                                        on par.centre_group_id equals cg.id
                                        where cg.company_id == companyId &&
                                        par.role_id == (int)UserRole.Orderer
                                        select par).ToList();

                if (compOrderers != null && compOrderers.Count > 0) {
                    foreach (var compO in compOrderers) {

                        AddFilterUser(
                            compO.Participants,
                            companyId,
                            htUsers,
                            rootUrl,
                            retItems,
                            isActiveOnly);
                    }
                }

                //Get Filter Data
                AddFilterUserFromFilter(companyId, htUsers, isActiveOnly, rootUrl, retItems, FilterType.Orderer);
            } else {
                List<ParticipantRole_CentreGroup> cgAdmins = GetCgAdmin(userId, companyId);
                if (cgAdmins != null && cgAdmins.Count > 0) {
                    foreach (var cgAdmin in cgAdmins) {
                        var cgOrderers = (from par in m_dbContext.ParticipantRole_CentreGroup
                                          where par.centre_group_id == cgAdmin.centre_group_id &&
                                          par.role_id == (int)UserRole.Orderer
                                          select par).ToList();
                        if (cgOrderers != null) {
                            foreach (var cgPart in cgOrderers) {
                                AddFilterUser(
                                    cgPart.Participants,
                                    companyId,
                                    htUsers,
                                    rootUrl,
                                    retItems,
                                    isActiveOnly);
                            }
                        }
                    }

                } else {

                    var compParticipant = (from par in m_dbContext.Participants
                                           where par.id == userId
                                           select par).FirstOrDefault();
                    AddFilterUser(
                        compParticipant,
                        companyId,
                        htUsers,
                        rootUrl,
                        retItems,
                        isActiveOnly);
                }
            }

            return retItems;

        }

        public List<StatisticsFilter> GetAreasFilter(
            int companyId, 
            int userId, 
            string rootUrl,
            bool isActiveOnly) {

            List<StatisticsFilter> retItems = new List<StatisticsFilter>();

            Hashtable htAreas = new Hashtable();

            bool isCompanyAdmin = new UserRepository().IsCompanyAdmin(companyId, userId);
            bool isCompanyStatAdmin = new UserRepository().IsCompanyStatAdmin(companyId, userId);
            
            if (isCompanyAdmin || isCompanyStatAdmin) {
                var compAreas = (from cgDb in m_dbContext.Centre_Group
                                        where cgDb.company_id == companyId
                                        select cgDb).ToList();

                if (compAreas != null && compAreas.Count > 0) {
                    foreach (var compArea in compAreas) {

                        AddArea(
                            compArea,
                            companyId,
                            htAreas,
                            rootUrl,
                            retItems,
                            isActiveOnly);
                    }
                }
            } else {
                List<ParticipantRole_CentreGroup> cgAdmins = GetCgAdmin(userId, companyId);
                if (cgAdmins != null && cgAdmins.Count > 0) {
                    foreach (var cgAdmin in cgAdmins) {

                        AddArea(
                            cgAdmin.Centre_Group,
                            companyId,
                            htAreas,
                            rootUrl,
                            retItems,
                            isActiveOnly);
                    }
                } 
            }

            return retItems;

        }

        public List<StatisticsFilter> GetCentresFilter(
            int companyId,
            int userId,
            string rootUrl,
            bool isActiveOnly) {
            List<StatisticsFilter> retItems = new List<StatisticsFilter>();

            Hashtable htCenters = new Hashtable();
                        
            bool isCompanyAdmin = new UserRepository().IsCompanyAdmin(companyId, userId);
            bool isCompanyStatAdmin = new UserRepository().IsCompanyStatAdmin(companyId, userId);
            if (isCompanyAdmin || isCompanyStatAdmin) {
                var cgs = (from cgd in m_dbContext.Centre_Group
                           where cgd.company_id == companyId
                           select cgd).ToList();

                if (cgs != null && cgs.Count > 0) {
                    foreach (var cg in cgs) {
                        if (isActiveOnly && cg.active != true) {
                            continue;
                        }

                        foreach (var centre in cg.Centre) {

                            AddCentre(
                                centre,
                                companyId,
                                htCenters,
                                rootUrl,
                                retItems,
                                isActiveOnly);
                        }
                    }
                }
            } else {
                var prcgs = (from prcg in m_dbContext.ParticipantRole_CentreGroup
                             join cg in m_dbContext.Centre_Group
                               on prcg.centre_group_id equals cg.id
                             where cg.company_id == companyId &&
                             prcg.participant_id == userId &&
                             (prcg.role_id == (int)UserRole.CentreGroupPropAdmin ||
                             prcg.role_id == (int)UserRole.OrdererAdmin ||
                             prcg.role_id == (int)UserRole.RequestorAdmin ||
                             prcg.role_id == (int)UserRole.ApproveMatrixAdmin)
                             select prcg).ToList();
                if (prcgs != null && prcgs.Count > 0) {
                    foreach (var prcg in prcgs) {
                        if (isActiveOnly && prcg.Centre_Group.active != true) {
                            continue;
                        }

                        foreach (var centre in prcg.Centre_Group.Centre) {

                            AddCentre(
                                centre,
                                companyId,
                                htCenters,
                                rootUrl,
                                retItems,
                                isActiveOnly);
                        }
                    }
                }
            }



            return retItems;


        }

        public List<StatisticsFilter> GetSuppliersFilter(
            int companyId,
            int userId,
            string rootUrl,
            bool isActiveOnly) {
            List<StatisticsFilter> retItems = new List<StatisticsFilter>();

            Hashtable htUsers = new Hashtable();

            bool isCompanyAdmin = new UserRepository().IsCompanyAdmin(companyId, userId);
            bool isCompanyStatAdmin = new UserRepository().IsCompanyStatAdmin(companyId, userId);
            if (!isCompanyAdmin && !isCompanyStatAdmin) {
                return retItems;
            }
            var companies = (from compDb in m_dbContext.Company
                       where compDb.id == companyId
                       select compDb).ToList();

            if (companies != null && companies.Count > 0) {
                foreach (var company in companies) {
                    if (isActiveOnly && company.active != true) {
                        continue;
                    }

                    List<Supplier> suppliers = null;
                    if (isActiveOnly) {
                        suppliers = (from suppDb in m_dbContext.Supplier
                                     where suppDb.supplier_group_id == company.supplier_group_id &&
                                     suppDb.active == true 
                                     select suppDb).ToList();
                    } else {
                        suppliers = (from suppDb in m_dbContext.Supplier
                                     where suppDb.supplier_group_id == company.supplier_group_id
                                     select suppDb).ToList();
                    }

                    foreach (var supplier in suppliers) {
                        StatisticsFilter filterItem = new StatisticsFilter();
                        filterItem.id = supplier.id;
                        string suppName = supplier.supp_name;
                        if (!String.IsNullOrEmpty(supplier.supplier_id)) {
                            suppName += " (" + supplier.supplier_id + ")";
                        }
                        filterItem.name = suppName;
                        filterItem.name_search_key = RemoveDiacritics(suppName);
                        filterItem.flag_url = GetCountryFlag(companyId, rootUrl);

                        filterItem.company_id = company.id;

                        retItems.Add(filterItem);
                    }
                }
            }

            return retItems;

        }

        public List<StatisticsFilter> GetCommoditiesFilter(
            int companyId,
            int userId,
            string rootUrl,
            bool isActiveOnly) {
            List<StatisticsFilter> retItems = new List<StatisticsFilter>();

            Hashtable htUsers = new Hashtable();

            bool isCompanyAdmin = new UserRepository().IsCompanyAdmin(companyId, userId);
            bool isCompanyStatAdmin = new UserRepository().IsCompanyStatAdmin(companyId, userId);
            if (!isCompanyAdmin && !isCompanyStatAdmin) {
                return retItems;
            }
            var companies = (from compDb in m_dbContext.Company
                             where compDb.id == companyId
                             select compDb).ToList();

            Hashtable htCommodities = new Hashtable();
            if (companies != null && companies.Count > 0) {
                foreach (var company in companies) {
                    if (isActiveOnly && company.active != true) {
                        continue;
                    }

                    foreach (var ppg in company.Parent_Purchase_Group) {
                        
                        AddCommodity(
                            ppg,
                            companyId,
                            htCommodities,
                            rootUrl,
                            retItems,
                            isActiveOnly);

                    }
                                        
                }
            }

            return retItems;

        }

        private void AddFilterUser(
            Participants participant, 
            int companyId, 
            Hashtable htUsers, 
            string rootUrl, 
            List<StatisticsFilter> retItems,
            bool isActiveOnly) {

            if (isActiveOnly && participant.active != true) {
                return;
            }

            if (!htUsers.ContainsKey(participant.id)) {
                string strName = UserRepository.GetUserNameSurnameFirst(participant);

                StatisticsFilter filterItem = new StatisticsFilter();

                filterItem.id = participant.id;
                filterItem.name = strName;
                filterItem.name_search_key = RemoveDiacritics(strName);
                filterItem.company_id = companyId;
                filterItem.flag_url = GetCountryFlag(companyId, rootUrl);

                retItems.Add(filterItem);
                htUsers.Add(participant.id, null);
            }
        }

        private void AddArea(
            Centre_Group centreGroup,
            int companyId,
            Hashtable htUsers,
            string rootUrl,
            List<StatisticsFilter> retItems,
            bool isActiveOnly) {

            if (isActiveOnly && centreGroup.active != true) {
                return;
            }

            if (!htUsers.ContainsKey(centreGroup.id)) {
                string strName = centreGroup.name;

                StatisticsFilter filterItem = new StatisticsFilter();

                filterItem.id = centreGroup.id;
                filterItem.name = strName;
                filterItem.name_search_key = RemoveDiacritics(strName);
                filterItem.company_id = companyId;
                filterItem.flag_url = GetCountryFlag(companyId, rootUrl);

                retItems.Add(filterItem);
                htUsers.Add(centreGroup.id, null);
            }
        }

        private void AddCentre(
            Centre centre,
            int companyId,
            Hashtable htCentres,
            string rootUrl,
            List<StatisticsFilter> retItems,
            bool isActiveOnly) {

            if (isActiveOnly && centre.active != true) {
                return;
            }

            if (!htCentres.ContainsKey(centre.id)) {
                string strName = centre.name;

                StatisticsFilter filterItem = new StatisticsFilter();

                filterItem.id = centre.id;
                filterItem.name = strName;
                filterItem.name_search_key = RemoveDiacritics(strName);
                filterItem.company_id = companyId;
                filterItem.flag_url = GetCountryFlag(companyId, rootUrl);

                retItems.Add(filterItem);
                htCentres.Add(centre.id, null);
            }
        }

        private void AddCommodity(
            Parent_Purchase_Group ppg,
            int companyId,
            Hashtable htCommodity,
            string rootUrl,
            List<StatisticsFilter> retItems,
            bool isActiveOnly) {

            //if (isActiveOnly && ppg.active != true) {
            //    return;
            //}

            if (!htCommodity.ContainsKey(ppg.id)) {
                string strName = ppg.name;

                StatisticsFilter filterItem = new StatisticsFilter();

                filterItem.id = ppg.id;
                filterItem.name = strName;
                //filterItem.name_search_key = RemoveDiacritics(strName);
                filterItem.company_id = companyId;
                filterItem.flag_url = GetCountryFlag(companyId, rootUrl);

                retItems.Add(filterItem);
                htCommodity.Add(ppg.id, null);
            }
        }

        private List<ParticipantRole_CentreGroup> GetCgAdmin(int companyId, int userId) {
            var parCompRole = (from prcg in m_dbContext.ParticipantRole_CentreGroup
                               join cg in m_dbContext.Centre_Group
                               on prcg.centre_group_id equals cg.id
                               where cg.company_id == companyId &&
                               prcg.participant_id == userId &&
                               (prcg.role_id == (int)UserRole.CentreGroupPropAdmin ||
                               prcg.role_id == (int)UserRole.ApproveMatrixAdmin ||
                               prcg.role_id == (int)UserRole.RequestorAdmin ||
                               prcg.role_id == (int)UserRole.OrdererAdmin
                               )
                               select prcg).ToList();

            return parCompRole;
        }

        public List<StatisticsRequest> GetRequestsByFilter(
            DateTime dateFrom,
            DateTime dateTo,
            int[] companies) {

            //bool isAllRequestors = (requestors == null || requestors.Length == 0);

            var requestes = (from red in m_dbContext.Request_Event
                             //join cgd in m_dbContext.Centre_Group
                             //on red.centre_group_id equals cgd.id
                             where red.issued >= dateFrom &&
                             red.issued <= dateTo &&
                             red.last_event == true &&
                             (red.request_status == RequestRepository.STATUS_ORDERED ||
                             red.request_status == RequestRepository.STATUS_CLOSED) &&
                             companies.Contains((int)red.country_id) 
                             //(isAllRequestors || (red.requestor.HasValue && requestors.Contains(red.requestor.Value)))
                               select new StatisticsRequest {
                                   id = red.id,
                                   company_id = (int)red.country_id,
                                   issued = red.issued,
                                   request_status = red.request_status,
                                   price = red.actual_price,
                                   estimated_price = red.estimated_price,
                                   requestor = red.requestor,
                                   orderer_id = red.orderer_id,
                                   purchase_group_id = red.purchase_group_id,
                                   supplier_id = red.supplier_id,
                                   centre_group_id = red.centre_group_id,
                                   currency_id = red.currency_id,
                                   request_centre_id = red.request_centre_id}).ToList();

            return requestes;
        }

        
        #endregion
    }
}
