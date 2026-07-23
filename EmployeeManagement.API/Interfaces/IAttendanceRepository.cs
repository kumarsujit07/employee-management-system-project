using System;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.API.Models;

namespace EmployeeManagement.API.Interfaces
{
    public interface IAttendanceRepository
    {
        Task<Attendance?> GetByEmployeeAndDateAsync(int employeeId, DateTime date);
        Task<Attendance?> GetByIdAsync(int id);
        IQueryable<Attendance> GetAllQueryable();
        Task AddAsync(Attendance attendance);
        Task UpdateAsync(Attendance attendance);
        Task<bool> SaveChangesAsync();
    }
}
