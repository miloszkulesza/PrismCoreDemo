using Prism.Events;

namespace Infrastructure.Events
{
    public class TaskDeletedEvent : PubSubEvent<Models.Task>
    {
    }
}
