using Kamsyk.Reget.Controllers.Interface;
using Kamsyk.Reget.Controllers.RegetExceptions;
using Kamsyk.Reget.Model;
using Kamsyk.Reget.Model.Common;
using Kamsyk.Reget.Model.ExtendedModel;
using Kamsyk.Reget.Model.ExtendedModel.Chart;
using Kamsyk.Reget.Model.ExtendedModel.Statistics;
using Kamsyk.Reget.Model.Repositories;
using Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using static Kamsyk.Reget.Model.Repositories.FilterValueRepository;
using static Kamsyk.Reget.Model.Repositories.RequestRepository;
using static Kamsyk.Reget.Model.Repositories.UserRepository;

namespace Kamsyk.Reget.Controllers
{
    public class StatisticsController : BaseController {
        #region Constants
        public const int FILTER_REQUESTOR = 10;
        public const int FILTER_ORDERER = 20;
        public const int FILTER_SUPPLIER = 30;
        public const int FILTER_CENTER = 40;
        public const int FILTER_AREA = 50;
        public const int FILTER_COMMODITY = 60;
        //private const int FILTER_TOTAL = 70;
        public const int FILTER_COMPANY = 80;
        public const int FILTER_APPROVE_MAN = 90;

        public const int FILTER_PERIOD_ALL = 10;
        private const int FILTER_PERIOD_YEAR = 20;
        private const int FILTER_PERIOD_MONTH = 30;

        private const int FILTER_Y_PRICE = 10;
        private const int FILTER_Y_NUM_OF_REQUESTS = 20;

        public const int CHART_TYPE_BAR = 20;
        public const int CHART_TYPE_LINE = 10;
        public const int CHART_TYPE_PIE = 40;
        public const int CHART_TYPE_DOUGHNUT = 50;
        public const int CHART_TYPE_RADAR = 30;
        public const int CHART_TYPE_POLARAREA = 60;

        
        #endregion

        #region Struct
        public struct Period {
            public DateTime PeriodStart;
            public DateTime PeriodEnd;
            public decimal PeriodValue;

            public Period(DateTime periodStart, DateTime periodEnd, decimal periodValue) {
                PeriodStart = periodStart;
                PeriodEnd = periodEnd;
                PeriodValue = periodValue;
            }
        }
        #endregion

        #region Properties
        private List<StatisticsCompany> _m_StatisticsCompanies = null;
        private List<StatisticsCompany> m_StatisticsCompanies {
            get {
                if (_m_StatisticsCompanies == null) {
                    _m_StatisticsCompanies = GetStatisticsCompanies();
                }

                return _m_StatisticsCompanies;
            }
        }
        #endregion

        #region Static Properties
        public static string ControllerName {
            get {
                string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

                return GetControllerName(className);
            }
        }

        #endregion

        #region Virtual Properties
        public override string HeaderImgUrl {
            get {
                return GetRootUrl() + "/Content/Images/Controll/Statistics.png";
            }
        }

        public override string HeaderTitle {
            get {
                return RequestResource.Statistics;
            }
        }

        protected override bool IsGenerateDatePickerLocalization {
            get {
                return true;
            }
        }
        #endregion

        #region Properties
        private Random m_random = new Random();

        bool m_isCompanyDisabled {
            get { return !CurrentUser.IsParticipantCompanyAdmin && !CurrentUser.IsParticipantCompanyStatisticsManager; }
        }

        bool m_isAreaDisabled {
            get {
                return !CurrentUser.IsParticipantCompanyAdmin &&
                      !CurrentUser.IsParticipantAreaAdmin &&
                      !CurrentUser.IsParticipantAppAdmin &&
                      !CurrentUser.IsParticipantCompanyStatisticsManager;
            }
        }

        bool m_isSupplierDisabled {
            get { return !CurrentUser.IsParticipantCompanyAdmin && !CurrentUser.IsParticipantCompanyStatisticsManager; }
        }

        bool m_isPgDisabled {
            get { return !CurrentUser.IsParticipantCompanyAdmin && 
                    !CurrentUser.IsParticipantAreaAdmin && 
                    !CurrentUser.IsParticipantCompanyStatisticsManager; }
        }

        bool m_isRequestorDisabled {
            get {
                return (!CurrentUser.IsParticipantCompanyAdmin &&
                        !CurrentUser.IsParticipantAreaAdmin &&
                        !CurrentUser.IsParticipantRequestor && 
                        !CurrentUser.IsParticipantCompanyStatisticsManager);
            }
        }

        bool m_isOrdererDisabled {
            get {
                return !CurrentUser.IsParticipantCompanyAdmin &&
                        !CurrentUser.IsParticipantAreaAdmin &&
                        !CurrentUser.IsParticipantOrderer && 
                        !CurrentUser.IsParticipantCompanyStatisticsManager;
            }
        }
        #endregion

        #region Enums
        private enum AxisX {
            Requestor = FILTER_REQUESTOR,
            ApproveMan = FILTER_APPROVE_MAN,
            Orderer = FILTER_ORDERER,
            Supplier = FILTER_SUPPLIER,
            Centre = FILTER_CENTER,
            Area = FILTER_AREA,
            ParentPurchaseGroup = FILTER_COMMODITY,
            //TotalPriceCount = FILTER_TOTAL,
            Company = FILTER_COMPANY
        }

        private enum AxisY {
            Price,
            Count
        }
        #endregion

        #region Methods
        private void GetFilterItemIdName(
           StatisticsRequest statisticsRequest,
           int xItemType,
           Hashtable htColName,
           out int itemId,
           out string itemName) {

            itemId = DataNulls.INT_NULL;
            itemName = null;

            if (xItemType == FILTER_REQUESTOR) {

                itemId = (int)statisticsRequest.requestor;
                if (!htColName.ContainsKey(itemId)) {
                    var participant = new UserRepository().GetParticipantById(itemId);
                    htColName.Add(itemId, UserRepository.GetUserNameSurnameFirst(participant));
                }

                itemName = htColName[itemId].ToString();
            } else if (xItemType == FILTER_ORDERER) {

                itemId = (int)statisticsRequest.orderer_id;
                if (!htColName.ContainsKey(itemId)) {
                    var participant = new UserRepository().GetParticipantById(itemId);
                    htColName.Add(itemId, UserRepository.GetUserNameSurnameFirst(participant));
                }

                itemName = htColName[itemId].ToString();
            } else if (xItemType == FILTER_AREA) {

                itemId = (int)statisticsRequest.centre_group_id;
                if (!htColName.ContainsKey(itemId)) {
                    var cg = new CentreGroupRepository().GetCentreGroupFullById(itemId);
                    htColName.Add(itemId, cg.name);
                }

                itemName = htColName[itemId].ToString();
            } else if (xItemType == FILTER_CENTER) {

                itemId = (int)statisticsRequest.request_centre_id;
                if (!htColName.ContainsKey(itemId)) {
                    var centre = new CentreRepository().GetCentreById(itemId);
                    htColName.Add(itemId, centre.name);
                }

                itemName = htColName[itemId].ToString();
            } else if (xItemType == FILTER_COMPANY) {

                itemId = (int)statisticsRequest.company_id;
                if (!htColName.ContainsKey(itemId)) {
                    var company = new CompanyRepository().GetCompanyById(itemId);
                    htColName.Add(itemId, company.country_code);
                }

                itemName = htColName[itemId].ToString();
            } else if (xItemType == FILTER_COMMODITY) {
                if (statisticsRequest.purchase_group_id.HasValue) {
                    itemId = (int)statisticsRequest.purchase_group_id;
                    if (!htColName.ContainsKey(itemId)) {
                        var ppg = new ParentPgRepository().GetParentPgByPgId(itemId);
                        htColName.Add(itemId, GetParentPgLocName(ppg));
                    }

                    itemName = htColName[itemId].ToString();
                }
            } else if (xItemType == FILTER_SUPPLIER) {
                if (statisticsRequest.supplier_id.HasValue) {

                    itemId = (int)statisticsRequest.supplier_id;
                    if (!htColName.ContainsKey(itemId)) {
                        var supplier = new SupplierRepository().GetSupplierDataById(itemId);
                        string suppName = supplier.supp_name;
                        if (!String.IsNullOrEmpty(supplier.supplier_id)) {
                            suppName += " (" + supplier.supplier_id + ")";
                        }
                        htColName.Add(itemId, suppName);
                    }

                    itemName = htColName[itemId].ToString();
                }
            } 

            //if (xItemType == FILTER_APPROVE_MAN) {
            //    if (statisticsRequest.supplier_id.HasValue) {

            //        itemId = (int)statisticsRequest.supplier_id;
            //        if (!htColName.ContainsKey(itemId)) {
            //            var supplier = new SupplierRepository().GetSupplierDataById(itemId);
            //            htColName.Add(itemId, supplier.supp_name);
            //        }

            //        itemName = htColName[itemId].ToString();
            //    }
            //}
        }

        private string GetParentPgLocName(Parent_Purchase_Group ppg) {
            if (ppg == null) {
                return RequestResource.NotFound;
            }

            var ppgLocName = (from dbPpgLoc in ppg.Parent_Purchase_Group_Local
                             where dbPpgLoc.culture == CurrentCultureCode
                             select dbPpgLoc).FirstOrDefault();
            if (ppgLocName != null) {
                return ppgLocName.local_text;
            } else {
                ppgLocName = (from dbPpgLoc in ppg.Parent_Purchase_Group_Local
                              where dbPpgLoc.culture == DefaultLanguage
                              select dbPpgLoc).FirstOrDefault();
                if (ppgLocName != null) {
                    return ppgLocName.local_text;
                }
            }

            if (ppg.Parent_Purchase_Group_Local.Count > 0) {
                return ppg.Parent_Purchase_Group_Local.ElementAt(0).local_text;
            }

            return RequestResource.NotFound;
        }

        private void SetPeriodValue(
            ref List<Period> periods,
            DateTime date,
            int periodType,
            decimal value) {

            if (periods == null) {
                Period newPeriod = GetPeriod(date, periodType);
                newPeriod.PeriodValue = value;
                periods = new List<Period>();
                periods.Add(newPeriod);

                return;
                //return periods[0];
            }

            DateTime dateStart = DateTime.MinValue;
            DateTime dateEnd = DateTime.MaxValue;

            if (periodType == FILTER_PERIOD_MONTH) {
                int year = date.Year;
                int month = date.Month;
                dateStart = new DateTime(year, month, 1);
                dateEnd = new DateTime(year, month, 1).AddMonths(1);
            }

            if (periodType == FILTER_PERIOD_YEAR) {
                int year = date.Year;
                dateStart = new DateTime(year, 1, 1);
                dateEnd = new DateTime(year + 1, 1, 1);
            }

            for (int i = 0; i < periods.Count; i++) {
                if (periods[i].PeriodStart == dateStart && periods[i].PeriodEnd == dateEnd) {
                    Period period = periods[i];
                    period.PeriodValue += value;
                    periods[i] = period;
                    return;
                }
            }

            Period addPeriod = new Period(dateStart, dateEnd, 0);
            addPeriod.PeriodValue = value;
            periods.Add(addPeriod);
            //return periods[periods.Count - 1];
        }

        private Period GetPeriod(DateTime date, int periodType) {
            if (periodType == FILTER_PERIOD_MONTH) {
                int year = date.Year;
                int month = date.Month;
                return new Period(new DateTime(year, month, 1), new DateTime(year, month, 1).AddMonths(1), 0);
            }

            if (periodType == FILTER_PERIOD_YEAR) {
                int year = date.Year;
                return new Period(new DateTime(year, 1, 1), new DateTime(year + 1, 1, 1), 0);
            }

            return new Period(DateTime.MinValue, DateTime.MaxValue, 0);
        }

        public List<Period> GetAllPeriods(int periodType, DateTime dateStart, DateTime dateEnd) {
            List<Period> periods = new List<Period>();

            if (periodType == FILTER_PERIOD_MONTH) {
                DateTime dStart = new DateTime(dateStart.Year, dateStart.Month, 1);
                DateTime dEnd = dStart.AddMonths(1);
                Period monthPeriod = new Period(dStart, dEnd, 0);
                periods.Add(monthPeriod);

                while (dEnd < dateEnd) {
                    dStart = dStart.AddMonths(1);
                    dEnd = dStart.AddMonths(1);
                    monthPeriod = new Period(dStart, dEnd, 0);
                    periods.Add(monthPeriod);
                }

                return periods;
            }

            if (periodType == FILTER_PERIOD_YEAR) {
                DateTime dStart = new DateTime(dateStart.Year, 1, 1);
                DateTime dEnd = dStart.AddYears(1);
                Period yearPeriod = new Period(dStart, dEnd, 0);
                periods.Add(yearPeriod);

                while (dEnd < dateEnd) {
                    dStart = dStart.AddYears(1);
                    dEnd = dStart.AddYears(1);
                    yearPeriod = new Period(dStart, dEnd, 0);
                    periods.Add(yearPeriod);
                }

                return periods;
            }

            Period period = new Period(DateTime.MinValue, DateTime.MaxValue, 0);
            periods.Add(period);

            return periods;
        }

        public List<string> GetChartLabels(int periodType, DateTime dateFrom, DateTime dateTo, List<Period> allPeriods) {
            List<string> labels = new List<string>();
            if (periodType == FILTER_PERIOD_ALL) {
                string strLabel = ConvertData.ToStringFromDateLocal(dateFrom) + " - " + ConvertData.ToStringFromDateLocal(dateTo);
                labels.Add(strLabel);
            }

            if (periodType == FILTER_PERIOD_YEAR) {
                foreach (var period in allPeriods) {
                    labels.Add(period.PeriodStart.Year.ToString());
                }

            }

            if (periodType == FILTER_PERIOD_MONTH) {
                foreach (var period in allPeriods) {
                    labels.Add(period.PeriodStart.Month + "/" + period.PeriodStart.Year);
                }

            }

            return labels;
        }

        private List<int> GetChartDataSetValues(List<Period> periods, List<Period> allPeriods) {
            List<int> data = new List<int>();

            int yLast = 0;
            for (int i = 0; i < allPeriods.Count; i++) {
                if (yLast >= periods.Count) {
                    data.Add(0);
                } else {
                    bool isFound = false;
                    for (int y = yLast; y < periods.Count; y++) {
                        if (allPeriods[i].PeriodStart == periods[y].PeriodStart && allPeriods[i].PeriodEnd == periods[y].PeriodEnd) {
                            data.Add(ConvertData.ToInt32(periods[y].PeriodValue));
                            yLast = y + 1;
                            isFound = true;
                            break;
                        }

                    }

                    if (!isFound) {
                        data.Add(0);
                    }
                }
            }

            return data;
        }

        private string GetRandomHtmlColor() {
            //var letters = '0123456789ABCDEF';
            //var color = '#';
            //for (var i = 0; i < 6; i++) {
            //    color += letters[Math.Floor(Math.random() * 16)];
            //}
            string color = String.Format("#{0:X6}", m_random.Next(0x1000000));

            return color;
        }

        private List<StatisticsFilter> GetFilterRequestorsList(int[] companiesIds, bool isActiveOnly) {

            Hashtable htReq = new Hashtable();
            List<StatisticsFilter> filterRequestors = new List<StatisticsFilter>();
            
            StatisticsRepository statisticsRepository = new StatisticsRepository();
            foreach (var iCompId in companiesIds) {
               
                List<StatisticsFilter> compRequestors = statisticsRepository.GetRequestorsFilter(
                    iCompId,
                    CurrentUser.ParticipantId,
                    GetRootUrl(),
                    isActiveOnly);

                if (compRequestors != null) {
                    foreach (var compReq in compRequestors) {
                        if (!htReq.ContainsKey(compReq.id)) {
                            filterRequestors.Add(compReq);
                            htReq.Add(compReq.id, null);
                        }
                    }
                }
            }

            if (filterRequestors == null) {
                return null;
            }

            return filterRequestors;
        }

        private List<StatisticsFilter> GetFilterOrderersList(int[] companiesIds, bool isActiveOnly) {

            Hashtable htOrd = new Hashtable();
            List<StatisticsFilter> filterOrderers = new List<StatisticsFilter>();
            
            StatisticsRepository statisticsRepository = new StatisticsRepository();
            foreach (var iCompId in companiesIds) {
               
                //if (!IsCompanyRoleAllowed(iCompId, FILTER_ORDERER)) {
                //    throw new Exception("User is not authorized for company");
                //}

                List<StatisticsFilter> compOrderers = statisticsRepository.GetOrderersFilter(
                    iCompId,
                    CurrentUser.ParticipantId,
                    GetRootUrl(),
                    isActiveOnly);

                if (compOrderers != null) {
                    foreach (var compReq in compOrderers) {
                        if (!htOrd.ContainsKey(compReq.id)) {
                            filterOrderers.Add(compReq);
                            htOrd.Add(compReq.id, null);
                        }
                    }
                }
            }

            if (filterOrderers == null) {
                return null;
            }

            return filterOrderers;
        }

        private List<StatisticsFilter> GetFilterAreasList(int[] companiesIds, bool isActiveOnly) {
            
            Hashtable htArea = new Hashtable();
            List<StatisticsFilter> filterAreas = new List<StatisticsFilter>();
            StatisticsRepository statisticsRepository = new StatisticsRepository();
            foreach (var iCompId in companiesIds) {
                
                if (!IsCompanyRoleAllowed(iCompId, FILTER_AREA)) {
                    throw new Exception("User is not authorized for company");
                }

                List<StatisticsFilter> compAreas = statisticsRepository.GetAreasFilter(
                    iCompId,
                    CurrentUser.ParticipantId,
                    GetRootUrl(),
                    isActiveOnly);

                if (compAreas != null) {
                    foreach (var compArea in compAreas) {
                        if (!htArea.ContainsKey(compArea.id)) {
                            filterAreas.Add(compArea);
                            htArea.Add(compArea.id, null);
                        }
                    }
                }
            }

            if (filterAreas == null) {
                return null;
            }

            return filterAreas;
        }

        private List<StatisticsFilter> GetFilterCentresList(int[] companiesIds, bool isActiveOnly) {

            Hashtable htCentres = new Hashtable();
            List<StatisticsFilter> filterCentres = new List<StatisticsFilter>();
            
            StatisticsRepository statisticsRepository = new StatisticsRepository();
            foreach (var iCompId in companiesIds) {
                if (!IsCompanyRoleAllowed(iCompId, FILTER_CENTER)) {
                    throw new Exception("User is not authorized for company");
                }

                List<StatisticsFilter> compCentres = statisticsRepository.GetCentresFilter(
                    iCompId,
                    CurrentUser.ParticipantId,
                    GetRootUrl(),
                    isActiveOnly);

                if (compCentres != null) {
                    foreach (var compCenter in compCentres) {
                        if (!htCentres.ContainsKey(compCenter.id)) {
                            filterCentres.Add(compCenter);
                            htCentres.Add(compCenter.id, null);
                        }
                    }
                }
            }

            if (filterCentres == null) {
                return null;
            }

            return filterCentres;
        }

        private List<StatisticsFilter> GetFilterSuppliersList(int[] companiesIds, bool isActiveOnly) {
            
            List<StatisticsFilter> filterSuppliers = new List<StatisticsFilter>();
           
            StatisticsRepository statisticsRepository = new StatisticsRepository();
            foreach (var iCompId in companiesIds) {
                
                if (!IsCompanyRoleAllowed(iCompId, FILTER_SUPPLIER)) {
                    throw new Exception("User is not authorized for company");
                }

                List<StatisticsFilter> compSuppliers = statisticsRepository.GetSuppliersFilter(
                    iCompId,
                    CurrentUser.ParticipantId,
                    GetRootUrl(),
                    isActiveOnly);

                if (compSuppliers != null) {
                    foreach (var supplier in compSuppliers) {
                        //if (!htCentres.ContainsKey(compCenter.id)) {
                        filterSuppliers.Add(supplier);
                            //htCentres.Add(compCenter.id, null);
                        //}
                    }
                }
            }

            if (filterSuppliers == null) {
                return null;
            }

            return filterSuppliers;
        }

        private List<StatisticsFilter> GetFilterCommoditiesList(int[] companiesIds, bool isActiveOnly) {
             
            Hashtable htCommodities = new Hashtable();
            List<StatisticsFilter> filterCommodities = new List<StatisticsFilter>();

            StatisticsRepository statisticsRepository = new StatisticsRepository();
            foreach (var iCompId in companiesIds) {
                
                if (!IsCompanyRoleAllowed(iCompId, FILTER_COMMODITY)) {
                    throw new Exception("User is not authorized for company");
                }

                List<StatisticsFilter> compCommodities = statisticsRepository.GetCommoditiesFilter(
                    iCompId,
                    CurrentUser.ParticipantId,
                    GetRootUrl(),
                    isActiveOnly);

                if (compCommodities != null) {
                    foreach (var commodity in compCommodities) {
                        if (!htCommodities.ContainsKey(commodity.id)) {
                            var ppg = new ParentPgRepository().GetParentPgById(commodity.id);
                            commodity.company_id = -1;
                            commodity.flag_url = null;
                            commodity.name = GetParentPgLocName(ppg);
                            commodity.name_search_key = ConvertData.RemoveDiacritics(commodity.name);
                            filterCommodities.Add(commodity);
                            htCommodities.Add(commodity.id, null);
                        }
                    }
                }
            }

            if (filterCommodities == null) {
                return null;
            }

            return filterCommodities;
        }

        private bool IsCompanyRoleAllowed(int companyId, int filterXItem) {
            if (filterXItem == FILTER_APPROVE_MAN) {
                var userComp = (from uc in m_StatisticsCompanies
                                where uc.id == companyId &&
                                uc.is_user_company_stat_admin
                                select uc).FirstOrDefault();
                return (userComp != null);
            }

            if (filterXItem == FILTER_AREA) {
                var userComp = (from uc in m_StatisticsCompanies
                                where uc.id == companyId &&
                                uc.is_user_company_stat_cgadmin
                                select uc).FirstOrDefault();
                return (userComp != null);
            }

            if (filterXItem == FILTER_CENTER) {
                var userComp = (from uc in m_StatisticsCompanies
                                where uc.id == companyId &&
                                uc.is_user_company_stat_cgadmin
                                select uc).FirstOrDefault();
                return (userComp != null);
            }

            if (filterXItem == FILTER_COMMODITY) {
                var userComp = (from uc in m_StatisticsCompanies
                                where uc.id == companyId &&
                                uc.is_user_company_stat_admin
                                select uc).FirstOrDefault();
                return (userComp != null);
            }

            if (filterXItem == FILTER_COMPANY) {
                var userComp = (from uc in m_StatisticsCompanies
                                where uc.id == companyId &&
                                uc.is_user_company_stat_admin
                                select uc).FirstOrDefault();
                return (userComp != null);
            }

            if (filterXItem == FILTER_ORDERER) {
                var userComp = (from uc in m_StatisticsCompanies
                                where uc.id == companyId &&
                                uc.is_user_company_stat_orderer
                                select uc).FirstOrDefault();
                return (userComp != null);
            }

            if (filterXItem == FILTER_REQUESTOR) {
                var userComp = (from uc in m_StatisticsCompanies
                                where uc.id == companyId &&
                                uc.is_user_company_stat_requestor
                                select uc).FirstOrDefault();
                return (userComp != null);
            }

            if (filterXItem == FILTER_SUPPLIER) {
                var userComp = (from uc in m_StatisticsCompanies
                                where uc.id == companyId &&
                                uc.is_user_company_stat_admin
                                select uc).FirstOrDefault();
                return (userComp != null);
            }

            return false;
        }

        public static int[] GetCompaniesIds(string companies) {
            string[] companyItems = companies.Split('|');
            int[] iCompaniesIds = new int[companyItems.Length];
            for (int i = 0; i < companyItems.Length; i++) {
                iCompaniesIds[i] = ConvertData.ToInt32(companyItems[i]);
            }

            return iCompaniesIds;
        }

        //private List<CompanyExtended> GetFilterCompanies() {
        //    List<CompanyExtended> filterCompanies = new List<CompanyExtended>();
        //    foreach (var comp in CurrentUser.UserCompanies) {
        //        filterCompanies.Add(comp);
        //    }

        //    var filterValues = new FilterValueRepository().GetFilterValuesByUserId(CurrentUser.ParticipantId);
        //    if (filterValues != null) {
        //        foreach (var filterValue in filterValues) {
        //            int compId = (int)filterValue.country_id;
        //            var selComp = (from selCompDb in filterCompanies
        //                           where selCompDb.id == compId
        //                           select selCompDb).FirstOrDefault();
        //            if (selComp == null) {
        //                var comp = new CompanyRepository().GetCompanyById(compId);
        //                CompanyExtended compEx = new CompanyExtended();
        //                compEx.id = comp.id;
        //                compEx.country_code = comp.country_code;
        //                filterCompanies.Add(compEx);
        //            }
        //        }
        //    }

        //    return filterCompanies;
        //}

        private List<StatisticsCompany> GetStatisticsCompanies() {
            List<StatisticsCompany> statCompanies = new List<StatisticsCompany>();
            foreach (var comp in CurrentUser.UserCompanies) {
                StatisticsCompany statisticsCompany = new StatisticsCompany();
                statisticsCompany.id = comp.id;
                statisticsCompany.country_code = comp.country_code;

                Participants participant = new UserRepository().GetParticipantById(CurrentUser.ParticipantId);

                var admin = (from roleDb in participant.Participant_Office_Role
                             where roleDb.role_id == (int)UserRole.OfficeAdministrator ||
                             roleDb.role_id == (int)UserRole.StatisticsCompanyManager
                             select roleDb).FirstOrDefault();
                bool isAdmin = (admin != null);
                statisticsCompany.is_user_company_stat_admin = isAdmin;
                if (isAdmin) {
                    statisticsCompany.is_user_company_stat_appman = true;
                    statisticsCompany.is_user_company_stat_cgadmin = true;
                    statisticsCompany.is_user_company_stat_appman = true;
                    statisticsCompany.is_user_company_stat_orderer = true;
                    statisticsCompany.is_user_company_stat_requestor = true;
                } else {

                    var cgadmins = (from roleDb in participant.ParticipantRole_CentreGroup
                                    where roleDb.role_id == (int)UserRole.CentreGroupPropAdmin ||
                                    roleDb.role_id == (int)UserRole.ApproveMatrixAdmin ||
                                    roleDb.role_id == (int)UserRole.OrdererAdmin ||
                                    roleDb.role_id == (int)UserRole.RequestorAdmin
                                    select roleDb).ToList();
                    foreach (var cgAdmin in cgadmins) {
                        if (cgAdmin.Centre_Group.company_id == comp.id) {
                            statisticsCompany.is_user_company_stat_cgadmin = true;
                            break;
                        }
                    }

                    var cgRequestors = (from roleDb in participant.ParticipantRole_CentreGroup
                                    where roleDb.role_id == (int)UserRole.Requestor
                                    select roleDb).ToList();
                    foreach (var cgRequestor in cgRequestors) {
                        if (cgRequestor.Centre_Group.company_id == comp.id) {
                            statisticsCompany.is_user_company_stat_requestor = true;
                            break;
                        }
                    }

                    var cgOrderers = (from roleDb in participant.ParticipantRole_CentreGroup
                                        where roleDb.role_id == (int)UserRole.Orderer
                                        select roleDb).ToList();
                    foreach (var cgOrderer in cgOrderers) {
                        if (cgOrderer.Centre_Group.company_id == comp.id) {
                            statisticsCompany.is_user_company_stat_orderer = true;
                            break;
                        }
                    }

                    var cgAppMen = (from roleDb in participant.ParticipantRole_CentreGroup
                                      where roleDb.role_id == (int)UserRole.ApprovalManager 
                                      select roleDb).ToList();
                    foreach (var cgAppMan in cgAppMen) {
                        if (cgAppMan.Centre_Group.company_id == comp.id) {
                            statisticsCompany.is_user_company_stat_appman = true;
                            break;
                        }
                    }
                }

                statCompanies.Add(statisticsCompany);
            }

            var filterValues = new FilterValueRepository().GetFilterValuesByUserId(CurrentUser.ParticipantId);
            if (filterValues != null) {
                foreach (var filterValue in filterValues) {
                    int compId = (int)filterValue.country_id;
                    StatisticsCompany selStatComp = (from selCompDb in statCompanies
                                   where selCompDb.id == compId
                                   select selCompDb).FirstOrDefault();
                    if (selStatComp == null) {
                        var comp = new CompanyRepository().GetCompanyById(compId);
                        StatisticsCompany statisticsCompany = new StatisticsCompany();
                        statisticsCompany.id = comp.id;
                        statisticsCompany.country_code = comp.country_code;
                        statCompanies.Add(statisticsCompany);

                        selStatComp = statCompanies.Last();
                    }

                    if (filterValue.filed_type == (int)FilterType.AppMan) {
                        selStatComp.is_user_company_stat_appman = true;
                    } else if(filterValue.filed_type == (int)FilterType.Orderer) {
                        selStatComp.is_user_company_stat_orderer = true;
                    } else if (filterValue.filed_type == (int)FilterType.Requestor) {
                        selStatComp.is_user_company_stat_requestor = true;
                    }
                }
            }

            return statCompanies;
        }
        #endregion

        #region Http Get
        [HttpGet]
        public ActionResult GetXOptions() {
            List<DropDownItem> xOptions = new List<DropDownItem>();

            //bool isCompanyDisabled = (!CurrentUser.IsParticipantCompanyAdmin && !CurrentUser.IsParticipantCompanyStatisticsManager);
            //bool isAreaDisabled = (!CurrentUser.IsParticipantCompanyAdmin && 
            //    !CurrentUser.IsParticipantAreaAdmin &&
            //    !CurrentUser.IsParticipantAppAdmin &&
            //    !CurrentUser.IsParticipantCompanyStatisticsManager);
            //bool isSupplierDisabled = (!CurrentUser.IsParticipantCompanyAdmin && !CurrentUser.IsParticipantCompanyStatisticsManager);
            //bool isPgDisabled = (!CurrentUser.IsParticipantCompanyAdmin && !CurrentUser.IsParticipantAreaAdmin && !CurrentUser.IsParticipantCompanyStatisticsManager);
            //bool isRequestorDisabled = (!CurrentUser.IsParticipantCompanyAdmin && 
            //    !CurrentUser.IsParticipantAreaAdmin &&
            //    !CurrentUser.IsParticipantRequestor && !CurrentUser.IsParticipantCompanyStatisticsManager);
            //bool isOrdererDisabled = (!CurrentUser.IsParticipantCompanyAdmin &&
            //    !CurrentUser.IsParticipantAreaAdmin &&
            //    !CurrentUser.IsParticipantOrderer && !CurrentUser.IsParticipantCompanyStatisticsManager);

            xOptions.Add(new DropDownItem((int)AxisX.Company, RequestResource.Company, m_isCompanyDisabled));
            xOptions.Add(new DropDownItem((int)AxisX.Area, RequestResource.Area, m_isAreaDisabled));
            xOptions.Add(new DropDownItem((int)AxisX.Centre, RequestResource.Centre, m_isAreaDisabled));
            xOptions.Add(new DropDownItem((int)AxisX.Supplier, RequestResource.Supplier, m_isSupplierDisabled));
            xOptions.Add(new DropDownItem((int)AxisX.ParentPurchaseGroup, RequestResource.PurchaseGroup, m_isPgDisabled));
            xOptions.Add(new DropDownItem((int)AxisX.Requestor, RequestResource.Requestor, m_isRequestorDisabled));
            xOptions.Add(new DropDownItem((int)AxisX.Orderer, RequestResource.Orderer, m_isOrdererDisabled)); //user can be orderer in self orderer commodities

            //JsonResult jsonResult = Json(xOptions, JsonRequestBehavior.AllowGet);
            var jsonResult = GetJson(xOptions);

            return jsonResult;
        }

        [HttpGet]
        public ActionResult GetYOptions() {
            List<DropDownItem> yOptions = new List<DropDownItem>();

            yOptions.Add(new DropDownItem((int)AxisY.Price, RequestResource.Price));
            yOptions.Add(new DropDownItem((int)AxisY.Count, RequestResource.RequestCount));


            //JsonResult jsonResult = Json(yOptions, JsonRequestBehavior.AllowGet);
            var jsonResult = GetJson(yOptions);

            return jsonResult;
        }

        [HttpGet]
        public ActionResult GetUserCompanies() {
            //List<CompanyExtended> filterCompanies = GetFilterCompanies();
            List<StatisticsCompany> filterCompanies = m_StatisticsCompanies;

            //JsonResult jsonResult = Json(filterCompanies, JsonRequestBehavior.AllowGet);
            var jsonResult = GetJson(filterCompanies);

            return jsonResult;
        }

        [HttpGet]
        public ActionResult GetCurrencies() {
            var currencies = new CurrencyRepository().GetActiveCurrenciesJs();

            //JsonResult jsonResult = Json(currencies, JsonRequestBehavior.AllowGet);
            var jsonResult = GetJson(currencies);

            return jsonResult;
        }

        [HttpGet]
        public ActionResult GetFilterRequestors(string companies, bool isActiveOnly) {
            if (String.IsNullOrEmpty(companies)) {
                throw new Exception("No Company was selected");
            }

            int[] iCompaniesIds = GetCompaniesIds(companies);
            //string[] companyItems = companies.Split(',');
            //int[] iCompaniesIds = new int[companies.Length];
            //for (int i = 0; i < companyItems.Length; i++) {
            //    iCompaniesIds[i] = ConvertData.ToInt32(companyItems[i]);
            //}

            List<StatisticsFilter> filterRequestors = GetFilterRequestorsList(iCompaniesIds, isActiveOnly);

            //JsonResult jsonResult = Json(filterRequestors.OrderBy(x => x.name).ToList(), JsonRequestBehavior.AllowGet);
            var jsonResult = GetJson(filterRequestors.OrderBy(x => x.name).ToList());

            return jsonResult;
        }

        [HttpGet]
        public ActionResult GetFilterOrderers(string companies, bool isActiveOnly) {
            if (String.IsNullOrEmpty(companies)) {
                throw new Exception("No Company was selected");
            }

            int[] iCompaniesIds = GetCompaniesIds(companies);
           
            List<StatisticsFilter> filterRequestors = GetFilterOrderersList(iCompaniesIds, isActiveOnly);

            //JsonResult jsonResult = Json(filterRequestors.OrderBy(x => x.name).ToList(), JsonRequestBehavior.AllowGet);
            var jsonResult = GetJson(filterRequestors.OrderBy(x => x.name).ToList());

            return jsonResult;
        }

        [HttpGet]
        public ActionResult GetFilterAreas(string companies, bool isActiveOnly) {
            if (String.IsNullOrEmpty(companies)) {
                throw new Exception("No Company was selected");
            }

            int[] iCompaniesIds = GetCompaniesIds(companies);
            //string[] companyItems = companies.Split(',');
            //int[] iCompaniesIds = new int[companies.Length];
            //for (int i = 0; i < companyItems.Length; i++) {
            //    iCompaniesIds[i] = ConvertData.ToInt32(companyItems[i]);
            //}

            List<StatisticsFilter> filterAreas = GetFilterAreasList(iCompaniesIds, isActiveOnly);

            //JsonResult jsonResult = Json(filterAreas.OrderBy(x => x.name).ToList(), JsonRequestBehavior.AllowGet);
            var jsonResult = GetJson(filterAreas.OrderBy(x => x.name).ToList());

            return jsonResult;
        }

        [HttpGet]
        public ActionResult GetFilterCentres(string companies, bool isActiveOnly) {
            if (String.IsNullOrEmpty(companies)) {
                throw new Exception("No Company was selected");
            }

            int[] iCompaniesIds = GetCompaniesIds(companies);
            //string[] companyItems = companies.Split(',');
            //int[] iCompaniesIds = new int[companies.Length];
            //for (int i = 0; i < companyItems.Length; i++) {
            //    iCompaniesIds[i] = ConvertData.ToInt32(companyItems[i]);
            //}

            List<StatisticsFilter> filterCentres = GetFilterCentresList(iCompaniesIds, isActiveOnly);

            //JsonResult jsonResult = Json(filterCentres.OrderBy(x => x.name).ToList(), JsonRequestBehavior.AllowGet);
            var jsonResult = GetJson(filterCentres.OrderBy(x => x.name).ToList());

            return jsonResult;
        }

        [HttpGet]
        public ActionResult GetFilterSuppliers(string companies, bool isActiveOnly) {
            if (String.IsNullOrEmpty(companies)) {
                throw new Exception("No Company was selected");
            }

            int[] iCompaniesIds = GetCompaniesIds(companies);
            

            List<StatisticsFilter> filterSuppliers = GetFilterSuppliersList(iCompaniesIds, isActiveOnly);

            //JsonResult jsonResult = Json(filterSuppliers.OrderBy(x => x.name).ToList(), JsonRequestBehavior.AllowGet);
            var jsonResult = GetJson(filterSuppliers.OrderBy(x => x.name).ToList());

            return jsonResult;
        }

        [HttpGet]
        public ActionResult GetFilterCommodities(string companies, bool isActiveOnly) {
            if (String.IsNullOrEmpty(companies)) {
                throw new Exception("No Company was selected");
            }

            int[] iCompaniesIds = GetCompaniesIds(companies);


            List<StatisticsFilter> filterCommodities = GetFilterCommoditiesList(iCompaniesIds, isActiveOnly);

            //JsonResult jsonResult = Json(filterSuppliers.OrderBy(x => x.name).ToList(), JsonRequestBehavior.AllowGet);
            var jsonResult = GetJson(filterCommodities.OrderBy(x => x.name).ToList());

            return jsonResult;
        }

        [HttpGet]
        public ActionResult GetChartData(
            string strDateFrom,
            string strDateTo,
            string companiesList,
            int xItemType,
            int yItemType,
            int periodType,
            string xItemList,
            int currencyId,
            int iTop,
            bool isActiveOnly) {

            if (String.IsNullOrEmpty(companiesList)) {
                return null;
            }

            if (periodType != FILTER_PERIOD_ALL) {
                iTop = 0;
            }

            //bool isCompanyDisabled = (!CurrentUser.IsParticipantCompanyAdmin);
            //bool isAreaDisabled = (!CurrentUser.IsParticipantCompanyAdmin && !CurrentUser.IsParticipantAreaAdmin);
            //bool isSupplierDisabled = (!CurrentUser.IsParticipantCompanyAdmin);
            //bool isPgDisabled = (!CurrentUser.IsParticipantCompanyAdmin && !CurrentUser.IsParticipantAreaAdmin);
            //bool isRequestorDisabled = (!CurrentUser.IsParticipantCompanyAdmin &&
            //    !CurrentUser.IsParticipantAreaAdmin &&
            //    !CurrentUser.IsParticipantRequestor);
            //bool isOrdererDisabled = (!CurrentUser.IsParticipantCompanyAdmin &&   
            //    !CurrentUser.IsParticipantAreaAdmin && !CurrentUser.IsParticipantOrderer);

            if (xItemType == FILTER_COMPANY && m_isCompanyDisabled) {
                throw new Exception("Access Denied");
            } else if (xItemType == FILTER_AREA && m_isCompanyDisabled) {
                throw new Exception("Access Denied");
            } else if (xItemType == FILTER_SUPPLIER && m_isSupplierDisabled) {
                throw new Exception("Access Denied");
            } else if (xItemType == FILTER_COMMODITY && m_isPgDisabled) {
                throw new Exception("Access Denied");
            } else if (xItemType == FILTER_REQUESTOR && m_isRequestorDisabled) {
                throw new Exception("Access Denied");
            } else if (xItemType == FILTER_ORDERER && m_isOrdererDisabled) {
                throw new Exception("Access Denied");
            }
            
            string tmpXItemList = null;
            List<StatisticsFilter> filterItems = null;

            string[] companies = companiesList.Split('|');
            int[] companiesId = new int[companies.Length];
            for (int i = 0; i < companies.Length; i++) {
                companiesId[i] = ConvertData.ToInt32(companies[i]);
            }

            UserRepository userRepository = new UserRepository();
            bool isAllComAdmin = true;
            foreach (int compId in companiesId) {
               // int iComp = ConvertData.ToInt32(strComp);
                bool isCompanyAdmin = userRepository.IsCompanyAdmin(compId, CurrentUser.ParticipantId);
                bool isCompanyStatAdmin = userRepository.IsCompanyStatAdmin(compId, CurrentUser.ParticipantId);
                
                if (!isCompanyAdmin && !isCompanyStatAdmin) {
                    isAllComAdmin = false;
                    break;
                }
            }

            if (String.IsNullOrEmpty(xItemList)) {
                if (!isAllComAdmin) {
                    if (xItemType == FILTER_REQUESTOR) {
                        filterItems = GetFilterRequestorsList(companiesId, isActiveOnly);
                    } else if (xItemType == FILTER_ORDERER) {
                        filterItems = GetFilterOrderersList(companiesId, isActiveOnly);
                    } else if (xItemType == FILTER_AREA) {
                        filterItems = GetFilterAreasList(companiesId, isActiveOnly);
                    } else if (xItemType == FILTER_CENTER) {
                        filterItems = GetFilterCentresList(companiesId, isActiveOnly);
                    } else if (xItemType == FILTER_SUPPLIER) {
                        filterItems = GetFilterSuppliersList(companiesId, isActiveOnly);
                    } else if (xItemType == FILTER_COMMODITY) {
                        filterItems = GetFilterCommoditiesList(companiesId, isActiveOnly);
                    }
                }

                if (filterItems != null) {
                    tmpXItemList = "";
                    foreach (var item in filterItems) {
                        if (tmpXItemList.Length > 0) {
                            tmpXItemList += ",";
                        }
                        tmpXItemList += item.id;
                    }
                }
            } else {
                tmpXItemList = xItemList;
            }

            
            ChartItems chartItems = new ChartItems();
            SortedList<string, List<Period>> chartSourceDataList = null;
            List<Exchange_Rate> exchangeRates = null;
            try {
                chartSourceDataList = GetStatisticsData(
                    strDateFrom,
                    strDateTo,
                    companiesId,
                    xItemType,
                    yItemType,
                    periodType,
                    tmpXItemList,
                    iTop,
                    isActiveOnly,
                    ref currencyId,
                    out exchangeRates);
            } catch (Exception ex) {
                if (ex is ExchangeRateException) {
                    chartItems.ErrMsg = ex.Message;
                    JsonResult jsonErrResult = Json(chartItems, JsonRequestBehavior.AllowGet);

                    return jsonErrResult;
                }
            }

            
            chartItems.CurrencyId = currencyId;
            List<ChartDataSet> chartDataSets = new List<ChartDataSet>();

            ChartData chartData = new ChartData();

            DateTime dateFrom = ConvertUrlStringToDate(strDateFrom);
            DateTime dateTo = ConvertUrlStringToDate(strDateTo);
            List<Period> allPeriods = GetAllPeriods(periodType, dateFrom, dateTo);
            List<string> labels = GetChartLabels(periodType, dateFrom, dateTo, allPeriods);
            
            foreach (KeyValuePair<string, List<Period>> pair in chartSourceDataList) {
                ChartDataSet chartDataSet = new ChartDataSet();
                chartDataSet.label = GetChartDatasetLabel(pair.Key);
                chartDataSet.fill = false;
                chartDataSet.backgroundColor = GetRandomHtmlColor();
                chartDataSet.borderColor = chartDataSet.backgroundColor;
                //List<int> data = new List<int>();
                List<Period> periods = (List<Period>)pair.Value;

                //data.Add(ConvertData.ToInt32(periods[0].PeriodValue));
                List<int> data = GetChartDataSetValues(periods, allPeriods);

                chartDataSet.data = data;
                chartDataSets.Add(chartDataSet);
            }

            chartData.labels = labels;
            chartData.datasets = chartDataSets;

            #region Test Chart Data

            //List<string> labels = new List<string>();
            //labels.Add("Jan");
            //labels.Add("Feb");
            //labels.Add("Mar");
            ////labels.Add("Apr");

            //#region Dataset
            ////Datasets
            //List<ChartDataSet> chartDataSets = new List<ChartDataSet>();

            //ChartDataSet chartDataSet = new ChartDataSet();
            //chartDataSet.label = "test 1";
            //chartDataSet.backgroundColor = GetRandomHtmlColor();
            //List<int> data = new List<int>();
            //data.Add(10);
            //data.Add(15);
            //data.Add(16);
            //chartDataSet.data = data;
            //chartDataSets.Add(chartDataSet);

            //chartDataSet = new ChartDataSet();
            //chartDataSet.label = "test 2";
            //chartDataSet.backgroundColor = GetRandomHtmlColor();
            //data = new List<int>();
            //data.Add(15);
            //data.Add(15);
            //data.Add(2);
            //chartDataSet.data = data;
            //chartDataSets.Add(chartDataSet);

            //chartDataSet = new ChartDataSet();
            //chartDataSet.label = "test 3";
            //chartDataSet.backgroundColor = GetRandomHtmlColor();
            //data = new List<int>();
            //data.Add(145);
            //data.Add(18);
            //data.Add(20);
            //chartDataSet.data = data;
            //chartDataSets.Add(chartDataSet);

            //chartDataSet = new ChartDataSet();
            //chartDataSet.label = "test 4";
            //chartDataSet.backgroundColor = GetRandomHtmlColor();
            //data = new List<int>();
            //data.Add(14);
            //data.Add(18);
            //data.Add(26);
            //chartDataSet.data = data;
            //chartDataSets.Add(chartDataSet);

            //chartData.labels = labels;
            //chartData.datasets = chartDataSets;
            //#endregion
            #endregion

            #region Options
            //options
            ChartOptions chartOptions = new ChartOptions();

            ChartScales chartScales = new ChartScales();

            //List<ChartAxes> chartXAxeses = new List<ChartAxes>();
            //ChartAxes chartXAxes = new ChartAxes();
            //ChartTicks chartXTicks = new ChartTicks();
            //chartXTicks.maxTicksLimit = 20;
            //chartXAxes.ticks = chartXTicks;

            List<ChartAxes> chartYAxeses = new List<ChartAxes>();
            ChartAxes chartYAxes = new ChartAxes();
            ChartTicks chartYTicks = new ChartTicks();
            chartYTicks.beginAtZero = true;
            chartYAxes.ticks = chartYTicks;

            ChartScaleLabel chartScaleLabel = new ChartScaleLabel();
            chartScaleLabel.display = true;
            string strCurrency = null;
            if (yItemType == FILTER_Y_NUM_OF_REQUESTS) {
                chartScaleLabel.labelString = RequestResource.RequestCount;
            } else {
                var currency = new CurrencyRepository().GetCurrencyById(currencyId);
                strCurrency = currency.currency_code;
                chartScaleLabel.labelString = RequestResource.Cost + " (" + strCurrency + ")";
            }
            chartYAxes.scaleLabel = chartScaleLabel;

            //chartXAxeses.Add(chartXAxes);
            chartYAxeses.Add(chartYAxes);

            //chartScales.xAxes = chartXAxeses;
            chartScales.yAxes = chartYAxeses;

            chartOptions.scales = chartScales;

            ChartTitle chartTitle = new ChartTitle();
            chartTitle.display = true;
            chartTitle.text = GetChartTitle(xItemType, yItemType, periodType, dateFrom, dateTo, currencyId, strCurrency);
            chartTitle.fontSize = 16;
            chartOptions.title = chartTitle;
            chartOptions.maintainAspectRatio = false;
            #endregion

            chartItems.ChartData = chartData;
            chartItems.ChartOptions = chartOptions;
            chartItems.ExchangeRates = exchangeRates;

            //JsonResult jsonResult = Json(chartItems, JsonRequestBehavior.AllowGet);
            var jsonResult = GetJson(chartItems);

            return jsonResult;
        }

        public static string GetChartDatasetLabel(string origLabel) {
            if (String.IsNullOrEmpty(origLabel)) {
                return origLabel;
            }

            string[] items = origLabel.Split('~');

            return items[items.Length - 1];
        }

        public SortedList<string, List<Period>> GetStatisticsData(
            string strDateFrom,
            string strDateTo,
            int[] companiesId,
            int xItemType,
            int yItemType,
            int periodType,
            string xItemList,
            int iTop,
            bool isActiveOnly,
            ref int currencyId,
            out List<Exchange_Rate> exchangeRates) {

            
            if (Request != null) {
                foreach (var companyId in companiesId) {
                    if (!IsCompanyRoleAllowed(companyId, xItemType)) {
                        throw new Exception("User is not authorized for company " + companyId);
                    }
                }
            }

            DateTime dateFrom = ConvertUrlStringToDate(strDateFrom);
            DateTime dateTo = ConvertUrlStringToDate(strDateTo);

            
            Hashtable htSelXItems = GetSelXItems(xItemList);

            
            StatisticsRepository statisticsRepository = new StatisticsRepository();
            List<StatisticsRequest> statisticsRequests = statisticsRepository.GetRequestsByFilter(
                dateFrom,
                dateTo,
                companiesId);

            int defaultCurrencyId = currencyId;
            exchangeRates = null;
            if (yItemType == FILTER_Y_PRICE) {
                if (currencyId < 0) {
                    currencyId = GetDefaultCurrency(companiesId);
                }

                exchangeRates = new ExchangeRateRepository().GetCrossExchangeRatesJs();
            }

            if (htSelXItems != null && htSelXItems.Count > 0) {
                if (xItemType == FILTER_COMMODITY) {
                    IDictionaryEnumerator iEnum = htSelXItems.GetEnumerator();
                    ParentPgRepository ppgRepository = new ParentPgRepository();
                    Hashtable htTmpFilterItems = new Hashtable();
                    while (iEnum.MoveNext()) {
                        int parentPgId = (int)iEnum.Key;
                        Parent_Purchase_Group ppg = ppgRepository.GetParentPgById(parentPgId);
                        foreach (var pg in ppg.Purchase_Group) {
                            if (isActiveOnly && pg.active != true) {
                                continue;
                            }
                            if (!htTmpFilterItems.ContainsKey(pg.id)) {
                                htTmpFilterItems.Add(pg.id, null);
                            }
                        }
                    }

                    htSelXItems = htTmpFilterItems;
                }
            }

            Hashtable htColName = new Hashtable();
            SortedList<string, List<Period>> chartSourceDataList = new SortedList<string, List<Period>>();
            foreach (var statisticsRequest in statisticsRequests) {
                if (htSelXItems != null && htSelXItems.Count > 0) {
                    if (xItemType == FILTER_AREA && !htSelXItems.ContainsKey(statisticsRequest.centre_group_id)) {
                        continue;
                    } else if (statisticsRequest.supplier_id == null || xItemType == FILTER_SUPPLIER && !htSelXItems.ContainsKey(statisticsRequest.supplier_id)) {
                        continue;
                    } else if (xItemType == FILTER_COMMODITY && !htSelXItems.ContainsKey(statisticsRequest.purchase_group_id)) {
                        continue;
                    } else if (xItemType == FILTER_REQUESTOR && !htSelXItems.ContainsKey(statisticsRequest.requestor)) {
                        continue;
                    } else if (xItemType == FILTER_ORDERER && !htSelXItems.ContainsKey(statisticsRequest.orderer_id)) {
                        continue;
                    } else if (xItemType == FILTER_CENTER && !htSelXItems.ContainsKey(statisticsRequest.request_centre_id)) {
                        continue;
                    } 
                }

                if (yItemType == FILTER_Y_PRICE) {
                    if ((int)statisticsRequest.request_status == (int)RequestStatus.Closed) {
                        if (!statisticsRequest.price.HasValue) {
                            continue;
                        }
                        if (statisticsRequest.price.HasValue && statisticsRequest.price == 0) {
                            continue;
                        }
                    }

                    if ((int)statisticsRequest.request_status == (int)RequestStatus.Ordered) {
                        if (!statisticsRequest.estimated_price.HasValue) {
                            continue;
                        }
                        if (statisticsRequest.estimated_price.HasValue && statisticsRequest.estimated_price == 0) {
                            continue;
                        }
                    }
                }

                int itemId = DataNulls.INT_NULL;
                string itemName = null;

                GetFilterItemIdName(
                    statisticsRequest,
                    xItemType,
                    htColName,
                    out itemId,
                    out itemName);

                if (itemId == DataNulls.INT_NULL) {
                    continue;
                }
                                
                if (!chartSourceDataList.ContainsKey(itemName)) {
                    chartSourceDataList.Add(itemName, new List<Period>());
                }

                var periods = chartSourceDataList[itemName];
                decimal value = 0;
                if (yItemType == FILTER_Y_PRICE) {
                    if (statisticsRequest.request_status == (int)RequestStatus.Closed) {
                        value = GetRequestCost(statisticsRequest.price, (int)statisticsRequest.currency_id, currencyId, exchangeRates);
                    }
                    if (statisticsRequest.request_status == (int)RequestStatus.Ordered) {
                        value = GetRequestCost(statisticsRequest.estimated_price, (int)statisticsRequest.currency_id, currencyId, exchangeRates);
                    }
                    //value = (statisticsRequest.price == null) ? 0 : (Decimal)statisticsRequest.price;
                    //if (value == 0) {
                    //    value = (statisticsRequest.estimated_price == null) ? 0 : (Decimal)statisticsRequest.estimated_price;
                    //}
                } else {
                    value = 1;
                }
                SetPeriodValue(ref periods, (DateTime)statisticsRequest.issued, periodType, value);

            }

            if (iTop > 0) {
                return GetTopChartSourceDataList(chartSourceDataList, iTop);
            } 

            return chartSourceDataList;
        }

        
        private SortedList<string, List<Period>> GetTopChartSourceDataList(SortedList<string, List<Period>> chartSourceDataList, int iTop) {
            SortedList<decimal, string> topSortedList = new SortedList<decimal, string>();
            foreach (var chartDataList in chartSourceDataList) {
                decimal topValue = chartDataList.Value[0].PeriodValue;
                while (topSortedList.ContainsKey(topValue)) {
                    topValue += 0.001M;
                }
                topSortedList.Add(topValue, chartDataList.Key);
            }

            SortedList<string, List<Period>> topChartSourceDataList = new SortedList<string, List<Period>>();

            int iCount = 0;
            for (int i = topSortedList.Count - 1; i >= 0; i--) {
                string origName = topSortedList.ElementAt(i).Value;
                //decimal dValue = topSortedList.ElementAt(i).Key;
                string sortName = iCount.ToString().PadLeft(3, '0') + "~" + origName;

                var selItem = (from origItem in chartSourceDataList
                               where origItem.Key == origName
                               select origItem).FirstOrDefault();

                topChartSourceDataList.Add(sortName, selItem.Value);

                iCount++;

                if (iCount == iTop) {
                    break;
                }
            }

            return topChartSourceDataList;
        }

        

        private decimal GetRequestCost(decimal? price, int sourceCurrencyId, int destinCurrencyId, List<Exchange_Rate> exchangeRates) {
            if (sourceCurrencyId == destinCurrencyId) {
                decimal cost = (price == null) ? 0 : (Decimal)price;
                return cost;
            }

            if (price == null || price == 0) {
                return 0;
            }

            var exchangeRateS = (from exrDb in exchangeRates
                                where exrDb.source_currency_id == sourceCurrencyId &&
                                exrDb.destin_currency_id == destinCurrencyId
                                 select exrDb).FirstOrDefault();

            if (exchangeRateS != null) {
                decimal exCost = (decimal)price / exchangeRateS.exchange_rate1;
                return exCost;
            }


            var exchangeRateD = (from exrDb in exchangeRates
                                where exrDb.source_currency_id == destinCurrencyId &&
                                exrDb.destin_currency_id == sourceCurrencyId
                                 select exrDb).FirstOrDefault();

            if (exchangeRateD != null) {
                decimal exCost = (decimal)price * exchangeRateD.exchange_rate1;
                return exCost;
            }

            //var tmpExchangeRates = (from exrDb in exchangeRates
            //                     where exrDb.source_currency_id == currencyId 
            //                     select exrDb).ToList();
            //foreach (var exr in tmpExchangeRates) {
            //    int nextCurrencyId = exr.destin_currency_id;
            //    var nextExchangeRate = (from exrDb in exchangeRates
            //                         where (exrDb.source_currency_id == nextCurrencyId &&
            //                         exrDb.destin_currency_id == statisticsRequest.currency_id) ||
            //                         (exrDb.source_currency_id == statisticsRequest.currency_id &&
            //                         exrDb.destin_currency_id == nextCurrencyId)
            //                         select exrDb).FirstOrDefault();
            //    if (nextExchangeRate != null) {
            //        if (nextExchangeRate.source_currency_id == nextCurrencyId) {
            //            //decimal nextValue = 
            //        } else {

            //        }
            //    }
            //}

            var sourceCurr = new CurrencyRepository().GetCurrencyById((int)sourceCurrencyId);
            var destinCurr = new CurrencyRepository().GetCurrencyById(destinCurrencyId);

            string msg = String.Format(RequestResource.ExchangeRateMissing, sourceCurr.currency_code, destinCurr.currency_code);
            throw new ExchangeRateException(msg);
        }

        private int GetDefaultCurrency(int[] companiesId) {
            //if (companiesId.Length == 1) {
                var comp = new CompanyRepository().GetCompanyById(companiesId[0]);
                return (int)comp.default_currency_id;
            //}
        }

        private Hashtable GetSelXItems(string xItemList) {
            Hashtable htSelXItems = new Hashtable();
            if (!String.IsNullOrEmpty(xItemList)) {
                string[] selXItems = xItemList.Split(';');
                if (selXItems.Length > 0) {
                    //iSelXItems = new int[selXItems.Length];
                    for (int i = 0; i < selXItems.Length; i++) {
                        //    iSelXItems[i] = ConvertData.ToInt32(selXItems[i]);
                        if (!htSelXItems.ContainsKey(ConvertData.ToInt32(selXItems[i]))) {
                            htSelXItems.Add(ConvertData.ToInt32(selXItems[i]), null);
                        }
                    }

                }
            }

            return htSelXItems;
        }

        public static string GetChartTitle(
            int xItemType, 
            int yItemType, 
            int iPeriodType, 
            DateTime startDate, 
            DateTime endDate,
            int currencyId,
            string currencyCode) {

            string strCurrency = "";
            if (String.IsNullOrEmpty(currencyCode)) {
                if (yItemType == FILTER_Y_PRICE) {
                    var currency = new CurrencyRepository().GetCurrencyById(currencyId);
                    strCurrency = currency.currency_code;
                }
            } else {
                strCurrency = currencyCode;
            }

            string chartTitle = "";
            if (xItemType == FILTER_REQUESTOR) {
                chartTitle = RequestResource.Requestors;
            } else if (xItemType == FILTER_ORDERER) {
                chartTitle = RequestResource.Orderer;
            } else if (xItemType == FILTER_CENTER) {
                chartTitle = RequestResource.Centre;
            } else if (xItemType == FILTER_COMPANY) {
                chartTitle = RequestResource.Company;
            } else if (xItemType == FILTER_COMMODITY) {
                chartTitle = RequestResource.Commodity;
            } else if (xItemType == FILTER_AREA) {
                chartTitle = RequestResource.Area;
            } else if (xItemType == FILTER_APPROVE_MAN) {
                chartTitle = RequestResource.ApproveMan;
            } else if (xItemType == FILTER_SUPPLIER) {
                chartTitle = RequestResource.Supplier;
            }

            string numCost = (yItemType == FILTER_Y_PRICE) ? RequestResource.Cost + " (" + strCurrency + ")" : RequestResource.RequestCount;

            chartTitle += " - " + numCost;

            chartTitle += " (" + ConvertData.ToStringFromDateLocal(startDate) + " - " + ConvertData.ToStringFromDateLocal(endDate) + ")";

            return chartTitle;
        }




        /*
         * function getRandomColor() {
    var letters = '0123456789ABCDEF';
    var color = '#';
    for (var i = 0; i < 6; i++) {
        color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
}
         * */
        #endregion

        #region Http Post
        
        #endregion
    }

    public class ExchangeRateException : Exception {
        public ExchangeRateException(string errMsg) : base(errMsg) {
        }
    }
}