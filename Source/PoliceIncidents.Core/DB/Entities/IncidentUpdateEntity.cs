// <copyright file="IncidentUpdateEntity.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Core.DB.Entities
{
    using System;

    public class IncidentUpdateEntity
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public IncedentUpdateType UpdateType { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid CreatedById { get; set; }

        public UserEntity CreatedBy { get; set; }

        public long ParentIncidentId { get; set; }

        public IncidentDetailsEntity ParentIncident { get; set; }
    }
}
