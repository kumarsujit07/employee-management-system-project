using System;

namespace EmployeeManagement.API.DTOs
{
    public class AttendanceResponseDto
    {
        public int AttendanceId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
