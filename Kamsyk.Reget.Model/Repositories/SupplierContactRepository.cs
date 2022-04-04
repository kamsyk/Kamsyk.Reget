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
using static Kamsyk.Reget.Model.Repositories.UserRepository;
using Kamsyk.Reget.Model.ExtendedModel.Supplier;
using Kamsyk.Reget.Model.ExtendedModel.HttpResult;

namespace Kamsyk.Reget.Model.Repositories {
    public class SupplierContactRepository : BaseRepository<Supplier_Contact> {
        #region Methods
        
        public int SaveSupplierContact(
            Supplier_Contact modifSupplierContact,
            int userId,
            out List<string> msg) {
            msg = new List<string>();

            HttpResult httpResult = new HttpResult();

            //var compDb = (from cd in m_dbContext.Company
            //             where cd.id == companyId 
            //             select cd).FirstOrDefault();
            //int suppGroupId = (int)compDb.supplier_group_id;
                        
            ////Check unique key - duplicity
            //if (modifSupplierContact.id < 0) {
            //    //new 
            //    var dbSupplierContact = (from sd in m_dbContext.Supplier_Contact
            //                     where sd.supp_name == modifSupplier.supp_name &&
            //                     sd.supplier_group_id == modifSupplier.supplier_group_id
            //                     && (sd.supplier_id == modifSupplier.supplier_id || sd.supplier_id == null || sd.supplier_id == "")
            //                      select sd).FirstOrDefault();

            //    if (dbSupplier != null) {
            //        //duplicity
            //        msg.Add(DUPLICITY);
            //        return -1;
            //    }
            //} else {
            //    //existing 
            //    var dbSupplier = (from sd in m_dbContext.Supplier
            //                     where sd.supp_name == modifSupplier.supp_name &&
            //                     sd.id != modifSupplier.id &&
            //                     sd.supplier_group_id == modifSupplier.supplier_group_id
            //                      select sd).FirstOrDefault();

            //    if (dbSupplier != null) {
            //        //duplicity
                    
            //        msg.Add(DUPLICITY);
            //        return -1;
            //    }
            //}


            using (TransactionScope transaction = new TransactionScope()) {
                try {
                    var dbSupplierContact = new Supplier_Contact();

                    if (modifSupplierContact.id >= 0) {

                        dbSupplierContact = (from cd in m_dbContext.Supplier_Contact
                                    where cd.id == modifSupplierContact.id
                                    select cd).FirstOrDefault();

                    }

                    dbSupplierContact.supplier_id = modifSupplierContact.supplier_id;
                    dbSupplierContact.first_name  = modifSupplierContact.first_name;
                    dbSupplierContact.surname = modifSupplierContact.surname;
                    dbSupplierContact.phone = modifSupplierContact.phone;
                    dbSupplierContact.email = modifSupplierContact.email;
                    dbSupplierContact.modified_user_id = userId;
                    dbSupplierContact.modified_date = DateTime.Now;

                    if (modifSupplierContact.id < 0) {

                        var lastSupp = (from scd in m_dbContext.Supplier_Contact
                                         orderby scd.id descending
                                         select scd).Take(1).FirstOrDefault();

                        int lastId = -1;
                        if (lastSupp != null) {
                            lastId = lastSupp.id;
                        }
                                               
                        lastId++;
                        dbSupplierContact.id = lastId;
                        
                        m_dbContext.Supplier_Contact.Add(dbSupplierContact);
                    }

                    SaveChanges();
                    transaction.Complete();

                    return dbSupplierContact.id;
                } catch (Exception ex) {
                    throw ex;
                }
            }
        }


        public void DeleteSupplierContact(int contactId) {
            var dbSupplierContact = (from cd in m_dbContext.Supplier_Contact
                                 where cd.id == contactId
                                 select cd).FirstOrDefault();

            m_dbContext.Supplier_Contact.Remove(dbSupplierContact);
            SaveChanges();
        }
        #endregion
    }
}
