// <copyright file="ReAssignInput.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Tab.Models
{
    using System;

    public class ReAssignInput
    {
        public int IncidentId { get; set; }

        public Guid IncidentManagerId { get; set; }
    }
}
