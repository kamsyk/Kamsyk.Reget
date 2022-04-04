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
using System.IO;
using Kamsyk.Reget.Model.ExtendedModel.Attachment;

namespace Kamsyk.Reget.Model.Repositories {
    public class AttachmentRepository : BaseRepository<Attachement> {
        #region Constants
        private const int ATT_REQUESTOR = 1;
        private const int ATT_APP_MAN = 1;
        private const int ATT_ORDERER = 1;
        #endregion

        #region Enums
        public enum AttachmentType {
            Requestor = ATT_REQUESTOR,
            AppMan = ATT_APP_MAN,
            Orderer = ATT_ORDERER
        }
        #endregion

        #region Methods

        public int AddAttachment(string fileName, byte[] fileContent, decimal fileSizeinKb, int userId) {
            try {
                int newId = GetNewId();

                Attachement newAtt = new Attachement();
                newAtt.id = newId;
                newAtt.file_name = fileName;
                newAtt.file_content = fileContent;
                newAtt.size_kb = fileSizeinKb;
                newAtt.modify_user = userId;
                newAtt.modify_date = DateTime.Now;

                m_dbContext.Attachement.Add(newAtt);
                m_dbContext.SaveChanges();

                return newId;
            } catch (Exception ex) {
                throw ex;
            }
        }

        private int GetNewId() {
            var lastAtt = (from attDb in m_dbContext.Attachement
                           orderby attDb.id descending
                           select new { id = attDb.id }).Take(1).FirstOrDefault();
            int lastId = -1;
            if (lastAtt != null) {
                lastId = lastAtt.id;
            }

            int newId = ++lastId;

            return newId;
        }

        public Attachement GetAttachmentById(int id) {
            var att = (from attDb in m_dbContext.Attachement
                           where attDb.id == id
                           select attDb).FirstOrDefault();

            
            return att;
        }

        public AttachmentLight GetAttachmentLightById(int id) {
            var att = (from attDb in m_dbContext.Attachement
                       where attDb.id == id
                       select new AttachmentLight {
                           id = attDb.id,
                           modify_user_id =  attDb.modify_user}).FirstOrDefault();


            return att;
        }
        #endregion

    }
}
