﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBus.Events;

namespace AdminAPI.Infrastructure.IntegrationEvents.Events
{

    public record EmployeeCreatedEvent : IntegrationEvent
    {
        public int EmployeeId { get; }
        public string EmployeeName { get; }
        public string Description { get; set; }
        public string Email { get; set; }
        
        public EmployeeCreatedEvent(int employeeid, string employeename, string description, string email)
        {
            EmployeeId = employeeid;
            EmployeeName = employeename;
            Description = description;
            Email = email;
        }

    }
}
