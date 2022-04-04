using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Kamsyk.Reget.Interface.DocInterface {
    public interface IOrderDoc {
        ActionResult GenerateOrder(
            int iOrderType,
            string orderNr,
            string requestorCompanyAddress,
            string requestorAddress,
            string ordererName,
            string ordererMail,
            string ordererPhone,
            string supplierName,
            string supplierMail,
            string supplierPhone,
            string supplierAddress,
            string requestText,
            string priceCurrency,
            DateTime delivDate,
            string cultureName,
            object[] otherItems);
    }
}
