// <copyright file="IncidentInputModel.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

using System;

namespace PoliceIncidents.Core.Models
{
    public class IncidentInputModel
    {
        public string ExternalId { get; set; }

        public string ExternalLink { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }

        public string Region { get; set; }

        public string RegionId { get; set; }

        public Guid? ManagerId { get; set; }

        public Guid? Author { get; set; }
    }
}
