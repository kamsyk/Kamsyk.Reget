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
using System.Data.Entity.Infrastructure;
using Kamsyk.Reget.Model.ExtendedModel.Order;

namespace Kamsyk.Reget.Model.Repositories {
    public class OrderRepository : BaseRepository<Request_Order> {
        #region Methods
        public OrderExtend GetOrderByRequestIdJs(int requestId) {
            Request_Order requestOrder = (from orderDb in m_dbContext.Request_Order
                                where orderDb.request_id == requestId
                                orderby orderDb.request_version descending
                                select orderDb).FirstOrDefault();

            OrderExtend orderExtend = new OrderExtend();
            var requestEvent = new RequestRepository().GetRequestEventById(requestId);
            orderExtend.request_nr = requestEvent.request_nr;
            if (requestOrder != null) {
                SetValues(requestOrder, orderExtend);
            } else {
                
                orderExtend.otis_company_address = GetCompanyAddress(requestEvent);
                orderExtend.supplier_address = GetSupplierAddress(requestEvent.Supplier1);
                orderExtend.orderer_name = UserRepository.GetUserNameSurnameFirst(requestEvent.Orderer);
                orderExtend.orderer_mail = requestEvent.Orderer.email;
                orderExtend.orderer_phone = requestEvent.Orderer.phone;
                orderExtend.requestor_name = UserRepository.GetUserNameSurnameFirst(requestEvent.Requestor);
                orderExtend.requestor_mail = requestEvent.Requestor.email;
                orderExtend.requestor_phone = requestEvent.Requestor.phone;
                orderExtend.request_text = requestEvent.request_text;
                orderExtend.requestor_id = (int)requestEvent.requestor;
                AddMailAddresses(requestEvent.Supplier1, orderExtend);
            }
                        
            return orderExtend;
        }

        private void AddMailAddresses(Supplier supplier, OrderExtend orderExtend) {
            orderExtend.mail_addresses = new List<Checkbox>();
            if (supplier.Supplier_Contact != null) {
                foreach (var contact in supplier.Supplier_Contact) {
                    if (String.IsNullOrEmpty(contact.email)) {
                        continue;
                    }
                    Checkbox checkbox = new Checkbox();
                    checkbox.id = contact.id;
                    checkbox.text = contact.email;
                    orderExtend.mail_addresses.Add(checkbox);
                }
            }
        }

        private string GetCompanyAddress(Request_Event requestEvent) {
            string strCompanyAddress = "";

            if (!String.IsNullOrEmpty(requestEvent.ship_to_address_company_name)) {
                if (strCompanyAddress.Length > 0) {
                    strCompanyAddress += Environment.NewLine;
                }
                strCompanyAddress += requestEvent.ship_to_address_company_name;
            }
            if (!String.IsNullOrEmpty(requestEvent.ship_to_address_street)) {
                if (strCompanyAddress.Length > 0) {
                    strCompanyAddress += Environment.NewLine;
                }
                strCompanyAddress += requestEvent.ship_to_address_street;
            }

            string strCity = "";
            string strZipCode = "";
            if (!String.IsNullOrEmpty(requestEvent.ship_to_address_city)) {
                strCity = requestEvent.ship_to_address_city;
            }
            if (!String.IsNullOrEmpty(requestEvent.ship_to_address_zip)) {
                strZipCode = requestEvent.ship_to_address_zip + " "; 
            }

            if (strCompanyAddress.Length > 0) {
                strCompanyAddress += Environment.NewLine;
            }
            strCompanyAddress += strZipCode + strCity;

            return strCompanyAddress;
        }

        private string GetSupplierAddress(Supplier supplier) {
            string strSupplierAddress = "";

            if (!String.IsNullOrEmpty(supplier.supp_name)) {
                if (strSupplierAddress.Length > 0) {
                    strSupplierAddress += Environment.NewLine;
                }
                strSupplierAddress += supplier.supp_name;
            }
            if (!String.IsNullOrEmpty(supplier.street_part1)) {
                if (strSupplierAddress.Length > 0) {
                    strSupplierAddress += Environment.NewLine;
                }
                strSupplierAddress += supplier.street_part1;
            }
            string strCity = "";
            string strZipCode = "";
            if (!String.IsNullOrEmpty(supplier.city)) {
                strCity = supplier.city;
            }
            if (!String.IsNullOrEmpty(supplier.zip)) {
                strZipCode = supplier.zip + " ";
            }

            if (strSupplierAddress.Length > 0) {
                strSupplierAddress += Environment.NewLine;
            }
            strSupplierAddress += strZipCode + strCity;

            return strSupplierAddress;
        }
        #endregion
    }
}
