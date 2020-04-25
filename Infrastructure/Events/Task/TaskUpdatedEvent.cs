using Prism.Events;

namespace Infrastructure.Events
{
    public class TaskUpdatedEvent : PubSubEvent<Models.Task>
    {
    }
}
