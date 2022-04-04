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
    public class PrivacyRepository : BaseRepository<Privacy> {
        #region Constants
        private const int PRIVACY_PRIVATE = 0;
        private const int PRIVACY_CENTRE = 10;
        private const int PRIVACY_PUBLIC = 20;
        #endregion

        #region Enum
        public enum RequestPrivacy {
            Private = PRIVACY_PRIVATE,
            Centre = PRIVACY_CENTRE,
            Public = PRIVACY_PUBLIC
        }
        #endregion

        #region Statis Methods
        public static RequestPrivacy GetRequestPrivacy(int privacyId) {
            if(privacyId == PRIVACY_PRIVATE) {
                return RequestPrivacy.Private;
            } else if (privacyId == PRIVACY_CENTRE) {
                return RequestPrivacy.Centre;
            } else if (privacyId == PRIVACY_PUBLIC) {
                return RequestPrivacy.Public;
            }

            return RequestPrivacy.Private;
        }
        #endregion

        #region Methods
        public List<Privacy> GetPrivacies() {
            var privacies = (from privacyDb in m_dbContext.Privacy
                        orderby privacyDb.id
                         select privacyDb).ToList();

            return privacies;
        }
        #endregion
    }
}
