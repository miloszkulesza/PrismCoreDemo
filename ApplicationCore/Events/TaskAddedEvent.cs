using Prism.Events;

namespace ApplicationCore.Events
{
    public class TaskAddedEvent : PubSubEvent<Core.Models.Task>
    {
    }
}
