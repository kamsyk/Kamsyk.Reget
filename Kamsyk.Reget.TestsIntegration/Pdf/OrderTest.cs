using Kamsyk.Reget.PdfGenerator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Kamsyk.Reget.PdfGenerator.PdfCommon;
using static Kamsyk.Reget.PdfGenerator.PdfOrderPageEvent;

namespace Kamsyk.Reget.TestsIntegration.Pdf {
    public class OrderTest {
        [Fact()]
        public void GenerateCzZonaOrder() {
            //Arrange
            string pdfFileName = @"c:\Temp\order.pdf";

            //Act
            var pdfOrder = new PdfOrder();
            var pdfByte = pdfOrder.GenerateOrder(
                OrderType.ZonaCZ,
                @"D:\Develop\CSharpProjects\Kamsyk.Reget\Kamsyk.Reget\Content\Images\LogoBw.png",
                null,
                null,
                null,
                null,
                "O4569",
                "Breclav",
                "Praha",
                "Matulikova",
                "mm.ps@cz",
                "605 236 987",
                null,
                "Fruta",
                "po@ok.cz",
                "752 245 963",
                "45698",
                "Kolin",
                "Objednavam ...",
                "210 CZK",
                DateTime.Now,
                "Contract A45",
                "bv@otis.com",
                "cz-CZ");
            File.WriteAllBytes(pdfFileName, pdfByte);

            //Assert
            Assert.True(File.Exists(pdfFileName));
            
        }
    }
}
