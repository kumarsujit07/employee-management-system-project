using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagement.API.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Password { get; set; } = string.Empty; // Plain-text as requested in comment feedback

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = "HR"; // Roles: 'Admin', 'HR'

        public int? EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
