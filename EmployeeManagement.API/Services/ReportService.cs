using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using EmployeeManagement.API.Interfaces;
using EmployeeManagement.API.Models;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EmployeeManagement.API.Services
{
    public class ReportService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAttendanceRepository _attendanceRepository;

        static ReportService()
        {
            // Set QuestPDF license to community mode
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public ReportService(IEmployeeRepository employeeRepository, IAttendanceRepository attendanceRepository)
        {
            _employeeRepository = employeeRepository;
            _attendanceRepository = attendanceRepository;
        }

        // 1. Employee Directory EXCEL
        public async Task<byte[]> GenerateEmployeesExcelAsync()
        {
            var employees = await _employeeRepository.GetAllQueryable().OrderBy(e => e.LastName).ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Employees");
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "First Name";
                worksheet.Cell(1, 3).Value = "Last Name";
                worksheet.Cell(1, 4).Value = "Email";
                worksheet.Cell(1, 5).Value = "Phone";
                worksheet.Cell(1, 6).Value = "Department";
                worksheet.Cell(1, 7).Value = "Designation";
                worksheet.Cell(1, 8).Value = "Salary";
                worksheet.Cell(1, 9).Value = "Joining Date";
                worksheet.Cell(1, 10).Value = "Status";

                var headerRange = worksheet.Range("A1:J1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightSkyBlue;

                for (int i = 0; i < employees.Count; i++)
                {
                    var emp = employees[i];
                    int row = i + 2;
                    worksheet.Cell(row, 1).Value = emp.EmployeeId;
                    worksheet.Cell(row, 2).Value = emp.FirstName;
                    worksheet.Cell(row, 3).Value = emp.LastName;
                    worksheet.Cell(row, 4).Value = emp.Email;
                    worksheet.Cell(row, 5).Value = emp.Phone;
                    worksheet.Cell(row, 6).Value = emp.Department;
                    worksheet.Cell(row, 7).Value = emp.Designation;
                    worksheet.Cell(row, 8).Value = emp.Salary;
                    worksheet.Cell(row, 8).Style.NumberFormat.Format = "$#,##0.00";
                    worksheet.Cell(row, 9).Value = emp.JoiningDate.ToString("yyyy-MM-dd");
                    worksheet.Cell(row, 10).Value = emp.Status;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        // 2. Employee Directory PDF
        public async Task<byte[]> GenerateEmployeesPdfAsync()
        {
            var employees = await _employeeRepository.GetAllQueryable().OrderBy(e => e.LastName).ToListAsync();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Employee Management System").FontSize(10).FontColor(Colors.Grey.Medium);
                            col.Item().Text("Employee Directory").FontSize(24).Bold().FontColor(Colors.Blue.Medium);
                        });
                        row.ConstantItem(100).AlignRight().Text(DateTime.Now.ToString("yyyy-MM-dd")).FontSize(10).FontColor(Colors.Grey.Medium);
                    });

                    page.Content().PaddingVertical(15).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1); // ID
                            columns.RelativeColumn(4); // Name
                            columns.RelativeColumn(5); // Email
                            columns.RelativeColumn(4); // Department
                            columns.RelativeColumn(4); // Designation
                            columns.RelativeColumn(2); // Status
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Blue.Lighten4).Padding(5).Text("ID").Bold();
                            header.Cell().Background(Colors.Blue.Lighten4).Padding(5).Text("Name").Bold();
                            header.Cell().Background(Colors.Blue.Lighten4).Padding(5).Text("Email").Bold();
                            header.Cell().Background(Colors.Blue.Lighten4).Padding(5).Text("Department").Bold();
                            header.Cell().Background(Colors.Blue.Lighten4).Padding(5).Text("Designation").Bold();
                            header.Cell().Background(Colors.Blue.Lighten4).Padding(5).Text("Status").Bold();
                        });

                        foreach (var emp in employees)
                        {
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text(emp.EmployeeId.ToString());
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text($"{emp.FirstName} {emp.LastName}");
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text(emp.Email);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text(emp.Department);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text(emp.Designation);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text(emp.Status);
                        }
                    });

                    page.Footer().AlignCenter().Text(text => { text.Span("Page "); text.CurrentPageNumber(); });
                });
            });

            return document.GeneratePdf();
        }

        // 3. Attendance EXCEL
        public async Task<byte[]> GenerateAttendanceExcelAsync()
        {
            var attendanceList = await _attendanceRepository.GetAllQueryable()
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Attendance");
                worksheet.Cell(1, 1).Value = "Date";
                worksheet.Cell(1, 2).Value = "Employee Name";
                worksheet.Cell(1, 3).Value = "Check In";
                worksheet.Cell(1, 4).Value = "Check Out";
                worksheet.Cell(1, 5).Value = "Status";

                var headerRange = worksheet.Range("A1:E1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGreen;

                for (int i = 0; i < attendanceList.Count; i++)
                {
                    var att = attendanceList[i];
                    int row = i + 2;
                    worksheet.Cell(row, 1).Value = att.Date.ToString("yyyy-MM-dd");
                    worksheet.Cell(row, 2).Value = att.Employee != null ? $"{att.Employee.FirstName} {att.Employee.LastName}" : "Unknown";
                    worksheet.Cell(row, 3).Value = att.CheckIn.ToString("hh:mm tt");
                    worksheet.Cell(row, 4).Value = att.CheckOut?.ToString("hh:mm tt") ?? "-";
                    worksheet.Cell(row, 5).Value = att.Status;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        // 4. Attendance PDF
        public async Task<byte[]> GenerateAttendancePdfAsync()
        {
            var attendanceList = await _attendanceRepository.GetAllQueryable()
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Employee Management System").FontSize(10).FontColor(Colors.Grey.Medium);
                            col.Item().Text("Attendance History Report").FontSize(24).Bold().FontColor(Colors.Green.Medium);
                        });
                        row.ConstantItem(100).AlignRight().Text(DateTime.Now.ToString("yyyy-MM-dd")).FontSize(10).FontColor(Colors.Grey.Medium);
                    });

                    page.Content().PaddingVertical(15).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3); // Date
                            columns.RelativeColumn(5); // Employee
                            columns.RelativeColumn(4); // Check-in
                            columns.RelativeColumn(4); // Check-out
                            columns.RelativeColumn(3); // Status
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Green.Lighten4).Padding(5).Text("Date").Bold();
                            header.Cell().Background(Colors.Green.Lighten4).Padding(5).Text("Employee Name").Bold();
                            header.Cell().Background(Colors.Green.Lighten4).Padding(5).Text("Check In").Bold();
                            header.Cell().Background(Colors.Green.Lighten4).Padding(5).Text("Check Out").Bold();
                            header.Cell().Background(Colors.Green.Lighten4).Padding(5).Text("Status").Bold();
                        });

                        foreach (var att in attendanceList)
                        {
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text(att.Date.ToString("yyyy-MM-dd"));
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text(att.Employee != null ? $"{att.Employee.FirstName} {att.Employee.LastName}" : "Unknown");
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text(att.CheckIn.ToString("hh:mm tt"));
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text(att.CheckOut?.ToString("hh:mm tt") ?? "-");
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text(att.Status);
                        }
                    });

                    page.Footer().AlignCenter().Text(text => { text.Span("Page "); text.CurrentPageNumber(); });
                });
            });

            return document.GeneratePdf();
        }

        // 5. Salary Report PDF (with totals and payroll summary)
        public async Task<byte[]> GenerateSalaryPdfAsync()
        {
            var employees = await _employeeRepository.GetAllQueryable().OrderByDescending(e => e.Salary).ToListAsync();
            decimal totalSalary = employees.Sum(e => e.Salary);
            decimal averageSalary = employees.Any() ? employees.Average(e => e.Salary) : 0;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Employee Management System").FontSize(10).FontColor(Colors.Grey.Medium);
                            col.Item().Text("Salary & Payroll Report").FontSize(24).Bold().FontColor(Colors.Teal.Medium);
                        });
                        row.ConstantItem(100).AlignRight().Text(DateTime.Now.ToString("yyyy-MM-dd")).FontSize(10).FontColor(Colors.Grey.Medium);
                    });

                    page.Content().PaddingVertical(15).Column(column =>
                    {
                        // Payroll Summary Cards
                        column.Item().PaddingBottom(15).Row(row =>
                        {
                            row.RelativeItem().Background(Colors.Teal.Lighten5).Padding(10).Column(col =>
                            {
                                col.Item().Text("Total Monthly Payroll").FontSize(10).FontColor(Colors.Teal.Darken2);
                                col.Item().Text($"${totalSalary:N2}").FontSize(18).Bold().FontColor(Colors.Teal.Darken3);
                            });
                            row.ConstantItem(20);
                            row.RelativeItem().Background(Colors.Teal.Lighten5).Padding(10).Column(col =>
                            {
                                col.Item().Text("Average Annual Salary").FontSize(10).FontColor(Colors.Teal.Darken2);
                                col.Item().Text($"${averageSalary:N2}").FontSize(18).Bold().FontColor(Colors.Teal.Darken3);
                            });
                        });

                        // Payroll details table
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1); // ID
                                columns.RelativeColumn(4); // Employee Name
                                columns.RelativeColumn(4); // Department
                                columns.RelativeColumn(4); // Designation
                                columns.RelativeColumn(3); // Salary
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Teal.Lighten4).Padding(5).Text("ID").Bold();
                                header.Cell().Background(Colors.Teal.Lighten4).Padding(5).Text("Employee Name").Bold();
                                header.Cell().Background(Colors.Teal.Lighten4).Padding(5).Text("Department").Bold();
                                header.Cell().Background(Colors.Teal.Lighten4).Padding(5).Text("Designation").Bold();
                                header.Cell().Background(Colors.Teal.Lighten4).Padding(5).Text("Salary").Bold();
                            });

                            foreach (var emp in employees)
                            {
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text(emp.EmployeeId.ToString());
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text($"{emp.FirstName} {emp.LastName}");
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text(emp.Department);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text(emp.Designation);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text($"${emp.Salary:N2}");
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(text => { text.Span("Page "); text.CurrentPageNumber(); });
                });
            });

            return document.GeneratePdf();
        }

        // 6. Department Headcount and Budget Summary PDF
        public async Task<byte[]> GenerateDepartmentPdfAsync()
        {
            var employees = await _employeeRepository.GetAllQueryable().ToListAsync();

            var departmentStats = employees
                .GroupBy(e => e.Department)
                .Select(g => new
                {
                    Department = g.Key,
                    Headcount = g.Count(),
                    TotalBudget = g.Sum(e => e.Salary),
                    AverageSalary = g.Average(e => e.Salary)
                })
                .OrderByDescending(d => d.Headcount)
                .ToList();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Employee Management System").FontSize(10).FontColor(Colors.Grey.Medium);
                            col.Item().Text("Department Summary Report").FontSize(24).Bold().FontColor(Colors.Orange.Medium);
                        });
                        row.ConstantItem(100).AlignRight().Text(DateTime.Now.ToString("yyyy-MM-dd")).FontSize(10).FontColor(Colors.Grey.Medium);
                    });

                    page.Content().PaddingVertical(15).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(5); // Department
                            columns.RelativeColumn(3); // Headcount
                            columns.RelativeColumn(4); // Budget Expense
                            columns.RelativeColumn(4); // Average Salary
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Orange.Lighten4).Padding(5).Text("Department").Bold();
                            header.Cell().Background(Colors.Orange.Lighten4).Padding(5).Text("Headcount").Bold();
                            header.Cell().Background(Colors.Orange.Lighten4).Padding(5).Text("Monthly Budget").Bold();
                            header.Cell().Background(Colors.Orange.Lighten4).Padding(5).Text("Average Salary").Bold();
                        });

                        foreach (var stat in departmentStats)
                        {
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text(stat.Department);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text(stat.Headcount.ToString());
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text($"${stat.TotalBudget:N2}");
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text($"${stat.AverageSalary:N2}");
                        }
                    });

                    page.Footer().AlignCenter().Text(text => { text.Span("Page "); text.CurrentPageNumber(); });
                });
            });

            return document.GeneratePdf();
        }
    }
}
