// <copyright file="IncidentUpdateInputModel.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Tab.Models
{
    using System;
    using PoliceIncidents.Core.DB.Entities;

    public class IncidentUpdateInputModel
    {
        public long ParentIncidentId { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public Guid CreatedByUserId { get; set; }

        public IncedentUpdateType UpdateType { get; set; }
    }
}
