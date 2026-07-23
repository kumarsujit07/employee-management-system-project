using System.Threading.Tasks;
using EmployeeManagement.API.Models;

namespace EmployeeManagement.API.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
    }
}
