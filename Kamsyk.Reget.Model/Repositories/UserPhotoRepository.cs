using Kamsyk.Reget.Model.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Kamsyk.Reget.Model.Repositories {
    public class UserPhotoRepository : BaseRepository<ParticipantPhoto>, IParticipantPhotoRepository {
        #region Methods
        public ParticipantPhoto GetParticipantPhoto(int userId) {
            var participantPhoto = GetFirstOrDefault(p => p.participant_id == userId);
                        
            return participantPhoto;
        }

        public void SaveParticipantPhotoData(ParticipantPhoto modifParticipantPhoto) {

            using (TransactionScope transaction = new TransactionScope()) {
                try {

                    bool isDelete = (modifParticipantPhoto.user_picture_240 == null);

                    var dbParticipantPhoto = (from partPhotoDb in m_dbContext.ParticipantPhoto
                                              where partPhotoDb.participant_id == modifParticipantPhoto.participant_id
                                              select partPhotoDb).FirstOrDefault();

                    if (dbParticipantPhoto == null) {
                        if (!isDelete) {
                            ParticipantPhoto newParticipantPhoto = new ParticipantPhoto();
                            SetValues(modifParticipantPhoto, newParticipantPhoto);
                            m_dbContext.ParticipantPhoto.Add(newParticipantPhoto);
                        }
                    } else {
                        if (isDelete) {
                            m_dbContext.ParticipantPhoto.Remove(dbParticipantPhoto);
                        } else { 
                            dbParticipantPhoto.user_picture_240 = modifParticipantPhoto.user_picture_240;
                        }
                    }

                    SaveChanges();
                    transaction.Complete();

                } catch (Exception ex) {
                    throw ex;
                }
            }
        }
               
        #endregion
    }
}
