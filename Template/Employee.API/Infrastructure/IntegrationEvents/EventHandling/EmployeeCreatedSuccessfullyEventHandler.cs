using EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Upscript.Services.Employee.API.Infrastructure.BusnessLogic.ServiceInterface;

namespace Employee.API.Infrastructure.IntegrationEvents.Events
{
    public class EmployeeCreatedSuccessfullyEventHandler: IIntegrationEventHandler<EmployeeCreatedSuccessfullyEvent>
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;
        public EmployeeCreatedSuccessfullyEventHandler(
           ILogger<EmployeeCreatedSuccessfullyEventHandler> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }
        public async Task Handle(EmployeeCreatedSuccessfullyEvent @event) {
            await _employeeService.CommitEmployee(@event.EmployeeId);
            _logger.LogInformation($"Message: {@event.message}");
            await Task.CompletedTask;
        }
    }
}
