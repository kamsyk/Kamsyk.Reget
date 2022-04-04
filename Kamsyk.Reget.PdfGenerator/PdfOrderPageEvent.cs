using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.PdfGenerator {
    public class PdfOrderPageEvent : PdfPageEventHelper {
        #region Constants
        

        private enum TextItemType {
            A = 1,
            B = 2,
            Text = 0
        }
        #endregion

        #region Properties
        private PdfTemplate m_totalPages;
        private PdfTemplate m_generatedDateTime;
        private BaseFont m_fontTotalPages;
        private float m_FooterTextSize = 8;
        private string m_InvoiceMail = null;
        private PdfCommon.OrderType m_OrderType;

        private Font m_FontStandard {
            get {
                Font fontTable = FontFactory.GetFont(
                FontFactory.HELVETICA,
                BaseFont.CP1250,
                10,
                Font.NORMAL);

                return fontTable;
            }
        }

        private Font m_FontStandardBold {
            get {
                Font fontTable = FontFactory.GetFont(
                FontFactory.HELVETICA,
                BaseFont.CP1250,
                10,
                Font.BOLD);

                return fontTable;
            }
        }

        private Font m_FontLink {
            get {
                Font fontTable = FontFactory.GetFont(
                    FontFactory.HELVETICA,
                    BaseFont.CP1250,
                    10,
                    Font.BOLD);
                fontTable.SetStyle(Font.UNDERLINE);
                fontTable.SetColor(0, 0, 255);

                return fontTable;
            }
        }

        private bool m_IsFooterSwitchedOff = false;
        public bool IsFooterSwitchedOff {
            get { return m_IsFooterSwitchedOff; }
            set { m_IsFooterSwitchedOff = value; }
        }

        private int m_totalPagesCount = -1;
        public int TotalPagesCount {
            get { return m_totalPagesCount; }
            set { m_totalPagesCount = value; }
        }
        #endregion

        #region Constructor
        public PdfOrderPageEvent(string invoiceMail, PdfCommon.OrderType orderType) : base() {
            m_InvoiceMail = invoiceMail;
            m_OrderType = orderType;
        }
        #endregion
                
        #region Overriden Methods
        public override void OnOpenDocument(PdfWriter writer, Document document) {
            base.OnOpenDocument(writer, document);
            m_totalPages = writer.DirectContent.CreateTemplate(100, 100);
            m_totalPages.BoundingBox = new Rectangle(-20, -20, 100, 100);

            m_generatedDateTime = writer.DirectContent.CreateTemplate(165, 100);
            m_generatedDateTime.BoundingBox = new Rectangle(-20, -20, 165, 100);

            m_fontTotalPages = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        }

        public override void OnParagraphEnd(PdfWriter pdfWriter, Document pdfDocument, float paragraphPosition) {
            base.OnParagraphEnd(pdfWriter, pdfDocument, paragraphPosition);
        }

        /*public override void OnStartPage(PdfWriter writer, Document pdfDocument) {
            base.OnStartPage(writer, pdfDocument);
            CreateVisitPlanHeader(pdfDocument, writer);
            
        }*/

        public override void OnEndPage(PdfWriter writer, Document pdfDocument) {
            base.OnEndPage(writer, pdfDocument);
            if (!m_IsFooterSwitchedOff) {
                CreateOrderFooter(pdfDocument, writer, m_InvoiceMail, m_OrderType);
            }
        }

        public override void OnCloseDocument(PdfWriter writer, Document document) {
            base.OnCloseDocument(writer, document);
            //total pages
            m_totalPages.BeginText();
            m_totalPages.SetFontAndSize(m_fontTotalPages, m_FooterTextSize);
            m_totalPages.SetTextMatrix(0, 0);

            int pageNumber = 0;
            if (m_totalPagesCount > 0) {
                pageNumber = m_totalPagesCount;
            } else {
                pageNumber = writer.PageNumber - 1;
            }
            m_totalPages.ShowText(Convert.ToString(pageNumber));
            m_totalPages.EndText();

            //generated time stamp
            m_generatedDateTime.BeginText();
            m_generatedDateTime.SetFontAndSize(m_fontTotalPages, m_FooterTextSize);
            m_generatedDateTime.SetTextMatrix(0, 0);
            m_generatedDateTime.ShowText(GetVisitPlanTimeStamp());
            m_generatedDateTime.EndText();

        }

        private void CreateOrderFooter(Document pdfDoc, PdfWriter writer, string invoiceMail, PdfCommon.OrderType orderType) {
            int textBasePos = 140;
            if (orderType == PdfCommon.OrderType.ZonaCZ || orderType == PdfCommon.OrderType.ZonaSK) {
                textBasePos = 140;
            } else if (orderType == PdfCommon.OrderType.Bg) {
                textBasePos = 30;
            }

            PdfContentByte cb = writer.DirectContent;
            cb.SaveState();
            string text = Resources.PdfGeneratorResource.Page + " " + writer.PageNumber + " / ";
            float textBase = pdfDoc.Bottom;

            cb.BeginText();
            cb.SetFontAndSize(m_fontTotalPages, m_FooterTextSize);

            float pageFromAdjust = m_fontTotalPages.GetWidthPoint(text, m_FooterTextSize);
            float totalNrAdjust = m_fontTotalPages.GetWidthPoint(" 00", m_FooterTextSize);
            pageFromAdjust = pageFromAdjust + totalNrAdjust;

            cb.SetTextMatrix(pdfDoc.Right - pageFromAdjust, textBase - textBasePos);
            cb.ShowText(text);
            cb.EndText();
                       
            if (orderType != PdfCommon.OrderType.Bg) {
                GenerateNotifTable(pdfDoc, writer, invoiceMail, orderType);
            }
                        
            cb.AddTemplate(m_totalPages, pdfDoc.Right - totalNrAdjust, textBase - textBasePos);

            float dateTimeStampWidth = m_fontTotalPages.GetWidthPoint(GetVisitPlanTimeStamp(), m_FooterTextSize);
            float dateTimeStampLeft = (pdfDoc.PageSize.Width / 2) - (dateTimeStampWidth / 2);
            cb.AddTemplate(m_generatedDateTime, dateTimeStampLeft, textBase - textBasePos);

            cb.RestoreState();

        }

        private void GenerateNotifTable(Document pdfDoc, PdfWriter writer, string invoiceMail, PdfCommon.OrderType orderType) {

            PdfPTable tableDeliveryConfirm = new PdfPTable(1);
                        
            tableDeliveryConfirm.TotalWidth = pdfDoc.PageSize.Width - pdfDoc.LeftMargin - pdfDoc.RightMargin;
            tableDeliveryConfirm.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                      
            Phrase p = GetCombinedFormatedText(Resources.PdfGeneratorResource.ConfirmInfo);
            PdfPCell cellPos = new PdfPCell(p);
            cellPos.Border = 0;
            cellPos.BorderWidthTop = 1;
            cellPos.BorderWidthLeft = 1;
            cellPos.BorderWidthRight = 1;
            cellPos.PaddingTop = 5;
            cellPos.PaddingLeft = 5;
            cellPos.PaddingRight = 5;
            cellPos.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            tableDeliveryConfirm.AddCell(cellPos);

            if (orderType == PdfCommon.OrderType.ClcBreclav) {
                cellPos = new PdfPCell(new Phrase(Resources.PdfGeneratorResource.OrderNrInfo, m_FontStandard));
                
                cellPos.Border = 0;
                cellPos.BorderWidthLeft = 1;
                cellPos.BorderWidthRight = 1;
                cellPos.PaddingTop = 5;
                cellPos.PaddingLeft = 5;
                cellPos.PaddingRight = 5;
                cellPos.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                tableDeliveryConfirm.AddCell(cellPos);
            } else if (orderType == PdfCommon.OrderType.ZonaCZ || orderType == PdfCommon.OrderType.ZonaSK) {
                p = GetCombinedFormatedText(Resources.PdfGeneratorResource.IntegralPart);
                cellPos = new PdfPCell(p);
                
                cellPos.Border = 0;
                cellPos.BorderWidthLeft = 1;
                cellPos.BorderWidthRight = 1;
                cellPos.PaddingLeft = 5;
                cellPos.PaddingRight = 5;
                cellPos.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                tableDeliveryConfirm.AddCell(cellPos);

                cellPos = new PdfPCell(new Phrase(Resources.PdfGeneratorResource.Important + ":", m_FontStandardBold));
                cellPos.Border = 0;
                cellPos.BorderWidthLeft = 1;
                cellPos.BorderWidthRight = 1;
                cellPos.PaddingLeft = 5;
                cellPos.PaddingRight = 5;
                cellPos.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                tableDeliveryConfirm.AddCell(cellPos);

                string companyName = "";
                if (orderType == PdfCommon.OrderType.ZonaSK) {
                    companyName = "OTIS Výťahy, s.r.o.";
                } else {
                    companyName = "OTIS a.s.";
                }

                string strOderInfozona = String.Format(Resources.PdfGeneratorResource.OrderNrInfoZona, invoiceMail, companyName);
                
                p = GetCombinedFormatedText(strOderInfozona);
                cellPos = new PdfPCell(p);
                cellPos.Border = 0;
                cellPos.BorderWidthLeft = 1;
                cellPos.BorderWidthRight = 1;
                cellPos.BorderWidthBottom = 1;
                cellPos.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                cellPos.PaddingLeft = 5;
                cellPos.PaddingRight = 5;
                cellPos.PaddingBottom = 8;
                tableDeliveryConfirm.AddCell(cellPos);


            }

            if (orderType == PdfCommon.OrderType.ClcBreclav) {
                string strInvoiceInfo = String.Format(Resources.PdfGeneratorResource.EInvoiceInfo, invoiceMail);
                cellPos = new PdfPCell(new Phrase(strInvoiceInfo, m_FontStandard));
                cellPos.Border = 0;
                
                cellPos.BorderWidthLeft = 1;
                cellPos.BorderWidthRight = 1;
                
                cellPos.PaddingTop = 5;
                cellPos.PaddingLeft = 5;
                cellPos.PaddingRight = 5;
                cellPos.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                tableDeliveryConfirm.AddCell(cellPos);

                string integralPart = (Resources.PdfGeneratorResource.IntegralPartBV);
                var pBv = GetCombinedFormatedText(integralPart);
                cellPos = new PdfPCell(pBv);
                cellPos.Border = 0;
                cellPos.BorderWidthBottom = 1;
                cellPos.BorderWidthLeft = 1;
                cellPos.BorderWidthRight = 1;
                cellPos.PaddingBottom = 8;
                cellPos.PaddingTop = 5;
                cellPos.PaddingLeft = 5;
                cellPos.PaddingRight = 5;
                cellPos.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                tableDeliveryConfirm.AddCell(cellPos);
            }

            int bootomMarginDelta = 25;
            
            tableDeliveryConfirm.WriteSelectedRows(0, -1, pdfDoc.LeftMargin, (pdfDoc.BottomMargin - bootomMarginDelta), writer.DirectContent);
        }

        private Phrase GetCombinedFormatedText(string strText) {
            List<string> strItems = GetCombineTextItems(strText);
                       
            Phrase p = new Phrase();
            foreach (string strItem in strItems) {
                if (strItem.StartsWith("<b>")) {
                    Chunk chunk = new Chunk(strItem.Replace("<b>", "").Replace("</b>", ""), m_FontStandardBold);
                    p.Add(chunk);
                } else if (strItem.StartsWith("<a")) {
                    int posAEnd = strItem.IndexOf(">");
                    string linkText = strItem.Substring(posAEnd + 1);
                    linkText = linkText.Replace("</a>", "");

                    int posHrefStart = strItem.IndexOf("href=");
                    int posHrefEnd = strItem.IndexOf(">");
                    string linkUrl = strItem.Substring(posHrefStart, posHrefEnd - posHrefStart).Replace("href=", "");
                    Chunk chunk = new Chunk(linkText, m_FontLink);
                    chunk.SetAnchor(linkUrl);
                    p.Add(chunk);
                } else {
                    Chunk chunk = new Chunk(strItem, m_FontStandard);
                    p.Add(chunk);
                }
            }

            return p;
        }

        private List<string> GetCombineTextItems(string strText) {


            List<string> strItems = new List<string>();

            int iPosStart = 0;
            int iPosEnd = 0;
            while (iPosStart >= 0) {
                int iPosStartA = strText.IndexOf("<a", iPosStart);
                int iPosStartB = strText.IndexOf("<b>", iPosStart);

                TextItemType textItemType = TextItemType.Text;
                if (iPosStartA == -1) {
                    iPosStart = iPosStartB;
                    textItemType = TextItemType.B;
                } else if (iPosStartB == -1) {
                    iPosStart = iPosStartA;
                    textItemType = TextItemType.A;
                } else if (iPosStartA < iPosStartB) {
                    iPosStart = iPosStartA;
                    textItemType = TextItemType.A;
                } else if (iPosStartB < iPosStartA) {
                    iPosStart = iPosStartB;
                    textItemType = TextItemType.B;
                }


                if (iPosStart >= 0) {

                    strItems.Add(strText.Substring(iPosEnd, iPosStart - iPosEnd));
                    if (textItemType == TextItemType.A) {
                        iPosEnd = strText.IndexOf("</a>", iPosStart);
                        if (iPosEnd > -1) {
                            iPosEnd += "</a>".Length;
                        }
                    } else if (textItemType == TextItemType.B) {
                        iPosEnd = strText.IndexOf("</b>", iPosStart);
                        if (iPosEnd > -1) {
                            iPosEnd += "</b>".Length;
                        }
                    }

                    strItems.Add(strText.Substring(iPosStart, iPosEnd - iPosStart));
                    iPosStart = iPosEnd;
                } else {
                    strItems.Add(strText.Substring(iPosEnd));
                }

            }

            return strItems;
        }

        private string GetVisitPlanTimeStamp() {
            return Resources.PdfGeneratorResource.Generated + " " + DateTime.Now.ToString("dd.MM.yyyy HH:mm");
        }
        #endregion

        
    }
}