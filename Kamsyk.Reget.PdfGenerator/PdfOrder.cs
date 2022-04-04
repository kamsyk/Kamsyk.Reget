using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Kamsyk.Reget.PdfGenerator.PdfOrderPageEvent;

namespace Kamsyk.Reget.PdfGenerator {
    public class PdfOrder {

        #region Fonts
        private static BaseFont m_Unicode = null;

        private static string m_ArialFont = null;
        public static string ArialFont {
            get { return m_ArialFont; }
            set {
                m_ArialFont = value;
                m_Unicode = iTextSharp.text.pdf.BaseFont.CreateFont(m_ArialFont, BaseFont.IDENTITY_H, iTextSharp.text.pdf.BaseFont.EMBEDDED);
            }
        }

        private Font m_FontTableHeader {
            get {
                
                Font fontTable = new iTextSharp.text.Font(
                    m_Unicode, 18, iTextSharp.text.Font.BOLD);

                return fontTable;
            }
        }
        #endregion

        #region Methods
        public byte[] GenerateOrder(
            PdfCommon.OrderType orderType,
            string otisLogoPath,
            string barCodeFilePath,
            string imgArrowPath,
            string purchTermCondFilePath,
            string supplierCodexFilePath,
            string orderNr,
            string otisCompanyAddress,
            string requestorAddress,
            string ordererName,
            string ordererMail,
            string ordererPhone,
            string ordererFax,
            string supplierName,
            string supplierMail,
            string supplierPhone,
            string supplierFax,
            string supplierAddress,
            string requestText,
            string estPriceCurrency,
            DateTime delivDate,
            string contractNrs,
            string invoiceMail,
            string cultureName) {

            if (orderType == PdfCommon.OrderType.ZonaCZ || orderType == PdfCommon.OrderType.ZonaSK) {
                return GenerateOrderZonaCZ(
                    orderType,
                    otisLogoPath,
                    orderNr,
                    otisCompanyAddress,
                    requestorAddress,
                    ordererName,
                    ordererMail,
                    ordererPhone,
                    ordererFax,
                    supplierName,
                    supplierMail,
                    supplierPhone,
                    supplierFax,
                    supplierAddress,
                    requestText,
                    estPriceCurrency,
                    delivDate,
                    contractNrs,
                    invoiceMail,
                    cultureName);
            //} else if (orderType == OrderType.Bg) {
            //    GenerateOrderBg(
            //        orderType,
            //        barCodeFilePath,
            //        otisLogoPath,
            //        imgArrowPath,
            //        purchTermCondFilePath,
            //        supplierCodexFilePath,
            //        orderNr,
            //        otisCompanyAddress,
            //        supplierAddress,
            //        requestorAddress,
            //        ordererName,
            //        requestText,
            //        estPriceCurrency,
            //        delivDate,
            //        cultureName,
            //        out fileContent);
            //} else {
            //    GenerateOrderClcBreclav(
            //        orderType,
            //        otisLogoPath,
            //        orderNr,
            //        otisCompanyAddress,
            //        requestorAddress,
            //        ordererName,
            //        ordererMail,
            //        ordererPhone,
            //        ordererFax,
            //        supplierAddress,
            //        requestText,
            //        estPriceCurrency,
            //        delivDate,
            //        invoiceMail,
            //        cultureName,
            //        out fileContent);
            }

            return null;
        }

        private byte[] GenerateOrderZonaCZ(
            PdfCommon.OrderType orderType,
            string otisLogoPath,
            string orderNr,
            string otisCompanyAddress,
            string requestorAddress,
            string ordererName,
            string ordererMail,
            string ordererPhone,
            string ordererFax,
            string supplierName,
            string supplierMail,
            string supplierPhone,
            string supplierFax,
            string supplierAddress,
            string requestText,
            string estPriceCurrency,
            DateTime delivDate,
            string contractNrs,
            string invoiceMail,
            string cultureName) {

            Document pdfDoc = new Document(PageSize.A4);

            CultureInfo origCi = Thread.CurrentThread.CurrentCulture;

            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureName);

            byte[] fileContent = null;
            try {
                Stream resultStream = new MemoryStream();
                
                using (resultStream) {
                    PdfWriter pdfWriter = PdfWriter.GetInstance(
                        pdfDoc,
                        resultStream);

                    pdfWriter.PageEvent = new PdfOrderPageEvent(invoiceMail, orderType);

                    int bottomMargin = 175;
                    
                    pdfDoc.SetMargins(30, 30, 30, bottomMargin);

                    pdfDoc.Open();

                    GenerateHeader(pdfDoc, otisLogoPath);
                    //GenerateOrderNrZonaCz(pdfDoc, orderNr);
                    //GenerateAddressHeaderZonaCz(pdfDoc);

                    //GenerateOrdererSupplierAddressesZonaCz(
                    //    pdfDoc,
                    //    otisCompanyAddress,
                    //    supplierAddress);
                    
                    //GenerateContactPersonZonaCz(
                    //    pdfDoc,
                    //    ordererName,
                    //    ordererMail,
                    //    ordererPhone,
                    //    ordererFax,
                    //    supplierName,
                    //    supplierMail,
                    //    supplierPhone,
                    //    supplierFax);

                    //if (!String.IsNullOrEmpty(contractNrs)) {
                    //    GenerateContractNr(pdfDoc, contractNrs);
                    //}

                    //GenerateLeadTimeData(pdfDoc, delivDate);

                    //GenerateTextHeaderZonaCz(pdfDoc);
                                       
                    //GenerateOrderBodyZonaCz(pdfDoc, requestText, estPriceCurrency);
                                       
                    pdfDoc.Close();

                    fileContent = ((MemoryStream)resultStream).ToArray();

                    return fileContent;
                }
            } catch (Exception ex) {
                throw ex;
            } finally {
                Thread.CurrentThread.CurrentCulture = origCi;
                Thread.CurrentThread.CurrentUICulture = origCi;
                if (pdfDoc.IsOpen()) {
                    pdfDoc.Close();
                }
            }
        }

        private void GenerateHeader(Document pdfDoc, string otisLogoFullPath) {
            PdfPTable tableHeader = new PdfPTable(2);
            tableHeader.WidthPercentage = 100;
            tableHeader.HorizontalAlignment = PdfPCell.ALIGN_LEFT;

            PdfPCell cellPos = new PdfPCell();
            cellPos.VerticalAlignment = PdfPCell.ALIGN_LEFT;
            Image imgOtisLog = Image.GetInstance(otisLogoFullPath);
            //imgOtisLog.ScaleToFit(229f, 58f);
            imgOtisLog.ScalePercent(20f, 20f);
            cellPos.AddElement(imgOtisLog);
            cellPos.Border = 0;
            tableHeader.AddCell(cellPos);

            //cellPos = new PdfPCell(new Phrase("OBJEDNÁVKA (ORDER)", m_FontTableHeader));
            cellPos = new PdfPCell(new Phrase(Resources.PdfGeneratorResource.Order.ToUpper(), m_FontTableHeader));
            cellPos.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            cellPos.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            cellPos.Border = 0;
            tableHeader.AddCell(cellPos);

            pdfDoc.Add(tableHeader);
        }
        #endregion
    }
}
