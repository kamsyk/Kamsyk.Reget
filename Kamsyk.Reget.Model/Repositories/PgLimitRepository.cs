
using Kamsyk.Reget.Model.Common;
using Kamsyk.Reget.Model.DataDictionary;
using Kamsyk.Reget.Model.ExtendedModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Kamsyk.Reget.Model.Repositories {
    public class PgLimitRepository : BaseRepository<Purchase_Group_Limit> {
        #region Enum
        public enum PgLimitError {
            BottomGreaterTop = 1
        } 
        #endregion

        string logFile = @"c:\temp\pg_limit_fix.txt";

        #region Methods
        public void CheckPgLimitsAppLevel() {
            var pgs = (from pgDb in m_dbContext.Purchase_Group
                       select pgDb).ToList();

            List<Purchase_Group_Limit> delLimit = new List<Purchase_Group_Limit>();
            List<Purchase_Group> delPg = new List<Purchase_Group>();
            foreach (var pg in pgs) {
                if (pg.id < 2291) {
                    continue;
                }

                //int DataNull = 0;
                if (pg.Purchase_Group_Limit.Count == 0 || pg.Purchase_Group_Limit.ElementAt(0).approve_level_id != DataNulls.INT_NULL) {
                //if (pg.Purchase_Group_Limit.Count == 0) {
                        continue;
                }

                if (pg.Centre_Group == null || pg.Centre_Group.Count == 0) {
                    delPg.Add(pg);
                    foreach (var pgLimit in pg.Purchase_Group_Limit) {
                        delLimit.Add(pgLimit);
                    }
                }
                //if (pg.active == false) {
                //    continue;
                //}

                //if (pg.id == 0) {
                //    int h = 5;
                //}

                Console.WriteLine("Checking " + pg.id + " ...");

                var pgLimits = (from pgLimitDb in m_dbContext.Purchase_Group_Limit
                                where pgLimitDb.purchase_group_id == pg.id
                                select pgLimitDb).ToList();

                bool isProblem = false;
                SortedList<int, Purchase_Group_Limit> appLevelIds = new SortedList<int, Purchase_Group_Limit>();
                bool isNonActive = false;
                foreach (var pgLimit in pgLimits) {

                    //if (pgLimit.active != false) {
                    //    continue;
                    //}

                    //if (pgLimit.approve_level_id == null) {
                    //    continue;
                    //}

                    int appLevelId = -1;
                    foreach (var appMan in pgLimit.Manager_Role) {
                        if (appMan.active == false) {
                            isNonActive = true;
                            continue;
                        }

                        if (appLevelId == -1) {
                            appLevelId = appMan.approve_level_id;
                            pgLimit.approve_level_id = appMan.approve_level_id;
                        } else {
                            if (appLevelId != appMan.approve_level_id) {
                                isProblem = true;
                                Console.WriteLine("Pg Limit " + pgLimit.limit_id);
                            }
                        }

                        if (!appLevelIds.ContainsKey(appLevelId)) {
                            appLevelIds.Add(appLevelId, pgLimit);
                        } else {
                            if (pg.Centre_Group.ElementAt(0).id != appMan.centre_group_id || 
                                appLevelIds[appLevelId].limit_id != appMan.purchase_group_limit_id ||
                                appLevelIds[appLevelId].approve_level_id != appMan.approve_level_id) {
                                Console.WriteLine("Purchase group " + pg.id);
                                isProblem = true;
                                break;
                            }
                        }
                    }

                }

                //if (appLevelIds.First().Key != 0) {
                //    Console.WriteLine("Purchase group " + pg.id);
                //    isProblem = true;
                //}

                if (!isProblem) {
                    //missibg app level
                    int lastAppId = -1;
                    foreach (KeyValuePair<int, Purchase_Group_Limit> kvp in appLevelIds) {
                        if ((kvp.Key - 1) != lastAppId) {
                            Console.WriteLine("Purchase group " + pg.id);
                            isProblem = true;
                            break;
                        }

                        lastAppId = kvp.Key;
                    }

                    if (isProblem && !isNonActive) {
                        if (FixAppLevels(pg, appLevelIds)) {
                            isProblem = false;
                        }
                    }
                }


                if (isProblem) {
                    foreach (var pgLimit in pgLimits) {
                        DbEntityEntry entry = m_dbContext.Entry(pgLimit);
                        entry.State = EntityState.Unchanged;
                    }

                } else {
                    m_dbContext.SaveChanges();
                }
            }

            ////delete pg, limits not linked to Cg
            //foreach (var pgLimit in delLimit) {
            //    using (StreamWriter sw = new StreamWriter(logFile, true)) {
            //        sw.WriteLine("delete limit " + pgLimit.limit_id);
            //    }
            //    pgLimit.active = false;
            //    m_dbContext.SaveChanges();
            //}

            //foreach (var pg in delPg) {
            //    using (StreamWriter sw = new StreamWriter(logFile, true)) {
            //        sw.WriteLine("delete pg " + pg.id);
            //    }
            //    pg.active = false;
            //    m_dbContext.SaveChanges();
            //    //m_dbContext.Purchase_Group.Remove(pg);
            //}
        }

        private bool FixAppLevels(Purchase_Group pg, SortedList<int, Purchase_Group_Limit> appLevelIds) {
            decimal limitBottom = -1;
            
            foreach (KeyValuePair<int, Purchase_Group_Limit> kvp in appLevelIds) {
                
                if (limitBottom == -1 && kvp.Value != null) {
                    limitBottom = (decimal)kvp.Value.limit_bottom;
                } else {
                    if (kvp.Value.limit_bottom == null || (decimal)kvp.Value.limit_bottom >= limitBottom) {
                        if (kvp.Value.limit_bottom != null) {
                            limitBottom = (decimal)kvp.Value.limit_bottom;
                        }
                    } else {
                        
                        return false;
                        
                    }
                }
            }

            //if (isForFix) {
                int iAppLevel = 0;
                foreach (KeyValuePair<int, Purchase_Group_Limit> kvp in appLevelIds) {
                    var pgLimit = (from pgLimitDb in m_dbContext.Purchase_Group_Limit
                                    where pgLimitDb.limit_id == kvp.Value.limit_id
                                    select pgLimitDb).FirstOrDefault();
                    if (pgLimit != null) {

                        List<Manager_Role> delManRoles = new List<Manager_Role>();
                        List<Manager_Role> newManRoles = new List<Manager_Role>();

                        foreach (var appMan in pgLimit.Manager_Role) {
                            if (appMan.approve_level_id != iAppLevel) {
                                Manager_Role newManRole = new Manager_Role();
                                SetValues(appMan, newManRole);
                                newManRole.approve_level_id = iAppLevel;
                                delManRoles.Add(appMan);
                                newManRoles.Add(newManRole);
                            }
                            pgLimit.approve_level_id = iAppLevel;
                        }

                        

                        for (int i = delManRoles.Count - 1; i >= 0; i--) {
                            m_dbContext.Manager_Role.Remove(delManRoles.ElementAt(i));
                            m_dbContext.SaveChanges();
                        }

                        for (int i = newManRoles.Count - 1; i >= 0; i--) {
                            m_dbContext.Manager_Role.Add(newManRoles.ElementAt(i));
                            m_dbContext.SaveChanges();
                        }

                        iAppLevel++;
                    }
                }


            //}

            using (StreamWriter sw = new StreamWriter(logFile, true)) {
                sw.WriteLine(pg.id);
            }

            return true;
        }

        
        public MultiplyLimitResult MultiplyAppLevels(
            List<CompanyCheckbox> companies, 
            decimal multipl, 
            bool isAll, 
            string cultureName) {

            MultiplyLimitResult multiResult = new MultiplyLimitResult();
            using (TransactionScope transaction = new TransactionScope()) {
                try {
                    foreach (var company in companies) {
                        if (!company.is_selected) {
                            continue;
                        }
                        
                        string sql = "SELECT pgld.*, cgd.id AS 'cgId' FROM " + PurchaseGroupLimitData.TABLE_NAME + " pgld" +
                            " INNER JOIN " + PurchaseGroupData.TABLE_NAME + " pgd" +
                            " ON pgld." + PurchaseGroupLimitData.PURCHASE_GROUP_ID_FIELD + "=pgd." + PurchaseGroupData.ID_FIELD +
                            " INNER JOIN " + CentregroupPurchasegroupData.TABLE_NAME + " cgpgd" +
                            " ON cgpgd." + CentregroupPurchasegroupData.PURCHASE_GROUP_ID_FIELD + "=pgd." + PurchaseGroupData.ID_FIELD +
                            " INNER JOIN " + CentreGroupData.TABLE_NAME + " cgd" +
                            " ON cgd." + CentreGroupData.ID_FIELD + "=cgpgd." + CentregroupPurchasegroupData.CENTRE_GROUP_ID_FIELD +
                            " WHERE cgd." + CentreGroupData.COMPANY_ID_FIELD + "=" + company.company_id +
                            " AND pgld." + PurchaseGroupLimitData.ACTIVE_FIELD + "=1" +
                            " AND pgd." + PurchaseGroupData.ACTIVE_FIELD + "=1" +
                            " AND cgd." + CentreGroupData.ACTIVE_FIELD + "=1";

                        if (!isAll) {
                            sql += " AND (pgld." + PurchaseGroupLimitData.IS_LIMIT_BOTTOM_MULTIPL_FIELD + "=1" +
                                " OR pgld." + PurchaseGroupLimitData.IS_LIMIT_TOP_MULTIPL_FIELD + "=1)";
                        }
                                                
                        List<Purchase_Group_Limit> pgLimits = m_dbContext.Purchase_Group_Limit
                            .SqlQuery(sql)
                            .ToList<Purchase_Group_Limit>();

                        foreach (var pgLimit in pgLimits) {
                            bool isAffected = false;
                            if ((pgLimit.is_limit_bottom_multipl == true || isAll) && pgLimit.limit_bottom != null) {
                                decimal dDb = ConvertData.ToDecimal(pgLimit.limit_bottom);
                                dDb *= multipl;
                                pgLimit.limit_bottom = dDb;
                                isAffected = true;
                            }

                            if ((pgLimit.is_limit_top_multipl == true || isAll) && pgLimit.limit_top != null) {
                                decimal dDb = ConvertData.ToDecimal(pgLimit.limit_top);
                                dDb *= multipl;
                                pgLimit.limit_top = dDb;
                                isAffected = true;
                            }

                            if (isAffected) {
                                if (!isAll && pgLimit.limit_top != null && pgLimit.limit_bottom > pgLimit.limit_top) {
                                    MultiplyLimitMsg multiplyLimitMsg = new MultiplyLimitMsg();
                                    string pgName = new PgRepository().GetLocalName(pgLimit.purchase_group_id, cultureName);
                                    multiplyLimitMsg.pgName = pgName;
                                    multiplyLimitMsg.pgId = pgLimit.purchase_group_id;
                                    multiplyLimitMsg.reason = (int)PgLimitError.BottomGreaterTop;

                                    var pg = new PgRepository().GetPgById(pgLimit.purchase_group_id);
                                    multiplyLimitMsg.cgId = pg.Centre_Group.ElementAt(0).id;
                                    multiplyLimitMsg.cgName = pg.Centre_Group.ElementAt(0).name;

                                    if (multiResult.err_msg == null) {
                                        multiResult.err_msg = new List<MultiplyLimitMsg>();
                                    }

                                    multiResult.err_msg.Add(multiplyLimitMsg);
                                } else {
                                    multiResult.affected_limits_count++;
                                }
                                
                            }
                        }
                    }

                    if (multiResult.err_msg == null || multiResult.err_msg.Count == 0) {
                        SaveChanges();
                        transaction.Complete();
                    }

                    return multiResult;
                } catch (Exception ex) {
                    throw ex;
                }
            }
        }

        public void SetMultiLimitBoth() {
            var pgLimitsTofix = (from pgLimitDb in m_dbContext.Purchase_Group_Limit
                                where pgLimitDb.is_limit_bottom_multipl != pgLimitDb.is_limit_top_multipl
                                select pgLimitDb).ToList();
            foreach (var pgLimit in pgLimitsTofix) {
                pgLimit.is_limit_bottom_multipl = pgLimit.is_limit_top_multipl;
            }

            m_dbContext.SaveChanges();
        }

        private int GetLastPgLimitId() {
            var lasLimit = (from limitDb in m_dbContext.Purchase_Group_Limit
                            orderby limitDb.limit_id descending
                            select limitDb).Take(1).FirstOrDefault();

            int lastId = -1;
            if (lasLimit != null) {
                lastId = lasLimit.limit_id;
            }

            return lastId;
        }

        public Purchase_Group_Limit GetLimitByLimitId(int pgLimitId) {
            var pgLimit = (from pgLimitDb in m_dbContext.Purchase_Group_Limit
                           where pgLimitDb.limit_id == pgLimitId
                           select pgLimitDb).FirstOrDefault();

            return pgLimit;
        }

        public List<PurchaseGroupLimitExtended> GetLimitsByPgIdJs(int pgId) {
            var pgLimits = (from pgLimitDb in m_dbContext.Purchase_Group_Limit
                            where pgLimitDb.purchase_group_id == pgId
                            && pgLimitDb.active == true
                            orderby pgLimitDb.approve_level_id
                            select pgLimitDb).ToList();

           List<PurchaseGroupLimitExtended> purchaseGroupLimitsExtended = new List<PurchaseGroupLimitExtended>();

            foreach (var pgLimit in pgLimits) {
                PurchaseGroupLimitExtended purchaseGroupLimitExtended = new PurchaseGroupLimitExtended();
                SetValues(pgLimit, purchaseGroupLimitExtended);
                purchaseGroupLimitExtended.manager_role = new List<ManagerRoleExtended>();
                if (pgLimit.Manager_Role != null) {
                    foreach (var manRole in pgLimit.Manager_Role) {
                        ManagerRoleExtended appManRoleExt = new ManagerRoleExtended();
                        if (manRole.Participants != null) {
                            appManRoleExt.participant = new ParticipantsExtended();
                            SetValues(manRole.Participants, appManRoleExt.participant);
                        }
                        purchaseGroupLimitExtended.manager_role.Add(appManRoleExt);
                    }
                    
                }
                purchaseGroupLimitsExtended.Add(purchaseGroupLimitExtended);
            }

            return purchaseGroupLimitsExtended;
        }
        #endregion
    }
}
