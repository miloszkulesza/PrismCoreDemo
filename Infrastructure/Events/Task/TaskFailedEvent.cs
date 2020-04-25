using Prism.Events;

namespace Infrastructure.Events
{
    public class TaskFailedEvent : PubSubEvent<Models.Task>
    {
    }
}
