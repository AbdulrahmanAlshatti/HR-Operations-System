using HR_Operations_System.Business;
using HR_Operations_System.Data;
using HR_Operations_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfReportGenerator;
using System.Globalization;

namespace HR_Operations_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private IRepository _rep;
        private readonly ILogger<ReportsController> _logger;

        private readonly AppDbContext _db;

        public ReportsController(IRepository rep, ILogger<ReportsController> logger, AppDbContext db)
        {
            _db = db;
            _rep = rep;
            _logger = logger;
        }

        public class AttendanceRequestBody
        {
            public DateTime fromDate { get; set; }
            public DateTime toDate { get; set; }
        }

        [HttpPost]
        [Route("GetAttendanceReport")]
        public async Task<ActionResult> GetAttendanceOfDay(AttendanceRequestBody body)
        {
            var fromDate = body.fromDate.Date;
            var toDate = body.toDate.Date;
            var data2 = await _db.Attendances.Where(x => x.IODateTime >= fromDate.Date && x.IODateTime < toDate.Date.AddDays(1)).Include(x => x.Employee).ToListAsync();
            var data = data2.OrderBy(x => x.IODateTime).ToList();

            var filePath = "report.pdf";
            PdfData pdfData = new PdfData();

            pdfData.Title = "حركة الدخول والخروج";
            TextElement fromHeader = new TextElement($"من: {fromDate.ToMyDate()}", TextAlignment.AlwaysCentered);
            TextElement toHeader = new TextElement($"الى: {toDate.ToMyDate()}", TextAlignment.AlwaysCentered);
            pdfData.PageHeaderElements.Add(new GridSection([fromHeader, toHeader]));

            List<string> headers = [
                "رقم البصمة",
                "اسم الموظف",
                "نوع الحركة",
                "وقت",
            ];

            if (data.Any())
            {
                var currentDaysDate = data.First().IODateTime;

                List<List<string>> tableData = new();

                foreach (Attendance attendance in data)
                {
                    if (attendance.IODateTime >= currentDaysDate.AddDays(1))
                    {
                        pdfData.PageContentElements.Add(new GridSection(currentDaysDate.Date.ToMyDate()));
                        pdfData.PageContentElements.Add(new Table(headers, tableData));
                        tableData = new();
                        currentDaysDate = attendance.IODateTime;
                    }
                    List<string> row = [attendance.FingerCode.ToString(), attendance.Employee.NameA, GetTrTypeString(attendance.TrType), attendance.IODateTime.ToString("hh:mm tt")];
                    tableData.Add(row);

                }
                pdfData.PageContentElements.Add(new GridSection(currentDaysDate.Date.ToMyDate()));
                pdfData.PageContentElements.Add(new Table(headers, tableData));
            }




            PdfPrinter.PrintDoc(pdfData, filePath);


            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/pdf", "downloaded-file.pdf");
        }

        string GetTrTypeString(int trType)
        {
            switch (trType)
            {
                case 0: return "دخول";
                case 1: return "خروج";
                case 2: return "غير معرف";
                default: return "";
            }
        }

        [HttpPost]
        [Route("GetEmployeeReport")]
        public async Task<ActionResult> GetEmployeeReport(EmployeCalculationRequestBody entity)
        {
            var attendanceData = await _rep.GetAsync<Attendance>();
            var filePath = "report.pdf";
            PdfData pdfData = new PdfData();

            List<string> header = [
                "test",
                "test",
                "test",
                "test",
            ];

            List<List<string>> data = new List<List<string>>();
            for (int i = 0; i < 20; i++)
            {
                data.Add([
                    "test",
                    "test",
                    "test",
                    "test",
                ]);
            }
            ;

            Table table = new Table(header, data);
            pdfData.PageContentElements.Add(table);
            PdfPrinter.PrintDoc(pdfData, filePath);

            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/pdf", "downloaded-file.pdf");
        }


    }
}



