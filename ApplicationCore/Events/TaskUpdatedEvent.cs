using Prism.Events;

namespace ApplicationCore.Events
{
    public class TaskUpdatedEvent : PubSubEvent<Core.Models.Task>
    {
    }
}
