using System.Collections.Generic;
using System.Threading.Tasks;
using Upscript.Services.Employee.API.Model;

namespace Upscript.Services.Employee.API.Infrastructure.BusnessLogic.ServiceInterface
{
    public interface IEmployeeService
    {
        Task<List<Employees>> GetEmployeesAsync(int pageSize = 10, int pageIndex = 0, string ids = null);
         Task<IEnumerable<Employees>> GetEmployeeByIdsAsync(string ids);
        Task<long> CountEmployeeByIdsAsync(string ids = null);
        Task<Employees> EmployeeByIdAsync(int id);
        Task<List<Employees>> EmployeeWithNameAsync(string name, int pageSize = 10, int pageIndex = 0);
        Task<long> CountEmployeeWithNameAsync(string name);
        Task<Employees> CreateEmployeeAsync(Employees employee);
        Task<Employees> UpdateEmployeeAsync(Employees employeeToUpdate);
        Task DeleteEmployeeAsync(int id);
        Task<bool> CommitEmployee(int id);
    }
}
