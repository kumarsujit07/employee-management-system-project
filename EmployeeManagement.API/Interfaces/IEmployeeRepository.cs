using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.API.Models;

namespace EmployeeManagement.API.Interfaces
{
    public interface IEmployeeRepository
    {
        IQueryable<Employee> GetAllQueryable();
        Task<Employee?> GetByIdAsync(int id);
        Task<Employee?> GetByEmailAsync(string email);
        Task AddAsync(Employee employee);
        Task UpdateAsync(Employee employee);
        Task DeleteAsync(Employee employee);
        Task DeleteMultipleAsync(IEnumerable<int> ids);
        Task<bool> SaveChangesAsync();
    }
}
