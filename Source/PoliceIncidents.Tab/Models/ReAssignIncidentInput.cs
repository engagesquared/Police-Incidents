// <copyright file="ReAssignIncidentInput.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Tab.Models
{
    using System;

    public class ReAssignIncidentInput
    {
        public long IncidentId { get; set; }

        public Guid IncidentManagerId { get; set; }
    }
}
