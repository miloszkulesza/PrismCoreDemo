using Prism.Events;

namespace ApplicationCore.Events
{
    public class TaskDeletedEvent : PubSubEvent<Core.Models.Task>
    {
    }
}
