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
    public class AttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public class DuplicateCheckInException : Exception
        {
            public DuplicateCheckInException(string message) : base(message) { }
        }

        public class AttendanceNotFoundException : Exception
        {
            public AttendanceNotFoundException(string message) : base(message) { }
        }

        public AttendanceService(IAttendanceRepository attendanceRepository, IEmployeeRepository employeeRepository)
        {
            _attendanceRepository = attendanceRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<AttendanceResponseDto> CheckInAsync(int employeeId)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with ID {employeeId} not found.");
            }

            var today = DateTime.Today;
            var existingRecord = await _attendanceRepository.GetByEmployeeAndDateAsync(employeeId, today);
            if (existingRecord != null)
            {
                throw new DuplicateCheckInException("You have already checked in for today.");
            }

            var now = DateTime.Now; // Use server local time for check-in status rules
            var checkInDeadline = new TimeSpan(9, 0, 0); // 9:00 AM
            string status = now.TimeOfDay > checkInDeadline ? "Late" : "Present";

            var attendance = new Attendance
            {
                EmployeeId = employeeId,
                Date = today,
                CheckIn = now,
                CheckOut = null,
                Status = status
            };

            await _attendanceRepository.AddAsync(attendance);
            await _attendanceRepository.SaveChangesAsync();

            // Fetch again to populate navigation properties (Employee details)
            var record = await _attendanceRepository.GetByIdAsync(attendance.AttendanceId);
            return MapToResponseDto(record!);
        }

        public async Task<AttendanceResponseDto> CheckOutAsync(int attendanceId)
        {
            var record = await _attendanceRepository.GetByIdAsync(attendanceId);
            if (record == null)
            {
                throw new AttendanceNotFoundException($"Attendance record with ID {attendanceId} not found.");
            }

            if (record.CheckOut.HasValue)
            {
                throw new InvalidOperationException("You have already checked out for today.");
            }

            record.CheckOut = DateTime.Now;
            await _attendanceRepository.UpdateAsync(record);
            await _attendanceRepository.SaveChangesAsync();

            return MapToResponseDto(record);
        }

        public async Task<List<AttendanceResponseDto>> GetAttendanceHistoryAsync(
            DateTime? startDate, DateTime? endDate, int? employeeId)
        {
            var query = _attendanceRepository.GetAllQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(a => a.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                query = query.Where(a => a.Date <= endDate.Value.Date);
            }

            if (employeeId.HasValue)
            {
                query = query.Where(a => a.EmployeeId == employeeId.Value);
            }

            var results = await query
                .OrderByDescending(a => a.Date)
                .ThenByDescending(a => a.CheckIn)
                .ToListAsync();

            return results.Select(MapToResponseDto).ToList();
        }

        private AttendanceResponseDto MapToResponseDto(Attendance attendance)
        {
            return new AttendanceResponseDto
            {
                AttendanceId = attendance.AttendanceId,
                EmployeeId = attendance.EmployeeId,
                EmployeeName = attendance.Employee != null 
                    ? $"{attendance.Employee.FirstName} {attendance.Employee.LastName}" 
                    : "Unknown Employee",
                Date = attendance.Date,
                CheckIn = attendance.CheckIn,
                CheckOut = attendance.CheckOut,
                Status = attendance.Status
            };
        }
    }
}
