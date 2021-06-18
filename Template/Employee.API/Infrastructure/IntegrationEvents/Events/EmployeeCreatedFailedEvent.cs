using EventBus.Events;

namespace Employee.API.Infrastructure.IntegrationEvents.Events
{

    public record EmployeeCreatedFailedEvent : IntegrationEvent
    {
        public int EmployeeId { get; }
        public bool status { get; set; }
        public string message { get; set; }
        public EmployeeCreatedFailedEvent(int employeeid, bool _status, string _message)
        {
            EmployeeId = employeeid;
            status = _status;
            message = _message;
        }
    }
}
