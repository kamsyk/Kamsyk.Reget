
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
    public class PgCgRepository : BaseRepository<ParticipantRole_CentreGroup> {
        #region Methods
        public ParticipantRole_CentreGroup GetPgCg(int cgId, int userId, UserRole userRole) {
            var pgCg = (from pgCgDb in m_dbContext.ParticipantRole_CentreGroup
                        where pgCgDb.centre_group_id == cgId
                        && pgCgDb.participant_id == userId
                        && pgCgDb.role_id == (int)userRole
                        select pgCgDb).FirstOrDefault();

            return pgCg;
        }

        public List<ParticipantRole_CentreGroup> GetPgCgs(InternalRequestEntities dbContext, int userId, UserRole userRole) {
            var pgCgs = (from pgCgDb in dbContext.ParticipantRole_CentreGroup
                        where pgCgDb.participant_id == userId
                        && pgCgDb.role_id == (int)userRole
                        select pgCgDb).ToList();

            
            return pgCgs;
        }

        //public List<ParticipantRole_CentreGroup> GetPgCgsJs(int userId, UserRole userRole) {
        //    var pgCgs = (from pgCgDb in m_dbContext.ParticipantRole_CentreGroup
        //                 where pgCgDb.participant_id == userId
        //                 && pgCgDb.role_id == (int)userRole
        //                 select pgCgDb).ToList();

        //    List<ParticipantRole_CentreGroup> retPgCgs = new List<ParticipantRole_CentreGroup>();
        //    foreach (var pgCg in pgCgs) {
        //        ParticipantRole_CentreGroup tmpPrCg = new ParticipantRole_CentreGroup();
        //        SetValues(pgCg, tmpPrCg);
        //        retPgCgs.Add(tmpPrCg);
        //    }

        //    return retPgCgs;
        //}
        #endregion
    }
}
