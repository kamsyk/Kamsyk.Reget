
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
    public class CustomFieldRepository : BaseRepository<Custom_Field> {
        #region Methods
        public List<CustomFieldExtend> GetCustomFieldsByPgIdJs(int pgId, string cultureName) {
            var custFields = (from custFieldDb in m_dbContext.Custom_Field
                              join pgCfDb in m_dbContext.PurchaseGroup_CustomField
                              on custFieldDb.id equals pgCfDb.custom_field_id
                              where pgCfDb.purchase_group_id == pgId
                              && custFieldDb.is_active == true
                              select custFieldDb).ToList();

            if (custFields == null) {
                return null;
            }

            List<CustomFieldExtend> retCustFields = new List<CustomFieldExtend>();
            foreach (var custField in custFields) {
                CustomFieldExtend customFieldExpand = new CustomFieldExtend();
                SetValues(custField, customFieldExpand);
                //customFieldExpand.string_value = "xxx";

                var custLocal = (from locDb in custField.CustomField_Local
                                 where locDb.culture == cultureName
                                 select locDb).FirstOrDefault();

                if (custLocal != null) {
                    customFieldExpand.label = custLocal.local_text;
                }


                retCustFields.Add(customFieldExpand);
            }

            return retCustFields;
        }

        //public RequestEvent_CustomFieldValue GetCustomFieldsById(int requestId, int requestVersion, int customFieldId) {
        //    var custField = (from custFieldDb in m_dbContext.RequestEvent_CustomFieldValue
        //                      where custFieldDb.request_event_id == requestId
        //                      && custFieldDb.request_event_version == requestVersion
        //                      && custFieldDb.custom_field_id == customFieldId
        //                      select custFieldDb).FirstOrDefault();

        //    return custField;
        //}

        public RequestEvent_CustomFieldValue GetRequestEventCustomFieldsById(int requestId, int customFieldId) {
            var custField = (from custFieldDb in m_dbContext.RequestEvent_CustomFieldValue
                             where custFieldDb.request_event_id == requestId
                             && custFieldDb.is_active == true
                             && custFieldDb.custom_field_id == customFieldId
                             select custFieldDb).FirstOrDefault();

            return custField;
        }

        public Custom_Field GetCustomFieldById(int customFieldId) {
            var custField = (from custFieldDb in m_dbContext.Custom_Field
                             where custFieldDb.id == customFieldId
                             select custFieldDb).FirstOrDefault();

            return custField;
        }
        #endregion
    }
}
