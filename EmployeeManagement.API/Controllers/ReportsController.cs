using System.Threading.Tasks;
using EmployeeManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly ReportService _reportService;

        public ReportsController(ReportService reportService)
        {
            _reportService = reportService;
        }

        // 1. Employee Directory EXCEL
        [HttpGet("employees/excel")]
        public async Task<IActionResult> GetEmployeesExcel()
        {
            var fileContents = await _reportService.GenerateEmployeesExcelAsync();
            string fileName = "Employee_Directory.xlsx";
            string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(fileContents, mimeType, fileName);
        }

        // 2. Employee Directory PDF
        [HttpGet("employees/pdf")]
        public async Task<IActionResult> GetEmployeesPdf()
        {
            var fileContents = await _reportService.GenerateEmployeesPdfAsync();
            string fileName = "Employee_Directory.pdf";
            string mimeType = "application/pdf";
            return File(fileContents, mimeType, fileName);
        }

        // 3. Attendance EXCEL
        [HttpGet("attendance/excel")]
        public async Task<IActionResult> GetAttendanceExcel()
        {
            var fileContents = await _reportService.GenerateAttendanceExcelAsync();
            string fileName = "Attendance_Report.xlsx";
            string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(fileContents, mimeType, fileName);
        }

        // 4. Attendance PDF
        [HttpGet("attendance/pdf")]
        public async Task<IActionResult> GetAttendancePdf()
        {
            var fileContents = await _reportService.GenerateAttendancePdfAsync();
            string fileName = "Attendance_Report.pdf";
            string mimeType = "application/pdf";
            return File(fileContents, mimeType, fileName);
        }

        // 5. Salary Report PDF
        [HttpGet("salary/pdf")]
        public async Task<IActionResult> GetSalaryPdf()
        {
            var fileContents = await _reportService.GenerateSalaryPdfAsync();
            string fileName = "Salary_Payroll_Report.pdf";
            string mimeType = "application/pdf";
            return File(fileContents, mimeType, fileName);
        }

        // 6. Department Headcount Summary PDF
        [HttpGet("departments/pdf")]
        public async Task<IActionResult> GetDepartmentsPdf()
        {
            var fileContents = await _reportService.GenerateDepartmentPdfAsync();
            string fileName = "Department_Summary_Report.pdf";
            string mimeType = "application/pdf";
            return File(fileContents, mimeType, fileName);
        }
    }
}
