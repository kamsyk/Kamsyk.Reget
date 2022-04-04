using Kamsyk.Reget.Model.Common;
using Kamsyk.Reget.Model.DataDictionary;
using Kamsyk.Reget.Model.ExtendedModel;
using Kamsyk.Reget.Model.ExtendedModel.Supplier;
using Kamsyk.Reget.Model.Repositories.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using static Kamsyk.Reget.Model.Repositories.UserRepository;

namespace Kamsyk.Reget.Model.Repositories {
    public class CentreGroupRepository : BaseRepository<Centre_Group>, ICentreGroupRepository {
        #region Constants
        private const string UNIQUE_IDENT = "~";
        #endregion

        #region Enum
        public enum PurchaseGroupRequestor {
            CentreGroupAdmin = 1,
            AppMatrixCopy = 2
        }

        
        #endregion

        #region Properties
        private int m_TmpId = -1;

        Hashtable m_htImpliciteRequestors = null;
        Hashtable m_htImpliciteOrderers = null;


        #endregion

        #region Methods

        public List<Centre_Group> GetActiveCentreGroupsMyFirstByUserId(int userId) {
            Hashtable htCg = new Hashtable();

            //participant is Admin in CG
            var myCentreGroups = from cg in m_dbContext.Centre_Group
                                 join prcg in m_dbContext.ParticipantRole_CentreGroup on
                                 cg.id equals prcg.centre_group_id
                                 //new { cg.id, userId } equals new { prcg.centre_group_id, prcg.Field2 }
                                 where (cg.active == true &&
                                 prcg.participant_id == userId)
                                 //orderby cg.name
                                 select new {
                                     id = cg.id,
                                     name = cg.name
                                 };

            var tmpMysCgs = myCentreGroups.Distinct().OrderBy(x => x.name).ToList();
            List<Centre_Group> resCentreGroups = new List<Centre_Group>();
            foreach (var tmpCg in tmpMysCgs) {
                if (!htCg.ContainsKey(tmpCg.id)) {
                    htCg.Add(tmpCg.id, null);
                }
                Centre_Group cg = new Centre_Group();
                SetValues(tmpCg, cg);
                resCentreGroups.Add(cg);
            }


            var centreGroups = from cg in m_dbContext.Centre_Group
                               where cg.active == true
                               select new {
                                   id = cg.id,
                                   name = cg.name
                               };

            var tmpCgs = centreGroups.Distinct().OrderBy(x => x.name).ToList();


            foreach (var tmpCg in tmpCgs) {
                if (htCg.ContainsKey(tmpCg.id)) {
                    continue;
                }
                Centre_Group cg = new Centre_Group();
                SetValues(tmpCg, cg);
                resCentreGroups.Add(cg);
            }

            return resCentreGroups;

        }

        public List<CentreGroupAdminList> GetCentreGroupsByUserCompanies(int userId, List<int> companies, bool isDeactivatedCgLoaded, string rootUrl) {


            var centreGroups = (from cg in m_dbContext.Centre_Group
                                join prcg in m_dbContext.ParticipantRole_CentreGroup
                                on cg.id equals prcg.centre_group_id
                                where cg.id > -1 && companies.Contains(cg.company_id)
                                select new { id = cg.id, name = cg.name, company_id = cg.company_id, active = cg.active }).ToList();


            var tmpCgs = centreGroups.Distinct().OrderBy(x => x.name).ToList();
            Hashtable htCg = new Hashtable();
            //foreach (var cg in tmpCgs) {
            //    if (isDeactivatedCgLoaded || cg.active == true) {
            //        htCg.Add(cg.id, null);
            //    }
            //}

            List<CentreGroupAdminList> resCentreGroups = new List<CentreGroupAdminList>();
            foreach (var tmpCg in tmpCgs) {
                if (isDeactivatedCgLoaded || tmpCg.active == true) {

                    CentreGroupAdminList cg = new CentreGroupAdminList();
                    SetValues(tmpCg, cg);
                    if (!htCg.ContainsKey(cg.id)) {
                        cg.flag_url = GetCountryFlag(tmpCg.company_id, rootUrl);
                        cg.status_url = GetCgStatusImg(userId, tmpCg.id, tmpCg.active == true, rootUrl);
                        cg.is_active = tmpCg.active == true;

                        resCentreGroups.Add(cg);
                        htCg.Add(cg.id, null);
                    }
                }
            }

            var cgAdminRole = GetActiveCentreGroupsByUserId(userId, rootUrl);
            foreach (var cg in cgAdminRole) {
                if (!htCg.ContainsKey(cg.id)) {
                    resCentreGroups.Add(cg);
                    htCg.Add(cg.id, null);
                }
            }

            return resCentreGroups.OrderBy(x => x.name).ToList();

        }

        public List<CentreGroupAdminList> GetActiveCentreGroupsByUserId(int userId, string rootUrl) {

            var centreGroups = (from cg in m_dbContext.Centre_Group
                                join prcg in m_dbContext.ParticipantRole_CentreGroup on
                                cg.id equals prcg.centre_group_id
                                //new { cg.id, userId } equals new { prcg.centre_group_id, prcg.Field2 }
                                where (prcg.participant_id == userId &&
                                cg.active == true &&
                                cg.id > -1)
                                //orderby cg.name
                                select cg).Distinct().ToList();

            var centreGroupsComp = (from cg in m_dbContext.Centre_Group
                                    join prc in m_dbContext.Participant_Office_Role on
                                    cg.company_id equals prc.office_id
                                    where (prc.participant_id == userId &&
                                    cg.active == true &&
                                    cg.id > -1)
                                    select cg).Distinct().ToList();

            List<Centre_Group> tmpCgs = null;
            if (centreGroups == null && centreGroupsComp == null) {
                return null;
            } else if (centreGroups == null) {
                tmpCgs = centreGroupsComp.Distinct().OrderBy(x => x.name).ToList();
            } else if (centreGroupsComp == null) {
                tmpCgs = centreGroups.Distinct().OrderBy(x => x.name).ToList();
            } else {
                tmpCgs = new List<Centre_Group>();
                foreach (var cg in centreGroups) {
                    tmpCgs.Add(cg);
                }
                foreach (var cg in centreGroupsComp) {
                    var tmpCgEx = (from cgEx in tmpCgs
                                   where cgEx.id == cg.id
                                   select cgEx).FirstOrDefault();
                    if (tmpCgEx == null) {
                        tmpCgs.Add(cg);
                    }
                }
            }

            List<CentreGroupAdminList> resCentreGroups = new List<CentreGroupAdminList>();
            foreach (var tmpCg in tmpCgs) {
                CentreGroupAdminList cg = new CentreGroupAdminList();
                SetValues(tmpCg, cg);
                cg.flag_url = GetCountryFlag(tmpCg.company_id, rootUrl);
                cg.status_url = GetCgStatusImg(userId, tmpCg.id, tmpCg.active == true, rootUrl);
                cg.is_active = (tmpCg.active == true);
                resCentreGroups.Add(cg);
            }

            return resCentreGroups;

        }

        public List<CentreGroupAppMatrixCopy> GetAdministaredCentreGroupsByUserId(int userId, List<int> companies, bool isDeactivatedCgLoaded, string rootUrl) {
            var centreGroups = (from cg in m_dbContext.Centre_Group
                                join prcg in m_dbContext.ParticipantRole_CentreGroup
                                on cg.id equals prcg.centre_group_id
                                where cg.id > -1 && companies.Contains(cg.company_id)
                                select new { id = cg.id, name = cg.name, company_id = cg.company_id, currency_code = cg.Currency.currency_code, active = cg.active }).ToList();


            var tmpCgs = centreGroups.Distinct().OrderBy(x => x.name).ToList();
            Hashtable htCg = new Hashtable();
            foreach (var cg in tmpCgs) {
                if (isDeactivatedCgLoaded || cg.active == true) {
                    htCg.Add(cg.id, null);
                }
            }

            List<CentreGroupAppMatrixCopy> resCentreGroups = new List<CentreGroupAppMatrixCopy>();
            foreach (var tmpCg in tmpCgs) {
                if (isDeactivatedCgLoaded || tmpCg.active == true) {
                    CentreGroupAppMatrixCopy cg = new CentreGroupAppMatrixCopy();
                    SetValues(tmpCg, cg);
                    cg.flag_url = GetCountryFlag(tmpCg.company_id, rootUrl);
                    cg.status_url = GetCgStatusImg(userId, tmpCg.id, tmpCg.active == true, rootUrl);
                    cg.is_active = tmpCg.active == true;
                    resCentreGroups.Add(cg);
                }
            }

            return resCentreGroups;
        }

        public Centre_Group GetCentreGroupFullById(int cgId) {
            var centreGroup = (from cg in m_dbContext.Centre_Group
                               where cg.id == cgId &&
                               cg.id >= 0
                               select cg).FirstOrDefault();

            return centreGroup;
        }

        public CentreGroupExtended GetCentreGroupDataById(int cgId, int currentUserId) {
            if (cgId < 0) {
                return GetNewCentreGroup(currentUserId);
            }


            LightCg tmpCg = (from cg in m_dbContext.Centre_Group
                             where cg.id == cgId
                             select new LightCg {
                                 id = cg.id,
                                 name = cg.name,
                                 currency_id = cg.currency_id,
                                 company_id = cg.company_id,
                                 supplier_group = cg.SupplierGroup,
                                 orderer_supplier = cg.Orderer_Supplier,
                                 allow_free_supplier = cg.allow_free_supplier,
                                 is_active = cg.active
                             }).FirstOrDefault();

            //IQueryable<lightcg> tmpCg = centreGroup.FirstOrDefault();

            if (tmpCg == null) {
                return null;
            }

            List<CurrencyExtended> currencies = GetCurrencyData(cgId, tmpCg.currency_id, tmpCg.company_id);


            CentreGroupExtended resCentreGroup = new CentreGroupExtended();
            SetValues(tmpCg, resCentreGroup);

            #region Country Code
            //country code
            int companyId = resCentreGroup.company_id;
            var office = (from officeDb in m_dbContext.Company
                          where officeDb.id == companyId
                          select officeDb).FirstOrDefault();
            resCentreGroup.company_name = office.country_code;
            #endregion

            #region CgAdmins
            //check whether company admin is cg admin
            AddCgAdmins(tmpCg, resCentreGroup);
            // Hashtable htOfficeAdmins = new Hashtable();
            // var officeAdmins = (from pofDb in m_dbContext.Participant_Office_Role
            //                   where pofDb.office_id == tmpCg.company_id &&
            //                   pofDb.role_id == (int)UserRole.OfficeAdministrator
            //                   select pofDb).ToList();


            // if (officeAdmins != null) {
            //     foreach (var areaAdmin in officeAdmins) {
            //         if (areaAdmin.Participants.active == false) {
            //             continue;
            //         }
            //         var chAdmins = (from prCg in m_dbContext.ParticipantRole_CentreGroup
            //                       where prCg.centre_group_id == tmpCg.id &&
            //                       prCg.participant_id == areaAdmin.participant_id &&
            //                       prCg.role_id == (int)UserRole.CentreGroupAdmin
            //                       select prCg).FirstOrDefault();
            //         if (chAdmins == null) {
            //             //add missing role
            //             ParticipantRole_CentreGroup prCg = new ParticipantRole_CentreGroup();
            //             prCg.centre_group_id = tmpCg.id;
            //             prCg.participant_id = areaAdmin.participant_id;
            //             prCg.role_id = (int)UserRole.CentreGroupAdmin;
            //             m_dbContext.ParticipantRole_CentreGroup.Add(prCg);
            //             m_dbContext.SaveChanges();
            //         }

            //         if (!htOfficeAdmins.ContainsKey(areaAdmin.participant_id)) {
            //             htOfficeAdmins.Add(areaAdmin.participant_id, null);
            //         }
            //     }
            // }

            // //set cg admin
            // var admins = (from prCg in m_dbContext.ParticipantRole_CentreGroup
            //               where prCg.centre_group_id == tmpCg.id &&
            //               prCg.role_id == (int)UserRole.CentreGroupAdmin
            //               orderby prCg.Participants.surname, prCg.Participants.first_name
            //               select prCg).ToList();
            //// List<ParticipantRole_CentreGroup> prcgs = new List<ParticipantRole_CentreGroup>();
            // if (admins != null) {
            //     resCentreGroup.CgAdminsText = "";
            //     if(resCentreGroup.CgAdmins == null) {
            //         resCentreGroup.CgAdmins = new List<CgAdminsExtended>();
            //     }
            //     foreach (var admin in admins) {
            //         if (admin.Participants.active == false) {
            //             continue;
            //         }

            //         if (resCentreGroup.CgAdminsText.Length > 0) {
            //             resCentreGroup.CgAdminsText += ", ";
            //         }

            //         resCentreGroup.CgAdminsText += admin.Participants.first_name + " " + admin.Participants.surname;

            //         //is area admin
            //         var areaAdmin = (from areaAdminDb in m_dbContext.Participant_Office_Role
            //                       where areaAdminDb.office_id == tmpCg.company_id &&
            //                       areaAdminDb.participant_id == admin.participant_id &&
            //                       areaAdminDb.role_id == (int)UserRole.OfficeAdministrator
            //                       select new { participant_id = areaAdminDb .participant_id}).FirstOrDefault();

            //         bool isAreaAdmin = (areaAdmin != null);

            //         bool isCompanyAdmin = htOfficeAdmins.ContainsKey(admin.Participants.id);
            //         CgAdminsExtended pe = new CgAdminsExtended();
            //         pe.id = admin.Participants.id;
            //         pe.first_name = admin.Participants.first_name;
            //         pe.surname = admin.Participants.surname;
            //         pe.is_company_admin = isCompanyAdmin;
            //         if (isCompanyAdmin) {
            //             pe.is_appmatrix_admin = true;
            //             pe.is_cg_prop_admin = true;
            //             pe.is_orderer_admin = true;
            //             pe.is_requestor_admin = true;
            //         }
            //         resCentreGroup.CgAdmins.Add(pe);
            //     }
            // }
            #endregion

            #region Currencies
            //foreign currencies
            foreach (CurrencyExtended curr in currencies) {
                CurrencyExtended currencyExtended = new CurrencyExtended();
                SetValues(curr, currencyExtended);
                resCentreGroup.currency.Add(currencyExtended);
            }
            #endregion

            #region Deputy Orderers
            //deputy Orderers
            var depOrdRole = (from roleCgDb in m_dbContext.ParticipantRole_CentreGroup
                              where roleCgDb.centre_group_id == tmpCg.id &&
                              roleCgDb.role_id == (int)UserRole.DeputyOrderer
                              orderby roleCgDb.Participants.surname, roleCgDb.Participants.first_name
                              select roleCgDb).ToList();

            resCentreGroup.DeputyOrdererRO = "";
            if (depOrdRole != null) {
                foreach (var ordRole in depOrdRole) {
                    if (ordRole.Participants.active == false) {
                        continue;
                    }

                    OrdererExtended p = new OrdererExtended();
                    p.participant_id = ordRole.participant_id;
                    p.first_name = ordRole.Participants.first_name;
                    p.surname = ordRole.Participants.surname;

                    resCentreGroup.deputy_orderer.Add(p);

                    if (resCentreGroup.DeputyOrdererRO.Length > 0) {
                        resCentreGroup.DeputyOrdererRO += ", ";
                    }
                    resCentreGroup.DeputyOrdererRO += ordRole.Participants.surname + " " + ordRole.Participants.first_name;
                }
            }
            #endregion

            #region Supplier Groups
            //Supplier Groups
            if (tmpCg.supplier_group != null) {
                resCentreGroup.supplier_group_id = tmpCg.supplier_group.id;
            } else {
                resCentreGroup.supplier_group_id = DataNulls.INT_NULL;
            }

            //supplier orderer
            resCentreGroup.orderer_supplier_appmatrix = new List<OrdererSupplierAppMatrix>();
            if (tmpCg.orderer_supplier != null) {
                foreach (var ordSupp in tmpCg.orderer_supplier) {
                    if (ordSupp.Participants.active == false || ordSupp.Supplier.active == false) {
                        continue;
                    }

                    OrdererSupplierAppMatrix ordSuppAppM = new OrdererSupplierAppMatrix();
                    ordSuppAppM.orderer_id = ordSupp.orderer_id;
                    ordSuppAppM.supplier_id = ordSupp.supplier_id;
                    ordSuppAppM.first_name = ordSupp.Participants.first_name;
                    ordSuppAppM.surname = ordSupp.Participants.surname;
                    ordSuppAppM.supplier_name = ordSupp.Supplier.supp_name + " (" + ordSupp.Supplier.supplier_id + ")";

                    resCentreGroup.orderer_supplier_appmatrix.Add(ordSuppAppM);
                }
            }
            #endregion

            #region Centre
            //centre
            resCentreGroup.centre = new List<Centre>();
            List<Centre> centres = GetCgActiveCentreData(tmpCg.id);
            if (tmpCg.orderer_supplier != null) {
                foreach (var centre in centres) {
                    if (centre.active == false) {
                        continue;
                    }

                    Centre cgCentre = new Centre();
                    cgCentre.id = centre.id;
                    cgCentre.name = centre.name;

                    resCentreGroup.centre.Add(cgCentre);
                }
            }
            #endregion

            return resCentreGroup;

        }

        private void AddCgAdmins(LightCg tmpCg, CentreGroupExtended resCentreGroup) {
            //check whether company admin is cg admin
            Hashtable htOfficeAdmins = new Hashtable();
            var officeAdmins = (from pofDb in m_dbContext.Participant_Office_Role
                                where pofDb.office_id == tmpCg.company_id &&
                                pofDb.role_id == (int)UserRole.OfficeAdministrator
                                select pofDb).ToList();


            if (officeAdmins != null) {
                foreach (var areaAdmin in officeAdmins) {
                    if (areaAdmin.Participants.active == false) {
                        continue;
                    }

                    SetMissingCgAdminForOfficeAdmin(tmpCg.id, areaAdmin.participant_id, UserRole.CentreGroupPropAdmin);
                    SetMissingCgAdminForOfficeAdmin(tmpCg.id, areaAdmin.participant_id, UserRole.ApproveMatrixAdmin);
                    SetMissingCgAdminForOfficeAdmin(tmpCg.id, areaAdmin.participant_id, UserRole.RequestorAdmin);
                    SetMissingCgAdminForOfficeAdmin(tmpCg.id, areaAdmin.participant_id, UserRole.OrdererAdmin);

                    //var chAdmins = (from prCg in m_dbContext.ParticipantRole_CentreGroup
                    //                where prCg.centre_group_id == tmpCg.id &&
                    //                prCg.participant_id == areaAdmin.participant_id &&
                    //                prCg.role_id == (int)UserRole.CentreGroupAdmin
                    //                select prCg).FirstOrDefault();
                    //if (chAdmins == null) {
                    //    //add missing role
                    //    ParticipantRole_CentreGroup prCg = new ParticipantRole_CentreGroup();
                    //    prCg.centre_group_id = tmpCg.id;
                    //    prCg.participant_id = areaAdmin.participant_id;
                    //    prCg.role_id = (int)UserRole.CentreGroupAdmin;
                    //    m_dbContext.ParticipantRole_CentreGroup.Add(prCg);
                    //    m_dbContext.SaveChanges();
                    //}

                    if (!htOfficeAdmins.ContainsKey(areaAdmin.participant_id)) {
                        htOfficeAdmins.Add(areaAdmin.participant_id, null);
                    }
                }
            }

            //set cg admin
            var admins = (from prCg in m_dbContext.ParticipantRole_CentreGroup
                          where prCg.centre_group_id == tmpCg.id &&
                          (
                          prCg.role_id == (int)UserRole.CentreGroupPropAdmin ||
                          prCg.role_id == (int)UserRole.ApproveMatrixAdmin ||
                          prCg.role_id == (int)UserRole.RequestorAdmin ||
                          prCg.role_id == (int)UserRole.OrdererAdmin
                          )
                          orderby prCg.Participants.surname, prCg.Participants.first_name
                          select prCg).ToList();

            if (admins != null) {
                resCentreGroup.CgAdminsText = "";
                if (resCentreGroup.cg_admin == null) {
                    resCentreGroup.cg_admin = new List<CgAdminsExtended>();
                }

                foreach (var admin in admins) {
                    var cgAdmin = (from areaAdmin in resCentreGroup.cg_admin
                                   where areaAdmin.id == admin.participant_id
                                   select areaAdmin).FirstOrDefault();
                    if (cgAdmin == null) {
                        bool isCompanyAdmin = htOfficeAdmins.ContainsKey(admin.Participants.id);
                        cgAdmin = new CgAdminsExtended();
                        cgAdmin.id = admin.Participants.id;
                        cgAdmin.first_name = admin.Participants.first_name;
                        cgAdmin.surname = admin.Participants.surname;
                        cgAdmin.is_company_admin = isCompanyAdmin;
                        cgAdmin.is_appmatrix_admin = false;
                        cgAdmin.is_cg_prop_admin = false;
                        cgAdmin.is_orderer_admin = false;
                        cgAdmin.is_requestor_admin = false;
                        resCentreGroup.cg_admin.Add(cgAdmin);
                    }

                    if (admin.role_id == (int)UserRole.CentreGroupPropAdmin) {
                        cgAdmin.is_cg_prop_admin = true;
                    }
                    if (admin.role_id == (int)UserRole.ApproveMatrixAdmin) {
                        cgAdmin.is_appmatrix_admin = true;
                    }
                    if (admin.role_id == (int)UserRole.OrdererAdmin) {
                        cgAdmin.is_orderer_admin = true;
                    }
                    if (admin.role_id == (int)UserRole.RequestorAdmin) {
                        cgAdmin.is_requestor_admin = true;
                    }
                }
            }

            ////set cg admin
            //var admins = (from prCg in m_dbContext.ParticipantRole_CentreGroup
            //              where prCg.centre_group_id == tmpCg.id &&
            //              prCg.role_id == (int)UserRole.CentreGroupAdmin
            //              orderby prCg.Participants.surname, prCg.Participants.first_name
            //              select prCg).ToList();
            //// List<ParticipantRole_CentreGroup> prcgs = new List<ParticipantRole_CentreGroup>();
            //if (admins != null) {
            //    resCentreGroup.CgAdminsText = "";
            //    if (resCentreGroup.CgAdmins == null) {
            //        resCentreGroup.CgAdmins = new List<CgAdminsExtended>();
            //    }
            //    foreach (var admin in admins) {
            //        if (admin.Participants.active == false) {
            //            continue;
            //        }

            //        if (resCentreGroup.CgAdminsText.Length > 0) {
            //            resCentreGroup.CgAdminsText += ", ";
            //        }

            //        resCentreGroup.CgAdminsText += admin.Participants.first_name + " " + admin.Participants.surname;

            //        ////is area admin
            //        //var areaAdmin = (from areaAdminDb in m_dbContext.Participant_Office_Role
            //        //                 where areaAdminDb.office_id == tmpCg.company_id &&
            //        //                 areaAdminDb.participant_id == admin.participant_id &&
            //        //                 areaAdminDb.role_id == (int)UserRole.OfficeAdministrator
            //        //                 select new { participant_id = areaAdminDb.participant_id }).FirstOrDefault();

            //        //bool isAreaAdmin = (areaAdmin != null);

            //        bool isCompanyAdmin = htOfficeAdmins.ContainsKey(admin.Participants.id);
            //        CgAdminsExtended pe = new CgAdminsExtended();
            //        pe.id = admin.Participants.id;
            //        pe.first_name = admin.Participants.first_name;
            //        pe.surname = admin.Participants.surname;
            //        pe.is_company_admin = isCompanyAdmin;
            //        if (isCompanyAdmin) {
            //            pe.is_appmatrix_admin = true;
            //            pe.is_cg_prop_admin = true;
            //            pe.is_orderer_admin = true;
            //            pe.is_requestor_admin = true;
            //        }
            //        resCentreGroup.CgAdmins.Add(pe);
            //    }
            //}
        }

        private void SetMissingCgAdminForOfficeAdmin(int cgId, int participantId, UserRole userRole) {
            var chAdmins = (from prCg in m_dbContext.ParticipantRole_CentreGroup
                            where prCg.centre_group_id == cgId &&
                            prCg.participant_id == participantId &&
                            prCg.role_id == (int)userRole
                            select prCg).FirstOrDefault();
            if (chAdmins == null) {
                //add missing role
                ParticipantRole_CentreGroup prCg = new ParticipantRole_CentreGroup();
                prCg.centre_group_id = cgId;
                prCg.participant_id = participantId;
                prCg.role_id = (int)userRole;
                m_dbContext.ParticipantRole_CentreGroup.Add(prCg);
                m_dbContext.SaveChanges();
            }
        }

        private CentreGroupExtended GetNewCentreGroup(int currentUserId) {
            CentreGroupExtended newCg = new CentreGroupExtended();
            newCg.id = -1;
            newCg.company_id = -1;
            newCg.currency_id = -1;
            newCg.is_active = true;

            //List<CurrencyExtended> currencies = GetActiveCurrencyData();

            //foreign currencies
            newCg.currency = null;
            //foreach (CurrencyExtended curr in currencies) {
            //    CurrencyExtended currencyExtended = new CurrencyExtended();
            //    SetValues(curr, currencyExtended);
            //    newCg.CurrencyExtended.Add(currencyExtended);
            //}

            //purchase group data
            newCg.purchase_group = new List<Purchase_Group>();

            //centre data
            newCg.centre = new List<Centre>();


            var pd = (from pdDb in m_dbContext.Participants
                      where pdDb.id == currentUserId
                      select pdDb).FirstOrDefault();

            newCg.CgAdminsText = pd.surname + " " + pd.first_name;

            return newCg;
        }

        private List<CentreGroupCurrency> GetForeignCurrencies(int cgId) {

            string sql = "SELECT * FROM CentreGroup_Currency cgc " +
                " INNER JOIN Currency cd" +
                " ON cd.id=cgc.currency_id" +
                " WHERE cd.active=1 AND cgc.centre_group_id=" + cgId;

            var centreGroupCurrencies = m_dbContext.Database.SqlQuery<CentreGroupCurrency>(sql).ToList();

            return centreGroupCurrencies;

        }

        public Centre_Group GetCentreGroupAllDataById(int cgId) {

            var centreGroup = from cg in m_dbContext.Centre_Group
                              where cg.id == cgId
                              select cg;


            return centreGroup.FirstOrDefault();

        }

        public List<Centre_Group> GetActiveCentreGroupDataByCompanyId(int companyId) {

            var centreGroups = (from cg in m_dbContext.Centre_Group
                              where cg.company_id == companyId 
                              && cg.active == true
                              select cg).ToList();


            return centreGroups;

        }

        public List<Currency> GetCgCurrencies(int cgId) {

            string sql = "SELECT cgc.currency_id FROM CentreGroup_Currency"
                 + " WHERE cgc.centre_group_id=" + cgId;
            var currencyList = m_dbContext.Currency.SqlQuery(sql);


            return currencyList.ToList();

        }

        public List<ParticipantRole_CentreGroup> GetDeputyOrderers(int centreGroupId) {
            //deputy Orderers
            var depOrdRole = (from roleCgDb in m_dbContext.ParticipantRole_CentreGroup
                              where roleCgDb.centre_group_id == centreGroupId &&
                              roleCgDb.role_id == (int)UserRole.DeputyOrderer
                              orderby roleCgDb.Participants.surname, roleCgDb.Participants.first_name
                              select roleCgDb).ToList();

            return depOrdRole;
        }

        public List<ParticipantRole_CentreGroup> GetCgAdmins(int centreGroupId) {
            var depOrdRole = (from roleCgDb in m_dbContext.ParticipantRole_CentreGroup
                              where roleCgDb.centre_group_id == centreGroupId &&
                              (roleCgDb.role_id == (int)UserRole.CentreGroupPropAdmin ||
                              roleCgDb.role_id == (int)UserRole.ApproveMatrixAdmin ||
                              roleCgDb.role_id == (int)UserRole.OrdererAdmin ||
                              roleCgDb.role_id == (int)UserRole.RequestorAdmin)
                              orderby roleCgDb.Participants.surname, roleCgDb.Participants.first_name
                              select roleCgDb).ToList();

            return depOrdRole;
        }

        public CgAdminRoles GetCgAdminRoles(int cgId, int userId) {
            CgAdminRoles cgAdminRoles = new CgAdminRoles();
            cgAdminRoles.is_readonly = false;


            var participantRoleCentreGroup = from pgcg in m_dbContext.ParticipantRole_CentreGroup
                                             where pgcg.centre_group_id == cgId &&
                                             pgcg.participant_id == userId &&
                                             (
                                             pgcg.role_id == (int)UserRole.CentreGroupPropAdmin ||
                                             pgcg.role_id == (int)UserRole.ApproveMatrixAdmin ||
                                             pgcg.role_id == (int)UserRole.RequestorAdmin ||
                                             pgcg.role_id == (int)UserRole.OrdererAdmin
                                             )
                                             select pgcg;

            if (participantRoleCentreGroup.FirstOrDefault() != null) {
                foreach (var prcg in participantRoleCentreGroup) {
                    if (prcg.role_id == (int)UserRole.CentreGroupPropAdmin) {
                        cgAdminRoles.is_cg_property_admin = true;
                    } else if (prcg.role_id == (int)UserRole.ApproveMatrixAdmin) {
                        cgAdminRoles.is_cg_appmatrix_admin = true;
                    }
                    if (prcg.role_id == (int)UserRole.OrdererAdmin) {
                        cgAdminRoles.is_cg_orderer_admin = true;
                    }
                    if (prcg.role_id == (int)UserRole.RequestorAdmin) {
                        cgAdminRoles.is_cg_requestor_admin = true;
                    }
                }
            } else {
                int companyId = m_dbContext.Centre_Group.Find(cgId).company_id;

                var officeAdmin = (from pofDb in m_dbContext.Participant_Office_Role
                                   where pofDb.office_id == companyId &&
                                   pofDb.participant_id == userId &&
                                   pofDb.role_id == (int)UserRole.OfficeAdministrator
                                   select new { participant_id = pofDb.participant_id }).FirstOrDefault();

                cgAdminRoles.is_readonly = (officeAdmin == null);
            }



            return cgAdminRoles;
        }

        public string IsCentreAssigned(int centreId, int cgId) {
            var centre = (from cd in m_dbContext.Centre
                          where cd.id == centreId
                          select cd).FirstOrDefault();

            if (centre.Centre_Group == null || centre.Centre_Group.Count == 0) {
                return null;
            }

            if (centre.Centre_Group.ElementAt(0).id != cgId) {
                return centre.Centre_Group.ElementAt(0).name;
            }

            return null;
        }

        public List<CurrencyExtended> GetActiveCurrencyData() {
            var currencies = from cur in m_dbContext.Currency
                             where cur.active == true
                             orderby cur.currency_code
                             select new {
                                 id = cur.id,
                                 currency_code = cur.currency_code,
                                 currency_name = cur.currency_name,
                                 is_set = false
                             };

            if (currencies == null) {
                return null;
            }

            var tmpCurrencies = currencies.ToList();

            List<CurrencyExtended> retCurrencyList = new List<CurrencyExtended>();
            foreach (var tmpCurrency in tmpCurrencies) {
                CurrencyExtended currency = new CurrencyExtended();
                SetValues(tmpCurrency, currency);
                currency.currency_code_name = currency.currency_code + " - " + currency.currency_name;

                retCurrencyList.Add(currency);
            }

            return retCurrencyList;
        }

        public List<CurrencyExtended> GetCurrencyData(int cgId, int currentCurrencyId, int? companyId) {

            if (companyId == null) {
                var cg = (from cgDb in m_dbContext.Centre_Group
                          where cgDb.id == cgId
                          select new {
                              id = cgDb.id,
                              company_id = cgDb.company_id
                          }).FirstOrDefault();
                if (cg != null) {
                    companyId = cg.company_id;
                }
            }

            var currencies = from cur in m_dbContext.Currency
                             where cur.active == true || cur.id == currentCurrencyId
                             orderby cur.currency_code
                             select new {
                                 id = cur.id,
                                 currency_code = cur.currency_code,
                                 currency_name = cur.currency_name,
                                 company = cur.Company,
                                 //currency_code_name = cur.currency_name,
                                 is_set = false
                             };

            if (currencies == null) {
                return null;
            }

            var tmpCurrencies = currencies.ToList();

            List<CentreGroupCurrency> foreignCurrencies = GetForeignCurrencies(cgId);

            List<CurrencyExtended> retCurrencyList = new List<CurrencyExtended>();
            foreach (var tmpCurrency in tmpCurrencies) {
                if (tmpCurrency.company == null || tmpCurrency.company.Count == 0) {
                    continue;
                }


                //if (tmpCurrency.company.ElementAt(0).id != companyId) {
                //    continue;
                //}

                var comCurr = (from compCurrDb in tmpCurrency.company
                               where compCurrDb.id == companyId
                               select compCurrDb).FirstOrDefault();
                if (comCurr == null) {
                    continue;
                }

                CurrencyExtended currency = new CurrencyExtended();
                SetValues(tmpCurrency, currency);
                currency.currency_code_name = currency.currency_code + " - " + currency.currency_name;

                var forCur = from fc in foreignCurrencies
                             where fc.id == currency.id
                             select new { fc.id };

                if (forCur.FirstOrDefault() != null) {
                    currency.is_set = true;
                }

                retCurrencyList.Add(currency);
            }

            return retCurrencyList;

        }

        //private List<CurrencyExtended> PrepareCurrencies(List<> tmpCurrencies, List<CentreGroupCurrency> foreignCurrencies) {
        //    List<CurrencyExtended> retCurrencyList = new List<CurrencyExtended>();
        //    foreach (var tmpCurrency in tmpCurrencies) {
        //        CurrencyExtended currency = new CurrencyExtended();
        //        SetValues(tmpCurrency, currency);
        //        currency.currency_code_name = currency.currency_code + " - " + currency.currency_name;

        //        if (foreignCurrencies == null) {
        //            var forCur = from fc in foreignCurrencies
        //                         where fc.id == currency.id
        //                         select new { fc.id };

        //            if (forCur.FirstOrDefault() != null) {
        //                currency.is_set = true;
        //            }
        //        }

        //        retCurrencyList.Add(currency);
        //    }

        //    return retCurrencyList;
        //}

        public List<CentreExtended> GetActiveCentreData() {

            var centres = from cen in m_dbContext.Centre
                          where cen.active == true
                          orderby cen.name
                          select new {
                              id = cen.id,
                              name = cen.name,
                              centre_group_id = -1,
                              centreGroup = cen.Centre_Group
                          };

            if (centres == null) {
                return null;
            }

            var tmpCentres = centres.ToList();


            List<CentreExtended> retCentreList = new List<CentreExtended>();
            foreach (var tmpCentre in tmpCentres) {
                CentreExtended centreExtended = new CentreExtended();
                SetValues(tmpCentre, centreExtended);
                if (tmpCentre.centreGroup != null && tmpCentre.centreGroup.Count > 0) {
                    //centreExtended.name += ", " + strAreaLabel + ": " + tmpCentre.centreGroup.FirstOrDefault().name;
                    centreExtended.centre_group_id = tmpCentre.centreGroup.FirstOrDefault().id;
                }
                retCentreList.Add(centreExtended);
            }

            return retCentreList;

        }

        public List<CentreExtended> GetActiveNotAssignedCentresData(int cgId) {
            var cg = (from cgDb in m_dbContext.Centre_Group
                      where cgDb.id == cgId
                      select cgDb).FirstOrDefault();
            int compId = cg.company_id;

            var centres = from cen in m_dbContext.Centre
                          where cen.active == true &&
                          cen.company_id == compId &&
                          cen.name != CentreRepository.DRAFT
                          orderby cen.name
                          select new {
                              id = cen.id,
                              name = cen.name,
                              centre_group_id = -1,
                              centreGroup = cen.Centre_Group
                          };

            if (centres == null) {
                return null;
            }

            var tmpCentres = centres.ToList();


            List<CentreExtended> retCentreList = new List<CentreExtended>();
            foreach (var tmpCentre in tmpCentres) {
                if (tmpCentre.centreGroup.Count > 0 && tmpCentre.centreGroup.ElementAt(0).active == true) {
                    continue;
                }

                CentreExtended centreExtended = new CentreExtended();
                SetValues(tmpCentre, centreExtended);
                if (tmpCentre.centreGroup != null && tmpCentre.centreGroup.Count > 0) {
                    centreExtended.centre_group_id = tmpCentre.centreGroup.FirstOrDefault().id;
                }
                retCentreList.Add(centreExtended);
            }

            return retCentreList;

        }

        public List<CentreExtended> GetActiveCompanyCentresData(int cgId, int officeId) {
            //var cg = (from cgDb in m_dbContext.Centre_Group
            //          where cgDb.id == cgId
            //          select cgDb).FirstOrDefault();
            //int compId = cg.company_id;

            var centres = from cen in m_dbContext.Centre
                          where cen.active == true &&
                          cen.company_id == officeId &&
                          cen.name != CentreRepository.DRAFT
                          orderby cen.name
                          select new {
                              id = cen.id,
                              name = cen.name,
                              centre_group_id = -1,
                              active = cen.active,
                              centreGroup = cen.Centre_Group
                          };

            if (centres == null) {
                return null;
            }

            var tmpCentres = centres.ToList();


            List<CentreExtended> retCentreList = new List<CentreExtended>();
            foreach (var tmpCentre in tmpCentres) {
                if (tmpCentre.centreGroup.Count > 0 && tmpCentre.centreGroup.ElementAt(0).id == cgId) {
                    continue;
                }


                CentreExtended centreExtended = new CentreExtended();
                SetValues(tmpCentre, centreExtended);
                if (tmpCentre.centreGroup != null && tmpCentre.centreGroup.Count > 0) {
                    centreExtended.centre_group_id = tmpCentre.centreGroup.FirstOrDefault().id;
                }
                retCentreList.Add(centreExtended);
            }

            return retCentreList;

        }


        public List<Centre> GetCgActiveCentreData(int cgId) {

            var centreGroup = from cg in m_dbContext.Centre_Group
                              where cg.id == cgId //&&
                              //cg.active == true
                              select new {
                                  centres = cg.Centre
                              };

            if (centreGroup == null) {
                return null;
            }

            var tmpCg = centreGroup.FirstOrDefault();
            if (tmpCg.centres == null) {
                return null;
            }

            ////List< GetActiveCentreGroups();

            List<Centre> retCentreList = new List<Centre>();
            var tmpSortCentres = tmpCg.centres.OrderBy(x => x.name);
            foreach (var centre in tmpSortCentres) {
                if (centre.active == false) {
                    continue;
                }
                Centre tmpCentre = new Centre();
                SetValues(centre, tmpCentre, new List<string> { "id", "name", "active" });

                retCentreList.Add(tmpCentre);
            }

            return retCentreList;

        }

        public List<Parent_Purchase_Group> GetParentPurchaseGroups(int companyGroupId, string strCurrCultureName) {

            var tmpPpgList = (from ppgDb in m_dbContext.Parent_Purchase_Group
                              where ppgDb.company_group_id == companyGroupId
                              select ppgDb).ToList();

            if (tmpPpgList == null) {
                return null;
            }


            //List<PurchaseGroupExtended> retPurchaseGroupList = new List<PurchaseGroupExtended>();
            //foreach (KeyValuePair<string, PurchaseGroupExtended> kvpPg in sortPurchaseGroupList) {
            //    PurchaseGroupExtended purchaseGroup = kvpPg.Value;
            //    if (purchaseGroup.group_loc_name.IndexOf(UNIQUE_IDENT) > -1) {
            //        purchaseGroup.group_loc_name = purchaseGroup.group_loc_name.Substring(0, purchaseGroup.group_loc_name.IndexOf(UNIQUE_IDENT));
            //    }
            //    retPurchaseGroupList.Add(purchaseGroup);
            //}


            List<Parent_Purchase_Group> retPpgList = new List<Parent_Purchase_Group>();
            foreach (var ppg in tmpPpgList) {
                Parent_Purchase_Group tmpPpg = new Parent_Purchase_Group();
                SetValues(ppg, tmpPpg, new List<string> { "id", "name" });
                SetParentPurchaseGroupLocName(ppg, tmpPpg, strCurrCultureName);

                retPpgList.Add(tmpPpg);
            }

            SortedList<string, Parent_Purchase_Group> sortParentPurchaseGroupList = SortParentPurchaseGroupByName(retPpgList);

            List<Parent_Purchase_Group> sortRetPpgList = new List<Parent_Purchase_Group>();
            foreach (KeyValuePair<string, Parent_Purchase_Group> kvpPg in sortParentPurchaseGroupList) {
                Parent_Purchase_Group sortPpg = kvpPg.Value;
                Parent_Purchase_Group tmpPpg = new Parent_Purchase_Group();
                SetValues(sortPpg, tmpPpg, new List<string> { "id", "name" });
                SetParentPurchaseGroupLocName(sortPpg, tmpPpg, strCurrCultureName);

                sortRetPpgList.Add(tmpPpg);
            }


            return sortRetPpgList;

        }

        private void SetParentPurchaseGroupLocName(Parent_Purchase_Group parentPurchaseGroup, Parent_Purchase_Group tmpParentPurchaseGroup, string strCurrCultureName) {
            //Loc Name
            var locPgGroup = from locPg in parentPurchaseGroup.Parent_Purchase_Group_Local
                             where locPg.parent_purchase_group_id == parentPurchaseGroup.id &&
                             locPg.culture.ToLower() == strCurrCultureName.ToLower()
                             select locPg;

            var pgLocList = locPgGroup.ToList();

            if (pgLocList != null && pgLocList.Count > 0) {
                tmpParentPurchaseGroup.name = pgLocList[0].local_text;
            }
        }


        public List<PurchaseGroupExtended> GetCgPurchaseGroupData(
            int cgId,
            string strCurrCultureName,
            bool isCgAdmin,
            bool isDisabledLoaded,
            string rootUrl,
            PurchaseGroupRequestor pgReq) {

            using (TransactionScope transaction = new TransactionScope()) {
                try {
                    var centreGroup = from cg in m_dbContext.Centre_Group
                                      where cg.id == cgId
                                      select cg;

                    if (centreGroup == null) {
                        return new List<PurchaseGroupExtended>();
                    }

                    var tmpCg = centreGroup.FirstOrDefault();

                    if (tmpCg == null) {
                        return new List<PurchaseGroupExtended>();
                    }

                    if (tmpCg.Purchase_Group == null) {
                        tmpCg.Purchase_Group = new List<Purchase_Group>();
                    }


                    List<PurchaseGroupExtended> tmpRetPurchaseGroupList = new List<PurchaseGroupExtended>();


                    //syka todo to speed up the loading it is neccessary to load it partially

                    bool isNewStructLoaded = false;
                    foreach (var pg in tmpCg.Purchase_Group) {
                        if (pg.PurchaseGroup_Requestor.Count > 0 || pg.PurchaseGroup_Orderer.Count > 0) {
                            isNewStructLoaded = true;
                            break;
                        }
                    }

                    //Hashtable htPg = new Hashtable();
                    foreach (var purchaseGroup in tmpCg.Purchase_Group) {
                        if ((purchaseGroup.active == false && !isCgAdmin) || (purchaseGroup.active == false && !isDisabledLoaded)) {
                            continue;
                        }

                        PurchaseGroupExtended tmpPurchaseGroup = GetPurchaseGroup(purchaseGroup, tmpCg, strCurrCultureName, rootUrl, isNewStructLoaded);
                        tmpPurchaseGroup.is_visible = true;
                        //tmpPurchaseGroup.decimal_separator = strDecimalSeparator;

                        tmpRetPurchaseGroupList.Add(tmpPurchaseGroup);
                    }



                    SortedList<string, PurchaseGroupExtended> sortPurchaseGroupList = SortPurchaseGroupByLocName(tmpRetPurchaseGroupList);

                    List<PurchaseGroupExtended> retPurchaseGroupList = new List<PurchaseGroupExtended>();
                    if (sortPurchaseGroupList != null) {
                        foreach (KeyValuePair<string, PurchaseGroupExtended> kvpPg in sortPurchaseGroupList) {
                            PurchaseGroupExtended purchaseGroup = kvpPg.Value;
                            if (purchaseGroup.group_loc_name.IndexOf(UNIQUE_IDENT) > -1) {
                                purchaseGroup.group_loc_name = purchaseGroup.group_loc_name.Substring(0, purchaseGroup.group_loc_name.IndexOf(UNIQUE_IDENT));
                            }
                            retPurchaseGroupList.Add(purchaseGroup);
                        }
                    }

                    if (isCgAdmin && pgReq == PurchaseGroupRequestor.CentreGroupAdmin) {
                        AddNewEmptyPurchaseGroup(tmpCg, retPurchaseGroupList, strCurrCultureName, rootUrl);
                    }

                    transaction.Complete();

                    return retPurchaseGroupList;
                } catch (Exception ex) {
                    throw ex;
                }
            }
        }

        private PurchaseGroupExtended GetPurchaseGroup(
            Purchase_Group purchaseGroup,
            Centre_Group tmpCg,
            string strCurrCultureName,
            string rootUrl,
            bool isNewStructLoaded) {
            PurchaseGroupExtended tmpPurchaseGroup = new PurchaseGroupExtended();
            SetValues(purchaseGroup, tmpPurchaseGroup, new List<string> { "id", "group_name", "self_ordered" });

            tmpPurchaseGroup.centre_group_id = tmpCg.id;
            tmpPurchaseGroup.is_visible = (purchaseGroup.active == true);
            tmpPurchaseGroup.is_active = (purchaseGroup.active == true);

            //Loc Name
            SetPurchasegroupLocName(purchaseGroup, tmpPurchaseGroup, strCurrCultureName);
            SortedList<int, PurchaseGroupLimitExtended> tmpSortLimit = new SortedList<int, PurchaseGroupLimitExtended>();

            //Local Text
            string pgLang = tmpCg.Company.culture_info;
            tmpPurchaseGroup.local_text = GetPgLocalNames(purchaseGroup, pgLang, strCurrCultureName, rootUrl);

            //Parent Purchase Group
            if (purchaseGroup.Parent_Purchase_Group.Count > 0) {
                tmpPurchaseGroup.parent_pg_id = purchaseGroup.Parent_Purchase_Group.ElementAt(0).id;
                SetParentPurchaseGroupLocName(purchaseGroup.Parent_Purchase_Group.ElementAt(0), tmpPurchaseGroup, strCurrCultureName);

            }

            //pg type
            //tmpPurchaseGroup.purchase_type = purchaseGroup.purchase_type;
            //tmpPurchaseGroup.is_approval_only = (purchaseGroup.purchase_type == (int)PgRepository.PurchaseGroupType.ApprovalOnly);
            tmpPurchaseGroup.is_approval_needed = (purchaseGroup.is_approval_needed == null) ? false : (bool)purchaseGroup.is_approval_needed;
            tmpPurchaseGroup.is_order_needed = (purchaseGroup.is_order_needed == null) ? false : (bool)purchaseGroup.is_order_needed;

            //centres
            var tmpSortCentres = tmpCg.Centre.OrderBy(x => x.name);
            foreach (var centre in tmpSortCentres) {
                if (centre.active == false) {
                    continue;
                }

                Centre tmpCentre = new Centre();
                SetValues(centre, tmpCentre, new List<string> { "id", "name" });
                tmpPurchaseGroup.centre.Add(tmpCentre);
            }


            //Limits
            //int iMaxAppLevelId = -1;
            //Hashtable htAppLevel = new Hashtable();
            foreach (Purchase_Group_Limit pgLimit in purchaseGroup.Purchase_Group_Limit) {
                if (pgLimit.active == false || pgLimit.active == null) {
                    continue;
                }

                PurchaseGroupLimitExtended tmpPgLimit = new PurchaseGroupLimitExtended();
                SetValues(pgLimit, tmpPgLimit, new List<string> {
                    PurchaseGroupLimitData.LIMIT_ID_FIELD,
                    PurchaseGroupLimitData.LIMIT_BOTTOM_FIELD,
                    PurchaseGroupLimitData.LIMIT_TOP_FIELD,
                    PurchaseGroupLimitData.IS_LIMIT_BOTTOM_MULTIPL_FIELD,
                    PurchaseGroupLimitData.IS_LIMIT_TOP_MULTIPL_FIELD,
                    PurchaseGroupLimitData.APPROVE_LEVEL_ID_FIELD});

                tmpPgLimit.limit_bottom_text = ConvertData.GetStringValue(pgLimit.limit_bottom, strCurrCultureName, false);
                tmpPgLimit.limit_top_text = ConvertData.GetStringValue(pgLimit.limit_top, strCurrCultureName, false);
                tmpPgLimit.limit_bottom_text_ro = ConvertData.GetStringValue(pgLimit.limit_bottom, strCurrCultureName, true);
                tmpPgLimit.limit_top_text_ro = ConvertData.GetStringValue(pgLimit.limit_top, strCurrCultureName, true);
                tmpPgLimit.is_bottom_unlimited = (pgLimit.limit_bottom == null);
                tmpPgLimit.is_top_unlimited = (pgLimit.limit_top == null);
                tmpPgLimit.is_visible = true;


                //App Men
                tmpPgLimit.is_app_man_selected = false;
                UserRepository userRepository = new UserRepository();
                for (int i = pgLimit.Manager_Role.Count - 1; i >= 0; i--) {
                    if (pgLimit.Manager_Role.ElementAt(i).active != true) {
                        continue;
                    }

                    //int pgAppLevel = pgLimit.Manager_Role.ElementAt(i).approve_level_id;
                    int pgLimitId = pgLimit.limit_id;


                    //if (pgLimit.Manager_Role.ElementAt(i).centre_group_id != tmpCg.id) {
                    //    //it was possible thet 2 app man was assigned to the same limit and each was from differnt cg
                    //    //e.g.
                    //    /*
                    //     45	1131	2	1403	NULL	True	69	2016-07-26 13:58:51.843
                    //    266	1131	2	1403	NULL	True	69	2016-02-06 00:00:00.000
                    //     */
                    //    continue;
                    //}

                    tmpPgLimit.is_app_man_selected = true;

                    ManagerRoleExtended tmpAppMan = new ManagerRoleExtended();
                    SetValues(pgLimit.Manager_Role.ElementAt(i), tmpAppMan, new List<string> { "participant_id", "approve_level_id" });

                    //check participantrole centregroup
                    AddMissinAppManUserRole(tmpCg, tmpAppMan);

                    //App Participant
                    ParticipantsExtended tmpParticipants = new ParticipantsExtended();
                    if (pgLimit.Manager_Role.ElementAt(i).Participants != null) {
                        SetValues(pgLimit.Manager_Role.ElementAt(i).Participants, tmpParticipants, new List<string> { "id", "first_name", "surname" });
                        userRepository.AddSubstitute(tmpParticipants);
                    } else {
                        int manId = pgLimit.Manager_Role.ElementAt(i).participant_id;
                        var par = (from partDb in m_dbContext.Participants
                                   where partDb.id == manId
                                   select partDb).FirstOrDefault();
                        SetValues(par, tmpParticipants, new List<string> { "id", "first_name", "surname" });
                        userRepository.AddSubstitute(tmpParticipants);
                    }
                    tmpAppMan.participant = tmpParticipants;

                    tmpPgLimit.manager_role.Add(tmpAppMan);
                }


                //int iAppLevelId = -1;
                //if (tmpPgLimit.Manager_Role != null && tmpPgLimit.Manager_Role.Count > 0) {
                //    iAppLevelId = tmpPgLimit.Manager_Role.ElementAt(0).approve_level_id;
                //    tmpPgLimit.app_level_id = iAppLevelId;

                //}

                int iAppLevelId = (int)pgLimit.approve_level_id;

                if (tmpSortLimit.ContainsKey(iAppLevelId)) {
                    for (int i = iAppLevelId + 1; i < 6; i++) {
                        if (!tmpSortLimit.ContainsKey(i)) {
                            iAppLevelId = i;
                            //iMaxAppLevelId = i;
                            break;
                        }
                    }
                }

                if (!tmpSortLimit.ContainsKey(iAppLevelId)) {
                    tmpPgLimit.app_level_id = iAppLevelId;
                    tmpSortLimit.Add(iAppLevelId, tmpPgLimit);
                }
                tmpPurchaseGroup.purchase_group_limit.Add(tmpPgLimit);
            }

            AddMissingLimits(tmpSortLimit);

            if (tmpSortLimit.Count > 0) {
                foreach (KeyValuePair<int, PurchaseGroupLimitExtended> kvp in tmpSortLimit) {
                    tmpPurchaseGroup.purchase_group_limit.Add(kvp.Value);
                }

                tmpPurchaseGroup.purchase_group_limit.ElementAt(0).is_first = true;
                for (int i = (tmpPurchaseGroup.purchase_group_limit.Count - 1); i >= 0; i--) {
                    if (tmpPurchaseGroup.purchase_group_limit.ElementAt(i).is_visible) {
                        tmpPurchaseGroup.purchase_group_limit.ElementAt(i).is_last = true;
                        break;
                    }
                }

            }

            //Default supplier
            if (purchaseGroup.Supplier != null && purchaseGroup.Supplier.Count > 0) {
                tmpPurchaseGroup.default_supplier = new SupplierSimpleExtended();
                SetValues(purchaseGroup.Supplier.ElementAt(0), tmpPurchaseGroup.default_supplier, new List<string> { "id", "supp_name", "supplier_id" });
                tmpPurchaseGroup.default_supplier.supp_name += " (" + tmpPurchaseGroup.default_supplier.supplier_id + ")";
            }


            //Requestors
            if (purchaseGroup.PurchaseGroup_Requestor.Count > 0) {
                GetPurchaseGroupRequestors(purchaseGroup, tmpPurchaseGroup);
            } else if (!isNewStructLoaded) {
                GetPurchaseGroupRequestors(tmpCg, purchaseGroup, tmpPurchaseGroup, GetImpliciteRequestors(tmpCg));
                AddRequestor(tmpPurchaseGroup, purchaseGroup, tmpCg);
                DeleteImpliciteRequestor(purchaseGroup);
                DeleteExcludeRequestor(purchaseGroup);

                SaveChanges();
            }

            //Orderers
            if (purchaseGroup.PurchaseGroup_Orderer.Count > 0) {
                GetPurchaseGroupOrderers(purchaseGroup, tmpPurchaseGroup);
            } else if (!isNewStructLoaded) {
                GetPurchaseGroupOrderers(tmpCg, purchaseGroup, tmpPurchaseGroup, GetImpliciteOrderers(tmpCg));
                AddOrderer(tmpPurchaseGroup, purchaseGroup, tmpCg);
                DeleteImpliciteOrderer(purchaseGroup);
                DeleteExcludeOrderer(purchaseGroup);

                SaveChanges();
            }

            if (!isNewStructLoaded) {

                //Delete Requestors Orderers Initials
                for (int i = tmpCg.ParticipantRole_CentreGroup.Count - 1; i >= 0; i--) {
                    if (tmpCg.ParticipantRole_CentreGroup.ElementAt(i).role_id == (int)UserRole.Requestor) {
                        DeleteParticipantRequestorRole(tmpCg, tmpCg.ParticipantRole_CentreGroup.ElementAt(i).participant_id, DataNulls.INT_NULL, DataNulls.INT_NULL);
                    } else if (tmpCg.ParticipantRole_CentreGroup.ElementAt(i).role_id == (int)UserRole.Orderer) {
                        DeleteParticipantOrdererRole(tmpCg, tmpCg.ParticipantRole_CentreGroup.ElementAt(i).participant_id, DataNulls.INT_NULL);
                    }
                }

                //Add Requestors, Orderers
                AddRequestorsInitial(tmpCg);
                AddOrderersInitial(tmpCg);

                SaveChanges();
            }

            return tmpPurchaseGroup;
        }

        //private string GetStringValue(decimal? dNumber, string strCurrCultureName, bool isNumberGroupSeparator) {
        //    if (dNumber == null) {
        //        return "";
        //    }

        //    string strNumber = dNumber.ToString();
        //    string decSep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        //    var regex = new System.Text.RegularExpressions.Regex("(?<=[\\" + decSep+ "])[0-9]+");

        //    string decNumbers = "";
        //    if (regex.IsMatch(strNumber)) {
        //        decNumbers = regex.Match(strNumber).Value;
        //    }
        //    int iDecPartLength = decNumbers.Length;

        //    decimal tmpD = (Decimal)dNumber;
        //    return ConvertData.ToString(tmpD, iDecPartLength, strCurrCultureName, isNumberGroupSeparator);
        //}

        //private void AddSubstitute(ParticipantsExtended participant) {
        //    //substitution
        //    Participant_Substitute partSubst = new UserRepository().GetParticipantActiveSubstitution(participant.id);
        //    if (partSubst != null) {
        //        participant.substituted_by = partSubst.SubstituteUser.surname + " " + partSubst.SubstituteUser.first_name;
        //        participant.substituted_until = partSubst.substitute_end_date;

        //    }
        //}

        private void AddMissinAppManUserRole(Centre_Group tmpCg, ManagerRoleExtended tmpAppMan) {
            //check participantrole centregroup
            var prCg = (from pgCgDb in tmpCg.ParticipantRole_CentreGroup
                        where pgCgDb.participant_id == tmpAppMan.participant_id &&
                        pgCgDb.role_id == (int)UserRole.ApprovalManager
                        select pgCgDb).FirstOrDefault();
            if (prCg == null) {
                ParticipantRole_CentreGroup newPrCg = new ParticipantRole_CentreGroup();
                newPrCg.centre_group_id = tmpCg.id;
                newPrCg.participant_id = tmpAppMan.participant_id;
                newPrCg.role_id = (int)UserRole.ApprovalManager;

                m_dbContext.ParticipantRole_CentreGroup.Add(newPrCg);
                SaveChanges();
            }


        }

        private List<LocalText> GetPgLocalNames(Purchase_Group purchaseGroup, string pgLang, string strCurrCultureName, string rootUrl) {
            List<LocalText> localTexts = new List<LocalText>();
            var locCurrText = (from pgLocName in purchaseGroup.Purchase_Group_Local
                               where pgLocName.culture.ToLower() == strCurrCultureName.ToLower()
                               select pgLocName).FirstOrDefault();

            bool isDefaultLang = (strCurrCultureName == DefaultLanguage);
            Purchase_Group_Local locDefaultText = null;
            if (!isDefaultLang) {
                locDefaultText = (from pgLocName in purchaseGroup.Purchase_Group_Local
                                  where pgLocName.culture == DefaultLanguage
                                  select pgLocName).FirstOrDefault();
            }

            //first text
            if (locCurrText != null) {
                LocalText locTxt = new LocalText();
                locTxt.text_id = locCurrText.purchase_group_id;
                locTxt.culture = locCurrText.culture;
                locTxt.label = GetPgLabel(locCurrText.culture);
                locTxt.text = locCurrText.local_text;
                locTxt.flag_url = GetPgFlagUrl(strCurrCultureName, rootUrl);

                localTexts.Add(locTxt);
            } else {
                if (!isDefaultLang && locDefaultText != null) {
                    LocalText locTxt = new LocalText();
                    locTxt.text_id = locDefaultText.purchase_group_id;
                    locTxt.culture = strCurrCultureName;// locDefaultText.culture;
                    locTxt.label = GetPgLabel(strCurrCultureName);
                    locTxt.text = locDefaultText.local_text;
                    locTxt.flag_url = GetPgFlagUrl(strCurrCultureName, rootUrl);

                    localTexts.Add(locTxt);
                } else {
                    LocalText locTxt = new LocalText();
                    locTxt.text_id = purchaseGroup.id;
                    locTxt.culture = strCurrCultureName;
                    locTxt.label = GetPgLabel(strCurrCultureName);
                    locTxt.text = purchaseGroup.group_name;
                    locTxt.flag_url = GetPgFlagUrl(strCurrCultureName, rootUrl);

                    localTexts.Add(locTxt);
                }
            }

            //second default
            if (!isDefaultLang && locDefaultText != null) {
                LocalText locTxt = new LocalText();
                locTxt.text_id = locDefaultText.purchase_group_id;
                locTxt.culture = locDefaultText.culture;
                locTxt.label = GetPgLabel(locDefaultText.culture);
                locTxt.text = locDefaultText.local_text;
                locTxt.flag_url = GetPgFlagUrl(DefaultLanguage, rootUrl);

                localTexts.Add(locTxt);
            } else if (!isDefaultLang) {
                LocalText locTxt = new LocalText();
                locTxt.text_id = purchaseGroup.id;
                locTxt.culture = DefaultLanguage;
                locTxt.label = GetPgLabel(DefaultLanguage);
                locTxt.text = purchaseGroup.group_name;
                locTxt.flag_url = GetPgFlagUrl(DefaultLanguage, rootUrl);

                localTexts.Add(locTxt);
            }

            if (pgLang != strCurrCultureName && pgLang != DefaultLanguage) {
                AddOtherPurchaseGroupLocName(purchaseGroup, localTexts, pgLang, rootUrl);
            }

            //if (strCurrCultureName.ToLower() != CULTURE_CZ.ToLower()) {
            //    AddOtherPurchaseGroupLocName(purchaseGroup, localTexts, CULTURE_CZ, rootUrl);
            //}

            //if (strCurrCultureName.ToLower() != CULTURE_SK.ToLower()) {
            //    AddOtherPurchaseGroupLocName(purchaseGroup, localTexts, CULTURE_SK, rootUrl);
            //}

            //if (strCurrCultureName.ToLower() != CULTURE_RO.ToLower()) {
            //    AddOtherPurchaseGroupLocName(purchaseGroup, localTexts, CULTURE_RO, rootUrl);
            //}

            //if (strCurrCultureName.ToLower() != CULTURE_PL.ToLower()) {
            //    AddOtherPurchaseGroupLocName(purchaseGroup, localTexts, CULTURE_PL, rootUrl);
            //}

            //if (strCurrCultureName.ToLower() != CULTURE_BG.ToLower()) {
            //    AddOtherPurchaseGroupLocName(purchaseGroup, localTexts, CULTURE_BG, rootUrl);
            //}

            //if (strCurrCultureName.ToLower() != CULTURE_SL.ToLower()) {
            //    AddOtherPurchaseGroupLocName(purchaseGroup, localTexts, CULTURE_SL, rootUrl);
            //}

            //if (strCurrCultureName.ToLower() != CULTURE_SR.ToLower()) {
            //    AddOtherPurchaseGroupLocName(purchaseGroup, localTexts, CULTURE_SR, rootUrl);
            //}

            //if (strCurrCultureName.ToLower() != CULTURE_HU.ToLower()) {
            //    AddOtherPurchaseGroupLocName(purchaseGroup, localTexts, CULTURE_HU, rootUrl);
            //}

            return localTexts;
        }

        private List<LocalText> GetPgLocalNamesEmptyPg(PurchaseGroupExtended purchaseGroup, string pgLang, string strCurrCultureName, string rootUrl) {
            List<LocalText> localTexts = new List<LocalText>();


            bool isDefaultLang = (strCurrCultureName == DefaultLanguage);

            LocalText locTxt = new LocalText();
            locTxt.text_id = -1;
            locTxt.culture = strCurrCultureName;
            locTxt.label = GetPgLabel(strCurrCultureName);
            locTxt.text = "";
            locTxt.flag_url = GetPgFlagUrl(strCurrCultureName, rootUrl);
            localTexts.Add(locTxt);

            if (isDefaultLang) {
                locTxt = new LocalText();
                locTxt.text_id = -2;
                locTxt.culture = pgLang;
                locTxt.label = GetPgLabel(pgLang);
                locTxt.text = "";
                locTxt.flag_url = GetPgFlagUrl(pgLang, rootUrl);
                localTexts.Add(locTxt);
            } else {
                locTxt = new LocalText();
                locTxt.text_id = -2;
                locTxt.culture = DefaultLanguage;
                locTxt.label = GetPgLabel(DefaultLanguage);
                locTxt.text = "";
                locTxt.flag_url = GetPgFlagUrl(DefaultLanguage, rootUrl);
                localTexts.Add(locTxt);
            }

            return localTexts;
        }

        private void AddOtherPurchaseGroupLocName(Purchase_Group purchaseGroup, List<LocalText> localTexts, string strCurrCultureName, string rootUrl) {
            var locCurrTextOther = (from pgLocName in purchaseGroup.Purchase_Group_Local
                                    where pgLocName.culture == strCurrCultureName
                                    select pgLocName).FirstOrDefault();

            LocalText locTxt = new LocalText();
            locTxt.text_id = purchaseGroup.id;
            locTxt.culture = strCurrCultureName;
            locTxt.label = GetPgLabel(strCurrCultureName);
            if (locCurrTextOther != null) {
                locTxt.text = locCurrTextOther.local_text;
            }
            locTxt.flag_url = GetPgFlagUrl(strCurrCultureName, rootUrl);

            localTexts.Add(locTxt);
        }

                
        private void AddNewEmptyPurchaseGroup(
            Centre_Group tmpCg,
            List<PurchaseGroupExtended> retPurchaseGroupList,
            string strCurrCultureName,
            string rootUrl) {

            PurchaseGroupExtended pgNew = new PurchaseGroupExtended();
            pgNew.id = -1;
            pgNew.is_active = true;
            //pgNew.group_loc_name = "New";

            SortedList<int, PurchaseGroupLimitExtended> tmpSortLimit = new SortedList<int, PurchaseGroupLimitExtended>();
            AddMissingLimits(tmpSortLimit);
            foreach (KeyValuePair<int, PurchaseGroupLimitExtended> kvp in tmpSortLimit) {
                pgNew.purchase_group_limit.Add(kvp.Value);
            }

            pgNew.purchase_group_limit.ElementAt(0).is_first = true;
            for (int i = (pgNew.purchase_group_limit.Count - 1); i >= 0; i--) {
                if (pgNew.purchase_group_limit.ElementAt(i).is_visible) {
                    pgNew.purchase_group_limit.ElementAt(i).is_last = true;
                    break;
                }
            }

            //centres
            foreach (var centre in tmpCg.Centre) {
                if (centre.active == false) {
                    continue;
                }

                Centre tmpCentre = new Centre();
                SetValues(centre, tmpCentre, new List<string> { "id", "name" });
                pgNew.centre.Add(tmpCentre);
            }

            //pg name local
            string pgLang = tmpCg.Company.culture_info;
            pgNew.local_text = GetPgLocalNamesEmptyPg(pgNew, pgLang, strCurrCultureName, rootUrl);

            retPurchaseGroupList.Add(pgNew);
        }

        //private void SetRequestorsNew(PurchaseGroupExtended pgModif, Purchase_Group pgOrig, Centre_Group cg) {

        //    AddRequestor(pgModif, pgOrig, cg);

        //    SaveChanges();

        //    //delete old

        //}

        //private void SetDefaultRequestors(List<PurchaseGroupExtended> purchaseGroups) {
        //    foreach (PurchaseGroupExtended pg in purchaseGroups) {
        //        bool isDefault = false;

        //        var defRequestors = from defRequestor in pg.RequestorExtended
        //                          where (defRequestor.is_implicit == false || (defRequestor.is_implicit == true && defRequestor.is_implicit_other_allowed == true)) &&
        //                          defRequestor.is_excluded == false
        //                          select defRequestor;

        //        var defRequestorsList = defRequestors.ToList();
        //        if (defRequestorsList != null && defRequestorsList.Count > 0) {
        //            isDefault = true;
        //        }


        //        if (isDefault) {
        //            foreach (var requestor in pg.RequestorExtended) {
        //                requestor.is_default = true;

        //            }

        //        }

        //    }
        //}

        //private void SetDefaultOrderers(List<PurchaseGroupExtended> purchaseGroups) {
        //    foreach (PurchaseGroupExtended pg in purchaseGroups) {
        //        bool isDefault = false;

        //        var defOrderers = from defOrderer in pg.OrdererExtended
        //                            where (defOrderer.is_implicit == false || (defOrderer.is_implicit == true && defOrderer.is_implicit_other_allowed == true)) &&
        //                            defOrderer.is_excluded == false
        //                            select defOrderer;

        //        var defOrderersList = defOrderers.ToList();
        //        if (defOrderersList != null && defOrderersList.Count > 0) {
        //            isDefault = true;
        //        }


        //        if (isDefault) {
        //            foreach (var orderer in pg.OrdererExtended) {
        //                orderer.is_default = true;

        //            }

        //        }

        //    }
        //}

        //private void SetAllRequestors(List<PurchaseGroupExtended> purchaseGroups) {
        //    Hashtable htAll = new Hashtable();

        //    if (purchaseGroups.Count == 0) {
        //        return;
        //    }

        //    if (purchaseGroups[0].RequestorExtended.Count == 0) {
        //        return;
        //    }

        //    foreach (var requestor in purchaseGroups[0].RequestorExtended) {
        //        if (!htAll.ContainsKey(requestor.participant_id)) {
        //            htAll.Add(requestor.participant_id, null);
        //        }
        //    }


        //    foreach (PurchaseGroupExtended pg in purchaseGroups) {
        //        Hashtable htTmp = new Hashtable();

        //        foreach (var requestor in pg.RequestorExtended) {
        //            if (!htTmp.ContainsKey(requestor.participant_id)) {
        //                htTmp.Add(requestor.participant_id, null);
        //            }
        //        }

        //        List<int> delItems = new List<int>();
        //        IDictionaryEnumerator iEnum = htAll.GetEnumerator();
        //        while (iEnum.MoveNext()) {
        //            if (!htTmp.ContainsKey(iEnum.Key)) {
        //                delItems.Add((int)iEnum.Key);
        //            }
        //        }

        //        foreach (int delItem in delItems) {
        //            htAll.Remove(delItem);
        //        }

        //        if (htAll.Count == 0) {
        //            break;
        //        }

        //    }

        //    IDictionaryEnumerator iEnumDict = htAll.GetEnumerator();
        //    while (iEnumDict.MoveNext()) {

        //        foreach (PurchaseGroupExtended pg in purchaseGroups) {
        //            foreach (var requestor in pg.RequestorExtended) {
        //                if (requestor.participant_id == (int)iEnumDict.Key) {
        //                    requestor.is_all = true;
        //                    if (requestor.is_default) {
        //                        requestor.is_default = true;
        //                    }
        //                }
        //            }

        //        }

        //    }
        //}

        //private void SetAllOrderers(List<PurchaseGroupExtended> purchaseGroups) {
        //    Hashtable htAll = new Hashtable();

        //    if (purchaseGroups.Count == 0) {
        //        return;
        //    }

        //    if (purchaseGroups[0].OrdererExtended.Count == 0) {
        //        return;
        //    }

        //    foreach (var orderer in purchaseGroups[0].OrdererExtended) {
        //        if (!htAll.ContainsKey(orderer.participant_id)) {
        //            htAll.Add(orderer.participant_id, null);
        //        }
        //    }


        //    foreach (PurchaseGroupExtended pg in purchaseGroups) {
        //        Hashtable htTmp = new Hashtable();

        //        foreach (var orderer in pg.OrdererExtended) {
        //            if (!htTmp.ContainsKey(orderer.participant_id)) {
        //                htTmp.Add(orderer.participant_id, null);
        //            }
        //        }

        //        List<int> delItems = new List<int>();
        //        IDictionaryEnumerator iEnum = htAll.GetEnumerator();
        //        while (iEnum.MoveNext()) {
        //            if (!htTmp.ContainsKey(iEnum.Key)) {
        //                delItems.Add((int)iEnum.Key);
        //            }
        //        }

        //        foreach (int delItem in delItems) {
        //            htAll.Remove(delItem);
        //        }

        //        if (htAll.Count == 0) {
        //            break;
        //        }

        //    }

        //    IDictionaryEnumerator iEnumDict = htAll.GetEnumerator();
        //    while (iEnumDict.MoveNext()) {

        //        foreach (PurchaseGroupExtended pg in purchaseGroups) {
        //            foreach (var orderer in pg.OrdererExtended) {
        //                if (orderer.participant_id == (int)iEnumDict.Key) {
        //                    orderer.is_all = true;
        //                    if (orderer.is_default) {
        //                        orderer.is_default = true;
        //                    }
        //                }
        //            }

        //        }

        //    }
        //}

        private Hashtable GetImpliciteOrderers(Centre_Group cg) {
            if (m_htImpliciteOrderers != null) {
                return m_htImpliciteOrderers;
            }

            Hashtable htImpliciteOrderers = new Hashtable();
            foreach (var purchaseGroup in cg.Purchase_Group) {
                if (purchaseGroup.active == false) {
                    continue;
                }

                foreach (var implOrder in purchaseGroup.PurchaseGroup_ImplicitOrderer) {
                    if (implOrder.Participants.active == false) {
                        continue;
                    }

                    bool isForOthers = (implOrder.orderer_allowed_for_other_group != null && implOrder.orderer_allowed_for_other_group == true);
                    if (htImpliciteOrderers.ContainsKey(implOrder.orderer_id)) {
                        if ((bool)htImpliciteOrderers[implOrder.orderer_id]) {
                            htImpliciteOrderers[implOrder.orderer_id] = isForOthers;
                        }
                    } else {
                        htImpliciteOrderers.Add(implOrder.orderer_id, isForOthers);
                    }
                }
            }

            m_htImpliciteOrderers = htImpliciteOrderers;
            return m_htImpliciteOrderers;
        }

        private Hashtable GetImpliciteRequestors(Centre_Group cg) {
            if (m_htImpliciteRequestors != null) {
                return m_htImpliciteRequestors;
            }

            Hashtable htImpliciteRequestors = new Hashtable();
            foreach (var purchaseGroup in cg.Purchase_Group) {
                if (purchaseGroup.active == false) {
                    continue;
                }

                foreach (var implRequestor in purchaseGroup.PurchaseGroup_ImplicitRequestor) {
                    if (implRequestor.Participants.active == false) {
                        continue;
                    }

                    bool isForOthers = (implRequestor.requestor_allowed_for_other_group != null && implRequestor.requestor_allowed_for_other_group == true);
                    if (htImpliciteRequestors.ContainsKey(implRequestor.requestor_id)) {
                        if ((bool)htImpliciteRequestors[implRequestor.requestor_id]) {
                            htImpliciteRequestors[implRequestor.requestor_id] = isForOthers;
                        }
                    } else {
                        htImpliciteRequestors.Add(implRequestor.requestor_id, isForOthers);
                    }
                }
            }

            m_htImpliciteRequestors = htImpliciteRequestors;
            return m_htImpliciteRequestors;
        }

        private void GetPurchaseGroupOrderers(
            Centre_Group sourceCg,
            Purchase_Group sourcePg,
            PurchaseGroupExtended targetPg,
            Hashtable htIpliciteOrderer) {


            List<OrdererExtended> orderers = new List<OrdererExtended>();

            var imliciteOrderers = from implOrd in sourcePg.PurchaseGroup_ImplicitOrderer
                                   where implOrd.purchase_category_id == sourcePg.id
                                   select implOrd;

            var imliciteOrderersList = imliciteOrderers.ToList();
            if (imliciteOrderersList != null && imliciteOrderersList.Count > 0) {
                foreach (var imliciteOrderer in imliciteOrderersList) {
                    if (imliciteOrderer.Participants.active == false) {
                        continue;
                    }


                    OrdererExtended ordererExtended = new OrdererExtended();
                    ordererExtended.participant_id = imliciteOrderer.Participants.id;
                    ordererExtended.first_name = imliciteOrderer.Participants.first_name;
                    ordererExtended.surname = imliciteOrderer.Participants.surname;
                    // ordererExtended.is_all = false;
                    //ordererExtended.is_default = false;
                    ordererExtended.is_implicit = true;
                    ordererExtended.is_excluded = false;
                    ordererExtended.is_implicit_other_allowed = (imliciteOrderer.orderer_allowed_for_other_group == true);

                    orderers.Add(ordererExtended);
                }
            } else {
                var prcgOrderers = from pgcg in sourceCg.ParticipantRole_CentreGroup
                                   where pgcg.centre_group_id == sourceCg.id && pgcg.role_id == (int)UserRepository.UserRole.Orderer
                                   select pgcg;

                var prcgOrdererList = prcgOrderers.ToList();

                foreach (var prcgOrderer in prcgOrdererList) {
                    if (prcgOrderer.Participants.active == false) {
                        continue;
                    }

                    if (htIpliciteOrderer.ContainsKey(prcgOrderer.Participants.id) &&
                        !((bool)htIpliciteOrderer[prcgOrderer.Participants.id])) {
                        continue;
                    }

                    //Excluded Orderes
                    var prcgExcludeOrderers = from pd in sourcePg.ParticipantsExcludeOrderers
                                              where pd.active == true && pd.id == prcgOrderer.Participants.id
                                              select pd;

                    bool isExcluded = false;
                    var prcgExcludeOrderersList = prcgExcludeOrderers.ToList();
                    if (prcgExcludeOrderersList != null && prcgExcludeOrderersList.Count > 0) {
                        //Excluded Orderer
                        isExcluded = true;
                    }

                    if (!isExcluded) {
                        OrdererExtended ordererExtended = new OrdererExtended();
                        ordererExtended.participant_id = prcgOrderer.Participants.id;
                        ordererExtended.first_name = prcgOrderer.Participants.first_name;
                        ordererExtended.surname = prcgOrderer.Participants.surname;
                        //ordererExtended.is_all = false;
                        //ordererExtended.is_default = false;
                        //ordererExtended.is_implicit = true;
                        //ordererExtended.is_excluded = isExcluded;

                        orderers.Add(ordererExtended);
                    }
                }
            }

            targetPg.orderer = orderers;
        }

        private void GetPurchaseGroupRequestors(
            Purchase_Group sourcePg,
            PurchaseGroupExtended targetPg) {

            SortedList<string, RequestorExtended> sortRequestors = new SortedList<string, RequestorExtended>();

            foreach (var req in sourcePg.PurchaseGroup_Requestor) {
                if (req.Participants.active != true) {
                    continue;
                }

                RequestorExtended requestorExtended = new RequestorExtended();
                requestorExtended.participant_id = req.Participants.id;
                requestorExtended.first_name = req.Participants.first_name;
                requestorExtended.surname = req.Participants.surname;
                requestorExtended.centre_id = req.centre_id;

                string strKey = requestorExtended.surname + requestorExtended.first_name + requestorExtended.centre_id;
                if (!sortRequestors.ContainsKey(strKey)) {
                    sortRequestors.Add(strKey, requestorExtended);
                }
            }

            List<RequestorExtended> requestors = new List<RequestorExtended>();
            foreach (KeyValuePair<string, RequestorExtended> kvp in sortRequestors) {
                requestors.Add(kvp.Value);
            }

            targetPg.requestor = requestors;
        }

        private void GetPurchaseGroupOrderers(
            Purchase_Group sourcePg,
            PurchaseGroupExtended targetPg) {

            SortedList<string, OrdererExtended> sortOrderers = new SortedList<string, OrdererExtended>();

            UserRepository userRepository = new UserRepository();
            foreach (var req in sourcePg.PurchaseGroup_Orderer) {
                if (req.active != true) {
                    continue;
                }

                OrdererExtended ordererExtended = new OrdererExtended();
                ordererExtended.participant_id = req.id;
                ordererExtended.first_name = req.first_name;
                ordererExtended.surname = req.surname;
                userRepository.AddSubstitute(ordererExtended);

                string strKey = ordererExtended.surname + ordererExtended.first_name;
                if (!sortOrderers.ContainsKey(strKey)) {
                    sortOrderers.Add(strKey, ordererExtended);
                }
            }

            List<OrdererExtended> orderers = new List<OrdererExtended>();
            foreach (KeyValuePair<string, OrdererExtended> kvp in sortOrderers) {
                orderers.Add(kvp.Value);
            }

            targetPg.orderer = orderers;
        }

        private void GetPurchaseGroupRequestors(
            Centre_Group sourceCg,
            Purchase_Group sourcePg,
            PurchaseGroupExtended targetPg,
            Hashtable htIpliciteRequestor) {


            SortedList<string, RequestorExtended> sortRequestors = new SortedList<string, RequestorExtended>();

            var imliciteRequestors = from implReq in sourcePg.PurchaseGroup_ImplicitRequestor
                                     where implReq.purchase_category_id == sourcePg.id
                                     select implReq;

            var imliciteRequestorsList = imliciteRequestors.ToList();
            if (imliciteRequestorsList != null && imliciteRequestorsList.Count > 0) {
                AddImliciteRequestor(imliciteRequestorsList, sortRequestors);
                //foreach (var imliciteRequestor in imliciteRequestorsList) {
                //    if (imliciteRequestor.Participants.active == false) {
                //        continue;
                //    }


                //    RequestorExtended requestorExtended = new RequestorExtended();
                //    requestorExtended.participant_id = imliciteRequestor.Participants.id;
                //    requestorExtended.first_name = imliciteRequestor.Participants.first_name;
                //    requestorExtended.surname = imliciteRequestor.Participants.surname;
                //    //requestorExtended.is_all = false;
                //    //requestorExtended.is_default = false;
                //    requestorExtended.is_implicit = true;
                //    requestorExtended.is_implicit_other_allowed = (imliciteRequestor.requestor_allowed_for_other_group == true);
                //    requestorExtended.is_excluded = false;

                //    string strKey = requestorExtended.surname + requestorExtended.first_name;
                //    if (!sortRequestors.ContainsKey(strKey)) {
                //        sortRequestors.Add(strKey, requestorExtended);
                //    }
                //}
            } else {
                AddNormalRequestor(sourceCg, sourcePg, htIpliciteRequestor, sortRequestors);
                //var prcgRequestors = from pgcg in sourceCg.ParticipantRole_CentreGroup
                //                     where pgcg.centre_group_id == sourceCg.id && pgcg.role_id == (int)UserRepository.UserRole.Requestor
                //                     select pgcg;

                //var prcgRequestorList = prcgRequestors.ToList();

                //foreach (var prcgRequestor in prcgRequestors) {
                //    if (prcgRequestor.Participants.active == false) {
                //        continue;
                //    }

                //    if (htIpliciteRequestor.ContainsKey(prcgRequestor.Participants.id) &&
                //        !((bool)htIpliciteRequestor[prcgRequestor.Participants.id])) {
                //        continue;
                //    }

                //    //Excluded Requestors
                //    var prcgExcludeRequestors = (from pd in sourcePg.ParticipantsExcludeRequestor
                //                                 where pd.active == true && pd.id == prcgRequestor.Participants.id
                //                                 select pd).FirstOrDefault();

                //    bool isExcluded = false;
                //    //var prcgExcludeRequestorsList = prcgExcludeRequestors.ToList();
                //    if (prcgExcludeRequestors != null) {
                //        //Excluded Requestor
                //        //continue;
                //        isExcluded = true;
                //    }

                //    if (!isExcluded) {
                //        RequestorExtended requestorExtended = new RequestorExtended();
                //        requestorExtended.participant_id = prcgRequestor.Participants.id;
                //        requestorExtended.first_name = prcgRequestor.Participants.first_name;
                //        requestorExtended.surname = prcgRequestor.Participants.surname;
                //        //requestorExtended.is_all = false;
                //        //requestorExtended.is_default = false;
                //        //requestorExtended.is_excluded = isExcluded;


                //        string strKey = requestorExtended.surname + requestorExtended.first_name;
                //        if (!sortRequestors.ContainsKey(strKey)) {
                //            sortRequestors.Add(strKey, requestorExtended);
                //        }
                //    }
                //}

                //List<int> centreIds = new List<int>();
                //foreach (var centre in sourceCg.Centre) {
                //    if (centre.active == true) {
                //        centreIds.Add(centre.id);
                //    }
                //}

                ////Add Requestors with Centre_id only without ParticipantRole
                //var participants = (from partC in m_dbContext.Participants
                //                    where centreIds.Contains((int)partC.centre_id) &&
                //                    partC.active == true
                //                    select partC).ToList();
                //foreach (var partCentre in participants) {
                //    if(htIpliciteRequestor.ContainsKey(partCentre.id) && !((bool)htIpliciteRequestor[partCentre.id])) {
                //        continue;
                //    }
                //    string strKey = partCentre.surname + partCentre.first_name;
                //    if (!sortRequestors.ContainsKey(strKey)) {
                //        RequestorExtended requestorExtended = new RequestorExtended();
                //        requestorExtended.participant_id = partCentre.id;
                //        requestorExtended.first_name = partCentre.first_name;
                //        requestorExtended.surname = partCentre.surname;
                //        sortRequestors.Add(strKey, requestorExtended);
                //    }
                //}

                ////Add Requestors with centre%id without ParticipantRole
                //var reqCentres = (from reqCentreDb in m_dbContext.Centre
                //                  where centreIds.Contains((int)reqCentreDb.id)
                //                  select reqCentreDb).ToList();

                //foreach (var centre in reqCentres) {
                //    foreach (var partCentre in centre.Requestor_Centre) {
                //        if (partCentre.active == false) {
                //            continue;
                //        }
                //        if (htIpliciteRequestor.ContainsKey(partCentre.id) && !((bool)htIpliciteRequestor[partCentre.id])) {
                //            continue;
                //        }
                //        string strKey = partCentre.surname + partCentre.first_name;
                //        if (!sortRequestors.ContainsKey(strKey)) {
                //            RequestorExtended requestorExtended = new RequestorExtended();
                //            requestorExtended.participant_id = partCentre.id;
                //            requestorExtended.first_name = partCentre.first_name;
                //            requestorExtended.surname = partCentre.surname;
                //            sortRequestors.Add(strKey, requestorExtended);
                //        }
                //    }
                //}
            }

            List<RequestorOrdererExtended> requestors = new List<RequestorOrdererExtended>();
            foreach (KeyValuePair<string, RequestorExtended> kvp in sortRequestors) {
                requestors.Add(kvp.Value);
            }

            List<RequestorExtended> finalRequestors = new List<RequestorExtended>();
            foreach (var centre in targetPg.centre) {

                foreach (var requestor in requestors) {

                    Participants sourcePart = null;
                    var tmpDbRequestor = (from dbReq in sourceCg.ParticipantRole_CentreGroup
                                          where dbReq.participant_id == requestor.participant_id
                                          select dbReq).FirstOrDefault();

                    if (tmpDbRequestor != null) {
                        sourcePart = tmpDbRequestor.Participants;
                    } else {
                        var tmpDbImplRequestor = (from dbReq in sourcePg.PurchaseGroup_ImplicitRequestor
                                                  where dbReq.requestor_id == requestor.participant_id
                                                  select dbReq).FirstOrDefault();
                        if (tmpDbImplRequestor != null) {
                            sourcePart = tmpDbImplRequestor.Participants;
                        }
                    }

                    if (sourcePart == null) {
                        continue;
                    }

                    if (sourcePart.centre_id == centre.id) {
                        //default centre
                        RequestorExtended centreRequestor = new RequestorExtended();
                        SetValues(requestor, centreRequestor);
                        centreRequestor.centre_id = centre.id;
                        finalRequestors.Add(centreRequestor);
                    } else {
                        //requestor centre
                        var tmpReqCentre = (from dbReqCentre in sourcePart.Requestor_Centre
                                            where dbReqCentre.id == centre.id
                                            select dbReqCentre).FirstOrDefault();
                        if (tmpReqCentre != null) {
                            RequestorExtended centreRequestor = new RequestorExtended();
                            SetValues(requestor, centreRequestor);
                            centreRequestor.centre_id = tmpReqCentre.id;
                            finalRequestors.Add(centreRequestor);
                        }
                    }
                }
            }
            targetPg.requestor = finalRequestors;
        }

        private void AddImliciteRequestor(
            IEnumerable<PurchaseGroup_ImplicitRequestor> imliciteRequestorsList,
            SortedList<string, RequestorExtended> sortRequestors) {
            foreach (var imliciteRequestor in imliciteRequestorsList) {
                if (imliciteRequestor.Participants.active == false) {
                    continue;
                }


                RequestorExtended requestorExtended = new RequestorExtended();
                requestorExtended.participant_id = imliciteRequestor.Participants.id;
                requestorExtended.first_name = imliciteRequestor.Participants.first_name;
                requestorExtended.surname = imliciteRequestor.Participants.surname;
                //requestorExtended.is_all = false;
                //requestorExtended.is_default = false;
                requestorExtended.is_implicit = true;
                requestorExtended.is_implicit_other_allowed = (imliciteRequestor.requestor_allowed_for_other_group == true);
                requestorExtended.is_excluded = false;

                string strKey = requestorExtended.surname + requestorExtended.first_name;
                if (!sortRequestors.ContainsKey(strKey)) {
                    sortRequestors.Add(strKey, requestorExtended);
                }
            }
        }

        private void AddNormalRequestor(
            Centre_Group sourceCg,
            Purchase_Group sourcePg,
            Hashtable htIpliciteRequestor,
            SortedList<string, RequestorExtended> sortRequestors) {

            var prcgRequestors = from pgcg in sourceCg.ParticipantRole_CentreGroup
                                 where pgcg.centre_group_id == sourceCg.id && pgcg.role_id == (int)UserRepository.UserRole.Requestor
                                 select pgcg;

            var prcgRequestorList = prcgRequestors.ToList();

            foreach (var prcgRequestor in prcgRequestors) {
                if (prcgRequestor.Participants.active == false) {
                    continue;
                }

                if (htIpliciteRequestor.ContainsKey(prcgRequestor.Participants.id) &&
                    !((bool)htIpliciteRequestor[prcgRequestor.Participants.id])) {
                    continue;
                }

                //Excluded Requestors
                var prcgExcludeRequestors = (from pd in sourcePg.ParticipantsExcludeRequestor
                                             where pd.active == true && pd.id == prcgRequestor.Participants.id
                                             select pd).FirstOrDefault();

                bool isExcluded = false;
                //var prcgExcludeRequestorsList = prcgExcludeRequestors.ToList();
                if (prcgExcludeRequestors != null) {
                    //Excluded Requestor
                    //continue;
                    isExcluded = true;
                }

                if (!isExcluded) {
                    RequestorExtended requestorExtended = new RequestorExtended();
                    requestorExtended.participant_id = prcgRequestor.Participants.id;
                    requestorExtended.first_name = prcgRequestor.Participants.first_name;
                    requestorExtended.surname = prcgRequestor.Participants.surname;
                    //requestorExtended.is_all = false;
                    //requestorExtended.is_default = false;
                    //requestorExtended.is_excluded = isExcluded;


                    string strKey = requestorExtended.surname + requestorExtended.first_name;
                    if (!sortRequestors.ContainsKey(strKey)) {
                        sortRequestors.Add(strKey, requestorExtended);
                    }
                }
            }

            List<int> centreIds = new List<int>();
            foreach (var centre in sourceCg.Centre) {
                if (centre.active == true) {
                    centreIds.Add(centre.id);
                }
            }

            //Add Requestors with Centre_id only without ParticipantRole
            var participants = (from partC in m_dbContext.Participants
                                where centreIds.Contains((int)partC.centre_id) &&
                                partC.active == true
                                select partC).ToList();
            foreach (var partCentre in participants) {
                if (htIpliciteRequestor.ContainsKey(partCentre.id) && !((bool)htIpliciteRequestor[partCentre.id])) {
                    continue;
                }
                string strKey = partCentre.surname + partCentre.first_name;
                if (!sortRequestors.ContainsKey(strKey)) {
                    RequestorExtended requestorExtended = new RequestorExtended();
                    requestorExtended.participant_id = partCentre.id;
                    requestorExtended.first_name = partCentre.first_name;
                    requestorExtended.surname = partCentre.surname;
                    sortRequestors.Add(strKey, requestorExtended);
                }
            }

            //Add Requestors with centre%id without ParticipantRole
            var reqCentres = (from reqCentreDb in m_dbContext.Centre
                              where centreIds.Contains((int)reqCentreDb.id)
                              select reqCentreDb).ToList();

            foreach (var centre in reqCentres) {
                foreach (var partCentre in centre.Requestor_Centre) {
                    if (partCentre.active == false) {
                        continue;
                    }
                    if (htIpliciteRequestor.ContainsKey(partCentre.id) && !((bool)htIpliciteRequestor[partCentre.id])) {
                        continue;
                    }
                    string strKey = partCentre.surname + partCentre.first_name;
                    if (!sortRequestors.ContainsKey(strKey)) {
                        RequestorExtended requestorExtended = new RequestorExtended();
                        requestorExtended.participant_id = partCentre.id;
                        requestorExtended.first_name = partCentre.first_name;
                        requestorExtended.surname = partCentre.surname;
                        sortRequestors.Add(strKey, requestorExtended);
                    }
                }
            }
        }


        private void AddMissingLimits(SortedList<int, PurchaseGroupLimitExtended> tmpSortLimit) {
            for (int iLevel = 0; iLevel < 6; iLevel++) {
                if (!tmpSortLimit.ContainsKey(iLevel)) {
                    PurchaseGroupLimitExtended tmpPgLimit = new PurchaseGroupLimitExtended();
                    //SetValues(pgLimit, tmpPgLimit, new List<string> { "limit_id", "limit_bottom", "limit_top" });

                    tmpPgLimit.limit_id = m_TmpId;
                    tmpPgLimit.limit_bottom = 0;
                    tmpPgLimit.limit_top = 0;
                    //tmpPgLimit.limit_bottom_text = "0";
                    //tmpPgLimit.limit_top_text = "0";
                    tmpPgLimit.is_bottom_unlimited = false;
                    tmpPgLimit.is_top_unlimited = false;
                    tmpPgLimit.app_level_id = iLevel;
                    tmpPgLimit.is_visible = false;

                    tmpSortLimit.Add(iLevel, tmpPgLimit);

                    m_TmpId--;
                }
            }

            //for (int iLevel = (iMaxAppLevelId + 1); iLevel < 6; iLevel++) {
            //    PurchaseGroupLimitExtended tmpPgLimit = new PurchaseGroupLimitExtended();
            //    //SetValues(pgLimit, tmpPgLimit, new List<string> { "limit_id", "limit_bottom", "limit_top" });

            //    tmpPgLimit.limit_id = m_TmpId;
            //    tmpPgLimit.limit_bottom = 0;
            //    tmpPgLimit.limit_top = 0;
            //    tmpPgLimit.is_bottom_unlimited = false;
            //    tmpPgLimit.is_top_unlimited = false;
            //    tmpPgLimit.app_level_id = iLevel;
            //    tmpPgLimit.is_visible = false;

            //    tmpSortLimit.Add(iLevel, tmpPgLimit);

            //    m_TmpId--;
            //}
        }

        private void SetPurchasegroupLocName(Purchase_Group purchaseGroup, PurchaseGroupExtended tmpPurchaseGroup, string strCurrCultureName) {
            //Loc Name
            var locPgGroup = from locPg in purchaseGroup.Purchase_Group_Local
                             where locPg.purchase_group_id == purchaseGroup.id &&
                             locPg.culture.ToLower() == strCurrCultureName.ToLower()
                             select locPg;

            var pgLocList = locPgGroup.ToList();

            if (pgLocList != null && pgLocList.Count > 0 && pgLocList[0].local_text != null) {
                tmpPurchaseGroup.group_loc_name = pgLocList[0].local_text;
            } else {
                tmpPurchaseGroup.group_loc_name = tmpPurchaseGroup.group_name;
            }
        }

        private void SetParentPurchaseGroupLocName(Parent_Purchase_Group parentPg, PurchaseGroupExtended tmpPurchaseGroup, string strCurrCultureName) {
            //Loc Name
            var locPgGroup = from locParPg in parentPg.Parent_Purchase_Group_Local
                             where locParPg.parent_purchase_group_id == parentPg.id &&
                             locParPg.culture.ToLower() == strCurrCultureName.ToLower()
                             select locParPg;

            var pgLocList = locPgGroup.ToList();

            if (pgLocList != null && pgLocList.Count > 0 && pgLocList[0].local_text != null) {
                tmpPurchaseGroup.parent_pg_loc_name = pgLocList[0].local_text;
            } else {
                tmpPurchaseGroup.parent_pg_loc_name = parentPg.name;
            }
        }

        private SortedList<string, PurchaseGroupExtended> SortPurchaseGroupByLocName(List<PurchaseGroupExtended> pgList) {
            SortedList<string, PurchaseGroupExtended> purchGroupSorted = new SortedList<string, PurchaseGroupExtended>();

            if (pgList == null || pgList.Count == 0) {
                return null;
            }

            foreach (PurchaseGroupExtended pg in pgList) {

                int iIndex = 1;
                while (purchGroupSorted.ContainsKey(pg.group_loc_name)) {
                    pg.group_loc_name += UNIQUE_IDENT + iIndex;
                    iIndex++;
                }

                purchGroupSorted.Add(pg.group_loc_name, pg);
            }

            return purchGroupSorted;
        }

        private SortedList<string, Parent_Purchase_Group> SortParentPurchaseGroupByName(List<Parent_Purchase_Group> pgList) {
            SortedList<string, Parent_Purchase_Group> purchGroupSorted = new SortedList<string, Parent_Purchase_Group>();

            if (pgList == null || pgList.Count == 0) {
                return null;
            }

            foreach (Parent_Purchase_Group pg in pgList) {

                int iIndex = 1;
                while (purchGroupSorted.ContainsKey(pg.name)) {
                    pg.name += UNIQUE_IDENT + iIndex;
                    iIndex++;
                }

                purchGroupSorted.Add(pg.name, pg);
            }

            return purchGroupSorted;
        }

        //public List<ParticipantsExtended> GetActiveParticipantsData(string rootUrl, bool isUserNameDisplayed, int currUserId) {
        //    return GetActiveParticipantsData(rootUrl, null, isUserNameDisplayed, currUserId);
        //}

        //public List<ParticipantsExtended> GetActiveParticipantsData(
        //    string rootUrl, List<int> 
        //    companyAdminIds, 
        //    bool isUserNameDisplayed,
        //    int currUserId) {
        //    IEnumerable tmpParticipants = null;
        //    if (companyAdminIds != null && companyAdminIds.Count > 0) {
        //        tmpParticipants = (from pd in m_dbContext.Participants
        //                               //join officeRole in m_dbContext.Participant_Office_Role
        //                               //on pd.id equals officeRole.participant_id
        //                           where pd.active == true && !(pd.user_name.StartsWith("NO_DOMAIN")) &&
        //                           (pd.is_external == null || pd.is_external == false) &&
        //                           //officeRole.role_id == (int)UserRole.OfficeAdministrator &&
        //                           companyAdminIds.Contains(pd.company_id)
        //                           orderby pd.surname, pd.first_name
        //                           select new {
        //                               id = pd.id,
        //                               user_name = pd.user_name,
        //                               surname = pd.surname,
        //                               first_name = pd.first_name,
        //                               company_id = pd.company_id,
        //                               user_search_key = pd.user_search_key
        //                           }).ToList();
        //    } else {
        //        tmpParticipants = (from pd in m_dbContext.Participants
        //                           where pd.active == true && !(pd.user_name.StartsWith("NO_DOMAIN")) &&
        //                           (pd.is_external == null || pd.is_external == false) &&
        //                           pd.Company_Group.id == currUserId
        //                           orderby pd.surname, pd.first_name
        //                           select new {
        //                               id = pd.id,
        //                               user_name = pd.user_name,
        //                               surname = pd.surname,
        //                               first_name = pd.first_name,
        //                               company_id = pd.company_id,
        //                               user_search_key = pd.user_search_key
        //                           }).ToList();
        //    }

        //    //var tmpParticipants = participants.ToList();


        //    List<ParticipantsExtended> retParticipants = new List<ParticipantsExtended>();
        //    foreach (var tmpParticipant in tmpParticipants) {
        //        ParticipantsExtended participant = new ParticipantsExtended();
        //        SetValues(tmpParticipant, participant);

        //        if (isUserNameDisplayed) {
        //            participant.first_name += " (" + participant.user_name + ")";
        //        }

        //        //set name
        //        participant.name_surname_first = participant.surname + " " + participant.first_name;
        //        //country flag
        //        participant.country_flag = GetCountryFlag(participant.company_id, rootUrl);

        //        //substitution
        //        Participant_Substitute partSubst = new UserRepository().GetParticipantActiveSubstitution(participant.id);
        //        if (partSubst != null) {
        //            participant.substituted_by = partSubst.SubstituteUser.surname + " " + partSubst.SubstituteUser.first_name;
        //            participant.substituted_until = partSubst.substitute_end_date.ToString("dd.MM.yyyy");

        //        }

        //        retParticipants.Add(participant);
        //    }

        //    return retParticipants;

        //}

        public List<CompanyCgDropDown> GetActiveOfficeCgData(int userId) {
            var offices = (from officesDb in m_dbContext.Company
                           join companyAdmin in m_dbContext.Participant_Office_Role
                           on officesDb.id equals companyAdmin.office_id
                           where officesDb.active == true &&
                           officesDb.supplier_group_id != null &&
                           companyAdmin.participant_id == userId &&
                           companyAdmin.role_id == (int)UserRole.OfficeAdministrator
                           orderby officesDb.country_code
                           select officesDb).ToList();

            List<CompanyCgDropDown> officesCd = new List<CompanyCgDropDown>();
            foreach (var office in offices) {
                CompanyCgDropDown officeCd = new CompanyCgDropDown();
                SetValues(office, officeCd, new List<string> { "id", "country_code" });
                officeCd.company_id = office.id;
                officeCd.default_currency_id = -1;
                if (office.default_currency_id != null) {
                    officeCd.default_currency_id = (int)office.default_currency_id;
                }
                officesCd.Add(officeCd);
            }

            return officesCd;
        }

        //private string GetCountryFlag(int companyId, string urlRoot) {
        //    switch (companyId) {
        //        case 0:
        //        case 2:
        //            return urlRoot + "Content/Images/Culture/cz.gif";
        //        case 1:
        //            return urlRoot + "Content/Images/Culture/sk.gif";
        //        case 3:
        //            return urlRoot + "Content/Images/Culture/ro.gif";
        //        case 4:
        //            return urlRoot + "Content/Images/Culture/pl.gif";
        //        case 5:
        //            return urlRoot + "Content/Images/Culture/bg.gif";
        //        case 6:
        //            return urlRoot + "Content/Images/Culture/sl.gif";
        //        case 7:
        //            return urlRoot + "Content/Images/Culture/sr.gif";
        //        case 8:
        //            return urlRoot + "Content/Images/Culture/hu.gif";
        //        case 9:
        //            return urlRoot + "Content/Images/Culture/uk.gif";
        //        default:
        //            return urlRoot + "Content/Images/Culture/empty.gif"; ;
        //    }
        //}

        private string GetCgStatusImg(int userId, int cgId, bool isCgActive, string urlRoot) {

            var admins = (from cgDb in m_dbContext.ParticipantRole_CentreGroup
                          where cgDb.participant_id == userId &&
                          (cgDb.role_id == (int)UserRole.CentreGroupPropAdmin ||
                          cgDb.role_id == (int)UserRole.ApproveMatrixAdmin ||
                          cgDb.role_id == (int)UserRole.OrdererAdmin ||
                          cgDb.role_id == (int)UserRole.RequestorAdmin)
                          select cgDb).FirstOrDefault();

            if (isCgActive == true) {
                if (admins == null) {
                    return urlRoot + "Content/Images/Controll/lockenable.png";
                } else {
                    return urlRoot + "Content/Images/Controll/editenable.png";
                }
            } else {
                if (admins == null) {
                    //return urlRoot + "Content/Images/Controll/lockdisable.png";
                    return urlRoot + "Content/Images/Controll/lockenable.png";
                } else {
                    //return urlRoot + "Content/Images/Controll/editdisable.png";
                    return urlRoot + "Content/Images/Controll/editenable.png";
                }
            }

            //return urlRoot + "Content/Images/Controll/lockenable.png";
        }

        public int SaveCentreGroupData(CentreGroupExtended modifCg) {

            if (modifCg != null) {
                using (TransactionScope transaction = new TransactionScope()) {
                    try {
                        Centre_Group origCg = null;
                        bool isNew = false;
                        if (modifCg.id < 0) {
                            isNew = true;
                            origCg = new Centre_Group();
                            var lastCg = (from lastCgDb in m_dbContext.Centre_Group
                                          orderby lastCgDb.id descending
                                          select new { id = lastCgDb.id }).Take(1).FirstOrDefault();
                            int lastId = -1;
                            if (lastCg != null) {
                                lastId = lastCg.id;
                            }
                            int newId = ++lastId;
                            origCg.id = newId;
                            modifCg.id = newId;
                            origCg.multi_invoice = true;

                            foreach (var prCg in modifCg.participantrole_centregroup) {
                                prCg.centre_group_id = newId;
                            }

                            m_dbContext.Entry(origCg).State = EntityState.Added;
                        } else {
                            origCg = GetCentreGroupAllDataById(modifCg.id);
                            m_dbContext.Entry(origCg).State = EntityState.Modified;
                        }
                        origCg.active = modifCg.is_active;
                        origCg.name = modifCg.name;
                        origCg.currency_id = modifCg.currency_id;
                        origCg.allow_free_supplier = modifCg.allow_free_supplier;


                        //currency
                        UpdateForeignCurrencies(modifCg, origCg);

                        //deputy orderer
                        UpdateDeputyOrderer(modifCg, origCg);

                        //orderer supplier
                        UpdateOrdererSupplier(modifCg, origCg);

                        //Admin
                        UpdateCgAdministrators(modifCg, origCg);

                        //company
                        if (modifCg.company_id != origCg.company_id || isNew) {
                            origCg.company_id = modifCg.company_id;
                            var company = (from compDb in m_dbContext.Company
                                           where compDb.id == modifCg.company_id
                                           select compDb).FirstOrDefault();
                            origCg.SupplierGroup = company.SupplierGroup;
                        }

                        //update centre
                        UpdateCentres(modifCg, origCg);

                        SaveChanges();
                        transaction.Complete();

                        return modifCg.id;
                    } catch (Exception ex) {
                        throw ex;
                    }
                }

            }

            return DataNulls.INT_NULL;
        }


        private void UpdateDeputyOrderer(CentreGroupExtended modfiCg, Centre_Group origCg) {
            var dbDeputyOrderers = (from prcg in origCg.ParticipantRole_CentreGroup
                                    where prcg.role_id == (int)UserRole.DeputyOrderer
                                    select prcg).ToList();

            //delete
            if (dbDeputyOrderers != null) {
                for (int i = dbDeputyOrderers.Count - 1; i >= 0; i--) {
                    if (dbDeputyOrderers.ElementAt(i).Participants.active == false) {
                        origCg.ParticipantRole_CentreGroup.Remove(dbDeputyOrderers.ElementAt(i));
                        continue;
                    }

                    var modifDeputyOrderer = (from deputyOrd in modfiCg.deputy_orderer
                                              where deputyOrd.participant_id == dbDeputyOrderers.ElementAt(i).participant_id
                                              select deputyOrd).FirstOrDefault();
                    if (modifDeputyOrderer == null) {
                        origCg.ParticipantRole_CentreGroup.Remove(dbDeputyOrderers.ElementAt(i));
                    }
                }
            }

            //new
            foreach (var modifDepOrd in modfiCg.deputy_orderer) {
                var origDeputyOrderer = (from origCgDb in origCg.ParticipantRole_CentreGroup
                                         where origCgDb.participant_id == modifDepOrd.participant_id &&
                                         origCgDb.role_id == (int)UserRole.DeputyOrderer
                                         select origCgDb).FirstOrDefault();
                if (origDeputyOrderer == null) {
                    ParticipantRole_CentreGroup prCg = new ParticipantRole_CentreGroup();
                    prCg.participant_id = modifDepOrd.participant_id;
                    prCg.centre_group_id = origCg.id;
                    prCg.role_id = (int)UserRole.DeputyOrderer;

                    origCg.ParticipantRole_CentreGroup.Add(prCg);
                }
            }
        }

        private void UpdateOrdererSupplier(CentreGroupExtended modifCg, Centre_Group origCg) {
            var dbOrdererSuppliers = (from orsupp in origCg.Orderer_Supplier
                                      select orsupp).ToList();

            //delete
            if (dbOrdererSuppliers != null) {
                for (int i = dbOrdererSuppliers.Count - 1; i >= 0; i--) {
                    if (dbOrdererSuppliers.ElementAt(i).Participants.active == false ||
                        dbOrdererSuppliers.ElementAt(i).Supplier.active == false) {
                        origCg.Orderer_Supplier.Remove(dbOrdererSuppliers.ElementAt(i));
                        continue;
                    }

                    var modifDeputyOrderer = (from ordererSupplier in modifCg.orderer_supplier_appmatrix
                                              where ordererSupplier.orderer_id == dbOrdererSuppliers.ElementAt(i).orderer_id &&
                                              ordererSupplier.supplier_id == dbOrdererSuppliers.ElementAt(i).supplier_id
                                              select ordererSupplier).FirstOrDefault();
                    if (modifDeputyOrderer == null) {
                        origCg.Orderer_Supplier.Remove(dbOrdererSuppliers.ElementAt(i));
                    }

                    //participant role centre group
                    bool isPgOrderer = false;
                    foreach (var pg in origCg.Purchase_Group) {
                        foreach (var pgo in pg.PurchaseGroup_Orderer) {
                            if (pgo.id == dbOrdererSuppliers.ElementAt(i).orderer_id) {
                                isPgOrderer = true;
                                break;
                            }
                        }
                        if (isPgOrderer) {
                            break;
                        }
                    }

                    if (!isPgOrderer) {
                        int ordererId = dbOrdererSuppliers.ElementAt(i).orderer_id;
                        var partRoleCg = (from prCgDb in m_dbContext.ParticipantRole_CentreGroup
                                          where prCgDb.centre_group_id == origCg.id &&
                                          prCgDb.participant_id == ordererId &&
                                          prCgDb.role_id == (int)UserRole.Orderer
                                          select prCgDb).FirstOrDefault();
                        if (partRoleCg != null) {
                            m_dbContext.ParticipantRole_CentreGroup.Remove(partRoleCg);
                        }
                    }
                }
            }

            //new
            foreach (var modifOrdererSupplier in modifCg.orderer_supplier_appmatrix) {
                var origOrdererSupplier = (from ordSuppDb in origCg.Orderer_Supplier
                                           where ordSuppDb.orderer_id == modifOrdererSupplier.orderer_id &&
                                           ordSuppDb.supplier_id == modifOrdererSupplier.supplier_id
                                           select ordSuppDb).FirstOrDefault();
                if (origOrdererSupplier == null) {
                    Orderer_Supplier ordSupp = new Orderer_Supplier();
                    ordSupp.centre_group_id = origCg.id;
                    ordSupp.orderer_id = modifOrdererSupplier.orderer_id;
                    ordSupp.supplier_id = modifOrdererSupplier.supplier_id;
                    ordSupp.orderer_allowed_for_other_group = true;

                    origCg.Orderer_Supplier.Add(ordSupp);

                    var prCg = (from prCgDb in m_dbContext.ParticipantRole_CentreGroup
                                where prCgDb.participant_id == modifOrdererSupplier.orderer_id &&
                                prCgDb.centre_group_id == modifCg.id &&
                                prCgDb.role_id == (int)UserRole.Orderer
                                select prCgDb).FirstOrDefault();

                    if (prCg == null) {
                        ParticipantRole_CentreGroup newPrCg = new ParticipantRole_CentreGroup();
                        newPrCg.centre_group_id = modifCg.id;
                        newPrCg.participant_id = modifOrdererSupplier.orderer_id;
                        newPrCg.role_id = (int)UserRole.Orderer;
                        m_dbContext.ParticipantRole_CentreGroup.Add(newPrCg);
                    }
                }
            }
        }

        private void UpdateForeignCurrencies(CentreGroupExtended modfiCg, Centre_Group origCg) {

            //delete
            if (origCg.Foreign_Currency != null) {
                for (int i = origCg.Foreign_Currency.Count - 1; i >= 0; i--) {

                    var modifForeignCurrency = (from fcd in modfiCg.currency
                                                where fcd.id == origCg.Foreign_Currency.ElementAt(i).id &&
                                                fcd.is_set == true
                                                select fcd).FirstOrDefault();
                    if (modifForeignCurrency == null) {
                        origCg.Foreign_Currency.Remove(origCg.Foreign_Currency.ElementAt(i));
                    }
                }
            }

            //new
            foreach (var modifForeignCurr in modfiCg.currency) {
                if (modifForeignCurr.is_set == false) {
                    continue;
                }
                var origForeignCurr = (from origCgDb in origCg.Foreign_Currency
                                       where origCgDb.id == modifForeignCurr.id
                                       select origCgDb).FirstOrDefault();
                if (origForeignCurr == null) {
                    var newFCurr = (from curDb in m_dbContext.Currency
                                    where curDb.id == modifForeignCurr.id
                                    select curDb).FirstOrDefault();

                    //ParticipantRole_CentreGroup prCg = new ParticipantRole_CentreGroup();
                    //prCg.participant_id = modifDepOrd.participant_id;
                    //prCg.centre_group_id = origCg.id;
                    //prCg.role_id = (int)UserRole.DeputyOrderer;

                    origCg.Foreign_Currency.Add(newFCurr);
                }
            }
            //List<CentreGroupCurrency> foreignCurrencies = GetForeignCurrencies(sourceCg.id);

            //if (sourceCg.CurrencyExtended == null || sourceCg.CurrencyExtended.Count == 0) {
            //    //Delete all foreign currencies
            //}

            //foreach (CurrencyExtended currencyExtended in sourceCg.CurrencyExtended) {
            //    if (currencyExtended.is_set) {
            //        //Add
            //        var tmpDbCurr = from fc in foreignCurrencies
            //                        where fc.id == currencyExtended.id
            //                        select new { fc.id };
            //        if (tmpDbCurr.FirstOrDefault() == null) {
            //            //string sql = "INSERT INTO CentreGroup_Currency (centre_group_id, currency_id) VALUES (" + sourceCg.id + "," + currencyExtended.id + ")";
            //            //m_dbContext.Database.ExecuteSqlCommand(sql);

            //            sourceCg.Foreign_Currency = 
            //        }
            //    } else {
            //        if (sourceCg.currency_id != currencyExtended.id) {
            //            //Delete
            //            var tmpDbCurr = from fc in foreignCurrencies
            //                            where fc.id == currencyExtended.id
            //                            select new { fc.id };
            //            if (tmpDbCurr.FirstOrDefault() != null) {
            //                string sql = "DELETE FROM CentreGroup_Currency WHERE centre_group_id =  " + sourceCg.id + " AND currency_id=" + currencyExtended.id;
            //                m_dbContext.Database.ExecuteSqlCommand(sql);
            //            }
            //        }
            //    }
            //}

            ////m_dbContext.Entry(foreignCurrencies).State = EntityState.Modified;
        }

        private void UpdateCgAdministrators(CentreGroupExtended modifCg, Centre_Group origCg) {
            //var cgAdminsOrig = (from cgAdminsDb in origCg.ParticipantRole_CentreGroup
            //                where cgAdminsDb.role_id == (int)UserRole.CentreGroupAdmin
            //                select cgAdminsDb).ToList();

            //delete
            //if (cgAdminsOrig != null) {
            //    for (int i = cgAdminsOrig.Count - 1; i >= 0; i--) {
            //        int userId = cgAdminsOrig.ElementAt(i).participant_id;
            //        var cgAdminsModif = (from cgAdminsModifDb in modifCg.CgAdmins
            //                             where cgAdminsModifDb.id == userId
            //                             select cgAdminsModifDb).FirstOrDefault();
            //        if (cgAdminsModif == null) {
            //            m_dbContext.ParticipantRole_CentreGroup.Remove(cgAdminsOrig.ElementAt(i));
            //        }
            //    }
            //}
            DeleteCgAdminRole(modifCg, origCg, UserRole.CentreGroupPropAdmin);
            DeleteCgAdminRole(modifCg, origCg, UserRole.ApproveMatrixAdmin);
            DeleteCgAdminRole(modifCg, origCg, UserRole.OrdererAdmin);
            DeleteCgAdminRole(modifCg, origCg, UserRole.RequestorAdmin);

            //new
            foreach (var modiCgAdmin in modifCg.cg_admin) {
                if (modiCgAdmin.is_cg_prop_admin) {
                    AddNewAdminRole(modiCgAdmin, origCg, UserRole.CentreGroupPropAdmin);
                }
                if (modiCgAdmin.is_appmatrix_admin) {
                    AddNewAdminRole(modiCgAdmin, origCg, UserRole.ApproveMatrixAdmin);
                }
                if (modiCgAdmin.is_orderer_admin) {
                    AddNewAdminRole(modiCgAdmin, origCg, UserRole.OrdererAdmin);
                }
                if (modiCgAdmin.is_requestor_admin) {
                    AddNewAdminRole(modiCgAdmin, origCg, UserRole.RequestorAdmin);
                }
                //int userId = modiCgAdmin.id;
                //var origCgAdmin = (from origCgAdminDb in origCg.ParticipantRole_CentreGroup
                //                   where origCgAdminDb.role_id == (int)UserRole.CentreGroupAdmin &&
                //                         origCgAdminDb.participant_id == userId
                //                   select origCgAdminDb).FirstOrDefault();
                //if (origCgAdmin == null) {
                //    ParticipantRole_CentreGroup newPrCg = new ParticipantRole_CentreGroup();
                //    newPrCg.centre_group_id = modifCg.id;
                //    newPrCg.participant_id = modiCgAdmin.id;
                //    newPrCg.role_id = (int)UserRole.CentreGroupAdmin;
                //    m_dbContext.ParticipantRole_CentreGroup.Add(newPrCg);
                //}
            }
        }

        private void AddNewAdminRole(CgAdminsExtended modifCgAdmin, Centre_Group origCg, UserRole userRole) {

            int userId = modifCgAdmin.id;
            var origCgAdmin = (from origCgAdminDb in origCg.ParticipantRole_CentreGroup
                               where origCgAdminDb.role_id == (int)userRole &&
                                     origCgAdminDb.participant_id == userId
                               select origCgAdminDb).FirstOrDefault();
            if (origCgAdmin == null) {
                ParticipantRole_CentreGroup newPrCg = new ParticipantRole_CentreGroup();
                newPrCg.centre_group_id = origCg.id;
                newPrCg.participant_id = modifCgAdmin.id;
                newPrCg.role_id = (int)userRole;

                m_dbContext.ParticipantRole_CentreGroup.Add(newPrCg);
            }

        }

        private void DeleteCgAdminRole(CentreGroupExtended modifCg, Centre_Group origCg, UserRole userRole) {
            var cgAdminsOrig = (from cgAdminsDb in origCg.ParticipantRole_CentreGroup
                                where cgAdminsDb.role_id == (int)userRole
                                select cgAdminsDb).ToList();

            if (cgAdminsOrig == null) {
                return;
            }
            for (int i = cgAdminsOrig.Count - 1; i >= 0; i--) {
                int userId = cgAdminsOrig.ElementAt(i).participant_id;
                CgAdminsExtended cgAdminsModif = null;
                switch (userRole) {
                    case UserRole.CentreGroupPropAdmin:
                        cgAdminsModif = (from cgAdminsModifDb in modifCg.cg_admin
                                         where cgAdminsModifDb.id == userId && cgAdminsModifDb.is_cg_prop_admin
                                         select cgAdminsModifDb).FirstOrDefault();
                        break;
                    case UserRole.ApproveMatrixAdmin:
                        cgAdminsModif = (from cgAdminsModifDb in modifCg.cg_admin
                                         where cgAdminsModifDb.id == userId && cgAdminsModifDb.is_appmatrix_admin
                                         select cgAdminsModifDb).FirstOrDefault();
                        break;
                    case UserRole.RequestorAdmin:
                        cgAdminsModif = (from cgAdminsModifDb in modifCg.cg_admin
                                         where cgAdminsModifDb.id == userId && cgAdminsModifDb.is_requestor_admin
                                         select cgAdminsModifDb).FirstOrDefault();
                        break;
                    case UserRole.OrdererAdmin:
                        cgAdminsModif = (from cgAdminsModifDb in modifCg.cg_admin
                                         where cgAdminsModifDb.id == userId && cgAdminsModifDb.is_orderer_admin
                                         select cgAdminsModifDb).FirstOrDefault();
                        break;
                }

                if (cgAdminsModif == null) {
                    m_dbContext.ParticipantRole_CentreGroup.Remove(cgAdminsOrig.ElementAt(i));
                    //m_dbContext.SaveChanges();
                }
            }
        }

        private void UpdateCentres(CentreGroupExtended modfiCg, Centre_Group origCg) {

            //delete
            if (origCg.Centre != null) {
                for (int i = origCg.Centre.Count - 1; i >= 0; i--) {
                    var modifCentre = (from mCentre in modfiCg.centre
                                       where mCentre.id == origCg.Centre.ElementAt(i).id
                                       select mCentre).FirstOrDefault();
                    if (modifCentre == null) {
                        RemoveCentreFromCg(origCg.Centre.ElementAt(i));
                    }
                }
            }

            //new
            foreach (var modifCentre in modfiCg.centre) {
                var origCentre = (from origCgDb in origCg.Centre
                                  where origCgDb.id == modifCentre.id
                                  select origCgDb).FirstOrDefault();
                if (origCentre == null) {
                    var moveCentre = (from origCgDb in m_dbContext.Centre
                                      where origCgDb.id == modifCentre.id
                                      select origCgDb).FirstOrDefault();
                    if (moveCentre.Centre_Group != null) {
                        //move centre
                        RemoveCentreFromCg(moveCentre);
                    }

                    origCg.Centre.Add(moveCentre);
                }
            }
        }

        private void RemoveCentreFromCg(Centre centre) {
            if (centre.Centre_Group == null || centre.Centre_Group.Count == 0) {
                return;
            }

            for (int i = centre.Centre_Group.Count - 1; i >= 0; i--) {
                var cg = centre.Centre_Group.ElementAt(i);

                //delete requestor centre
                DeleteRequestorCentres(centre);

                //delete req role
                DeleteRequestorCgRole(centre);

                //delete pr requestor
                DeletePgRequestor(centre);

                m_dbContext.Entry(centre).State = EntityState.Modified;

                cg.Centre.Remove(centre);
                m_dbContext.Entry(cg).State = EntityState.Modified;
            }
            
        }

        
        private void DeleteRequestorCgRole(Centre centre) {
            var cg = centre.Centre_Group.ElementAt(0);
            var reqRoles = (from partRoleDb in cg.ParticipantRole_CentreGroup
                            where partRoleDb.role_id == (int)UserRole.Requestor
                            select partRoleDb).ToList();

            if (reqRoles != null) {
                for (int i = reqRoles.Count - 1; i >= 0; i--) {
                    bool isDeleteReqRole = true;
                    foreach (var pg in cg.Purchase_Group) {
                        if (pg.active == false) {
                            continue;
                        }

                        foreach (var requestor in pg.PurchaseGroup_Requestor) {
                            if (requestor.centre_id == centre.id ||
                                requestor.requestor_id != reqRoles.ElementAt(i).participant_id) {
                                continue;
                            }

                            isDeleteReqRole = false;
                            break;
                        }
                    }

                    if (isDeleteReqRole) {
                        cg.ParticipantRole_CentreGroup.Remove(reqRoles.ElementAt(i));

                    }
                }

                m_dbContext.Entry(cg).State = EntityState.Modified;
            }
        }

        private void DeleteRequestorCentres(Centre centre) {
            //requestor centres
            //var centre = (from centreDb in m_dbContext.Centre
            //              where centreDb.id == centreId
            //              select centreDb).FirstOrDefault();


            if (centre.Requestor_Centre != null) {
                for (int i = centre.Requestor_Centre.Count - 1; i >= 0; i--) {
                    centre.Requestor_Centre.Remove(centre.Requestor_Centre.ElementAt(i));
                    //m_dbContext.Entry(centre.Requestor_Centre.ElementAt(i)).State = EntityState.Deleted;
                }
                //m_dbContext.Entry(centre).State = EntityState.Modified;
            }

            //participant centre
            var cg = centre.Centre_Group.ElementAt(0);
            var partCData = (from partDataDb in m_dbContext.Participants
                             where partDataDb.centre_id == centre.id
                             select partDataDb).ToList();
            if (partCData != null) {
                foreach (var part in partCData) {
                    part.centre_id = null;

                    if (part.Requestor_Centre != null && part.Requestor_Centre.Count > 0) {
                        int modCentreId = part.Requestor_Centre.ElementAt(0).id;
                        var centreMod = (from centreDb in m_dbContext.Centre
                                         where centreDb.id == modCentreId
                                         select centreDb).FirstOrDefault();
                        part.centre_id = (centreMod.id);

                        part.Requestor_Centre.Remove(part.Requestor_Centre.ElementAt(0));
                    }

                    m_dbContext.Entry(part).State = EntityState.Modified;
                    //m_dbContext.SaveChanges();
                }

            }

            //m_dbContext.SaveChanges();
        }

        private void DeletePgRequestor(Centre centre) {
            var cg = centre.Centre_Group.ElementAt(0);
            int centreId = centre.id;

            foreach (var pg in cg.Purchase_Group) {
                var pgReq = (from pgReqDb in pg.PurchaseGroup_Requestor
                             where pgReqDb.centre_id == centreId
                             select pgReqDb).ToList();
                if (pgReq != null) {
                    for (int i = pgReq.Count - 1; i >= 0; i--) {
                        m_dbContext.PurchaseGroup_Requestor.Remove(pgReq.ElementAt(i));
                    }
                }
            }

        }

        //private void DeleteImpRequestor(Centre centre) {
        //    var cg = centre.Centre_Group.ElementAt(0);
        //    int centreId = centre.id;

        //    foreach (var pg in cg.Purchase_Group) {
        //        var pgReq = (from pgReqDb in pg.PurchaseGroup_ImplicitRequestor
        //                     where pgReqDb.centre_id == centreId
        //                     select pgReqDb).ToList();
        //        if (pgReq != null) {
        //            for (int i = pgReq.Count - 1; i >= 0; i--) {
        //                m_dbContext.PurchaseGroup_Requestor.Remove(pgReq.ElementAt(i));
        //            }
        //        }
        //    }

        //}

        public void SavePurchaseGroupData(
            PurchaseGroupExtended pgModif,
            int currentUserId, string
            strCurrCultureName,
            out string eventErrorMsg) {

            eventErrorMsg = null;

            if (pgModif == null) {
                return;
            }
            int cgId = DataNulls.INT_NULL;

            Centre_Group cg = null;

            //fix potentially wrong limit order
            FixLimitOrder(pgModif);

            using (TransactionScope transaction = new TransactionScope()) {
                try {

                    var origPurchaseGroup = (from pg in m_dbContext.Purchase_Group
                                             where pg.id == pgModif.id
                                             select pg).FirstOrDefault();

                    cgId = origPurchaseGroup.Centre_Group.ElementAt(0).id;
                    cg = GetCentreGroupAllDataById(cgId);


                    if (IsAuthorized(currentUserId, (int)UserRole.ApproveMatrixAdmin, cgId)) {
                        //loc name
                        foreach (var localText in pgModif.local_text) {
                            if (localText.culture.ToLower() == DefaultLanguage && !String.IsNullOrEmpty(localText.text)) {
                                origPurchaseGroup.group_name = localText.text;
                            } else {
                                var pgLoc = (from pgLocDb in origPurchaseGroup.Purchase_Group_Local
                                             where pgLocDb.culture.ToLower() == localText.culture.ToLower()
                                             select pgLocDb).FirstOrDefault();
                                if (pgLoc == null) {
                                    if (String.IsNullOrEmpty(localText.text)) {
                                        continue;
                                    }
                                    Purchase_Group_Local pgl = new Purchase_Group_Local();
                                    pgl.purchase_group_id = origPurchaseGroup.id;
                                    pgl.culture = localText.culture;
                                    pgl.local_text = ShortString(localText.text, ParentPurchaseGroupLocalData.LOCAL_TEXT_LENGTH);
                                    origPurchaseGroup.Purchase_Group_Local.Add(pgl);

                                    try {
                                        AddMaintenanceEvent(
                                                EventRepository.EventCode.PgAddLocalName,
                                                "(" + localText.culture + ") " + localText.text,
                                                cg.company_id,
                                                cg.Company.country_code,
                                                cg.id,
                                                cg.name,
                                                DataNulls.INT_NULL,
                                                null,
                                                pgModif.id,
                                                pgModif.group_name,
                                                currentUserId,
                                                ref eventErrorMsg);
                                    } catch (Exception ex) {
                                        HandleDbError(ex);
                                    }

                                } else {
                                    if (!String.IsNullOrEmpty(localText.text) && pgLoc.local_text != localText.text) {
                                        pgLoc.local_text = ShortString(localText.text, ParentPurchaseGroupLocalData.LOCAL_TEXT_LENGTH);

                                        try {
                                            AddMaintenanceEvent(
                                                    EventRepository.EventCode.PgLocalNameModif,
                                                    "(" + localText.culture + ") " + localText.text,
                                                    cg.company_id,
                                                    cg.Company.country_code,
                                                    cg.id,
                                                    cg.name,
                                                    DataNulls.INT_NULL,
                                                    null,
                                                    pgModif.id,
                                                    pgModif.group_name,
                                                    currentUserId,
                                                    ref eventErrorMsg);
                                        } catch (Exception ex) {
                                            HandleDbError(ex);
                                        }
                                    }
                                }
                            }
                        }

                        //active
                        origPurchaseGroup.active = pgModif.is_active;

                        origPurchaseGroup.self_ordered = pgModif.self_ordered;
                        origPurchaseGroup.is_approval_needed = pgModif.is_approval_needed;
                        origPurchaseGroup.is_order_needed = pgModif.is_order_needed;

                        SetPurchaseType(pgModif, origPurchaseGroup);

                        //Parent Purchase Group
                        var ppg = (from ppgDb in m_dbContext.Parent_Purchase_Group
                                   where ppgDb.id == pgModif.parent_pg_id
                                   select ppgDb).FirstOrDefault();

                        if (origPurchaseGroup.Parent_Purchase_Group.Count == 0) {
                            origPurchaseGroup.Parent_Purchase_Group.Add(ppg);
                        } else {
                            if (origPurchaseGroup.Parent_Purchase_Group.ElementAt(0).id != ppg.id) {
                                origPurchaseGroup.Parent_Purchase_Group.Remove(origPurchaseGroup.Parent_Purchase_Group.ElementAt(0));
                                origPurchaseGroup.Parent_Purchase_Group.Add(ppg);
                            }
                        }

                        DeletePurchaseGroupLimits(pgModif, origPurchaseGroup);
                        NewUpdatePurchaseGroupLimits(pgModif, origPurchaseGroup, currentUserId);

                        //Default Supplier
                        SetDefaultSupplier(pgModif, origPurchaseGroup);
                    }

                    if (IsAuthorized(currentUserId, (int)UserRole.RequestorAdmin, cgId)) {
                        DeleteRequestors(pgModif, origPurchaseGroup, cg);
                        AddRequestor(pgModif, origPurchaseGroup, cg);
                    }

                    if (IsAuthorized(currentUserId, (int)UserRole.OrdererAdmin, cgId)) {
                        DeleteOrderers(pgModif, origPurchaseGroup, cg);
                        AddOrderer(pgModif, origPurchaseGroup, cg);
                    }

                    SaveChanges();
                    transaction.Complete();
                } catch (Exception ex) {
                    throw ex;
                }
            }

            cg = GetCentreGroupAllDataById(cgId);
            DeleteParticipantAppManRole(cg);

        }

        private void SetPurchaseType(PurchaseGroupExtended pgModif, Purchase_Group origPurchaseGroup) {
            //if (pgModif.is_approval_only) {
            //    origPurchaseGroup.purchase_type = (int)PgRepository.PurchaseGroupType.ApprovalOnly;
            //} else {
                origPurchaseGroup.purchase_type = (int)PgRepository.PurchaseGroupType.Standard;
            //}
        }

        private void FixLimitOrder(PurchaseGroupExtended pgModif) {
            for (int i = 0; i < pgModif.purchase_group_limit.Count; i++) {
                if (pgModif.purchase_group_limit.ElementAt(i).is_visible == true) {
                    pgModif.purchase_group_limit.ElementAt(i).app_level_id = i;
                    foreach (var manRole in pgModif.purchase_group_limit.ElementAt(i).manager_role) {
                        manRole.approve_level_id = i;
                    }
                }
            }
        }

        private void SetDefaultSupplier(PurchaseGroupExtended pgModif, Purchase_Group origPurchaseGroup) {
            //Default Supplier
            if (pgModif.default_supplier == null) {
                for (int i = origPurchaseGroup.Supplier.Count - 1; i >= 0; i--) {
                    origPurchaseGroup.Supplier.Remove(origPurchaseGroup.Supplier.ElementAt(i));
                }
            } else {
                bool isSuppSet = false;
                for (int i = origPurchaseGroup.Supplier.Count - 1; i >= 0; i--) {
                    if (origPurchaseGroup.Supplier.ElementAt(i).id == pgModif.default_supplier.id) {
                        isSuppSet = true;
                    } else {
                        origPurchaseGroup.Supplier.Remove(origPurchaseGroup.Supplier.ElementAt(i));
                    }
                }

                if (!isSuppSet) {
                    var supplier = (from suppDb in m_dbContext.Supplier
                                    where suppDb.id == pgModif.default_supplier.id
                                    select suppDb).FirstOrDefault();
                    origPurchaseGroup.Supplier.Add(supplier);
                }
            }
        }

        public PurchaseGroupExtended AddPurchaseGroupData(
            PurchaseGroupExtended newModif,
            int currentUserId,
            string strCurrCultureName,
            string rootUrl) {

            Centre_Group cg = (from cgDb in m_dbContext.Centre_Group
                               where cgDb.id == newModif.centre_group_id
                               select cgDb).FirstOrDefault();

            int lastId = new PgRepository().GetLastId();
            int newId = lastId+1;
            //int lastId = m_dbContext.Purchase_Group.Max(x => x.id);
            //int newId = ++lastId;

            using (TransactionScope transaction = new TransactionScope()) {
                try {


                    Purchase_Group origPurchaseGroup = new Purchase_Group();
                    origPurchaseGroup.id = newId;
                    origPurchaseGroup.self_ordered = newModif.self_ordered;
                    //origPurchaseGroup.group_name = newModif.group_loc_name;
                    origPurchaseGroup.is_approval_needed = newModif.is_approval_needed;
                    origPurchaseGroup.is_order_needed = newModif.is_order_needed;

                    var defPgName = (from locText in newModif.local_text
                                     where locText.culture == DefaultLanguage
                                     select locText).FirstOrDefault();
                    if (defPgName != null && !String.IsNullOrEmpty(defPgName.text)) {
                        origPurchaseGroup.group_name = defPgName.text;
                    } else {
                        origPurchaseGroup.group_name = newModif.local_text.ElementAt(0).text;
                    }

                    //loc name
                    foreach (var localText in newModif.local_text) {

                        var pgLoc = (from pgLocDb in origPurchaseGroup.Purchase_Group_Local
                                     where pgLocDb.culture.ToLower() == localText.culture.ToLower()
                                     select pgLocDb).FirstOrDefault();
                        if (pgLoc == null) {
                            if (String.IsNullOrEmpty(localText.text)) {
                                continue;
                            }
                            Purchase_Group_Local pgl = new Purchase_Group_Local();
                            pgl.purchase_group_id = origPurchaseGroup.id;
                            pgl.culture = localText.culture;
                            pgl.local_text = localText.text;
                            origPurchaseGroup.Purchase_Group_Local.Add(pgl);

                            //AddEvent(
                            //            EventRepository.EventCode.AppMatrixChange,
                            //            "Purchase Group Add Local Name",
                            //            cg.company_id,
                            //            cg.id,
                            //            origPurchaseGroup.id,
                            //            currentUserId);
                        } else {
                            if (pgLoc.local_text != localText.text) {
                                pgLoc.local_text = localText.text;

                                //AddEvent(
                                //        EventRepository.EventCode.AppMatrixChange,
                                //        "Purchase Group Local Name Modifiction",
                                //        cg.company_id,
                                //        cg.id,
                                //        origPurchaseGroup.id,
                                //        currentUserId);
                            }
                        }

                    }

                    origPurchaseGroup.active = true;

                    var ppg = (from ppgDb in m_dbContext.Parent_Purchase_Group
                               where ppgDb.id == newModif.parent_pg_id
                               select ppgDb).FirstOrDefault();



                    //parent pg
                    origPurchaseGroup.Parent_Purchase_Group.Add(ppg);
                    origPurchaseGroup.Centre_Group.Add(cg);

                    //limits
                    NewUpdatePurchaseGroupLimits(newModif, origPurchaseGroup, currentUserId);

                    //Requestors orderers
                    AddRequestor(newModif, origPurchaseGroup, cg);
                    AddOrderer(newModif, origPurchaseGroup, cg);

                    //Default supplier
                    SetDefaultSupplier(newModif, origPurchaseGroup);

                    m_dbContext.Purchase_Group.Add(origPurchaseGroup);

                    SaveChanges();
                    transaction.Complete();
                } catch (Exception ex) {
                    throw ex;
                }
            }

            Purchase_Group pg = (from pgDb in m_dbContext.Purchase_Group
                                 where pgDb.id == newId
                                 select pgDb).FirstOrDefault();



            PurchaseGroupExtended newPgReturn = GetPurchaseGroup(pg, cg, strCurrCultureName, rootUrl, true);

            return newPgReturn;

        }

        public void CopyAppMatrix(
            int pgId,
            bool isNew,
            int targetCgId,
            int currentUserId,
            string strCurrCultureName,
            string rootUrl,
            out string eventErrorMsg) {

            eventErrorMsg = null;

            //using (TransactionScope transaction = new TransactionScope()) {
            try {

                var sourcePg = (from pgDb in m_dbContext.Purchase_Group
                                where pgDb.id == pgId
                                select pgDb).FirstOrDefault();

                int pgCount = 0;
                Centre_Group cg = (from cgDb in m_dbContext.Centre_Group where cgDb.id == targetCgId select cgDb).FirstOrDefault();
                Purchase_Group targetPg = null;
                if (!isNew) {

                    var pgs = (from pgDb in cg.Purchase_Group where pgDb.active == true select pgDb).ToList();
                    //Purchase_Group targetPg = null;
                    foreach (var pg in pgs) {
                        if (pg.Parent_Purchase_Group != null &&
                            pg.Parent_Purchase_Group.Count > 0 &&
                            pg.Parent_Purchase_Group.ElementAt(0).id == sourcePg.Parent_Purchase_Group.ElementAt(0).id) {
                            targetPg = pg;
                            pgCount++;

                        }
                    }
                }

                string msg = null;
                if (pgCount == 0) {
                    CopyAppMatrixAddNew(
                        sourcePg,
                        targetCgId,
                        currentUserId,
                        strCurrCultureName,
                        rootUrl);

                    msg = sourcePg.group_name + " Added to " + cg.name + "(id=" + cg.id + ")";
                } else if (pgCount == 1) {
                    CopyAppMatrixRewrite(
                        sourcePg,
                        targetPg,
                        cg,
                        strCurrCultureName,
                        rootUrl,
                        currentUserId);

                    msg = sourcePg.group_name + " Rewritten to " + cg.name + "(id=" + cg.id + ")";
                } else {
                    throw new Exception("Cannot identify Target Purchase Group");
                }

                try {
                    int targetPgId = DataNulls.INT_NULL;
                    string targetPgName = null;

                    if (targetPg != null) {
                        targetPgId = targetPg.id;
                        targetPgName = targetPg.group_name;
                    } else {
                        targetPgName = sourcePg.group_name;
                    }

                    AddMaintenanceEvent(
                            EventRepository.EventCode.CopyAppMatrix,
                            msg,
                            cg.company_id,
                            cg.Company.country_code,
                            cg.id,
                            cg.name,
                            DataNulls.INT_NULL,
                            null,
                            targetPgId,
                            targetPgName,
                            currentUserId,
                            ref eventErrorMsg);
                } catch (Exception ex) {
                    HandleDbError(ex);
                }

            } catch (Exception ex) {
                throw ex;
            }
            //}
        }

        public int CopyAppMatrixAddNew(
            Purchase_Group sourcePg,
            int targetCgId,
            int currentUserId,
            string strCurrCultureName,
            string rootUrl) {
            int newPgId = DataNulls.INT_NULL;

            PurchaseGroupExtended pgNewPg = new PurchaseGroupExtended();
            CopyPgItems(sourcePg, pgNewPg);
            pgNewPg.id = newPgId;
            //pgNewPg.group_name = sourcePg.group_name;
            

            //local names
            LocalText pglDef = new LocalText();
            pglDef.text = sourcePg.group_name;
            pglDef.culture = DefaultLanguage;
            pgNewPg.local_text.Add(pglDef);
            foreach (var locPgName in sourcePg.Purchase_Group_Local) {
                LocalText pgl = new LocalText();
                pgl.text = locPgName.local_text;
                pgl.culture = locPgName.culture;
                pgNewPg.local_text.Add(pgl);
            }


            //parent pg
            if (sourcePg.Parent_Purchase_Group != null && sourcePg.Parent_Purchase_Group.Count > 0) {
                pgNewPg.parent_pg_id = sourcePg.Parent_Purchase_Group.ElementAt(0).id;
            }

            //centre group
            pgNewPg.centre_group_id = targetCgId;

            //app limits
            foreach (var sourceLimit in sourcePg.Purchase_Group_Limit) {
                PurchaseGroupLimitExtended targetLimit = new PurchaseGroupLimitExtended();
                targetLimit.limit_bottom = sourceLimit.limit_bottom;
                targetLimit.limit_top = sourceLimit.limit_top;
                //targetLimit.limit_bottom_text = ConvertData.ToString(sourceLimit.limit_bottom);
                //targetLimit.limit_top_text = ConvertData.ToString(sourceLimit.limit_top);
                targetLimit.app_level_id = sourceLimit.approve_level_id;
                targetLimit.is_visible = true;
                foreach (var sourceAppMan in sourceLimit.Manager_Role) {
                    if (sourceAppMan.active == false) {
                        continue;
                    }

                    ManagerRoleExtended targetAppMan = new ManagerRoleExtended();
                    //targetAppMan.centre_group_id = targetCgId;
                    //targetAppMan.is_inspector = sourceAppMan.is_inspector;
                    targetAppMan.participant_id = sourceAppMan.participant_id;

                    ParticipantsExtended targetAppManPart = new ParticipantsExtended();
                    targetAppManPart.id = sourceAppMan.participant_id;
                    targetAppMan.participant = targetAppManPart;

                    targetLimit.manager_role.Add(targetAppMan);
                }

                pgNewPg.purchase_group_limit.Add(targetLimit);
            }

            AddPurchaseGroupData(
                        pgNewPg,
                        currentUserId,
                        strCurrCultureName,
                        rootUrl);

            return newPgId;
        }

        private void CopyPgItems(Purchase_Group sourcePg, PurchaseGroupExtended pgNewPg) {
            //Type pgExtend = typeof(PurchaseGroupExtended);
            //PropertyInfo[] targetPropertyInfos = pgExtend.GetProperties();
            ////List<string> lstTargetProperties = new List<string>();
            //foreach (var propertyInfo in targetPropertyInfos) {
            //    //lstTargetProperties.Add(propertyInfo.Name);
            //    PropertyInfo piInstance = examType.GetProperty("InstanceProperty");
            //    piInstance.SetValue(exam, 37);
            //}

            Type sourceType = sourcePg.GetType();
            foreach (var sourceProperty in sourceType.GetProperties()) {
                                
                Type targetType = pgNewPg.GetType();
                PropertyInfo targetPropertyInfo = targetType.GetProperty(sourceProperty.Name);
                if (targetPropertyInfo != null) {
                    if (sourceProperty.PropertyType == typeof(int)
                        || sourceProperty.PropertyType == typeof(decimal)
                        || sourceProperty.PropertyType == typeof(double)
                        || sourceProperty.PropertyType == typeof(DateTime)
                        || sourceProperty.PropertyType == typeof(bool)
                        || sourceProperty.PropertyType == typeof(string)) {
                        targetPropertyInfo.SetValue(pgNewPg, sourceProperty.GetValue(sourcePg));
                    }
                }
            }

        }

        public void CopyAppMatrixRewrite(
            Purchase_Group sourcePg,
            Purchase_Group targetPg,
            Centre_Group targetCg,
            string strCurrCultureName,
            string rootUrl,
            int currentUserId) {

            bool isNewStructLoaded = false;
            foreach (var pg in targetCg.Purchase_Group) {
                if (pg.PurchaseGroup_Requestor.Count > 0 || pg.PurchaseGroup_Orderer.Count > 0) {
                    isNewStructLoaded = true;
                    break;
                }
            }

            PurchaseGroupExtended pgTargetExtend = GetPurchaseGroup(targetPg, targetCg, strCurrCultureName, rootUrl, isNewStructLoaded);
            for (int i = 0; i < sourcePg.Purchase_Group_Limit.Count; i++) {
                var sourceLimit = sourcePg.Purchase_Group_Limit.ElementAt(i);

                var tagetLimit = pgTargetExtend.purchase_group_limit.ElementAt(i);
                tagetLimit.limit_bottom = sourceLimit.limit_bottom;
                tagetLimit.limit_top = sourceLimit.limit_top;
                //tagetLimit.limit_bottom_text = ConvertData.ToString(sourceLimit.limit_bottom);
                //tagetLimit.limit_top_text = ConvertData.ToString(sourceLimit.limit_top);
                tagetLimit.is_visible = true;

                for (int j = tagetLimit.manager_role.Count - 1; j >= 0; j--) {
                    tagetLimit.manager_role.Remove(tagetLimit.manager_role.ElementAt(j));
                }

                foreach (var sourceAppManRole in sourceLimit.Manager_Role) {
                    if (sourceAppManRole.active == false) {
                        continue;
                    }
                    ManagerRoleExtended targetAppManRole = new ManagerRoleExtended();
                    targetAppManRole.participant_id = sourceAppManRole.participant_id;
                    targetAppManRole.approve_level_id = sourceAppManRole.approve_level_id;
                    ParticipantsExtended targetAppManPart = new ParticipantsExtended();
                    targetAppManPart.id = sourceAppManRole.participant_id;
                    targetAppManRole.participant = targetAppManPart;
                    tagetLimit.manager_role.Add(targetAppManRole);
                }
            }

            if (sourcePg.Purchase_Group_Limit.Count < pgTargetExtend.purchase_group_limit.Count && sourcePg.Purchase_Group_Limit.Count > 0) {
                for (int j = sourcePg.Purchase_Group_Limit.Count; j < pgTargetExtend.purchase_group_limit.Count; j++) {
                    pgTargetExtend.purchase_group_limit.ElementAt(j).is_visible = false;
                }
            }

            string errMsg;
            SavePurchaseGroupData(pgTargetExtend, currentUserId, strCurrCultureName, out errMsg);

            if (!String.IsNullOrEmpty(errMsg)) {
                throw new Exception(errMsg);
            }
        }

        private void AddRequestorsInitial(Centre_Group cg) {
            Hashtable htPgCg = new Hashtable();
            Hashtable htReqCentre = new Hashtable();
            foreach (var pg in cg.Purchase_Group) {
                foreach (var req in pg.PurchaseGroup_Requestor) {
                    if (!htPgCg.ContainsKey(req.requestor_id)) {
                        var prCg = (from prcgDb in cg.ParticipantRole_CentreGroup
                                    where prcgDb.participant_id == req.requestor_id &&
                                    prcgDb.role_id == (int)UserRole.Requestor
                                    select prcgDb).FirstOrDefault();
                        if (prCg == null) {
                            ParticipantRole_CentreGroup prCgNew = new ParticipantRole_CentreGroup();
                            prCgNew.participant_id = req.requestor_id;
                            prCgNew.role_id = (int)UserRole.Requestor;
                            prCgNew.centre_group_id = cg.id;
                            cg.ParticipantRole_CentreGroup.Add(prCgNew);
                        }
                        htPgCg.Add(req.requestor_id, null);
                    }

                    if (!htReqCentre.ContainsKey(req.requestor_id + "_" + req.centre_id)) {
                        var reqCentre = (from reqCentreDb in req.Participants.Requestor_Centre
                                         where reqCentreDb.id == req.centre_id
                                         select reqCentreDb).FirstOrDefault();
                        if (reqCentre == null) {
                            var centre = m_dbContext.Centre.Find(req.centre_id);
                            req.Participants.Requestor_Centre.Add(centre);
                        }

                        htReqCentre.Add(req.requestor_id + "_" + req.centre_id, null);
                    }
                }
            }
        }

        private void AddOrderersInitial(Centre_Group cg) {
            Hashtable htPgCg = new Hashtable();

            foreach (var pg in cg.Purchase_Group) {
                foreach (var ord in pg.PurchaseGroup_Orderer) {
                    if (!htPgCg.ContainsKey(ord.id)) {
                        var prCg = (from prcgDb in cg.ParticipantRole_CentreGroup
                                    where prcgDb.participant_id == ord.id &&
                                    prcgDb.role_id == (int)UserRole.Orderer
                                    select prcgDb).FirstOrDefault();
                        if (prCg == null) {
                            ParticipantRole_CentreGroup prCgNew = new ParticipantRole_CentreGroup();
                            prCgNew.participant_id = ord.id;
                            prCgNew.role_id = (int)UserRole.Orderer;
                            prCgNew.centre_group_id = cg.id;
                            cg.ParticipantRole_CentreGroup.Add(prCgNew);
                        }
                        htPgCg.Add(ord.id, null);
                    }


                }
            }
        }

        private void DeleteRequestors(PurchaseGroupExtended pgModif, Purchase_Group pgOrig, Centre_Group cg) {
            var defaultRequestors = (from reqRole in cg.ParticipantRole_CentreGroup
                                     where reqRole.role_id == (int)UserRole.Requestor
                                     select reqRole).ToList();

            Hashtable htDisabled = new Hashtable();
            for (int i = defaultRequestors.Count - 1; i >= 0; i--) {
                var ordererRole = defaultRequestors.ElementAt(i);

                if (htDisabled.ContainsKey(ordererRole.participant_id)) {
                    continue;
                }
                //deactivated user
                if (DeleteDisabledUser(cg, ordererRole)) {
                    if (!htDisabled.ContainsKey(ordererRole.participant_id)) {
                        htDisabled.Add(ordererRole.participant_id, null);
                    }
                }
            }

            //delete for whole centre
            foreach (var reqModif in pgModif.delete_requestors_all_categories) {
                foreach (var pg in cg.Purchase_Group) {
                    var origReq = (from origReqDB in pg.PurchaseGroup_Requestor
                                   where origReqDB.requestor_id == reqModif.participant_id &&
                                   origReqDB.centre_id == reqModif.centre_id
                                   select origReqDB).FirstOrDefault();
                    if (origReq != null && !htDisabled.ContainsKey(reqModif.participant_id)) {
                        pg.PurchaseGroup_Requestor.Remove(origReq);
                    }
                }

                DeleteRequestorCentre(reqModif.participant_id, reqModif.centre_id);
                DeleteParticipantRequestorRole(cg, reqModif.participant_id, DataNulls.INT_NULL, reqModif.centre_id);
            }

            for (int i = pgOrig.PurchaseGroup_Requestor.Count - 1; i >= 0; i--) {
                var modifReq = (from modifReqDb in pgModif.requestor
                                where modifReqDb.participant_id == pgOrig.PurchaseGroup_Requestor.ElementAt(i).requestor_id &&
                                modifReqDb.centre_id == pgOrig.PurchaseGroup_Requestor.ElementAt(i).centre_id
                                select modifReqDb).FirstOrDefault();
                if (modifReq == null) {
                    //check whether requestor is used for othe purchase groups
                    var reqCg = (from reqCgDb in pgOrig.PurchaseGroup_Requestor.ElementAt(i).Participants.PurchaseGroup_Requestor
                                 where reqCgDb.centre_id == pgOrig.PurchaseGroup_Requestor.ElementAt(i).centre_id
                                 select reqCgDb).ToList();
                    if (reqCg != null && reqCg.Count == 1) {
                        DeleteRequestorCentre(pgOrig.PurchaseGroup_Requestor.ElementAt(i).requestor_id, pgOrig.PurchaseGroup_Requestor.ElementAt(i).centre_id);
                        DeleteParticipantRequestorRole(cg, pgOrig.PurchaseGroup_Requestor.ElementAt(i).requestor_id, pgOrig.id, pgOrig.PurchaseGroup_Requestor.ElementAt(i).centre_id);
                    }

                    pgOrig.PurchaseGroup_Requestor.Remove(pgOrig.PurchaseGroup_Requestor.ElementAt(i));

                }
            }


        }

        public void DeleteParticipantRequestorRole(Centre_Group cg, int requestorId, int skipPgId, int skipCentreId) {
            bool isDelete = true;
            foreach (var pg in cg.Purchase_Group) {

                var pgReq = (from pgReqDb in pg.PurchaseGroup_Requestor
                             where pgReqDb.requestor_id == requestorId &&
                             pgReqDb.centre_id != skipCentreId &&
                             pgReqDb.purchase_group_id != skipPgId
                             select pgReqDb).FirstOrDefault();
                if (pgReq != null) {
                    isDelete = false;
                    break;
                }
            }

            if (isDelete) {
                var prCg = (from prCgDb in cg.ParticipantRole_CentreGroup
                            where prCgDb.participant_id == requestorId &&
                            prCgDb.role_id == (int)UserRole.Requestor
                            select prCgDb).FirstOrDefault();
                if (prCg != null) {
                    foreach (var centre in cg.Centre) {
                        //remove requestor_centre
                        DeleteRequestorCentre(requestorId, centre.id);
                    }

                    cg.ParticipantRole_CentreGroup.Remove(prCg);
                }
            }
        }


        private void DeleteParticipantOrdererRole(Centre_Group cg, int ordererId, int skipPgId) {
            bool isDelete = true;
            foreach (var pg in cg.Purchase_Group) {
                if (pg.id == skipPgId) {
                    continue;
                }
                var pgOrd = (from pgOrdDb in pg.PurchaseGroup_Orderer
                             where pgOrdDb.id == ordererId
                             select pgOrdDb).FirstOrDefault();
                if (pgOrd != null) {
                    isDelete = false;
                    break;
                }
            }

            if (isDelete) {
                var prCg = (from prCgDb in cg.ParticipantRole_CentreGroup
                            where prCgDb.participant_id == ordererId &&
                            prCgDb.role_id == (int)UserRole.Orderer
                            select prCgDb).FirstOrDefault();
                if (prCg != null) {
                    cg.ParticipantRole_CentreGroup.Remove(prCg);
                }
            }
        }

        //private string GetReqCentreKey(int participantId, int centreId) {
        //    return participantId + "_" + centreId;
        //}

        //private void GetReqCentreKeyParts(out int requestorId, out int centreId) {

        //}

        private void AddRequestor(PurchaseGroupExtended pgModif, Purchase_Group pgOrig, Centre_Group cgOrig) {
            foreach (var modifReq in pgModif.requestor) {
                if (modifReq.is_all) {
                    foreach (var pg in cgOrig.Purchase_Group) {
                        var reqOrig = (from reqOrigDB in pg.PurchaseGroup_Requestor
                                       where reqOrigDB.requestor_id == modifReq.participant_id &&
                                       reqOrigDB.centre_id == modifReq.centre_id
                                       select reqOrigDB).FirstOrDefault();
                        if (reqOrig == null) {
                            PurchaseGroup_Requestor pgr = new PurchaseGroup_Requestor();
                            pgr.requestor_id = modifReq.participant_id;
                            pgr.centre_id = modifReq.centre_id;
                            pgr.purchase_group_id = pgOrig.id;
                            pg.PurchaseGroup_Requestor.Add(pgr);

                            AddRequestorCentre(pgOrig, modifReq);
                            AddUserRoleCentreGroup(cgOrig, modifReq, (int)UserRole.Requestor);
                        }
                    }
                } else {
                    var reqOrig = (from reqOrigDB in pgOrig.PurchaseGroup_Requestor
                                   where reqOrigDB.requestor_id == modifReq.participant_id &&
                                   reqOrigDB.centre_id == modifReq.centre_id
                                   select reqOrigDB).FirstOrDefault();
                    if (reqOrig == null) {
                        PurchaseGroup_Requestor pgr = new PurchaseGroup_Requestor();
                        pgr.requestor_id = modifReq.participant_id;
                        pgr.centre_id = modifReq.centre_id;
                        pgr.purchase_group_id = pgOrig.id;
                        pgOrig.PurchaseGroup_Requestor.Add(pgr);

                        AddRequestorCentre(pgOrig, modifReq);
                        AddUserRoleCentreGroup(cgOrig, modifReq, (int)UserRole.Requestor);
                    }
                }


            }
        }

        private void AddRequestorCentre(Purchase_Group pgOrig, RequestorExtended modifRequestor) {
            var participant = (from partic in m_dbContext.Participants
                               where partic.id == modifRequestor.participant_id
                               select partic).FirstOrDefault();

            if (participant.centre_id != modifRequestor.centre_id || participant.centre_id == null) {
                if (participant.centre_id == null) {
                    participant.centre_id = modifRequestor.centre_id;
                    m_dbContext.SaveChanges();
                } else {
                    var requestorCentre = (from requestorCentreDb in participant.Requestor_Centre
                                           where requestorCentreDb.id == modifRequestor.centre_id
                                           select requestorCentreDb).FirstOrDefault();
                    if (requestorCentre == null) {
                        var partCentre = (from centreDb in m_dbContext.Centre
                                          where centreDb.id == modifRequestor.centre_id
                                          select centreDb).FirstOrDefault();
                        participant.Requestor_Centre.Add(partCentre);
                        m_dbContext.SaveChanges();
                    }
                }
            }
        }

        private void AddUserRoleCentreGroup(Centre_Group cg, RequestorOrdererExtended modifRequestor, int userRoleId) {
            var prcg = (from prcgDb in cg.ParticipantRole_CentreGroup
                        where prcgDb.participant_id == modifRequestor.participant_id &&
                        prcgDb.role_id == userRoleId
                        select prcgDb).FirstOrDefault();
            if (prcg == null) {
                ParticipantRole_CentreGroup prcgNew = new ParticipantRole_CentreGroup();
                prcgNew.participant_id = modifRequestor.participant_id;
                prcgNew.role_id = userRoleId;
                prcgNew.centre_group_id = cg.id;
                cg.ParticipantRole_CentreGroup.Add(prcgNew);
            }
        }

        private void DeleteRequestorCentre(int requestorId, int centreId) {
            var participant = (from partic in m_dbContext.Participants
                               where partic.id == requestorId
                               select partic).FirstOrDefault();
            if (participant.centre_id == centreId) {
                if (participant.Requestor_Centre.Count > 0) {
                    participant.centre_id = participant.Requestor_Centre.ElementAt(0).id;
                    participant.Requestor_Centre.Remove(participant.Requestor_Centre.ElementAt(0));
                } else {
                    participant.centre_id = null;
                }
            } else {
                var req_centre = (from rc in participant.Requestor_Centre
                                  where rc.id == centreId
                                  select rc).FirstOrDefault();

                if (req_centre != null) {
                    participant.Requestor_Centre.Remove(req_centre);
                }
            }
            //var requestorCentre = (from requestorCentreDb in participant.Requestor_Centre
            //                       where requestorCentreDb.id == modifRequestor.centre_id
            //                       select requestorCentreDb).FirstOrDefault();
            //if (requestorCentre == null) {
            //    var partCentre = (from centreDb in m_dbContext.Centre
            //                      where centreDb.id == modifRequestor.centre_id
            //                      select centreDb).FirstOrDefault();
            //    participant.Requestor_Centre.Add(partCentre);

            //}

        }

        private void DeleteOrderers(PurchaseGroupExtended pgModif, Purchase_Group pgOrig, Centre_Group cg) {
            var defaultOrderers = (from reqRole in cg.ParticipantRole_CentreGroup
                                   where reqRole.role_id == (int)UserRole.Orderer
                                   select reqRole).ToList();

            Hashtable htDelOrderer = new Hashtable();
            for (int i = defaultOrderers.Count - 1; i >= 0; i--) {
                var ordererRole = defaultOrderers.ElementAt(i);

                if (htDelOrderer.ContainsKey(ordererRole.participant_id)) {
                    continue;
                }
                //deactivated user
                if (DeleteDisabledUser(cg, ordererRole)) {
                    if (!htDelOrderer.ContainsKey(ordererRole.participant_id)) {
                        htDelOrderer.Add(ordererRole.participant_id, null);
                    }
                }
            }


            foreach (var ordModif in pgModif.delete_orderers_all_categories) {
                foreach (var pg in cg.Purchase_Group) {
                    var origOrdList = (from origReqDB in pg.PurchaseGroup_Orderer
                                       where origReqDB.id == ordModif
                                       select origReqDB).ToList();
                    if (origOrdList != null) {
                        for (int i = origOrdList.Count - 1; i >= 0; i--) {
                            if (htDelOrderer.ContainsKey(origOrdList.ElementAt(i).id)) {
                                continue;
                            } else {

                                origOrdList.RemoveAt(i);

                            }
                        }
                    }
                }

                var prCg = (from prCgDb in cg.ParticipantRole_CentreGroup
                            where prCgDb.participant_id == ordModif &&
                            prCgDb.role_id == (int)UserRole.Orderer
                            select prCgDb).FirstOrDefault();
                if (prCg != null) {
                    cg.ParticipantRole_CentreGroup.Remove(prCg);
                }

                if (!htDelOrderer.ContainsKey(ordModif)) {
                    htDelOrderer.Add(ordModif, null);
                }
            }

            for (int i = pgOrig.PurchaseGroup_Orderer.Count - 1; i >= 0; i--) {
                if (htDelOrderer.ContainsKey(pgOrig.PurchaseGroup_Orderer.ElementAt(i).id)) {
                    continue;
                }

                var modifOrd = (from modifReqDb in pgModif.orderer
                                where modifReqDb.participant_id == pgOrig.PurchaseGroup_Orderer.ElementAt(i).id
                                select modifReqDb).FirstOrDefault();
                if (modifOrd == null) {

                    DeleteParticipantOrdererRole(cg, pgOrig.PurchaseGroup_Orderer.ElementAt(i).id, pgOrig.id);
                    pgOrig.PurchaseGroup_Orderer.Remove(pgOrig.PurchaseGroup_Orderer.ElementAt(i));

                }
            }

        }

        private void AddOrderer(PurchaseGroupExtended pgModif, Purchase_Group pgOrig, Centre_Group cgOrig) {
            foreach (var modifOrd in pgModif.orderer) {
                if (modifOrd.is_all) {
                    foreach (var pg in cgOrig.Purchase_Group) {
                        var reqOrig = (from reqOrigDB in pg.PurchaseGroup_Orderer
                                       where reqOrigDB.id == modifOrd.participant_id
                                       select reqOrigDB).FirstOrDefault();
                        if (reqOrig == null) {
                            Participants orderer = (from partDb in m_dbContext.Participants
                                                    where partDb.id == modifOrd.participant_id
                                                    select partDb).FirstOrDefault();
                            pg.PurchaseGroup_Orderer.Add(orderer);
                            AddUserRoleCentreGroup(cgOrig, modifOrd, (int)UserRole.Orderer);
                        }
                    }
                } else {
                    var reqOrig = (from reqOrigDB in pgOrig.PurchaseGroup_Orderer
                                   where reqOrigDB.id == modifOrd.participant_id
                                   select reqOrigDB).FirstOrDefault();
                    if (reqOrig == null) {
                        Participants orderer = (from partDb in m_dbContext.Participants
                                                where partDb.id == modifOrd.participant_id
                                                select partDb).FirstOrDefault();
                        pgOrig.PurchaseGroup_Orderer.Add(orderer);
                        AddUserRoleCentreGroup(cgOrig, modifOrd, (int)UserRole.Orderer);
                    }
                }
            }
        }

        //public void DeleteOrderers(PurchaseGroupExtended pgModif, Purchase_Group origPurchaseGroup, Centre_Group cg) {
        //    var defaultOrderers = (from reqRole in cg.ParticipantRole_CentreGroup
        //                             where reqRole.role_id == (int)UserRole.Orderer
        //                             select reqRole).ToList();

        //    for (int i = defaultOrderers.Count - 1; i >= 0; i--) {
        //        var ordererRole = defaultOrderers.ElementAt(i);

        //        //deactivated user
        //        if (DeleteDisabledUser(cg, ordererRole)) {
        //            continue;
        //        }

        //        //delete from all purchase groups
        //        if (pgModif.DeleteOrderersAllCategories.Contains(ordererRole.participant_id)) {
        //            foreach (var pg in cg.Purchase_Group) {
        //                //delete implicite
        //                var implOrd = (from implOrdDb in pg.PurchaseGroup_ImplicitOrderer
        //                               where implOrdDb.orderer_id == ordererRole.participant_id
        //                               select implOrdDb).FirstOrDefault();
        //                if (implOrd != null) {
        //                    pg.PurchaseGroup_ImplicitOrderer.Remove(implOrd);
        //                }


        //                //delete exclude
        //                var exclOrd = (from exclOrdDb in pg.ParticipantsExcludeOrderers
        //                               where exclOrdDb.id == ordererRole.participant_id
        //                               select exclOrdDb).FirstOrDefault();
        //                if (exclOrd != null) {
        //                    pg.ParticipantsExcludeOrderers.Remove(exclOrd);
        //                }
        //            }

        //            cg.ParticipantRole_CentreGroup.Remove(ordererRole);

        //            continue;
        //        }


        //    }

        //    DeleteOrdererInSinglePg(cg, pgModif, origPurchaseGroup);

        //}

        //private void DeleteOrdererInSinglePg(Centre_Group cg, PurchaseGroupExtended pgModif, Purchase_Group origPurchaseGroup) {
        //    //delete single purchase group
        //    if (origPurchaseGroup.PurchaseGroup_ImplicitOrderer.Count > 0) {
        //        for (int i = origPurchaseGroup.PurchaseGroup_ImplicitOrderer.Count - 1; i >= 0; i--) {
        //            var modifOrd = (from modifOrdDb in pgModif.OrdererExtended
        //                            where modifOrdDb.participant_id == origPurchaseGroup.PurchaseGroup_ImplicitOrderer.ElementAt(i).orderer_id
        //                            select modifOrdDb).FirstOrDefault();
        //            if (modifOrd == null) {
        //                //delete
        //                origPurchaseGroup.PurchaseGroup_ImplicitOrderer.Remove(origPurchaseGroup.PurchaseGroup_ImplicitOrderer.ElementAt(i));
        //                if (origPurchaseGroup.PurchaseGroup_ImplicitOrderer.Count == 0) {
        //                    //exclude standar orderers, keep the orderers empty
        //                    SetExcludeOrdererForPgDeletedAllImplicite(cg, origPurchaseGroup);
        //                }
        //            }
        //        }


        //    } else {

        //    }
        //}

        //private void SetExcludeOrdererForPgDeletedAllImplicite(Centre_Group cg, Purchase_Group origPurchaseGroup) {
        //    foreach (var ordCg in cg.ParticipantRole_CentreGroup) {
        //        if (ordCg.role_id == (int)UserRole.Orderer) {
        //            var exclOrd = (from excOrdDb in origPurchaseGroup.ParticipantsExcludeOrderers
        //                           where excOrdDb.id == ordCg.participant_id
        //                           select excOrdDb).FirstOrDefault();
        //            if (exclOrd == null) {
        //                var participant = m_dbContext.Participants.Find(ordCg.participant_id);
        //                origPurchaseGroup.ParticipantsExcludeOrderers.Add(participant);
        //                //m_dbContext.Entry(participant).State = EntityState.Added;
        //            }
        //        }
        //    }
        //}

        //public void DeleteRequestors(PurchaseGroupExtended pgModif, Purchase_Group origPurchaseGroup, Centre_Group cg) {
        //    Hashtable htCentresInUse = new Hashtable();


        //    var defaultRequestors = (from reqRole in cg.ParticipantRole_CentreGroup
        //                             where reqRole.role_id == (int)UserRole.Requestor
        //                             select reqRole).ToList();

        //    bool isPgExcludedDefaultRequestors = false;

        //    for (int i = defaultRequestors.Count - 1; i >= 0; i--) {
        //        var requestorRole = defaultRequestors.ElementAt(i);

        //        foreach (var centre in cg.Centre) {
        //            if (!htCentresInUse.ContainsKey(centre.id)) {
        //                htCentresInUse.Add(centre.id, null);
        //            }
        //        }


        //        //deactivated user
        //        if (DeleteDisabledUser(cg, requestorRole)) {
        //            continue;
        //        }

        //        //Delete participant in whole centre
        //        DeleteRequestorFromCentre(pgModif, requestorRole, htCentresInUse);

        //        Hashtable htTmpCentresInUse = (Hashtable)htCentresInUse.Clone();
        //        IDictionaryEnumerator iEnumCentre = htTmpCentresInUse.GetEnumerator();
        //        while (iEnumCentre.MoveNext()) {
        //            //single change only one PG
        //            int centreId = (int)iEnumCentre.Key;
        //            var singleRequestors = (from modifReq in pgModif.RequestorExtended
        //                                    where modifReq.participant_id == requestorRole.participant_id &&
        //                                    modifReq.centre_id == centreId
        //                                    select modifReq).ToList();

        //            if (singleRequestors == null || singleRequestors.Count == 0) {
        //                //is implicite?
        //                bool isExcludedDefaultRequestors = DeleteImpliciteRequestorInCentre(
        //                    requestorRole, 
        //                    cg,
        //                    pgModif,
        //                    centreId,
        //                    htCentresInUse);
        //                if (isExcludedDefaultRequestors) {
        //                    isPgExcludedDefaultRequestors = true;
        //                }

        //                //not implicite
        //                DeleteRequestorInCentre(
        //                    requestorRole,
        //                    cg,
        //                    pgModif,
        //                    centreId);
        //            }
        //        }

        //        if (htCentresInUse.Count == 0) {
        //            cg.ParticipantRole_CentreGroup.Remove(requestorRole);
        //        }


        //    }

        //    if (isPgExcludedDefaultRequestors) {
        //        //all of the implicite requestors were deleted , so it it neccessary to exclude default requestors
        //        foreach (var requestorRole in defaultRequestors) {
        //            var excludeRequestor = (from excludeRequestorDb in origPurchaseGroup.ParticipantsExcludeRequestor
        //                                    where excludeRequestorDb.id == requestorRole.participant_id
        //                                    select excludeRequestorDb).FirstOrDefault();

        //            if (excludeRequestor == null) {
        //                var participant = m_dbContext.Participants.Find(requestorRole.participant_id);
        //                if (participant.active == false) {
        //                    continue;
        //                }

        //                origPurchaseGroup.ParticipantsExcludeRequestor.Add(participant);

        //            }
        //        }
        //    }


        //}

        //private void DeleteRequestorInCentre(
        //    ParticipantRole_CentreGroup requestorRole,
        //    Centre_Group cg,
        //    PurchaseGroupExtended pgModif,
        //    int centreId) {

        //    //bool isPgExcluded = false;

        //    //is requestor for other centre
        //    var singleRequestorsOtherCentres = (from modifReq in pgModif.RequestorExtended
        //                                        where modifReq.participant_id == requestorRole.participant_id &&
        //                                        modifReq.centre_id != centreId
        //                                        select modifReq).ToList();

        //    if (singleRequestorsOtherCentres == null || singleRequestorsOtherCentres.Count == 0) {
        //        //only one centre user
        //        var ecxludeReq = (from reqRoleDb in requestorRole.Participants.PurchaseGroup_ExcludeRequestor
        //                          where reqRoleDb.id == pgModif.id 
        //                          select reqRoleDb).FirstOrDefault();
        //        if (ecxludeReq == null) {
        //            var pg = m_dbContext.Purchase_Group.Find(pgModif.id);

        //            requestorRole.Participants.PurchaseGroup_ExcludeRequestor.Add(pg);
        //        }

        //        //isPgExcluded = true;
        //    } else {
        //        //more centres
        //        SetOtherReqCentres(requestorRole, singleRequestorsOtherCentres, centreId, pgModif.id);

        //    }

        //    DeleteRequestorFromPgCentre(requestorRole, pgModif.id, centreId);

        //    //return isPgExcluded;

        //}

        //private bool DeleteImpliciteRequestorInCentre(
        //    ParticipantRole_CentreGroup requestorRole,
        //    Centre_Group cg,
        //    PurchaseGroupExtended pgModif,
        //    int centreId,
        //    Hashtable htCentresInUse) {

        //    bool isPgExcluded = false;

        //    //is implicite?
        //    var impliciteReq = (from reqRoleDb in requestorRole.Participants.PurchaseGroup_ImplicitRequestor
        //                        where reqRoleDb.purchase_category_id == pgModif.id &&
        //                        reqRoleDb.Participants.id == requestorRole.participant_id
        //                        select reqRoleDb).FirstOrDefault();

        //    if (impliciteReq == null) {
        //        return false;
        //    }

        //    var singleRequestorsOtherCentres = (from modifReq in pgModif.RequestorExtended
        //                                        where modifReq.participant_id == requestorRole.participant_id &&
        //                                        modifReq.centre_id != centreId
        //                                        select modifReq).ToList();

        //    if (singleRequestorsOtherCentres == null || singleRequestorsOtherCentres.Count == 0) {
        //        //delete implicite requestor , used only in 1 centre
        //        if (impliciteReq.requestor_allowed_for_other_group == true) {

        //        } else { 

        //            var implRequestorsOtherPgs = (from implReq in requestorRole.Participants.PurchaseGroup_ImplicitRequestor
        //                                          join pg in cg.Purchase_Group on implReq.purchase_category_id equals pg.id
        //                                          where implReq.requestor_id == requestorRole.participant_id &&
        //                                          implReq.purchase_category_id != pgModif.id
        //                                          select implReq).FirstOrDefault();

        //            if (implRequestorsOtherPgs == null) {
        //                //requestor is not implicite in other pg within the cg
        //                var delReqCentres = (from reqCentre in requestorRole.Participants.Requestor_Centre
        //                                     where reqCentre.id == centreId
        //                                     select reqCentre).FirstOrDefault();

        //                if (delReqCentres != null) {
        //                    requestorRole.Participants.Requestor_Centre.Remove(delReqCentres);
        //                }
        //                htCentresInUse.Remove(centreId);

        //                //if all of the implicit requestors are deleted , default requestors must be deleted
        //                var imliciteRequestors = (from modifReq in pgModif.RequestorExtended
        //                                                    where modifReq.centre_id != centreId
        //                                                    select modifReq).FirstOrDefault();
        //                if(imliciteRequestors == null) {
        //                    isPgExcluded = true;
        //                }
        //            }
        //        }

        //        requestorRole.Participants.PurchaseGroup_ImplicitRequestor.Remove(impliciteReq);

        //    } else {
        //        //requestor is used in several cost centres
        //        //var reqPgCentre = (from reqPgCentreDb in requestorRole.Participants.Participant_PurchaseGroup_Centre
        //        //                   where reqPgCentreDb.purchase_group_id == pgModif.id &&
        //        //                   reqPgCentreDb.centre_id != centreId
        //        //                   select reqPgCentreDb).FirstOrDefault();
        //        //if (reqPgCentre != null) {
        //        //    requestorRole.Participants.Participant_PurchaseGroup_Centre.Remove(reqPgCentre);
        //        //}

        //        SetOtherReqCentres(requestorRole, singleRequestorsOtherCentres, centreId, pgModif.id);
        //    }

        //    DeleteRequestorFromPgCentre(requestorRole, pgModif.id, centreId);

        //    return isPgExcluded;
        //}

        //private void SetOtherReqCentres(
        //    ParticipantRole_CentreGroup requestorRole, 
        //    List<RequestorExtended> singleRequestorsOtherCentres, 
        //    int centreId,
        //    int pgId) {

        //    Hashtable htTmpOtherCentre = new Hashtable();
        //    foreach (var singleRequestorsOtherCentre in singleRequestorsOtherCentres) {
        //        //if (singleRequestorsOtherCentre.centre_id == centreId) {
        //        //    continue;
        //        //}

        //        if (!htTmpOtherCentre.ContainsKey(singleRequestorsOtherCentre.centre_id)) {
        //            var reqPgCentre = (from reqPgCentreDb in requestorRole.Participants.Participant_PurchaseGroup_Centre
        //                               where reqPgCentreDb.purchase_group_id == pgId &&
        //                               reqPgCentreDb.centre_id == singleRequestorsOtherCentre.centre_id
        //                               select reqPgCentreDb).FirstOrDefault();
        //            if (reqPgCentre == null) {
        //                Participant_PurchaseGroup_Centre ppc = new Participant_PurchaseGroup_Centre();
        //                ppc.participant_id = requestorRole.participant_id;
        //                ppc.purchase_group_id = pgId;
        //                ppc.centre_id = singleRequestorsOtherCentre.centre_id;

        //                requestorRole.Participants.Participant_PurchaseGroup_Centre.Add(ppc);
        //                m_dbContext.Entry(ppc).State = EntityState.Added;
        //            }
        //            htTmpOtherCentre.Add(singleRequestorsOtherCentre.centre_id, null);
        //        }
        //    }
        //}

        //private void DeleteRequestorFromPgCentre(ParticipantRole_CentreGroup requestorRole, int pgId, int centreId) {
        //    var reqPgCentre = (from reqPgCentreDb in requestorRole.Participants.Participant_PurchaseGroup_Centre
        //                       where reqPgCentreDb.participant_id == requestorRole.participant_id &&
        //                       reqPgCentreDb.purchase_group_id == pgId &&
        //                       reqPgCentreDb.centre_id == centreId
        //                       select reqPgCentreDb).FirstOrDefault();

        //    if (reqPgCentre != null) {
        //        requestorRole.Participants.Participant_PurchaseGroup_Centre.Remove(reqPgCentre);
        //    }
        //}

        private bool DeleteDisabledUser(Centre_Group cg, ParticipantRole_CentreGroup requestorRole) {
            if (requestorRole.Participants.active == false) {
                //cg.ParticipantRole_CentreGroup.Remove(requestorRole);

                //if (requestorRole.role_id == (int)UserRole.Requestor) {
                //    DeleteRequestorCompletely(cg, requestorRole);
                //}

                //if (requestorRole.role_id == (int)UserRole.Orderer) {
                //    DeleteOrdererCompletely(cg, requestorRole);
                //}

                new UserRepository().DeleteUserFromApprovalMatrix(requestorRole.participant_id);

                return true;
            }

            return false;
        }


        //private void DeleteOrdererCompletely(Centre_Group cg, ParticipantRole_CentreGroup requestorRole) {

        //    cg.ParticipantRole_CentreGroup.Remove(requestorRole);


        //    foreach (var pg in cg.Purchase_Group) {
        //        //implicite
        //        for (int j = pg.PurchaseGroup_ImplicitRequestor.Count - 1; j >= 0; j--) {
        //            pg.PurchaseGroup_ImplicitOrderer.Remove(pg.PurchaseGroup_ImplicitOrderer.ElementAt(j));

        //        }

        //    }

        //    //exclude
        //    for (int j = requestorRole.Participants.PurchaseGroup_ExcludeRequestor.Count - 1; j >= 0; j--) {
        //        requestorRole.Participants.PurchaseGroup_ExcludeOrderer.Remove(requestorRole.Participants.PurchaseGroup_ExcludeOrderer.ElementAt(j));

        //    }



        //}

        //private void DeleteRequestorCompletely(Centre_Group cg, ParticipantRole_CentreGroup requestorRole) {

        //    cg.ParticipantRole_CentreGroup.Remove(requestorRole);

        //    //requestor centre
        //    foreach (var centre in cg.Centre) {
        //        for (int i = centre.Requestor_Centre.Count - 1; i >= 0; i--) {
        //            if (centre.Requestor_Centre.ElementAt(i).id == requestorRole.participant_id) {
        //                centre.Requestor_Centre.Remove(centre.Requestor_Centre.ElementAt(i));
        //            }

        //        }
        //    }
        //    //for (int j = requestorRole.Participants.Requestor_Centre.Count - 1; j >= 0; j--) {
        //    //    requestorRole.Participants.Requestor_Centre.Remove(requestorRole.Participants.Requestor_Centre.ElementAt(j));
        //    //}


        //    foreach (var pg in cg.Purchase_Group) {
        //        //implicite
        //        for (int j = pg.PurchaseGroup_ImplicitRequestor.Count - 1; j >= 0; j--) {
        //            pg.PurchaseGroup_ImplicitRequestor.Remove(pg.PurchaseGroup_ImplicitRequestor.ElementAt(j));
        //            //m_dbContext.Entry(pg.PurchaseGroup_ImplicitRequestor.ElementAt(j)).State = EntityState.Deleted;
        //        }

        //    }

        //    //exclude
        //    for (int j = requestorRole.Participants.PurchaseGroup_ExcludeRequestor.Count - 1; j >= 0; j--) {
        //        requestorRole.Participants.PurchaseGroup_ExcludeRequestor.Remove(requestorRole.Participants.PurchaseGroup_ExcludeRequestor.ElementAt(j));
        //        //m_dbContext.Entry(requestorRole.Participants.PurchaseGroup_ExcludeRequestor.ElementAt(j)).State = EntityState.Deleted;
        //    }

        //    //pg requestor
        //    for (int j = requestorRole.Participants.PurchaseGroup_Requestor.Count - 1; j >= 0; j--) {
        //        requestorRole.Participants.PurchaseGroup_Requestor.Remove(requestorRole.Participants.PurchaseGroup_Requestor.ElementAt(j));

        //    }

        //    ////pg orderer
        //    //for (int j = requestorRole.Participants.PurchaseGroup_Orderer.Count - 1; j >= 0; j--) {
        //    //    requestorRole.Participants.PurchaseGroup_Orderer.Remove(requestorRole.Participants.PurchaseGroup_Orderer.ElementAt(j));

        //    //}

        //    ////requestor pg centre
        //    //for (int j = requestorRole.Participants.Participant_PurchaseGroup_Centre.Count - 1; j >= 0; j--) {
        //    //    requestorRole.Participants.Participant_PurchaseGroup_Centre.Remove(requestorRole.Participants.Participant_PurchaseGroup_Centre.ElementAt(j));
        //    //}


        //}

        //private void DeleteRequestorFromCentre(PurchaseGroupExtended pgModif, ParticipantRole_CentreGroup requestorRole, Hashtable htCentresInUse) {
        //    var delAllReqs = (from delAllRequestors in pgModif.DeleteRequestorsAllCategories
        //                      where delAllRequestors.participant_id == requestorRole.participant_id
        //                      select delAllRequestors).ToList();
        //    //bool isDeleted = false;
        //    foreach (var delAllReq in delAllReqs) {
        //        //isDeleted = true;
        //        if (htCentresInUse.ContainsKey(delAllReq.centre_id)) {
        //            htCentresInUse.Remove(delAllReq.centre_id);
        //            //delete requestor centre
        //            var delReqCentres = (from reqCentre in requestorRole.Participants.Requestor_Centre
        //                                 where reqCentre.id == delAllReq.centre_id
        //                                 select reqCentre).FirstOrDefault();

        //            if (delReqCentres != null) {
        //                requestorRole.Participants.Requestor_Centre.Remove(delReqCentres);
        //            }

        //            //replace default centre_id
        //            if (requestorRole.Participants.centre_id == delAllReq.centre_id && htCentresInUse.Count > 0) {
        //                IDictionaryEnumerator iEnum = htCentresInUse.GetEnumerator();
        //                iEnum.MoveNext();
        //                requestorRole.Participants.centre_id = (int)iEnum.Key;
        //                m_dbContext.Entry(requestorRole.Participants).State = EntityState.Modified;
        //            }

        //            //requestor pg centre
        //            var singleRequestorsOtherCentres = (from modifReq in pgModif.RequestorExtended
        //                                                where modifReq.participant_id == requestorRole.participant_id &&
        //                                                modifReq.centre_id != delAllReq.centre_id
        //                                                select modifReq).ToList();

        //            if (singleRequestorsOtherCentres == null || singleRequestorsOtherCentres.Count == 0) {
        //                //is requestor for more centres
        //                var delReqPgCentres = (from reqPgCentre in requestorRole.Participants.Participant_PurchaseGroup_Centre
        //                                       where reqPgCentre.centre_id == delAllReq.centre_id
        //                                       select reqPgCentre).ToList();
        //                for (int j = delReqPgCentres.Count - 1; j >= 0; j--) {
        //                    delReqPgCentres.RemoveAt(j);
        //                }
        //            } else {
        //                SetOtherReqCentres(requestorRole, singleRequestorsOtherCentres, delAllReq.centre_id, pgModif.id);
        //            }
        //        }
        //    }
        //}

        private void AddMissingParticipantRole(int cgId, UserRole userRole, int participantId) {
            //InternalRequestEntities dbContext = new InternalRequestEntities();
            var roleCentreGroup = (from prcg in m_dbContext.ParticipantRole_CentreGroup
                                   where prcg.role_id == (int)userRole &&
                                   prcg.centre_group_id == cgId &&
                                   prcg.participant_id == participantId
                                   select prcg).ToList();

            if (roleCentreGroup == null || roleCentreGroup.Count == 0) {
                ParticipantRole_CentreGroup pgcg = new ParticipantRole_CentreGroup();
                pgcg.participant_id = participantId;
                pgcg.centre_group_id = cgId;
                pgcg.role_id = (int)userRole;


                roleCentreGroup.Add(pgcg);
                m_dbContext.Entry(pgcg).State = EntityState.Added;

                SaveChanges();
            }

            //SaveChanges();
        }

        private void DeletePurchaseGroupLimits(PurchaseGroupExtended pgModif, Purchase_Group pgOrig) {
            for (int i = pgOrig.Purchase_Group_Limit.Count - 1; i >= 0; i--) {
                Purchase_Group_Limit origLimit = pgOrig.Purchase_Group_Limit.ElementAt(i);
                var modifLimit = (from modifPgLimit in pgModif.purchase_group_limit
                                  where modifPgLimit.limit_id == origLimit.limit_id &&
                                  modifPgLimit.is_visible == true
                                  select modifPgLimit).FirstOrDefault();
                if (modifLimit == null) {
                    for (int j = origLimit.Manager_Role.Count - 1; j >= 0; j--) {
                        //modifLimit.Manager_Role.Remove(pgOrig.Purchase_Group_Limit.ElementAt(i).Manager_Role.ElementAt(j));
                        //m_dbContext.Entry(origLimit.Manager_Role.ElementAt(j)).State = EntityState.Deleted;
                        //origLimit.Manager_Role.Remove(origLimit.Manager_Role.ElementAt(j));

                    }

                    //origLimit.Manager_Role.Clear();

                    //pgOrig.Purchase_Group_Limit.Remove(origLimit);
                    m_dbContext.Purchase_Group_Limit.Remove(origLimit);

                }
            }
        }

        private void NewUpdatePurchaseGroupLimits(PurchaseGroupExtended pgModif, Purchase_Group pgOrig, int currentUserId) {
            int lastLimitId = m_dbContext.Purchase_Group_Limit.Max(x => x.limit_id);
            int newLimitId = ++lastLimitId;
            Hashtable htAppManLevel = new Hashtable();
            foreach (var modifPgLimit in pgModif.purchase_group_limit) {
                if (!modifPgLimit.is_visible) {
                    continue;
                }

                if (modifPgLimit.is_bottom_unlimited) {
                    modifPgLimit.limit_bottom = null;
                    //modifPgLimit.limit_bottom_text = null;
                }

                if (modifPgLimit.is_top_unlimited) {
                    modifPgLimit.limit_top = null;
                    //modifPgLimit.limit_top_text = null;
                }

                var origLimit = (from origPgLimit in pgOrig.Purchase_Group_Limit
                                 where origPgLimit.limit_id == modifPgLimit.limit_id
                                 select origPgLimit).FirstOrDefault();
                if (origLimit == null) {
                    Purchase_Group_Limit newLimit = new Purchase_Group_Limit();
                    //newLimit.Participants = new Participants();

                    SetValues(modifPgLimit, newLimit);

                    newLimit.limit_id = newLimitId;
                    newLimit.approve_level_id = modifPgLimit.app_level_id;
                    newLimit.active = true;
                    newLimit.modify_date = DateTime.Now;
                    newLimit.modify_user = currentUserId;
                    newLimitId++;

                    origLimit = newLimit;

                    pgOrig.Purchase_Group_Limit.Add(newLimit);

                } else {
                    bool isModified = false;
                    if (ConvertData.ToDecimal(modifPgLimit.limit_bottom) != origLimit.limit_bottom) {
                        isModified = true;
                    }
                    if (ConvertData.ToDecimal(modifPgLimit.limit_top) != origLimit.limit_top) {
                        isModified = true;
                    }
                    if (modifPgLimit.is_limit_bottom_multipl != origLimit.is_limit_bottom_multipl) {
                        isModified = true;
                    }
                    if (modifPgLimit.is_limit_top_multipl != origLimit.is_limit_top_multipl) {
                        isModified = true;
                    }
                    if (modifPgLimit.app_level_id != origLimit.approve_level_id) {
                        isModified = true;
                    }
                    SetValues(modifPgLimit, origLimit);
                    origLimit.approve_level_id = modifPgLimit.app_level_id;
                    if (isModified) {
                        origLimit.modify_date = DateTime.Now;
                        origLimit.modify_user = currentUserId;
                    }
                }

                UpdateManagerRole(modifPgLimit, origLimit, pgOrig.Centre_Group.ElementAt(0).id, currentUserId, htAppManLevel);
            }
        }

        private void UpdateManagerRole(PurchaseGroupLimitExtended modifPgLimit, Purchase_Group_Limit origPgLimit, int cgId, int currentUserId, Hashtable htAppManLevel) {
            //Delete
            //List<Manager_Role> delManRoles = new List<Manager_Role>();
            if (origPgLimit != null) {
                for (int i = origPgLimit.Manager_Role.Count - 1; i >= 0; i--) {

                    if (origPgLimit.Manager_Role.ElementAt(i).centre_group_id != cgId) {
                        //it was possible thet 2 app man was assigned to the same limit and each was from differnt cg
                        //e.g.
                        /*
                         45	1131	2	1403	NULL	True	69	2016-07-26 13:58:51.843
                        266	1131	2	1403	NULL	True	69	2016-02-06 00:00:00.000
                         */
                        origPgLimit.Manager_Role.Remove(origPgLimit.Manager_Role.ElementAt(i));
                        origPgLimit.modify_date = DateTime.Now;
                        origPgLimit.modify_user = currentUserId;
                    } else {

                        var modifManRole = (from modifRole in modifPgLimit.manager_role
                                            where modifRole.participant_id == origPgLimit.Manager_Role.ElementAt(i).participant_id &&
                                            modifRole.approve_level_id == origPgLimit.Manager_Role.ElementAt(i).approve_level_id
                                            select modifRole).FirstOrDefault();

                        if (modifManRole == null) {
                            origPgLimit.Manager_Role.Remove(origPgLimit.Manager_Role.ElementAt(i));
                            origPgLimit.modify_date = DateTime.Now;
                            origPgLimit.modify_user = currentUserId;
                            //delManRoles.Add(origManRole);
                        }
                    }
                }
            }

            //New
            foreach (ManagerRoleExtended modifManRole in modifPgLimit.manager_role) {
                Manager_Role origManRole = null;
                if (origPgLimit != null) {
                    origManRole = (from origRole in origPgLimit.Manager_Role
                                   where origRole.participant_id == modifManRole.participant_id &&
                                   origRole.centre_group_id == cgId
                                   select origRole).FirstOrDefault();
                }

                if (origManRole == null) {
                    Manager_Role newManRole = new Manager_Role();
                    SetValues(modifManRole, newManRole);
                    newManRole.purchase_group_limit_id = origPgLimit.limit_id;
                    newManRole.centre_group_id = cgId;
                    newManRole.active = true;
                    newManRole.modify_date = DateTime.Now;
                    newManRole.modify_user = currentUserId;

                    origPgLimit.Manager_Role.Add(newManRole);


                    AddMissingParticipantRole(cgId, UserRole.ApprovalManager, modifManRole.participant.id);

                }


            }
        }

        //private void NewOrdererRole(PurchaseGroupExtended pgModif, Centre_Group cg) {
        //    Hashtable htDefaultOrderers = GetDefaultOrderers(cg);
        //    Purchase_Group pgOrig = (from pgDb in cg.Purchase_Group
        //                             where pgDb.id == pgModif.id
        //                             select pgDb).FirstOrDefault();

        //    foreach (var modifOrderer in pgModif.OrdererExtended) {
        //        var ordererRole = (from reqOrderRole in cg.ParticipantRole_CentreGroup
        //                           where reqOrderRole.role_id == (int)UserRole.Orderer &&
        //                           reqOrderRole.participant_id == modifOrderer.participant_id
        //                           select reqOrderRole).FirstOrDefault();
        //        if (ordererRole == null) {
        //            //new orderer in cg
        //            ParticipantRole_CentreGroup pgcg = new ParticipantRole_CentreGroup();
        //            pgcg.centre_group_id = cg.id;
        //            pgcg.participant_id = modifOrderer.participant_id;
        //            pgcg.role_id = (int)UserRole.Orderer;
        //            cg.ParticipantRole_CentreGroup.Add(pgcg);
        //        }

        //        if (modifOrderer.is_all) {
        //            foreach (var pg in cg.Purchase_Group) {
        //                if (pg.PurchaseGroup_ImplicitOrderer.Count > 0) {
        //                    var implReqAllPg = (from implOrdDb in pg.PurchaseGroup_ImplicitOrderer
        //                                        where implOrdDb.orderer_id == modifOrderer.participant_id
        //                                        select implOrdDb).FirstOrDefault();
        //                    if (implReqAllPg == null) {
        //                        PurchaseGroup_ImplicitOrderer pgio = new PurchaseGroup_ImplicitOrderer();
        //                        pgio.orderer_id = modifOrderer.participant_id;
        //                        pgio.purchase_category_id = pgOrig.id;
        //                        pgio.orderer_allowed_for_other_group = true;
        //                        pgOrig.PurchaseGroup_ImplicitOrderer.Add(pgio);
        //                    } else {
        //                        implReqAllPg.orderer_allowed_for_other_group = true;
        //                    }
        //                }
        //            }
        //        } else {
        //            var implicitOrderer = (from implOrdDb in pgOrig.PurchaseGroup_ImplicitOrderer
        //                                   where implOrdDb.orderer_id == modifOrderer.participant_id
        //                                   select implOrdDb).FirstOrDefault();

        //            if (implicitOrderer == null) {
        //                bool isAllImplicite = false;
        //                foreach (var pg in cg.Purchase_Group) {
        //                    var implReqAllPg = (from implOrdDb in pg.PurchaseGroup_ImplicitOrderer
        //                                        where implOrdDb.orderer_id == modifOrderer.participant_id &&
        //                                        implOrdDb.orderer_allowed_for_other_group == true
        //                                        select implOrdDb).FirstOrDefault();
        //                    if (implReqAllPg != null) {
        //                        isAllImplicite = true;
        //                        break;
        //                    }
        //                }

        //                PurchaseGroup_ImplicitOrderer pgio = new PurchaseGroup_ImplicitOrderer();
        //                pgio.orderer_id = modifOrderer.participant_id;
        //                pgio.purchase_category_id = pgOrig.id;
        //                pgio.orderer_allowed_for_other_group = isAllImplicite;
        //                pgOrig.PurchaseGroup_ImplicitOrderer.Add(pgio);
        //            }
        //        }
        //    }

        //}

        //private void NewRequestorsRole(PurchaseGroupExtended pgModif, Centre_Group cg) {
        //    Purchase_Group pgOrig = (from pgDb in cg.Purchase_Group
        //                             where pgDb.id == pgModif.id
        //                             select pgDb).FirstOrDefault();

        //    Hashtable htDefaultRequestors = GetDefaultRequestors(cg);
        //    Hashtable htProcessedRequestors = new Hashtable();

        //    //New
        //    bool isAllForAll = true;
        //    foreach (var modifRequestor in pgModif.RequestorExtended) {
        //        if (htProcessedRequestors.ContainsKey(modifRequestor.participant_id)) {
        //            continue;
        //        }

        //        var requestorRole = (from reqOrderRole in cg.ParticipantRole_CentreGroup
        //                             where reqOrderRole.role_id == (int)UserRole.Requestor &&
        //                             reqOrderRole.participant_id == modifRequestor.participant_id
        //                             select reqOrderRole).FirstOrDefault();

        //        if (requestorRole == null) {
        //            ParticipantRole_CentreGroup newPrCg = new ParticipantRole_CentreGroup();
        //            newPrCg.centre_group_id = cg.id;
        //            newPrCg.participant_id = modifRequestor.participant_id;
        //            newPrCg.role_id = (int)UserRole.Requestor;

        //            cg.ParticipantRole_CentreGroup.Add(newPrCg);
        //            m_dbContext.Entry(newPrCg).State = EntityState.Added;


        //        }

        //        //requestor centre
        //        var participant = (from partic in m_dbContext.Participants
        //                           where partic.id == modifRequestor.participant_id
        //                           select partic).FirstOrDefault();
        //        if (participant.centre_id != modifRequestor.centre_id) {
        //            var requestorCentre = (from requestorCentreDb in participant.Requestor_Centre
        //                                   where requestorCentreDb.id == modifRequestor.centre_id
        //                                   select requestorCentreDb).FirstOrDefault();
        //            if (requestorCentre == null) {
        //                var partCentre = (from centreDb in m_dbContext.Centre
        //                                  where centreDb.id == modifRequestor.centre_id
        //                                  select centreDb).FirstOrDefault();
        //                participant.Requestor_Centre.Add(partCentre);
        //                //m_dbContext.Entry(partCentre).State = EntityState.Added;
        //            }
        //        }

        //        if (modifRequestor.is_all) {
        //            //remove from exclude requestors
        //            var excludReq = (from particExc in pgOrig.ParticipantsExcludeRequestor
        //                             where particExc.id == modifRequestor.participant_id
        //                             select particExc).ToList();
        //            for (int i = excludReq.Count - 1; i >= 0; i--) {
        //                pgOrig.ParticipantsExcludeRequestor.Remove(excludReq.ElementAt(i));
        //            }

        //            foreach (var pg in cg.Purchase_Group) {
        //                AddRequestorToPg(pg, modifRequestor.participant_id, htDefaultRequestors, true);
        //                SetNewRequestorCgPgRequestor(cg, pgModif.RequestorExtended, modifRequestor.participant_id, pg.id);
        //            }

        //        } else {
        //            isAllForAll = false;
        //            AddRequestorToPg(pgOrig, modifRequestor.participant_id, htDefaultRequestors, false);
        //            SetNewRequestorCgPgRequestor(cg, pgModif.RequestorExtended, modifRequestor.participant_id, pgModif.id);
        //        }

        //        htProcessedRequestors.Add(modifRequestor.participant_id, null);
        //    }


        //    //delete implicite req if there are all requestor  for all

        //    if (isAllForAll) {
        //        foreach (var pg in cg.Purchase_Group) {
        //            for (int i = pg.PurchaseGroup_ImplicitRequestor.Count - 1; i >= 0; i--) {
        //                pg.PurchaseGroup_ImplicitRequestor.Remove(pg.PurchaseGroup_ImplicitRequestor.ElementAt(i));
        //            }
        //        }
        //    }
        //}

        //private void SetNewRequestorCgPgRequestor(Centre_Group cg, ICollection<RequestorExtended> modfiRequestors, int modifRequestorId, int pgId) {
        //    //Set centres
        //    var reqCentres = (from modifReqs in modfiRequestors
        //                      where modifReqs.participant_id == modifRequestorId
        //                      select modifReqs).ToList();

        //    var ppc = (from ppcDb in m_dbContext.Participant_PurchaseGroup_Centre
        //               where ppcDb.participant_id == modifRequestorId &&
        //               ppcDb.purchase_group_id == pgId
        //               select ppcDb).ToList();

        //    if (reqCentres.Count == cg.Centre.Count) {
        //        //all centres
        //        if (ppc != null) {
        //            for (int i = ppc.Count - 1; i >= 0; i--) {
        //                m_dbContext.Participant_PurchaseGroup_Centre.Remove(m_dbContext.Participant_PurchaseGroup_Centre.ElementAt(i));
        //            }
        //        }
        //    } else {
        //        //deleting is provided before in DeleteRequestors, it is not neccessary to deal with deleting here 
        //        foreach (var reqCentre in reqCentres) {
        //            var ppcCente = (from ppcDb in ppc
        //                       where ppcDb.participant_id == modifRequestorId &&
        //                       ppcDb.purchase_group_id == pgId &&
        //                       ppcDb.centre_id == reqCentre.centre_id
        //                       select ppcDb).FirstOrDefault();
        //            if (ppcCente != null) {
        //                Participant_PurchaseGroup_Centre ppcNew = new Participant_PurchaseGroup_Centre();
        //                ppcNew.participant_id = modifRequestorId;
        //                ppcNew.purchase_group_id = pgId;
        //                ppcNew.centre_id = reqCentre.centre_id;
        //                m_dbContext.Participant_PurchaseGroup_Centre.Add(ppcNew);
        //            }
        //        }
        //    }
        //}

        private void AddRequestorToPg(
            Purchase_Group pgOrig,
            int requestorId,
            Hashtable htDefaultRequestors,
            bool isAllRequestor) {

            if (pgOrig.PurchaseGroup_ImplicitRequestor.Count > 0) {
                var implReq = (from implReqDb in pgOrig.PurchaseGroup_ImplicitRequestor
                               where implReqDb.requestor_id == requestorId &&
                               implReqDb.purchase_category_id == pgOrig.id
                               select implReqDb).FirstOrDefault();

                if (implReq == null && !isAllRequestor) {
                    PurchaseGroup_ImplicitRequestor pgir = new PurchaseGroup_ImplicitRequestor();
                    pgir.purchase_category_id = pgOrig.id;
                    pgir.requestor_id = requestorId;
                    pgir.requestor_allowed_for_other_group = false;
                    pgOrig.PurchaseGroup_ImplicitRequestor.Add(pgir);
                }

                if (implReq != null && isAllRequestor) {
                    implReq.requestor_allowed_for_other_group = true;
                }
            } else {
                if (htDefaultRequestors.ContainsKey(requestorId) || isAllRequestor) {
                    return;
                }

                IDictionaryEnumerator iEnum = htDefaultRequestors.GetEnumerator();
                while (iEnum.MoveNext()) {
                    int iDefReq = (int)iEnum.Key;
                    var implReqOther = (from implReqDb in pgOrig.PurchaseGroup_ImplicitRequestor
                                        where implReqDb.requestor_id == iDefReq &&
                                        implReqDb.purchase_category_id == pgOrig.id
                                        select implReqDb).FirstOrDefault();

                    if (implReqOther == null) {

                        PurchaseGroup_ImplicitRequestor pgir = new PurchaseGroup_ImplicitRequestor();
                        pgir.purchase_category_id = pgOrig.id;
                        pgir.requestor_id = iDefReq;
                        pgir.requestor_allowed_for_other_group = true;
                        pgOrig.PurchaseGroup_ImplicitRequestor.Add(pgir);

                        break;

                    }
                }

                var implReq = (from implReqDb in pgOrig.PurchaseGroup_ImplicitRequestor
                               where implReqDb.requestor_id == requestorId &&
                               implReqDb.purchase_category_id == pgOrig.id
                               select implReqDb).FirstOrDefault();

                if (implReq == null) {
                    PurchaseGroup_ImplicitRequestor pgirNew = new PurchaseGroup_ImplicitRequestor();
                    pgirNew.purchase_category_id = pgOrig.id;
                    pgirNew.requestor_id = requestorId;
                    pgirNew.requestor_allowed_for_other_group = false;
                    pgOrig.PurchaseGroup_ImplicitRequestor.Add(pgirNew);
                }
            }



        }

        private Hashtable GetDefaultRequestors(Centre_Group cg) {
            Hashtable htDefaultRequestors = new Hashtable();

            var reqPgCg = (from pgCgDb in cg.ParticipantRole_CentreGroup
                           where pgCgDb.role_id == (int)UserRole.Requestor
                           select pgCgDb).ToList();

            if (reqPgCg != null) {
                foreach (var prCg in reqPgCg) {
                    if (!htDefaultRequestors.Contains(prCg.participant_id)) {
                        htDefaultRequestors.Add(prCg.participant_id, null);
                    }
                }
            }

            foreach (var pg in cg.Purchase_Group) {
                if (htDefaultRequestors.Count == 0) {
                    break;
                }
                foreach (var impliciteReq in pg.PurchaseGroup_ImplicitRequestor) {
                    if (htDefaultRequestors.ContainsKey(impliciteReq.requestor_id) && impliciteReq.requestor_allowed_for_other_group != true) {
                        htDefaultRequestors.Remove(impliciteReq.requestor_id);
                    }
                }
            }

            return htDefaultRequestors;
        }

        private Hashtable GetDefaultOrderers(Centre_Group cg) {
            Hashtable htDefaultOrderers = new Hashtable();

            var reqPgCg = (from pgCgDb in cg.ParticipantRole_CentreGroup
                           where pgCgDb.role_id == (int)UserRole.Orderer
                           select pgCgDb).ToList();

            if (reqPgCg != null) {
                foreach (var prCg in reqPgCg) {
                    if (!htDefaultOrderers.Contains(prCg.participant_id)) {
                        htDefaultOrderers.Add(prCg.participant_id, null);
                    }
                }
            }

            foreach (var pg in cg.Purchase_Group) {
                if (htDefaultOrderers.Count == 0) {
                    break;
                }
                foreach (var impliciteReq in pg.PurchaseGroup_ImplicitOrderer) {
                    if (htDefaultOrderers.ContainsKey(impliciteReq.orderer_id) && impliciteReq.orderer_allowed_for_other_group != true) {
                        htDefaultOrderers.Remove(impliciteReq.orderer_id);
                    }
                }
            }

            return htDefaultOrderers;
        }

        private void DeleteParticipantAppManRole(Centre_Group cg) {
            //Centre_Group cg = GetCentreGroupAllDataById(cgId);

            //Delete App Manager Role
            for (int i = cg.ParticipantRole_CentreGroup.Count - 1; i > 0; i--) {
                if (cg.ParticipantRole_CentreGroup.ElementAt(i).Participants != null && cg.ParticipantRole_CentreGroup.ElementAt(i).Participants.active == false) {
                    //pg cg
                    cg.ParticipantRole_CentreGroup.Remove(cg.ParticipantRole_CentreGroup.ElementAt(i));
                } else if (cg.ParticipantRole_CentreGroup.ElementAt(i).role_id == (int)UserRole.ApprovalManager) {
                    //Approve Manager
                    int participantId = cg.ParticipantRole_CentreGroup.ElementAt(i).participant_id;
                    var managerRole = (from mr in m_dbContext.Manager_Role
                                       where mr.centre_group_id == cg.id &&
                                       mr.participant_id == participantId
                                       select mr).FirstOrDefault();
                    if (managerRole == null) {
                        cg.ParticipantRole_CentreGroup.Remove(cg.ParticipantRole_CentreGroup.ElementAt(i));
                    }
                }


            }

            ////Delete excluded requestors for  all pgs
            //DeleteExcludedParticipantComplete(cg);



            SaveChanges();
        }

        private void DeleteImpliciteRequestor(Purchase_Group pg) {

            for (int j = pg.PurchaseGroup_ImplicitRequestor.Count - 1; j >= 0; j--) {
                pg.PurchaseGroup_ImplicitRequestor.Remove(pg.PurchaseGroup_ImplicitRequestor.ElementAt(j));

            }


        }

        private void DeleteExcludeRequestor(Purchase_Group pg) {

            for (int j = pg.ParticipantsExcludeRequestor.Count - 1; j >= 0; j--) {
                pg.ParticipantsExcludeRequestor.Remove(pg.ParticipantsExcludeRequestor.ElementAt(j));

            }


        }

        private void DeleteImpliciteOrderer(Purchase_Group pg) {

            for (int j = pg.PurchaseGroup_ImplicitOrderer.Count - 1; j >= 0; j--) {
                pg.PurchaseGroup_ImplicitOrderer.Remove(pg.PurchaseGroup_ImplicitOrderer.ElementAt(j));

            }


        }

        private void DeleteExcludeOrderer(Purchase_Group pg) {

            for (int j = pg.ParticipantsExcludeOrderers.Count - 1; j >= 0; j--) {
                pg.ParticipantsExcludeOrderers.Remove(pg.ParticipantsExcludeOrderers.ElementAt(j));

            }


        }



        //private void DeleteExcludedParticipantComplete(Centre_Group cg) {
        //    Hashtable htDelReq = null;
        //    foreach (var pg in cg.Purchase_Group) {
        //        if (htDelReq == null) {
        //            htDelReq = new Hashtable();
        //            foreach (var excReq in pg.ParticipantsExcludeRequestor) {
        //                if (!htDelReq.ContainsKey(excReq.id)) {
        //                    htDelReq.Add(excReq.id, null);
        //                }
        //            }

        //        } else {
        //            if (htDelReq.Count == 0) {
        //                break;
        //            } else {
        //                Hashtable tmpHt = (Hashtable)htDelReq.Clone();
        //                IDictionaryEnumerator iEnum = tmpHt.GetEnumerator();
        //                while (iEnum.MoveNext()) {
        //                    var exclRole = (from exReqDb in pg.ParticipantsExcludeRequestor
        //                                    where exReqDb.id == (int)iEnum.Key
        //                                    select exReqDb).FirstOrDefault();
        //                    if (exclRole == null) {
        //                        htDelReq.Remove(iEnum.Key);
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    if (htDelReq != null) {
        //        IDictionaryEnumerator iEnum = htDelReq.GetEnumerator();
        //        while (iEnum.MoveNext()) {
        //            //user is excluded in all of the PG so it will be deleted completely prom PG
        //            var prCg = (from prCgDb in cg.ParticipantRole_CentreGroup
        //                        where prCgDb.participant_id == (int)iEnum.Key &&
        //                        prCgDb.role_id == (int)UserRole.Requestor &&
        //                        prCgDb.centre_group_id == cg.id
        //                        select prCgDb).FirstOrDefault();

        //            if (prCg != null) {
        //                DeleteRequestorCompletely(cg, prCg);
        //            }

        //        }
        //    }
        //}

        public bool IsAuthorized(int userId, int roleId, int cgId) {

            var roleCentreGroup = from prcg in m_dbContext.ParticipantRole_CentreGroup
                                  where prcg.participant_id == userId &&
                                  prcg.role_id == roleId &&
                                  prcg.centre_group_id == cgId
                                  select prcg;

            var roleCentreGroupList = roleCentreGroup.FirstOrDefault();

            if (roleCentreGroupList != null) {
                return true;
            }

            var cg = (from cgDb in m_dbContext.Centre_Group
                      where cgDb.id == cgId
                      select new { company_id = cgDb.company_id }).FirstOrDefault();


            var officeAdmin = (from pofDb in m_dbContext.Participant_Office_Role
                               where pofDb.participant_id == userId &&
                               pofDb.office_id == cg.company_id &&
                               pofDb.role_id == (int)UserRole.OfficeAdministrator
                               select pofDb).FirstOrDefault();

            if (officeAdmin != null) {
                return true;
            }

            return false;

        }

        private bool IsManagerRoleRecordModified(ManagerRoleExtended modifManRole, Manager_Role origManRole) {
            if (m_dbContext.Entry(origManRole).State == EntityState.Added) {
                return true;
            }

            if (modifManRole.participant_id != origManRole.participant_id ||
                modifManRole.approve_level_id != origManRole.approve_level_id) {

                //Manager_Role newManRole = new Manager_Role();
                //SetValues(origManRole, newManRole);
                //newManRole.purchase_group_limit_id = pgLimitId;
                //newManRole.participant_id = modifManRole.participant_id;
                //newManRole.approve_level_id = modifManRole.approve_level_id;
                //newManRole.active = true;
                //m_dbContext.Manager_Role.Remove(origManRole);
                //m_dbContext.Manager_Role.Add(newManRole);

                return true;
            }

            return false;
        }

        public bool DeletePurchaseGroup(int pgId, int cgId) {
            bool isDeleted = true;

            var pg = (from pgDb in m_dbContext.Purchase_Group
                      where pgDb.id == pgId
                      select pgDb).FirstOrDefault();

            if (pg == null) {
                return isDeleted;
            }

            var cg = (from cgDb in m_dbContext.Centre_Group
                      where cgDb.id == cgId
                      select cgDb).FirstOrDefault();

            using (TransactionScope transaction = new TransactionScope()) {
                try {

                    //remove implicit excluded requestors orderers
                    for (int i = pg.PurchaseGroup_ImplicitOrderer.Count - 1; i >= 0; i--) {
                        pg.PurchaseGroup_ImplicitOrderer.Remove(pg.PurchaseGroup_ImplicitOrderer.ElementAt(i));
                    }
                    for (int i = pg.PurchaseGroup_ImplicitRequestor.Count - 1; i >= 0; i--) {
                        pg.PurchaseGroup_ImplicitRequestor.Remove(pg.PurchaseGroup_ImplicitRequestor.ElementAt(i));
                    }

                    for (int i = pg.ParticipantsExcludeOrderers.Count - 1; i >= 0; i--) {
                        pg.ParticipantsExcludeOrderers.Remove(pg.ParticipantsExcludeOrderers.ElementAt(i));
                    }
                    for (int i = pg.ParticipantsExcludeRequestor.Count - 1; i >= 0; i--) {
                        pg.ParticipantsExcludeRequestor.Remove(pg.ParticipantsExcludeRequestor.ElementAt(i));
                    }

                    for (int i = pg.Supplier.Count - 1; i >= 0; i--) {
                        pg.Supplier.Remove(pg.Supplier.ElementAt(i));
                    }

                    for (int i = pg.PurchaseGroup_Requestor.Count - 1; i >= 0; i--) {
                        DeleteParticipantRequestorRole(cg, pg.PurchaseGroup_Requestor.ElementAt(i).requestor_id, DataNulls.INT_NULL, DataNulls.INT_NULL);
                        pg.PurchaseGroup_Requestor.Remove(pg.PurchaseGroup_Requestor.ElementAt(i));
                    }

                    for (int i = pg.PurchaseGroup_Orderer.Count - 1; i >= 0; i--) {
                        DeleteParticipantOrdererRole(cg, pg.PurchaseGroup_Orderer.ElementAt(i).id, DataNulls.INT_NULL);
                        pg.PurchaseGroup_Orderer.Remove(pg.PurchaseGroup_Orderer.ElementAt(i));
                    }



                    ////remove req pg centre
                    //for (int i = pg.Participant_PurchaseGroup_Centre.Count - 1; i >= 0; i--) {
                    //    pg.Participant_PurchaseGroup_Centre.Remove(pg.Participant_PurchaseGroup_Centre.ElementAt(i));
                    //}




                    //SaveChanges();



                    bool isRequest = pg.Request_Event.Select(r => r.purchase_group_id == pg.id).FirstOrDefault();
                    if (isRequest) {

                        //deactivate
                        pg.active = false;
                        isDeleted = false;
                    }

                    #region Pg Limits
                    //pg limits
                    var pgLimits = (from pgLimitsDb in m_dbContext.Purchase_Group_Limit
                                    where pgLimitsDb.purchase_group_id == pg.id
                                    select pgLimitsDb).ToList();
                    if (pgLimits != null) {

                        for (int i = pgLimits.Count - 1; i >= 0; i--) {
                            for (int j = pgLimits.ElementAt(i).Manager_Role.Count - 1; j >= 0; j--) {
                                pgLimits.ElementAt(i).Manager_Role.Remove(pgLimits.ElementAt(i).Manager_Role.ElementAt(j));
                            }
                            m_dbContext.Purchase_Group_Limit.Remove(pgLimits.ElementAt(i));
                        }
                    }
                    #endregion

                    #region Requestors
                    //pg requestors
                    var pgRequestors = (from pgReqDb in m_dbContext.PurchaseGroup_Requestor
                                        where pgReqDb.purchase_group_id == pg.id
                                        select pgReqDb).ToList();

                    if (pgRequestors != null) {

                        for (int i = pgRequestors.Count - 1; i >= 0; i--) {
                            m_dbContext.PurchaseGroup_Requestor.Remove(pgRequestors.ElementAt(i));
                        }
                    }
                    #endregion

                    #region Implicit Orderers
                    var pgImpOrderers = (from pgImpOrdDb in m_dbContext.PurchaseGroup_ImplicitOrderer
                                         where pgImpOrdDb.purchase_category_id == pg.id
                                         select pgImpOrdDb).ToList();

                    if (pgImpOrderers != null) {

                        for (int i = pgImpOrderers.Count - 1; i >= 0; i--) {
                            m_dbContext.PurchaseGroup_ImplicitOrderer.Remove(pgImpOrderers.ElementAt(i));
                        }
                    }
                    #endregion

                    #region Implicit Requestors
                    var pgImpRequestors = (from pgImpReqDb in m_dbContext.PurchaseGroup_ImplicitRequestor
                                           where pgImpReqDb.purchase_category_id == pg.id
                                           select pgImpReqDb).ToList();

                    if (pgImpRequestors != null) {

                        for (int i = pgImpRequestors.Count - 1; i >= 0; i--) {
                            m_dbContext.PurchaseGroup_ImplicitRequestor.Remove(pgImpRequestors.ElementAt(i));
                        }
                    }
                    #endregion

                    #region Exclude Requestors
                    if (pg.ParticipantsExcludeRequestor != null) {
                        for (int i = pg.ParticipantsExcludeRequestor.Count - 1; i >= 0; i--) {
                            pg.ParticipantsExcludeRequestor.Remove(pg.ParticipantsExcludeRequestor.ElementAt(i));
                        }
                    }
                    #endregion

                    #region Exclude Orderer
                    if (pg.ParticipantsExcludeOrderers != null) {
                        for (int i = pg.ParticipantsExcludeOrderers.Count - 1; i >= 0; i--) {
                            pg.ParticipantsExcludeOrderers.Remove(pg.ParticipantsExcludeOrderers.ElementAt(i));
                        }
                    }
                    #endregion

                    if (isDeleted) {
                        //delete pg

                        //remove pg parent purchase 
                        for (int i = pg.Parent_Purchase_Group.Count - 1; i >= 0; i--) {
                            pg.Parent_Purchase_Group.Remove(pg.Parent_Purchase_Group.ElementAt(i));
                        }

                        //remove pg cengre group
                        for (int i = pg.Centre_Group.Count - 1; i >= 0; i--) {
                            pg.Centre_Group.Remove(pg.Centre_Group.ElementAt(i));
                        }

                        //remove local pg name
                        for (int i = pg.Purchase_Group_Local.Count - 1; i >= 0; i--) {
                            pg.Purchase_Group_Local.Remove(pg.Purchase_Group_Local.ElementAt(i));
                        }

                        pg = (from pgDb in m_dbContext.Purchase_Group
                              where pgDb.id == pgId
                              select pgDb).FirstOrDefault();

                        if (pg == null) {
                            return isDeleted;
                        }

                        m_dbContext.Purchase_Group.Remove(pg);

                    }

                    SaveChanges();
                    transaction.Complete();

                    return isDeleted;
                } catch (Exception ex) {
                    throw ex;
                }
            }
        }

        public List<SupplierSimpleExtended> GetActiveSupplierData(int supplierGroupId, string searchText) {

            var suppliers = (from sd in m_dbContext.Supplier
                             where sd.supplier_group_id == supplierGroupId &&
                             (sd.supp_name.Contains(searchText) || sd.supplier_id.Contains(searchText)) &&
                             sd.active == true
                             select new {
                                 id = sd.id,
                                 supp_name = sd.supp_name,
                                 supplier_id = sd.supplier_id
                             }).ToList();

            if (suppliers == null) {
                return null;
            }
            
            List<SupplierSimpleExtended> retSupplierSimpleList = new List<SupplierSimpleExtended>();
            foreach (var supp in suppliers) {
                SupplierSimpleExtended tmpSupplierSimpleExtended = new SupplierSimpleExtended();
                SetValues(supp, tmpSupplierSimpleExtended, new List<string> { "id", "supp_name" });
                if (!String.IsNullOrEmpty(supp.supplier_id)) {
                    tmpSupplierSimpleExtended.supp_name += " (" + supp.supplier_id + ")";
                }

                retSupplierSimpleList.Add(tmpSupplierSimpleExtended);
            }

            return retSupplierSimpleList;
        }

        public List<CentreCgSimple> GetCentres(int currentUserId, string searchText, bool isCgDisabledLoaded) {

            string sql = "SELECT DISTINCT cd.id, cd.name, cgd.id 'centre_group_id' FROM ParticipantRole_CentreGroup prcg " +
                " INNER JOIN Centre_CentreGroup cgc" +
                " ON cgc.centre_group_id=prcg.centre_group_id" +
                " INNER JOIN Centre_Group cgd" +
                " ON cgd.id=cgc.centre_group_id" +
                " INNER JOIN Centre cd" +
                " ON cd.id=cgc.centre_id" +
                //" WHERE prcg.role_id=" + (int)UserRole.CentreGroupAdmin + 
                " WHERE prcg.participant_id=" + currentUserId +
                " AND cgd.active=1" +
                " AND cd.active=1" +
                " AND cd.name LIKE '%" + searchText + "%'" +
                " ORDER BY cd.name";

            var centres = m_dbContext.Database.SqlQuery<CentreCgSimple>(sql).ToList();

            return centres;

        }

        public List<CentreCgSimple> GetCentres(string searchText, List<int> adminCompanyIds, bool isDisabledLoaded) {

            var centres = (from cd in m_dbContext.Centre
                           where cd.name.Contains(searchText) &&
                           cd.active == true
                           orderby cd.name
                           select cd).ToList();

            List<CentreCgSimple> centreCgs = new List<CentreCgSimple>();
            if (centres != null) {
                foreach (var centre in centres) {
                    if (centre.Centre_Group == null || centre.Centre_Group.Count == 0) {
                        continue;
                    }

                    if (adminCompanyIds != null && !adminCompanyIds.Contains(centre.Centre_Group.ElementAt(0).company_id)) {
                        if (centre.Centre_Group.ElementAt(0).active == false) {
                            continue;
                        }
                    }

                    if (!isDisabledLoaded && centre.Centre_Group.ElementAt(0).active == false) {
                        continue;
                    }

                    CentreCgSimple centreCgSimple = new CentreCgSimple();
                    centreCgSimple.id = centre.id;
                    centreCgSimple.name = centre.name;
                    centreCgSimple.centre_group_id = centre.Centre_Group.ElementAt(0).id;
                    centreCgs.Add(centreCgSimple);
                }

            }

            return centreCgs;

        }

        public List<CentreGroupSimple> GetCentreGroupsByCompany(int companyId) {

            var centreGroups = from cg in m_dbContext.Centre_Group
                               where (cg.company_id == companyId &&
                               cg.id >= 0)
                               select new {
                                   id = cg.id,
                                   name = cg.name
                               };

            var tmpCgs = centreGroups.Distinct().OrderBy(x => x.name).ToList();

            List<CentreGroupSimple> resCentreGroups = new List<CentreGroupSimple>();
            foreach (var tmpCg in tmpCgs) {
                CentreGroupSimple cg = new CentreGroupSimple();
                SetValues(tmpCg, cg);
                resCentreGroups.Add(cg);
            }

            return resCentreGroups;

        }

        public List<CentreGroupSimple> GetCentreGroupsByCompanyActiveOnly(int companyId) {

            var centreGroups = from cg in m_dbContext.Centre_Group
                               where (cg.company_id == companyId &&
                               cg.id >= 0 && cg.active == true)
                               select new {
                                   id = cg.id,
                                   name = cg.name
                               };

            var tmpCgs = centreGroups.Distinct().OrderBy(x => x.name).ToList();

            List<CentreGroupSimple> resCentreGroups = new List<CentreGroupSimple>();
            foreach (var tmpCg in tmpCgs) {
                CentreGroupSimple cg = new CentreGroupSimple();
                SetValues(tmpCg, cg);
                resCentreGroups.Add(cg);
            }

            return resCentreGroups;

        }

        public List<CentreGroupSimple> GetCentreGroupsByCompany(int companyId, int userId) {

            /*
             var query = (from RR in context.TableOne
             join M in context.TableTwo on new { oId = RR.OrderedProductId,  sId = RR.SoldProductId} equals new { oId = M.ProductID, sId = M.ProductID }
             where RR.CustomerID == CustomerID 
             && statusIds.Any(x => x.Equals(RR.StatusID.Value))
             select RR.OrderId).ToArray();
             */

            var centreGroups = from cg in m_dbContext.Centre_Group
                               join prcg in m_dbContext.ParticipantRole_CentreGroup
                               on cg.id equals prcg.centre_group_id
                               where (cg.company_id == companyId && prcg.participant_id == userId)
                               select new {
                                   id = cg.id,
                                   name = cg.name
                               };

            var tmpCgs = centreGroups.Distinct().OrderBy(x => x.name).ToList();

            List<CentreGroupSimple> resCentreGroups = new List<CentreGroupSimple>();
            foreach (var tmpCg in tmpCgs) {
                CentreGroupSimple cg = new CentreGroupSimple();
                SetValues(tmpCg, cg);
                resCentreGroups.Add(cg);
            }

            return resCentreGroups;

        }

        public CentreGroupSimple GetCentreGroupsByCompanyCgId(int cgId, string cgName, int companyId) {

            var centreGroup = (from cg in m_dbContext.Centre_Group
                               where (
                               cg.id != cgId &&
                               cg.name.Trim().ToUpper() == cgName.Trim().ToUpper() &&
                               //cg.active == true &&
                               cg.company_id == companyId)
                               select new {
                                   id = cg.id,
                                   name = cg.name
                               }).FirstOrDefault();

            if (centreGroup == null) {
                return null;
            }

            CentreGroupSimple retCg = new CentreGroupSimple();
            retCg.id = centreGroup.id;
            retCg.name = centreGroup.name;

            return retCg;

        }

        public bool DeleteCentreGroup(int cgId) {
            bool isDeleted = true;

            using (TransactionScope transaction = new TransactionScope()) {
                try {
                    var centreGroup = (from cg in m_dbContext.Centre_Group
                                       where cg.id == cgId
                                       select cg).FirstOrDefault();

                    var request = (from reqDb in m_dbContext.Request_Event
                                   where reqDb.centre_group_id == cgId
                                   select new { id = reqDb.id }).Take(1).FirstOrDefault();

                    if (request != null) {
                        isDeleted = false;
                    }

                    //Delete roles
                    if (centreGroup.ParticipantRole_CentreGroup != null) {
                        for (int i = centreGroup.ParticipantRole_CentreGroup.Count - 1; i >= 0; i--) {
                            if (centreGroup.ParticipantRole_CentreGroup.ElementAt(i).role_id == (int)UserRole.CentreGroupPropAdmin) {
                                continue;
                            }
                            centreGroup.ParticipantRole_CentreGroup.Remove(centreGroup.ParticipantRole_CentreGroup.ElementAt(i));
                        }
                    }

                    //Delete Requestors
                    if (centreGroup.Centre != null) {
                        foreach (var centre in centreGroup.Centre) {
                            if (centre.Requestor_Centre != null) {
                                for (int i = centre.Requestor_Centre.Count - 1; i >= 0; i--) {
                                    centre.Requestor_Centre.Remove(centre.Requestor_Centre.ElementAt(i));
                                }
                            }
                        }
                    }

                    //Delete Purchase Groups
                    if (centreGroup.Purchase_Group != null) {
                        for (int i = centreGroup.Purchase_Group.Count - 1; i >= 0; i--) {
                            DeletePurchaseGroup(centreGroup.Purchase_Group.ElementAt(i).id, cgId);
                        }
                    }

                    //Delete Orderer Supplier
                    if (centreGroup.Orderer_Supplier != null) {
                        for (int i = centreGroup.Orderer_Supplier.Count - 1; i >= 0; i--) {
                            centreGroup.Orderer_Supplier.Remove(centreGroup.Orderer_Supplier.ElementAt(i));
                        }
                    }

                    //Manager role
                    if (centreGroup.Manager_Role != null) {
                        for (int i = centreGroup.Manager_Role.Count - 1; i >= 0; i--) {
                            centreGroup.Manager_Role.Remove(centreGroup.Manager_Role.ElementAt(i));
                        }
                    }

                    //centre
                    if (centreGroup.Centre != null) {
                        for (int i = centreGroup.Centre.Count - 1; i >= 0; i--) {
                            centreGroup.Centre.Remove(centreGroup.Centre.ElementAt(i));
                        }
                    }

                    //foreign currencies
                    if (centreGroup.Foreign_Currency != null) {
                        for (int i = centreGroup.Foreign_Currency.Count - 1; i >= 0; i--) {
                            centreGroup.Foreign_Currency.Remove(centreGroup.Foreign_Currency.ElementAt(i));
                        }
                    }

                    //orderer supplier
                    if (centreGroup.Orderer_Supplier != null) {
                        for (int i = centreGroup.Orderer_Supplier.Count - 1; i >= 0; i--) {
                            centreGroup.Orderer_Supplier.Remove(centreGroup.Orderer_Supplier.ElementAt(i));
                        }
                    }

                    if (isDeleted) {
                        //supplier group
                        centreGroup.SupplierGroup = null;

                        //currency
                        centreGroup.Currency = null;

                        //ship to address
                        if (centreGroup.Ship_To_Address != null) {
                            for (int i = centreGroup.Ship_To_Address.Count - 1; i >= 0; i--) {
                                centreGroup.Ship_To_Address.Remove(centreGroup.Ship_To_Address.ElementAt(i));
                            }
                        }

                        m_dbContext.Centre_Group.Remove(centreGroup);
                    }


                    m_dbContext.SaveChanges();

                    transaction.Complete();

                    return isDeleted;
                } catch (Exception ex) {
                    throw ex;
                }
            }


        }

        public void DeleteCentreGroup(string cgName) {

            var centreGroup = (from cg in m_dbContext.Centre_Group
                               where cg.name == cgName
                               select cg).FirstOrDefault();
            if (centreGroup != null) {
                DeleteCentreGroup(centreGroup.id);
            }

        }

        public bool IsCentreGroupExists(string cgName) {
            var centreGroup = (from cg in m_dbContext.Centre_Group
                               where cg.name == cgName
                               select cg).FirstOrDefault();

            return (centreGroup != null);
        }

        public void AddCentreGroup(string cgName) {
            Centre_Group cg = new Centre_Group();
            cg.name = cgName;

            var lastCg = (from lastCgDb in m_dbContext.Centre_Group
                          orderby lastCgDb.id descending
                          select new { id = lastCgDb.id }).Take(1).FirstOrDefault();
            int lastId = -1;
            if (lastCg != null) {
                lastId = lastCg.id;
            }
            int newId = ++lastId;
            cg.id = newId;
            
            m_dbContext.Entry(cg).State = EntityState.Added;

            m_dbContext.SaveChanges();
        }
        #endregion
    }
}
