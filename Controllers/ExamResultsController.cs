using Fair_Recruitment_Web_Result.Handlers;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;
using OfficeOpenXml;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.IO;
using System.Threading.Tasks;
using static Fair_Recruitment_Web_Result.Models.Database;
using static Fair_Recruitment_Web_Result.Models.PageModels;
using iText.IO.Image;
using Fair_Recruitment_Web_Result.Data;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
namespace Fair_Recruitment_Web_Result.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamResultsController: ControllerBase
    {
        private readonly DbHandler _dbHandler;
        private readonly ApplicationDbContext _applicationDbContext;
        public ExamResultsController(DbHandler dbHandler, ApplicationDbContext applicationDbContext)
        {
            _dbHandler = dbHandler;
            _applicationDbContext= applicationDbContext;
        }

        [HttpGet("result")]
        public async Task<IActionResult> GetExamResult(string id)
        {
            var Results = await _dbHandler.GetExamList(id);
            if(Results == null)
                return NotFound();
            return Ok(Results);
        }

        [HttpPost("resultTable")]
        public async Task<IActionResult> GetExamResultTableById()
        {
            string draw = Request.Form["draw"];
            int start = Convert.ToInt32(Request.Form["start"]);
            int length = Convert.ToInt32(Request.Form["length"]);
            string search = Request.Form["search[value]"];
            string sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"] + "][data]"];
            string sortDirection = Request.Form["order[0][dir]"];
            var  ExamId = Request.Form["ExamId"].ToString();

            var Results = await _dbHandler.GetExamResultByExamId(ExamId);
            int recordsTotal = Results.Count();

            if (!string.IsNullOrEmpty(search))
            {
                Results = Results.Where(x => x.roll != null && x.roll.ToLower().Contains(search.ToLower()) ||
                (x.name != null && x.name.ToLower().Contains(search.ToLower())) 
                ).ToList();
            }
            int recordsFiltered = Results.Count();

            //Sorting
            if (!string.IsNullOrEmpty(sortColumn))
            {
                Results = Results.AsQueryable().OrderBy(sortColumn + " " + sortDirection).ToList();
            }
            //Paging
            Results = Results.Skip(start).Take(length).ToList();
            return Ok(new { draw, recordsTotal, recordsFiltered, data = Results });
        }

        [HttpGet("excelDownload")]
        public async Task<IActionResult> DownloadExcelFile(string ExamId)
        {
            var data = await _dbHandler.GetExamResultByExamId(ExamId);

            byte[]? excelFile = await GenerateExamResultExcelReportAsync(data);
            string excelName =await GetFileName(ExamId);
            excelName = $"{excelName}.xlsx";
            return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
        public async Task<string?> GetFileName(string exam_code)
        {
            Exam? Exam = new Exam();
            Exam = await _applicationDbContext.Exams.Where(x => x.exam_code == exam_code).FirstOrDefaultAsync();
            string FileName = $"{Exam.exam_date}_{Exam.office_name}";
            return FileName;
        }
        //public async Task<byte[]> GenerateExamResultExcelReportAsync(List<ExamResultDTO> data)
        //{
        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        //    using (var package = new ExcelPackage())
        //    {
        //        var worksheet = package.Workbook.Worksheets.Add("Sheet1");
        //        worksheet.Cells[1, 1].Value = "SL."; 
        //        worksheet.Cells[1, 2].Value = "Roll Number";
        //        worksheet.Cells[1, 3].Value = "Name";
        //        worksheet.Cells[1, 4].Value = "Similarity";
        //        worksheet.Cells[1, 5].Value = "Timestamp";
        //        worksheet.Cells[1, 6].Value = "Decision";
        //        worksheet.Cells[1, 7].Value = "Enrolled Image";
        //        worksheet.Cells[1, 8].Value = "Captured Image";

        //        for (int i = 0; i < data.Count; i++)
        //        {
        //            worksheet.Cells[i + 2, 1].Value = i + 1;
        //            worksheet.Cells[i + 2, 2].Value = data[i].roll_number;
        //            worksheet.Cells[i + 2, 3].Value = data[i].name;
        //            worksheet.Cells[i + 2, 4].Value = data[i].similarity;
        //            worksheet.Cells[i + 2, 5].Value = data[i].timestamp?.ToString("yyyy-MM-dd HH:mm:ss");
        //            worksheet.Cells[i + 2, 6].Value = data[i].decision.Value ? "Accept" : "Reject";

        //            if (!string.IsNullOrEmpty(data[i].enrolled_image))
        //            {
        //                byte[] enrolledImageBytes = Convert.FromBase64String(data[i].enrolled_image);
        //                using (var ms = new MemoryStream(enrolledImageBytes))
        //                {
        //                    var pic = worksheet.Drawings.AddPicture($"EnrolledImage{i}", ms);
        //                    pic.SetPosition(i + 1, 0, 6, 0); // Adjust the position as needed
        //                    pic.SetSize(80, 80); // Adjust the size as needed
        //                    worksheet.Row(i+2).Height = 105 * 0.75;
        //                    //worksheet.Column(7).Width = 110 / 7.5;
        //                }
        //            }

        //            if (!string.IsNullOrEmpty(data[i].captured_image))
        //            {
        //                byte[] capturedImageBytes = Convert.FromBase64String(data[i].captured_image);
        //                using (var ms = new MemoryStream(capturedImageBytes))
        //                {
        //                    var pic = worksheet.Drawings.AddPicture($"CapturedImage{i}", ms);
        //                    pic.SetPosition(i + 1, 0, 7, 0); // Adjust the position as needed
        //                    pic.SetSize(80, 80); // Adjust the size as needed
        //                    worksheet.Row(i + 2).Height = 105 * 0.75;
        //                    //worksheet.Column(8).Width = 110 / 7.5;
        //                }
        //            }
        //        }

        //        for (int i = 1; i <= 8; i++)
        //        {
        //            worksheet.Column(i).AutoFit();
        //            worksheet.Column(i).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //            worksheet.Column(i).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        //        }

        //        return await package.GetAsByteArrayAsync();
        //    }
        //}

        public async Task<byte[]?> GenerateExamResultExcelReportAsync(List<ExamResultDTO> data)
        {
            if (data.Count < 1)
            {
                return null;
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            try
            {
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                    // "Gate" header to merge over Captured Image, Similarity, and Decision (columns 7 to 9)
                    worksheet.Cells[1, 1, 1, 6].Merge = true;  // Merge cells from 7 to 9
                    worksheet.Cells[1, 1].Value = "Result";      // Set merged cell value to "Gate"
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[1, 1].Style.Font.Bold = true;

                    worksheet.Cells[2, 1].Value = "SL.";
                    worksheet.Cells[2, 2].Value = "Position";
                    worksheet.Cells[2, 3].Value = "Roll";
                    worksheet.Cells[2, 4].Value = "Name";
                    worksheet.Cells[2, 5].Value = "Marks";
                    worksheet.Cells[2, 6].Value = "Enrolled Image";

                    // "Gate" header to merge over Captured Image, Similarity, and Decision (columns 7 to 9)
                    worksheet.Cells[1, 7, 1, 10].Merge = true;  // Merge cells from 7 to 9
                    worksheet.Cells[1, 7].Value = "Gate";      // Set merged cell value to "Gate"
                    worksheet.Cells[1, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[1, 7].Style.Font.Bold = true;

                    worksheet.Cells[2, 7].Value = "Captured Image";
                    worksheet.Cells[2, 8].Value = "Similarity";
                    worksheet.Cells[2, 9].Value = "Decision";
                    worksheet.Cells[2, 10].Value = "Timestamp";

                    // "Room" header to merge over Captured Image, Similarity, and Decision (columns 11 to 13)
                    worksheet.Cells[1, 11, 1, 14].Merge = true;  // Merge cells from 11 to 13
                    worksheet.Cells[1, 11].Value = "Room";       // Set merged cell value to "Room"
                    worksheet.Cells[1, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[1, 11].Style.Font.Bold = true;

                    worksheet.Cells[2, 11].Value = "Captured Image";
                    worksheet.Cells[2, 12].Value = "Similarity";
                    worksheet.Cells[2, 13].Value = "Decision";
                    worksheet.Cells[2, 14].Value = "Timestamp";

                    for (int i = 0; i < data.Count; i++)
                    {
                        worksheet.Cells[i + 3, 1].Value = i + 1;
                        worksheet.Cells[i + 3, 2].Value = data[i].position;
                        worksheet.Cells[i + 3, 3].Value = data[i].roll;
                        worksheet.Cells[i + 3, 4].Value = data[i].name;
                        worksheet.Cells[i + 3, 5].Value = data[i].marks;


                        if (!string.IsNullOrEmpty(data[i].enrolled_image))
                        {
                            byte[] enrolledImageBytes = Convert.FromBase64String(data[i].enrolled_image);
                            using (var ms = new MemoryStream(enrolledImageBytes))
                            {
                                var pic = worksheet.Drawings.AddPicture($"EnrolledImage{i}", ms);
                                pic.SetPosition(i + 2, 10, 5, 10); // Adjust the position as needed
                                pic.SetSize(80, 80); // Adjust the size as needed
                                worksheet.Row(i + 3).Height = 105 * 0.75;
                                //worksheet.Column(7).Width = 110 / 7.5;
                            }
                        }

                        if (!string.IsNullOrEmpty(data[i].gate_captured_image))
                        {
                            byte[] capturedImageBytes = Convert.FromBase64String(data[i].gate_captured_image);
                            using (var ms = new MemoryStream(capturedImageBytes))
                            {
                                var pic = worksheet.Drawings.AddPicture($"GateCapturedImage{i}", ms);
                                pic.SetPosition(i + 2, 10, 6, 10); // Adjust the position as needed
                                pic.SetSize(80, 80); // Adjust the size as needed
                                worksheet.Row(i + 3).Height = 105 * 0.75;
                                //worksheet.Column(8).Width = 110 / 7.5;
                            }
                        }

                        worksheet.Cells[i + 3, 8].Value = data[i].gate_similarity;
                        if (data[i].gate_decision.HasValue)
                        {
                            worksheet.Cells[i + 3, 9].Value = data[i].gate_decision.Value ? "Accept" : "Reject";
                        }
                        worksheet.Cells[i + 3, 10].Value = data[i].gate_timestamp?.ToString("yyyy-MM-dd HH:mm:ss");

                        if (!string.IsNullOrEmpty(data[i].room_captured_image))
                        {
                            byte[] capturedImageBytes = Convert.FromBase64String(data[i].room_captured_image);
                            using (var ms = new MemoryStream(capturedImageBytes))
                            {
                                var pic = worksheet.Drawings.AddPicture($"RoomCapturedImage{i}", ms);
                                pic.SetPosition(i + 2, 10, 10, 10); // Adjust the position as needed
                                pic.SetSize(80, 80); // Adjust the size as needed
                                worksheet.Row(i + 3).Height = 105 * 0.75;
                                //worksheet.Column(8).Width = 110 / 7.5;
                            }
                        }

                        worksheet.Cells[i + 3, 12].Value = data[i].room_similarity;
                        if (data[i].room_decision.HasValue)
                        {
                            worksheet.Cells[i + 3, 13].Value = data[i].room_decision.Value ? "Accept" : "Reject";
                        }
                        worksheet.Cells[i + 3, 14].Value = data[i].room_timestamp?.ToString("yyyy-MM-dd HH:mm:ss");
                    }

                    for (int i = 1; i <= data.Count + 2; i++)
                    {
                        //Add Border
                        worksheet.Cells[i, 6].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    }

                    for (int i = 1; i <= 14; i++)
                    {
                        worksheet.Column(i).AutoFit();
                        worksheet.Column(i).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Column(i).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }

                    return await package.GetAsByteArrayAsync();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //[HttpGet("pdfDownload")]
        //public async Task<IActionResult> DownloadPdfFile(string ExamId)
        //{
        //    var data = await _dbHandler.GetExamResultByExamId(ExamId);
        //    byte[] excelFile = await GenerateExamResultExcelReportForPDfDownloadAsync(data);
        //    byte[] pdfBytes =await ConvertExcelToPdf(excelFile , ExamId);   // Convert Excel to PDF
        //    string FileName =await GetFileName(ExamId);
        //    FileName = $"{FileName}.pdf";
        //    return File(pdfBytes, "application/pdf", FileName);
        //}

        //private async Task<byte[]> ConvertExcelToPdf(byte[] excelData, string id)
        //{
        //    var ExamList = await _dbHandler.GetExamList(id);
        //    using (var excelStream = new MemoryStream(excelData))
        //    using (var pdfStream = new MemoryStream())
        //    {
        //        using (var package = new ExcelPackage(excelStream))
        //        {
        //            var pdfDoc = new PdfDocument(new PdfWriter(pdfStream));
        //            var document = new Document(pdfDoc);
        //            if (ExamList != null)
        //            {
        //                // Add headers to the document
        //                Paragraph header1 = new Paragraph($"{ExamList[0].office_name}")
        //                    .SetTextAlignment(TextAlignment.CENTER)
        //                    .SetFontSize(14).SetFixedLeading(8);

        //                Paragraph header2 = new Paragraph("Attendance Report")
        //                    .SetTextAlignment(TextAlignment.CENTER)
        //                    .SetBold()
        //                    .SetFontSize(14).SetFixedLeading(8);

        //                Paragraph header3 = new Paragraph($"Post Name: {ExamList[0].post_name}")
        //                    .SetTextAlignment(TextAlignment.CENTER)
        //                    .SetFontSize(10).SetFixedLeading(8);

        //                Paragraph header4 = new Paragraph($"{ExamList[0].exam_type_name}")
        //                    .SetTextAlignment(TextAlignment.CENTER)
        //                    .SetFontSize(10).SetFixedLeading(8);

        //                Paragraph header5 = new Paragraph($"Date: {ExamList[0].exam_date}")
        //                    .SetTextAlignment(TextAlignment.CENTER)
        //                    .SetFontSize(10).SetFixedLeading(8);

        //                document.Add(header1);
        //                document.Add(header2);
        //                document.Add(header3);
        //                document.Add(header4);
        //                document.Add(header5);
        //                document.Add(new Paragraph("\n")); // Adds a space between headers and table
        //            }

        //            // Process each worksheet in the Excel package
        //            foreach (var worksheet in package.Workbook.Worksheets)
        //            {
        //                // Set column widths, adjust as necessary for your data
        //                float[] columnWidths = new float[worksheet.Dimension.End.Column];
        //                for (int i = 0; i < worksheet.Dimension.End.Column; i++)
        //                {
        //                    columnWidths[i] = (i == worksheet.Dimension.End.Column - 2 || i == worksheet.Dimension.End.Column - 1) ? 2 : 1;
        //                }
        //                columnWidths[0] = 0.5f;  // Reduce the size of the first column
        //                columnWidths[5] = 0.5f;  // Example of setting another column width
        //                columnWidths[4] = 1.5f;  // Increase the width of another column

        //                // Create a table with specified column widths
        //                iText.Layout.Element.Table table = new iText.Layout.Element.Table(UnitValue.CreatePercentArray(columnWidths));
        //                table.SetWidth(UnitValue.CreatePercentValue(100));

        //                // Add table headers with bold formatting
        //                for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
        //                {
        //                    Cell headerCell = new Cell().Add(new Paragraph(worksheet.Cells[1, col].Text).SetBold().SetFontSize(10));
        //                    table.AddHeaderCell(headerCell);
        //                }

        //                // Iterate over the rows in the worksheet
        //                for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
        //                {
        //                    for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
        //                    {
        //                        var cell = worksheet.Cells[row, col];
        //                        if ((col == worksheet.Dimension.End.Column - 1 || col == worksheet.Dimension.End.Column) && !string.IsNullOrWhiteSpace(cell.Text))
        //                        {
        //                            try
        //                            {
        //                                byte[] imageBytes = Convert.FromBase64String(cell.Text);
        //                                ImageData imageData = ImageDataFactory.Create(imageBytes);
        //                                Image image = new Image(imageData).ScaleToFit(80, 80);
        //                                Cell imageCell = new Cell()
        //                                    .Add(image)
        //                                    .SetVerticalAlignment(VerticalAlignment.MIDDLE)
        //                                    .SetHorizontalAlignment(HorizontalAlignment.CENTER)
        //                                    .SetPadding(10);
        //                                table.AddCell(imageCell);
        //                            }
        //                            catch
        //                            {
        //                                table.AddCell(new Cell().Add(new Paragraph("Invalid Image")));
        //                            }
        //                        }
        //                        else
        //                        {
        //                            Cell textCell = new Cell().Add(new Paragraph(cell.Text).SetFontSize(10));
        //                            textCell.SetTextAlignment(TextAlignment.CENTER);
        //                            textCell.SetVerticalAlignment(VerticalAlignment.MIDDLE);
        //                            textCell.SetPadding(5);
        //                            table.AddCell(textCell);
        //                        }
        //                    }
        //                }

        //                // Add the constructed table to the document
        //                document.Add(table);
        //            }

        //            // Close the document and return the generated PDF bytes
        //            document.Close();
        //            return pdfStream.ToArray();
        //        }
        //    }
        //}



        //public async Task<byte[]> GenerateExamResultExcelReportForPDfDownloadAsync(List<ExamResultDTO> data)
        //{
        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        //    using (var package = new ExcelPackage())
        //    {
        //        var worksheet = package.Workbook.Worksheets.Add("Sheet1");
        //        worksheet.Cells[1, 1].Value = "SL.";
        //        worksheet.Cells[1, 2].Value = "Roll Number";
        //        worksheet.Cells[1, 3].Value = "Name";
        //        worksheet.Cells[1, 4].Value = "Similarity";
        //        worksheet.Cells[1, 5].Value = "Timestamp";
        //        worksheet.Cells[1, 6].Value = "Decision";
        //        worksheet.Cells[1, 7].Value = "Enrolled Image";
        //        worksheet.Cells[1, 8].Value = "Captured Image";

        //        for (int i = 0; i < data.Count; i++)
        //        {
        //            worksheet.Cells[i + 2, 1].Value = i + 1;
        //            worksheet.Cells[i + 2, 2].Value = data[i].roll;
        //            worksheet.Cells[i + 2, 3].Value = data[i].name;
        //            worksheet.Cells[i + 2, 4].Value = data[i].similarity;
        //            worksheet.Cells[i + 2, 5].Value = data[i].timestamp?.ToString("yyyy-MM-dd HH:mm:ss");
        //            worksheet.Cells[i + 2, 6].Value = data[i].decision.Value ? "Accept" : "Reject";
        //            worksheet.Cells[i + 2, 7].Value = data[i].enrolled_image; // Assuming this is a base64 string
        //            worksheet.Cells[i + 2, 8].Value = data[i].captured_image; // Assuming this is a base64 string
                    
        //        }
        //        return await package.GetAsByteArrayAsync();
        //    }
        //}

    }
}
