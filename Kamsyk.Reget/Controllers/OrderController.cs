using Kamsyk.Reget.Interface.DocInterface;
using Kamsyk.Reget.PdfGenerator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kamsyk.Reget.Controllers {
    public class OrderController : Controller, IOrderDoc {
        [HttpGet]
        public ActionResult GenerateOrder(
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
            object[] otherItems) {

            PdfOrder pdfOrder = new PdfOrder();
            byte[] pdfBytes = pdfOrder.GenerateOrder(
                PdfCommon.GetOrderType(iOrderType),
                null,
                null,
                null,
                null,
                null,
                orderNr,
                requestorCompanyAddress,
                requestorAddress,
                ordererName,
                ordererMail,
                ordererPhone,
                null,
                supplierName,
                supplierMail,
                supplierPhone,
                null,
                supplierAddress,
                requestText,
                priceCurrency,
                delivDate,
                null,
                null,
                null);

            Stream outputStream = new MemoryStream(pdfBytes);

            return File(
                outputStream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                orderNr + ".pdf");
        }
    }
}