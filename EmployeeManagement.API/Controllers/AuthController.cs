using System;
using System.Threading.Tasks;
using EmployeeManagement.API.DTOs;
using EmployeeManagement.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Validation failed", errors = ModelState });
            }

            try
            {
                var result = await _authService.LoginAsync(request);
                return Ok(new
                {
                    success = true,
                    message = "Login successful.",
                    data = result
                });
            }
            catch (AuthService.InvalidCredentialsException ex)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An unexpected error occurred during login."
                });
            }
        }
    }
}
