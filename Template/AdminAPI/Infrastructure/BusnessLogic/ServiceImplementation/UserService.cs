using Admin.API.IntegrationEvents;
using AdminAPI.Infrastructure.IntegrationEvents.Events;
using EventBus.Abstractions;
using EventBus.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Upscript.Services.Admin.API.Infrastructure.BusnessLogic.ServiceInterface;
using Upscript.Services.Admin.API.Model;
using UpScript.Services.Admin.API.Infrastructure;


namespace Upscript.Services.Admin.API.Infrastructure.BusnessLogic.ServiceImplementation
{
    public class UserService : IUserService
    {
        private readonly AdminContext _adminContext;
        private readonly IEventBus _eventBus;
        private readonly IAdminIntegrationEventService _adminIntegrationEventService;
        private readonly ILogger _logger;

        public UserService(AdminContext context,  IEventBus eventBus, IAdminIntegrationEventService adminIntegrationEventService, ILogger<UserService> logger)
        {
            _adminContext = context ?? throw new ArgumentNullException(nameof(context));
            _eventBus = eventBus;
            _adminIntegrationEventService = adminIntegrationEventService;
            _logger = logger;
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }



        public async Task<User> CreateUserAsync(User user)
        {
            var item = new User
            {
                Name = user.Name,
                Description = user.Description,
                Email = user.Email,
                IsActive = user.IsActive,
                Employee_Id = user.Employee_Id
            };
            _adminContext.User.Add(item);
            try
            {
                await _adminContext.SaveChangesAsync();
                //throw new Exception("Manual Exception");
                IntegrationEvent employeeCreatedSuccessfullyEvent = new EmployeeCreatedSuccessfullyEvent(item.Employee_Id, true, "Employee has been saved successfully");
                await _adminIntegrationEventService.SaveEventAsync(employeeCreatedSuccessfullyEvent);
                await _adminIntegrationEventService.PublishThroughEventBusAsync(employeeCreatedSuccessfullyEvent);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error is occured due to Message: {ex.Message}, Inner Exception: {ex.InnerException}, StackTrace: {ex.StackTrace}");
                IntegrationEvent employeeCreatedFailedEvent = new EmployeeCreatedFailedEvent(item.Employee_Id, false, "Employee has not been saved successfully");
                await _adminIntegrationEventService.SaveEventAsync(employeeCreatedFailedEvent);
                await _adminIntegrationEventService.PublishThroughEventBusAsync(employeeCreatedFailedEvent);
            }
            return await _adminContext.User.SingleOrDefaultAsync(i => i.Id == item.Id);
        }
    }
}
