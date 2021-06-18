using Employee.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upscript.Services.Employee.API.Model;
using UpScript.Services.Employee.API.Infrastructure;

namespace Employee.API.Repositories.Implementations
{
    public class EmployeeRepository : GenericRepository<Employees>,IEmployeeRepository
    {
        //private readonly IGenericRepository<Employees>  _genericRepository;
        public EmployeeRepository(EmployeeContext context) : base(context)
        {
            //_genericRepository = genericRepository;
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
        public async Task<List<Employees>> GetEmployeesAsync(int pageSize = 10, int pageIndex = 0, string ids = null)
        {
            var itemsOnPage = await _context.Employees
               .OrderBy(c => c.Name)
               .Skip(pageSize * pageIndex)
               .Take(pageSize)
               .ToListAsync();

            return itemsOnPage;
        }
        public async Task<IEnumerable<Employees>> GetEmployeeByIdsAsync(IEnumerable<int> idsToSelect)
        {
           return await Find(ci => idsToSelect.Contains(ci.Id));
        }

        public Task<long> CountEmployeeByIdsAsync(string ids = null)
        {
            if (ids == null)
                return _context.Employees.LongCountAsync();
            else
            {
                var numIds = ids.Split(',').Select(id => (Ok: int.TryParse(id, out int x), Value: x));
                var idsToSelect = numIds
                    .Select(id => id.Value);
                return _context.Employees.Where(ci => idsToSelect.Contains(ci.Id)).LongCountAsync();
            }
        }

        public async Task<Employees> EmployeeByIdAsync(int id)
        {
            //var emp=   Find(e => e.Id == id).Result.FirstOrDefault();
            //return emp;
                return await GetById(id);
        }

        public Task<List<Employees>> EmployeeWithNameAsync(string name, int pageSize = 10, int pageIndex = 0)
        {
            return _context.Employees
                 .Where(c => c.Name.StartsWith(name))
                 .Skip(pageSize * pageIndex)
                 .Take(pageSize)
                 .ToListAsync();
        }

        public Task<long> CountEmployeeWithNameAsync(string name)
        {
            return _context.Employees.Where(c => c.Name.StartsWith(name)).LongCountAsync();
        }

        public Employees CreateEmployeeAsync(Employees employee)
        {
             Add(employee);
            _context.SaveChangesAsync();
            return employee;
        }

        public async Task<Employees> UpdateEmployeeAsync(Employees employeeToUpdate)
        {
            
            var employee =await GetById(employeeToUpdate.Id);
            employee.Name = employeeToUpdate.Name;
            employee.Description = employeeToUpdate.Description;
            employee.Email = employeeToUpdate.Email;
            _context.Employees.Update(employee);
           await _context.SaveChangesAsync();
            return await GetById(employeeToUpdate.Id);
        }

        public Task DeleteEmployeeAsync(int id)
        {
            var employee = _context.Employees.SingleOrDefault(x => x.Id == id);
            _context.Employees.Remove(employee);
            return _context.SaveChangesAsync();
        }

        public async Task<bool> CommitEmployee(int id)
        {
            Employees employee;
            bool result = false;
            employee = await GetById(id);
            if (employee != null)
            {
                employee.IsActive = true;
                _context.Employees.Update(employee);
                _context.SaveChanges();
                result = true;
            }
            return result;
        }
    }
}
