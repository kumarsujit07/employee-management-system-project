using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.API.DTOs;
using EmployeeManagement.API.Interfaces;
using EmployeeManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.API.Services
{
    public class EmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public class DuplicateEmailException : Exception
        {
            public DuplicateEmailException(string message) : base(message) { }
        }

        public class EmployeeNotFoundException : Exception
        {
            public EmployeeNotFoundException(string message) : base(message) { }
        }

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<PaginatedResponseDto<EmployeeResponseDto>> GetEmployeesAsync(
            string? search, string? sortBy, string? sortOrder, int page, int pageSize)
        {
            var query = _employeeRepository.GetAllQueryable();

            // 1. Search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.ToLower();
                query = query.Where(e =>
                    e.FirstName.ToLower().Contains(lowerSearch) ||
                    e.LastName.ToLower().Contains(lowerSearch) ||
                    e.Email.ToLower().Contains(lowerSearch) ||
                    e.Department.ToLower().Contains(lowerSearch) ||
                    e.Designation.ToLower().Contains(lowerSearch)
                );
            }

            // 2. Sorting
            bool isAscending = string.IsNullOrWhiteSpace(sortOrder) || sortOrder.ToLower() != "desc";
            sortBy = sortBy?.ToLower() ?? "employeeid";

            query = sortBy switch
            {
                "firstname" => isAscending ? query.OrderBy(e => e.FirstName) : query.OrderByDescending(e => e.FirstName),
                "lastname" => isAscending ? query.OrderBy(e => e.LastName) : query.OrderByDescending(e => e.LastName),
                "email" => isAscending ? query.OrderBy(e => e.Email) : query.OrderByDescending(e => e.Email),
                "salary" => isAscending ? query.OrderBy(e => e.Salary) : query.OrderByDescending(e => e.Salary),
                "joiningdate" => isAscending ? query.OrderBy(e => e.JoiningDate) : query.OrderByDescending(e => e.JoiningDate),
                "status" => isAscending ? query.OrderBy(e => e.Status) : query.OrderByDescending(e => e.Status),
                _ => isAscending ? query.OrderBy(e => e.EmployeeId) : query.OrderByDescending(e => e.EmployeeId)
            };

            // 3. Pagination
            int totalCount = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            if (page < 1) page = 1;
            
            var employees = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mappedData = employees.Select(MapToResponseDto);

            return new PaginatedResponseDto<EmployeeResponseDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Data = mappedData
            };
        }

        public async Task<EmployeeResponseDto> GetEmployeeByIdAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                throw new EmployeeNotFoundException($"Employee with ID {id} not found.");
            }
            return MapToResponseDto(employee);
        }

        public async Task<EmployeeResponseDto> CreateEmployeeAsync(EmployeeCreateDto dto)
        {
            // Email Uniqueness check
            var existing = await _employeeRepository.GetByEmailAsync(dto.Email);
            if (existing != null)
            {
                throw new DuplicateEmailException($"Email address '{dto.Email}' is already registered.");
            }

            var employee = new Employee
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                Department = dto.Department,
                Designation = dto.Designation,
                Salary = dto.Salary,
                JoiningDate = dto.JoiningDate,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _employeeRepository.AddAsync(employee);
            await _employeeRepository.SaveChangesAsync();

            return MapToResponseDto(employee);
        }

        public async Task<EmployeeResponseDto> UpdateEmployeeAsync(int id, EmployeeUpdateDto dto)
        {
            if (id != dto.EmployeeId)
            {
                throw new ArgumentException("Path parameter ID does not match DTO ID.");
            }

            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                throw new EmployeeNotFoundException($"Employee with ID {id} not found.");
            }

            // Check if updated email is taken by another employee
            if (employee.Email.ToLower() != dto.Email.ToLower())
            {
                var existing = await _employeeRepository.GetByEmailAsync(dto.Email);
                if (existing != null)
                {
                    throw new DuplicateEmailException($"Email address '{dto.Email}' is already registered.");
                }
            }

            employee.FirstName = dto.FirstName;
            employee.LastName = dto.LastName;
            employee.Email = dto.Email;
            employee.Phone = dto.Phone;
            employee.Department = dto.Department;
            employee.Designation = dto.Designation;
            employee.Salary = dto.Salary;
            employee.JoiningDate = dto.JoiningDate;
            employee.Status = dto.Status;
            employee.UpdatedAt = DateTime.UtcNow;

            await _employeeRepository.UpdateAsync(employee);
            await _employeeRepository.SaveChangesAsync();

            return MapToResponseDto(employee);
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                throw new EmployeeNotFoundException($"Employee with ID {id} not found.");
            }

            await _employeeRepository.DeleteAsync(employee);
            return await _employeeRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteMultipleEmployeesAsync(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                throw new ArgumentException("No IDs provided for deletion.");
            }

            await _employeeRepository.DeleteMultipleAsync(ids);
            return await _employeeRepository.SaveChangesAsync();
        }

        private EmployeeResponseDto MapToResponseDto(Employee employee)
        {
            return new EmployeeResponseDto
            {
                EmployeeId = employee.EmployeeId,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Phone = employee.Phone,
                Department = employee.Department,
                Designation = employee.Designation,
                Salary = employee.Salary,
                JoiningDate = employee.JoiningDate,
                Status = employee.Status,
                CreatedAt = employee.CreatedAt,
                UpdatedAt = employee.UpdatedAt
            };
        }
    }
}
