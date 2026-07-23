using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagement.API.Models
{
    public class Attendance
    {
        [Key]
        public int AttendanceId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public DateTime CheckIn { get; set; }

        public DateTime? CheckOut { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Present"; // Status: 'Present', 'Late', 'Absent'
    }
}
