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

namespace Kamsyk.Reget.Model.Repositories {
    public class UnitRepository : BaseRepository<Unit_Measurement>, IUnitRepository {
        #region Constant
        
        #endregion

        #region Enums
       
        #endregion

        #region Methods
        public List<Unit_Measurement> GetActiveUnits() {
            var units = (from unitDb in m_dbContext.Unit_Measurement
                         where unitDb.active == true
                         orderby unitDb.code
                         select unitDb).ToList();

            return units;
        }

        
        #endregion
    }
}
