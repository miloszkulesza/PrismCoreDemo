using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Events
{
    public class RemoveHighlightCalendarDateEvent : PubSubEvent<DateTime>
    {
    }
}
