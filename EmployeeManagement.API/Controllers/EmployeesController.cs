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
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeService _employeeService;

        public EmployeesController(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees(
            [FromQuery] string? search,
            [FromQuery] string? sortBy,
            [FromQuery] string? sortOrder,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _employeeService.GetEmployeesAsync(search, sortBy, sortOrder, page, pageSize);
            return Ok(new
            {
                success = true,
                message = "Employees retrieved successfully.",
                data = result
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            try
            {
                var result = await _employeeService.GetEmployeeByIdAsync(id);
                return Ok(new
                {
                    success = true,
                    message = "Employee details retrieved successfully.",
                    data = result
                });
            }
            catch (EmployeeService.EmployeeNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeCreateDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Validation failed.", errors = ModelState });
            }

            try
            {
                var result = await _employeeService.CreateEmployeeAsync(request);
                return CreatedAtAction(nameof(GetEmployeeById), new { id = result.EmployeeId }, new
                {
                    success = true,
                    message = "Employee created successfully.",
                    data = result
                });
            }
            catch (EmployeeService.DuplicateEmailException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeUpdateDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Validation failed.", errors = ModelState });
            }

            try
            {
                var result = await _employeeService.UpdateEmployeeAsync(id, request);
                return Ok(new
                {
                    success = true,
                    message = "Employee updated successfully.",
                    data = result
                });
            }
            catch (EmployeeService.EmployeeNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (EmployeeService.DuplicateEmailException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                await _employeeService.DeleteEmployeeAsync(id);
                return Ok(new
                {
                    success = true,
                    message = $"Employee with ID {id} deleted successfully."
                });
            }
            catch (EmployeeService.EmployeeNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("delete-multiple")]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<int> ids)
        {
            try
            {
                await _employeeService.DeleteMultipleEmployeesAsync(ids);
                return Ok(new
                {
                    success = true,
                    message = "Selected employees deleted successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
