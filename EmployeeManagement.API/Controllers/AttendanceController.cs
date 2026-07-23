using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmployeeManagement.API.DTOs;
using EmployeeManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly AttendanceService _attendanceService;

        public AttendanceController(AttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAttendance(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] int? employeeId)
        {
            var result = await _attendanceService.GetAttendanceHistoryAsync(startDate, endDate, employeeId);
            return Ok(new
            {
                success = true,
                message = "Attendance history retrieved successfully.",
                data = result
            });
        }

        [HttpPost]
        public async Task<IActionResult> CheckIn([FromBody] CheckInRequestDto request)
        {
            try
            {
                var result = await _attendanceService.CheckInAsync(request.EmployeeId);
                return Ok(new
                {
                    success = true,
                    message = "Check-in recorded successfully.",
                    data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (AttendanceService.DuplicateCheckInException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred during check-in: {ex.Message}" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> CheckOut(int id)
        {
            try
            {
                var result = await _attendanceService.CheckOutAsync(id);
                return Ok(new
                {
                    success = true,
                    message = "Check-out recorded successfully.",
                    data = result
                });
            }
            catch (AttendanceService.AttendanceNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred during check-out: {ex.Message}" });
            }
        }
    }
}
