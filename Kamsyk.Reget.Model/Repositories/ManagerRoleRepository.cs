
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
using static Kamsyk.Reget.Model.Repositories.UserRepository;

namespace Kamsyk.Reget.Model.Repositories {
    public class ManagerRoleRepository : BaseRepository<Manager_Role> {
        #region Methods
        public Manager_Role SetApprovalManager(int participantId, int cgId, int purchaseGroupLimitId) {
            Manager_Role origManRole = null;
            //int origAppManId = -1;
            using (TransactionScope transaction = new TransactionScope()) {
                var manRole = (from manRoleDb in m_dbContext.Manager_Role
                               where manRoleDb.purchase_group_limit_id == purchaseGroupLimitId
                               select manRoleDb).FirstOrDefault();

                var newManRole = (from manRoleDb in m_dbContext.Manager_Role
                               where manRoleDb.purchase_group_limit_id == purchaseGroupLimitId
                               && manRoleDb.participant_id == participantId
                               select manRoleDb).FirstOrDefault();

                if (manRole != null) {
                    origManRole = new Manager_Role();
                    SetValues(manRole, origManRole);
                }

                if (newManRole == null) {
                    newManRole = new Manager_Role();

                    if (manRole == null) {
                        var pgLimit = new PgLimitRepository().GetLimitByLimitId(purchaseGroupLimitId);

                        newManRole.participant_id = participantId;
                        newManRole.approve_level_id = 0;
                        newManRole.centre_group_id = pgLimit.Purchase_Group.Centre_Group.ElementAt(0).id;
                        newManRole.purchase_group_limit_id = purchaseGroupLimitId;
                    } else { 
                        SetValues(manRole, newManRole);
                        newManRole.participant_id = participantId;
                    }
                    m_dbContext.Manager_Role.Add(newManRole);
                }

                if (manRole != null) {
                    m_dbContext.Manager_Role.Remove(manRole);
                }

                var prCg = (from prCgDb in m_dbContext.ParticipantRole_CentreGroup
                            where prCgDb.participant_id == participantId
                            && prCgDb.centre_group_id == cgId
                            select prCgDb).FirstOrDefault();
                if (prCg == null) {
                    ParticipantRole_CentreGroup newPrCg = new ParticipantRole_CentreGroup();
                    newPrCg.centre_group_id = cgId;
                    newPrCg.participant_id = participantId;
                    newPrCg.role_id = (int)UserRole.ApprovalManager;
                    m_dbContext.ParticipantRole_CentreGroup.Add(newPrCg);
                }

                SaveChanges();

                transaction.Complete();
            }

            //return origAppManId;
            return origManRole;
        }

        public void RemoveApprovalManager(int participantId, Manager_Role origManRole, int cgId, int purchaseGroupLimitId) {

            var manRole = (from manRoleDb in m_dbContext.Manager_Role
                           where manRoleDb.purchase_group_limit_id == purchaseGroupLimitId
                           select manRoleDb).FirstOrDefault();

            if (origManRole != null) {
                m_dbContext.Manager_Role.Add(origManRole);
            }

            if (manRole != null) {
                m_dbContext.Manager_Role.Remove(manRole);
                SaveChanges();
            }


            ParticipantRole_CentreGroup prCg = (from pgCgDb in m_dbContext.ParticipantRole_CentreGroup
                                                where pgCgDb.participant_id == participantId
                                                && pgCgDb.centre_group_id == cgId
                                                && pgCgDb.role_id == (int)UserRole.ApprovalManager
                                                select pgCgDb).FirstOrDefault();
            if (prCg != null) {
                m_dbContext.ParticipantRole_CentreGroup.Remove(prCg);
                SaveChanges();
            }

        }

        public Manager_Role GetManagerRoleByLimitIdUserId(int pgLimitId, int userId) {
            var manRole = (from manRoleDb in m_dbContext.Manager_Role
                           where manRoleDb.purchase_group_limit_id == pgLimitId
                           && manRoleDb.participant_id == userId
                           select manRoleDb).FirstOrDefault();

            return manRole;
        }
        #endregion
    }
}
