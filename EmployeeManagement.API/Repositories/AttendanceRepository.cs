using System;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.API.Data;
using EmployeeManagement.API.Interfaces;
using EmployeeManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.API.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly ApplicationDbContext _context;

        public AttendanceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Attendance?> GetByEmployeeAndDateAsync(int employeeId, DateTime date)
        {
            var targetDate = date.Date;
            return await _context.Attendance
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date == targetDate);
        }

        public async Task<Attendance?> GetByIdAsync(int id)
        {
            return await _context.Attendance
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.AttendanceId == id);
        }

        public IQueryable<Attendance> GetAllQueryable()
        {
            return _context.Attendance
                .Include(a => a.Employee)
                .AsQueryable();
        }

        public async Task AddAsync(Attendance attendance)
        {
            await _context.Attendance.AddAsync(attendance);
        }

        public async Task UpdateAsync(Attendance attendance)
        {
            _context.Attendance.Update(attendance);
            await Task.CompletedTask;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
