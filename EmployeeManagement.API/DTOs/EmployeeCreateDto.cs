using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.DTOs
{
    public class EmployeeCreateDto
    {
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Department is required.")]
        [StringLength(50, ErrorMessage = "Department cannot exceed 50 characters.")]
        public string Department { get; set; } = string.Empty;

        [Required(ErrorMessage = "Designation is required.")]
        [StringLength(50, ErrorMessage = "Designation cannot exceed 50 characters.")]
        public string Designation { get; set; } = string.Empty;

        [Required(ErrorMessage = "Salary is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Salary must be greater than zero.")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "Joining date is required.")]
        public DateTime JoiningDate { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters.")]
        public string Status { get; set; } = "Active";
    }
}
