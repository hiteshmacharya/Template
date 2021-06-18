using EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Upscript.Services.Employee.API.Infrastructure.BusnessLogic.ServiceInterface;

namespace Employee.API.Infrastructure.IntegrationEvents.Events
{
    public class EmployeeCreatedFailedEventHandler: IIntegrationEventHandler<EmployeeCreatedFailedEvent>
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;
        public EmployeeCreatedFailedEventHandler(
           ILogger<EmployeeCreatedFailedEventHandler> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }
        public async Task Handle(EmployeeCreatedFailedEvent @event) {
            await _employeeService.DeleteEmployeeAsync(@event.EmployeeId);
            _logger.LogInformation($"Message: {@event.message}");
            await Task.CompletedTask;
        }
    }
}
