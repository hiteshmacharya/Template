using AdminAPI.Infrastructure.IntegrationEvents.Events;
using EventBus.Abstractions;
using System.Threading.Tasks;
using Upscript.Services.Admin.API.Infrastructure.BusnessLogic.ServiceInterface;
using Upscript.Services.Admin.API.Model;

namespace AdminAPI.Infrastructure.IntegrationEvents.EventHandling
{
    public class EmployeeCreatedEventHandler: IIntegrationEventHandler<EmployeeCreatedEvent>
    {
        private readonly IUserService _userService;
        public EmployeeCreatedEventHandler(IUserService userService)
        {
            _userService = userService;
        }
        public async Task Handle(EmployeeCreatedEvent @event) {
            User user = new User();
            user.Name = @event.EmployeeName;
            user.Description = @event.Description;
            user.Email = @event.Email;
            user.Employee_Id = @event.EmployeeId;
            user.IsActive = true;
            await _userService.CreateUserAsync(user);
            await Task.CompletedTask;
        }
    }
}
