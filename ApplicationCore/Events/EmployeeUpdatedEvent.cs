using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Events
{
    public class EmployeeUpdatedEvent : PubSubEvent<Core.Models.Employee>
    {
    }
}
