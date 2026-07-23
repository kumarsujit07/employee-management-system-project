using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.API.Data;
using EmployeeManagement.API.Interfaces;
using EmployeeManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.API.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Employee> GetAllQueryable()
        {
            // Returns a queryable so that we can do deferred execution for pagination/sorting on the SQL server
            return _context.Employees.AsQueryable();
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees.FindAsync(id);
        }

        public async Task<Employee?> GetByEmailAsync(string email)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.Email.ToLower() == email.ToLower());
        }

        public async Task AddAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
        }

        public async Task UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Employee employee)
        {
            _context.Employees.Remove(employee);
            await Task.CompletedTask;
        }

        public async Task DeleteMultipleAsync(IEnumerable<int> ids)
        {
            var employeesToDelete = await _context.Employees
                .Where(e => ids.Contains(e.EmployeeId))
                .ToListAsync();

            if (employeesToDelete.Any())
            {
                _context.Employees.RemoveRange(employeesToDelete);
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
