﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBus.Events;

namespace AdminAPI.Infrastructure.IntegrationEvents.Events
{

    public record EmployeeCreatedSuccessfullyEvent : IntegrationEvent
    {
        public int EmployeeId { get; }
        public bool status { get; set; }
        public string message { get; set; }
        public EmployeeCreatedSuccessfullyEvent(int employeeid, bool _status, string _message)
        {
            EmployeeId = employeeid;
            status = _status;
            message = _message;
        }
    }
}
