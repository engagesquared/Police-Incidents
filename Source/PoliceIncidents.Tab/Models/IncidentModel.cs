// <copyright file="IncidentModel.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Tab.Models
{
    using System;
    using System.Collections.Generic;
    using PoliceIncidents.Core.DB.Entities;

    public class IncidentModel
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string WebEOCLink { get; set; }

        public Guid? IncidentManagerId { get; set; }

        public IncidentStatus Status { get; set; }

        public string Location { get; set; }

        public List<IncidentUpdateModel> IncidentUpdates { get; set; }

        public DateTime IncidentRaised { get; set; }

    }
}
