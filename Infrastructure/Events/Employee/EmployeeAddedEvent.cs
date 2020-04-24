using Prism.Events;

namespace Infrastructure.Events
{
    public class EmployeeAddedEvent : PubSubEvent<Models.Employee>
    {
    }
}
