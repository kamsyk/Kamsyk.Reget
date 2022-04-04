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
using System.Dynamic;
using System.Reflection.Emit;
using System.Reflection;
using static Kamsyk.Reget.Model.Repositories.UserRepository;
using Kamsyk.Reget.Model.ExtendedModel.PurchaseGroup;

namespace Kamsyk.Reget.Model.Repositories {
    public class RequestIdentificatorRepository : BaseRepository<Request_Identificator> {
        #region Constant
        #endregion

        #region Enums
        
        #endregion

        #region Methods
        public int GetRequestIdentificator(int centreId) {

            var ri = (from riDb in m_dbContext.Request_Identificator
                           where riDb.centre_id == centreId
                           && riDb.reueast_year == DateTime.Now.Year
                      select riDb).FirstOrDefault();

            if (ri == null) {
                Request_Identificator newRi = new Request_Identificator();
                newRi.centre_id = centreId;
                newRi.last_request_id = 1;
                newRi.reueast_year = DateTime.Now.Year;

                m_dbContext.Request_Identificator.Add(newRi);
                m_dbContext.SaveChanges();

                return 1;
            }

            int iLastId = ri.last_request_id;
            int newRequestId = iLastId + 1;
            ri.last_request_id = newRequestId;
            m_dbContext.SaveChanges();

            return iLastId;

        }

        
        #endregion
    }
}
