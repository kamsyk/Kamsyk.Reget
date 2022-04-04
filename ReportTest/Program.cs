
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;

using GemBox.Spreadsheet;

using System.Threading.Tasks;


namespace ReportTest {
    class Program {
        static void Main(string[] args) {

            //MemoryStream stream = new MemoryStream();

            //// Open a SpreadsheetDocument based on a stream.
            //SpreadsheetDocument spreadsheetDocument =
            //SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);

            //// Add a new worksheet.
            //WorksheetPart newWorksheetPart = spreadsheetDocument.WorkbookPart.AddNewPart<WorksheetPart>();
            //newWorksheetPart.Worksheet = new Worksheet(new SheetData());
            //newWorksheetPart.Worksheet.Save();

            //Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>();
            //string relationshipId = spreadsheetDocument.WorkbookPart.GetIdOfPart(newWorksheetPart);

            //// Get a unique ID for the new worksheet.
            //uint sheetId = 1;
            //if (sheets.Elements<Sheet>().Count() > 0) {
            //    sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            //}

            //// Give the new worksheet a name.
            //string sheetName = "Sheet" + sheetId;

            //// Append the new worksheet and associate it with the workbook.
            //Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetName };
            //sheets.Append(sheet);
            //spreadsheetDocument.WorkbookPart.Workbook.Save();

            //// Close the document handle.
            //spreadsheetDocument.Close();

            //// Caller must close the stream.

            //using (FileStream fileStream = System.IO.File.Create(@"c:\temp\myxls.xlsx", (int)stream.Length)) {
            //    // Fill the bytes[] array with the stream data
            //    byte[] bytesInStream = new byte[stream.Length];
            //    stream.Read(bytesInStream, 0, (int)bytesInStream.Length);

            //    // Use FileStream object to write to the specified file
            //    fileStream.Write(bytesInStream, 0, bytesInStream.Length);
            //}

            //stream.Close();

            //MemoryStream ms;

            //Excel exc = new Excel();
            //var xlsDoc = exc.GetNewXlsDocMemory();
            //var wb = exc.AddWorkbook(xlsDoc);
            //exc.AddWorkSheet(wb);
            ////xlsDoc.Save();
            //xlsDoc.Close();

            //ms = exc.GetWorkbookMemoryStream();
            ////ms.Position = 0;

            //File.WriteAllBytes(@"c:\temp\myxls8.xlsx", ms.ToArray());

            ////ms.Flush();
            //ms.Close();

            //using (FileStream fs = new FileStream(@"c:\temp\myxls.xlsx", FileMode.OpenOrCreate)) {
            //    ms.CopyTo(fs);
            //    fs.Flush();
            //}


            // Set license key to use GemBox.Spreadsheet in Free mode.
            //SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

            //// Create a new empty Excel file.
            //var workbook = new ExcelFile();

            //// Create a new worksheet and set cell A1 value to 'Hello world!'.
            ////workbook.Worksheets.Add("Sheet 1").Cells["A1"].Value = "Hello world!";
            //for (int i = 1; i < 150; i++) {
            //    var ws = workbook.Worksheets.Add("Sheet " + i);
            //    for (int j = 1; j < 500; j++) {
            //        ws.Cells["A" + j].Value = "j";
            //    }
            //}

            //// Save to XLSX file.
            //workbook.Save("Spreadsheet.xlsx");

            //var document = SpreadsheetDocument.Open(@"c:\temp\test.xlsx", true);
            //var wbPart = document.WorkbookPart;

            //// Add a blank WorksheetPart.
            //WorksheetPart newWorksheetPart = document.WorkbookPart.AddNewPart<WorksheetPart>();
            //newWorksheetPart.Worksheet = new Worksheet(new SheetData());

            //Sheets sheets = document.WorkbookPart.Workbook.GetFirstChild<Sheets>();
            //string relationshipId = document.WorkbookPart.GetIdOfPart(newWorksheetPart);

            //// Get a unique ID for the new worksheet.
            //uint sheetId = 1;
            //if (sheets.Elements<Sheet>().Count() > 0) {
            //    sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            //}

            //// Give the new worksheet a name.
            //string sheetName = "Sheet" + sheetId;

            //// Append the new worksheet and associate it with the workbook.
            //Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetName };
            //sheets.Append(sheet);


            //document.Save();
            //document.Close();

            //SpreadsheetDocument spreadSheet = SpreadsheetDocument.Create(@"c:\temp\test.xlsx", SpreadsheetDocumentType.Workbook);


            ////using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Open(docName, true)) {
            //    // Add a blank WorksheetPart.
            //    WorksheetPart newWorksheetPart = spreadSheet.WorkbookPart.AddNewPart<WorksheetPart>();
            //    newWorksheetPart.Worksheet = new Worksheet(new SheetData());

            //    Sheets sheets = spreadSheet.WorkbookPart.Workbook.GetFirstChild<Sheets>();
            //    string relationshipId = spreadSheet.WorkbookPart.GetIdOfPart(newWorksheetPart);

            //    // Get a unique ID for the new worksheet.
            //    uint sheetId = 1;
            //    if (sheets.Elements<Sheet>().Count() > 0) {
            //        sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            //    }

            //    // Give the new worksheet a name.
            //    string sheetName = "Sheet" + sheetId;

            //    // Append the new worksheet and associate it with the workbook.
            //    Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetName };
            //    sheets.Append(sheet);
            ////}

            //spreadSheet.Save();
            //spreadSheet.Close();

            // Create a spreadsheet document by supplying the filepath.
            // By default, AutoSave = true, Editable = true, and Type = xlsx.
            //SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.
            //    Create(@"c:\temp\test.xlsx", SpreadsheetDocumentType.Workbook);

            var SpreadsheetStream = new MemoryStream();
            SpreadsheetDocument spreadsheetDocument =
SpreadsheetDocument.Create(SpreadsheetStream, SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart to the document.
            WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();

            // Add a WorksheetPart to the WorkbookPart.
            WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            // Add Sheets to the Workbook.
            Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.
                AppendChild<Sheets>(new Sheets());

            // Append a new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet() {
                Id = spreadsheetDocument.WorkbookPart.
                GetIdOfPart(worksheetPart), SheetId = 1, Name = "mySheet"
            };
            sheets.Append(sheet);

            //workbookpart.Workbook.Save();
           //spreadsheetDocument.Save();

           
            // Close the document.
            spreadsheetDocument.Close();
            //SpreadsheetStream.Close();

            //SpreadsheetStream.Flush();
            //ms.Close();

            //using (FileStream fs = new FileStream(@"c:\temp\myxls2.xlsx", FileMode.OpenOrCreate)) {
            //SpreadsheetStream.CopyTo(fs);
            //fs.Flush();

            // SpreadsheetStream.Seek(0, SeekOrigin.Begin);
            SpreadsheetStream.Position = 0;

            File.WriteAllBytes(@"c:\temp\myxls2.xlsx", SpreadsheetStream.ToArray());

            //using (FileStream fileStream = System.IO.File.Create(@"c:\temp\myxls.xlsx", (int)SpreadsheetStream.Length)) {
            //    // Fill the bytes[] array with the stream data
            //    byte[] bytesInStream = new byte[SpreadsheetStream.Length];
            //    SpreadsheetStream.Read(bytesInStream, 0, (int)bytesInStream.Length);

            //    // Use FileStream object to write to the specified file
            //    fileStream.Write(bytesInStream, 0, bytesInStream.Length);
            //}

            ////}

            SpreadsheetStream.Close();
            //spreadsheetDocument.Close();
            //spreadsheetDocument.Close();
        }
        }
}
