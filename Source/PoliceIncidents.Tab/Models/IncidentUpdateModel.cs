// <copyright file="IncidentUpdateModel.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Tab.Models
{
    using System;
    using PoliceIncidents.Core.DB.Entities;

    public class IncidentUpdateModel
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public IncedentUpdateType UpdateType { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid CreatedById { get; set; }

        public IncidentUpdateModel Copy()
        {
            return this.MemberwiseClone() as IncidentUpdateModel;
        }
    }
}
