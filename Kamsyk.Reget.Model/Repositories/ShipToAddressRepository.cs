using Kamsyk.Reget.Model.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Kamsyk.Reget.Model.Repositories {
    public class ShipToAddressRepository : BaseRepository<Ship_To_Address> {
        #region Methods
        public List<Ship_To_Address> GetShipToAddressByUserCentreId(int userId, int centreId) {
            List<Ship_To_Address> addresses = (from addressDb in m_dbContext.Ship_To_Address
                                               where addressDb.participant_id == userId
                                               select addressDb).ToList();
            if (addresses != null && addresses.Count != 0) {
                return addresses;
            }

            addresses = (from addressDb in m_dbContext.Ship_To_Address
                         where addressDb.centre_id == centreId
                         select addressDb).ToList();
            if (addresses != null && addresses.Count != 0) {
                return addresses;
            }

            var centre = (from centreDb in m_dbContext.Centre
                          where centreDb.id == centreId
                          select centreDb).FirstOrDefault();
            int iCgId = centre.Centre_Group.ElementAt(0).id;

            addresses = (from addressDb in m_dbContext.Ship_To_Address
                         where addressDb.centre_group_id == iCgId
                         select addressDb).ToList();
            return addresses;
        }

        
        #endregion
    }
}
