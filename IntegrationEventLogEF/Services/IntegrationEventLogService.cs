using EventBus.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace IntegrationEventLogEF.Services
{
    public class IntegrationEventLogService : IIntegrationEventLogService, IDisposable
    {
        private readonly IntegrationEventLogContext _integrationEventLogContext;
        private readonly DbConnection _dbConnection;
        private readonly List<Type> _eventTypes;
        private volatile bool disposedValue;

        public IntegrationEventLogService(DbConnection dbConnection)
        {
            _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));


            _integrationEventLogContext = new IntegrationEventLogContext(
                new DbContextOptionsBuilder<IntegrationEventLogContext>()
                    .UseMySql(_dbConnection.ConnectionString, ServerVersion.AutoDetect(_dbConnection.ConnectionString))
                    .Options);

            _eventTypes = Assembly.Load(Assembly.GetEntryAssembly().FullName)
                .GetTypes()
                .Where(t => t.Name.EndsWith(nameof(IntegrationEvent)))
                .ToList();
        }

        public async Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync(Guid eventId)
        {
            var result = await _integrationEventLogContext.IntegrationEventLogs
                .Where(e => e.EventId == eventId && e.State == EventStateEnum.NotPublished).ToListAsync();

            if (result != null && result.Any())
            {
                return result.OrderBy(o => o.CreationTime)
                    .Select(e => e.DeserializeJsonContent(_eventTypes.Find(t => t.Name == e.EventTypeShortName)));
            }

            return new List<IntegrationEventLogEntry>();
        }

        public Task SaveEventAsync(IntegrationEvent @event)
        {
            var eventLogEntry = new IntegrationEventLogEntry(@event);
            _integrationEventLogContext.IntegrationEventLogs.Add(eventLogEntry);
            return _integrationEventLogContext.SaveChangesAsync();
        }

        public Task MarkEventAsPublishedAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventStateEnum.Published);
        }

        public Task MarkEventAsInProgressAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventStateEnum.InProgress);
        }

        public Task MarkEventAsFailedAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventStateEnum.PublishedFailed);
        }

        private Task UpdateEventStatus(Guid eventId, EventStateEnum status)
        {
            var eventLogEntry = _integrationEventLogContext.IntegrationEventLogs.Single(ie => ie.EventId == eventId);
            eventLogEntry.State = status;

            if (status == EventStateEnum.InProgress)
                eventLogEntry.TimesSent++;

            _integrationEventLogContext.IntegrationEventLogs.Update(eventLogEntry);

            return _integrationEventLogContext.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _integrationEventLogContext?.Dispose();
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
