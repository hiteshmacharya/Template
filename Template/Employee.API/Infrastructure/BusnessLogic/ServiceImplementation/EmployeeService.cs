using Employee.API.Infrastructure.IntegrationEvents.Events;
using Employee.API.IntegrationEvents;
using Employee.API.Repositories.Interfaces;
using EventBus.Abstractions;
using EventBus.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upscript.Services.Employee.API.Infrastructure.BusnessLogic.ServiceInterface;
using Upscript.Services.Employee.API.Model;
using UpScript.Services.Employee.API.Infrastructure;

namespace Upscript.Services.Employee.API.Infrastructure.BusnessLogic.ServiceImplementation
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly EmployeeSettings _settings;
        private readonly IEventBus _eventBus;
        private readonly IEmployeeIntegrationEventService _employeeIntegrationEventService;
        

        public EmployeeService( IEmployeeRepository employeeRepository,IOptionsSnapshot<EmployeeSettings> settings, IEventBus eventBus, IEmployeeIntegrationEventService employeeIntegrationEventService)
        {
            _employeeRepository = employeeRepository;
           _settings = settings.Value;
            _eventBus = eventBus;
            _employeeIntegrationEventService = employeeIntegrationEventService;                    
        }

        public async Task<List<Employees>> GetEmployeesAsync(int pageSize = 10, int pageIndex = 0, string ids = null)
        {
            return await  _employeeRepository.GetEmployeesAsync( pageSize ,  pageIndex,ids);
        }

        public async Task<IEnumerable<Employees>> GetEmployeeByIdsAsync(string ids)
        {
            var numIds = ids.Split(',').Select(id => (Ok: int.TryParse(id, out int x), Value: x));

            if (!numIds.All(nid => nid.Ok))
            {
                return new List<Employees>();
            }

            IEnumerable<int> idsToSelect = numIds
                .Select(id => id.Value);

            return await _employeeRepository.GetEmployeeByIdsAsync(idsToSelect);

        }

        public Task<long> CountEmployeeByIdsAsync(string ids = null)
        {
           return _employeeRepository.CountEmployeeByIdsAsync(ids);
           
        }


        public async Task<Employees> EmployeeByIdAsync(int id)
        {
            return await _employeeRepository.EmployeeByIdAsync(id);
        }

        public async Task<List<Employees>> EmployeeWithNameAsync(string name, int pageSize = 10, int pageIndex = 0)
        {
           return await _employeeRepository.EmployeeWithNameAsync(name,pageSize,pageIndex);
        }

        public Task<long> CountEmployeeWithNameAsync(string name)
        {
            return _employeeRepository.CountEmployeeWithNameAsync(name);
        }

        public async Task<Employees> CreateEmployeeAsync(Employees employee)
        {
            var item = new Employees
            {
                Name = employee.Name,
                Description = employee.Description,
                Email = employee.Email,
                IsActive = false
            };
           item=  _employeeRepository.CreateEmployeeAsync(item);
            

            if (item.Id > 0)
            {
                IntegrationEvent employeeCreatedEvent = new EmployeeCreatedEvent(item.Id, item.Name, item.Description, item.Email);
                await _employeeIntegrationEventService.SaveEventAsync(employeeCreatedEvent);
                await _employeeIntegrationEventService.PublishThroughEventBusAsync(employeeCreatedEvent);
            }
            return item;
        }

        public async Task<Employees> UpdateEmployeeAsync(Employees employeeToUpdate)
        {
            return await _employeeRepository.UpdateEmployeeAsync(employeeToUpdate);
          
        }

        public async Task DeleteEmployeeAsync(int id)
        {
           await _employeeRepository.DeleteEmployeeAsync(id);
        }

        public async Task<bool> CommitEmployee(int id)
        {
            Employees emp= await _employeeRepository.EmployeeByIdAsync(id);
            emp.IsActive = true;
            await _employeeRepository.UpdateEmployeeAsync(emp);
            bool result = false;
           
            return result;
        }
    }
}
