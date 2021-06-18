using EventBus.Abstractions;
using EventBus.Events;
using IntegrationEventLogEF.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Data.Common;
using System.Threading.Tasks;
using UpScript.Services.Admin.API.Infrastructure;

namespace Admin.API.IntegrationEvents
{
    public class AdminIntegrationEventService : IAdminIntegrationEventService, IDisposable
    {
        private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;
        private readonly IEventBus _eventBus;
        private readonly AdminContext _adminContext;
        private readonly IIntegrationEventLogService _eventLogService;
        private readonly ILogger<AdminIntegrationEventService> _logger;
        private volatile bool disposedValue;

        public AdminIntegrationEventService(
            ILogger<AdminIntegrationEventService> logger,
            IEventBus eventBus,
            AdminContext catalogContext,
            Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _adminContext = catalogContext ?? throw new ArgumentNullException(nameof(catalogContext));
            _integrationEventLogServiceFactory = integrationEventLogServiceFactory ?? throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _eventLogService = _integrationEventLogServiceFactory(_adminContext.Database.GetDbConnection());
        }

        public async Task PublishThroughEventBusAsync(IntegrationEvent evt)
        {
            try
            {
                _logger.LogInformation("----- Publishing integration event: {IntegrationEventId_published} from ({@IntegrationEvent})", evt.Id,  evt);

                await _eventLogService.MarkEventAsInProgressAsync(evt.Id);
                _eventBus.Publish(evt);
                await _eventLogService.MarkEventAsPublishedAsync(evt.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Publishing integration event: {IntegrationEventId} from ({@IntegrationEvent})", evt.Id,  evt);
                await _eventLogService.MarkEventAsFailedAsync(evt.Id);
            }
        }

        public async Task SaveEventAsync(IntegrationEvent evt)
        {
            _logger.LogInformation("----- EmployeeIntegrationEventService - Saving changes and integrationEvent: {IntegrationEventId}", evt.Id);
            await _eventLogService.SaveEventAsync(evt);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    (_eventLogService as IDisposable)?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    
}
