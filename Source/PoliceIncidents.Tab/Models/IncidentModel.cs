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

        public string ExternalLink { get; set; }

        public string PlannerLink { get; set; }

        public string ChatThreadLink { get; set; }

        public Guid? ManagerId { get; set; }

        public List<Guid> Members { get; set; }

        public IncidentStatus Status { get; set; }

        public string Location { get; set; }

        public List<IncidentUpdateModel> IncidentUpdates { get; set; }

        public DateTime Created { get; set; }
    }
}
