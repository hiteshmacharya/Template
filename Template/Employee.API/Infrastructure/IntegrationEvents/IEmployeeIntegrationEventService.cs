using EventBus.Events;
using System.Threading.Tasks;

namespace Employee.API.IntegrationEvents
{
    public interface IEmployeeIntegrationEventService
    {
        Task SaveEventAsync(IntegrationEvent evt);
        Task PublishThroughEventBusAsync(IntegrationEvent evt);
    }
}
