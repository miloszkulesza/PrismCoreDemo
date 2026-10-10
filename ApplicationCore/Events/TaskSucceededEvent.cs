using Prism.Events;

namespace ApplicationCore.Events
{
    public class TaskSucceededEvent : PubSubEvent<Core.Models.Task>
    {
    }
}
