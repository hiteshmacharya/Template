using EventBus.Events;
using System.Threading.Tasks;

namespace Admin.API.IntegrationEvents
{
    public interface IAdminIntegrationEventService
    {
        Task SaveEventAsync(IntegrationEvent evt);
        Task PublishThroughEventBusAsync(IntegrationEvent evt);
    }
}
