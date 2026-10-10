using Prism.Events;

namespace ApplicationCore.Events
{
    public class EmployeeAddedEvent : PubSubEvent<Core.Models.Employee>
    {
    }
}
