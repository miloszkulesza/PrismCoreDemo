﻿using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Events
{
    public class EmployeeUpdatedEvent : PubSubEvent<Models.Employee>
    {
    }
}
