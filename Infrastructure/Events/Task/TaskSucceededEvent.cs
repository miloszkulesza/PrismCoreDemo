using Prism.Events;

namespace Infrastructure.Events
{
    public class TaskSucceededEvent : PubSubEvent<Models.Task>
    {
    }
}
