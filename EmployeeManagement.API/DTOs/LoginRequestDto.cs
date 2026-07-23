using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.DTOs
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(50)]
        public string Password { get; set; } = string.Empty;
    }
}
