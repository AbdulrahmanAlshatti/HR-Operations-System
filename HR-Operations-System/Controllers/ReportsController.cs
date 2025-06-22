using HR_Operations_System.Business;
using HR_Operations_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.IO;
using static HR_Operations_System.Controllers.AttendancesController;
using PdfReportGenerator;

namespace HR_Operations_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private IRepository _rep;
        private readonly ILogger<NodesController> _logger;

        public ReportsController(IRepository rep, ILogger<NodesController> logger)
        {
            _rep = rep;
            _logger = logger;
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
            };

            Table table = new Table(header, data);
            pdfData.PageContentElements.Add(table);
            PdfPrinter.PrintDoc(pdfData, filePath);

            var mimeType = "application/pdf";
            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, mimeType, "downloaded-file.pdf");
        }


    }
}



