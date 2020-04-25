using Prism.Events;

namespace Infrastructure.Events
{
    public class TaskAddedEvent : PubSubEvent<Models.Task>
    {
    }
}
