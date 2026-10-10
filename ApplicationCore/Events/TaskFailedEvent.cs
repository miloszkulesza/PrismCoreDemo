using Prism.Events;

namespace ApplicationCore.Events
{
    public class TaskFailedEvent : PubSubEvent<Core.Models.Task>
    {
    }
}
